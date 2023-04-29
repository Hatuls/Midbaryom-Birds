using Midbaryom.Core;
using Midbaryom.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZedPoseDetector : MonoBehaviour
{

    [SerializeField]
    private BodyTrackingConfigSO _bodyTrackingConfigSO;
    [SerializeField]
    private ZEDBodyTrackingManager _zEDBodyTrackingManager;


    [SerializeField]
    private Color _gizmosColor;
    [SerializeField]
    private float _radius;


    private List<IBodyTrackAnalyzer> _bodyTrackAnalyzers;

    [SerializeField]
    private BodyTrackingConfigSO _config;

    [SerializeField]
    private bool _drawSkeleton;



    private void Awake()
    {

        if (_bodyTrackingConfigSO.InputModeType == BodyTrackingConfigSO.InputMode.Keyboard)
            Destroy(this.gameObject);
        else
        {

            _zEDBodyTrackingManager = ZEDBodyTrackingManager.Instance;
            if (_zEDBodyTrackingManager == null)
                throw new Exception("Zed body tracking is not found!");

        }

    }

    private IEnumerator Start()
    {

        yield return null;

        InitPose();
    }

    private void Update()
    {
        if (_zEDBodyTrackingManager == null)
            return;

        if (IsBodyTrackingWorking())
        {
            foreach (SkeletonHandler item in _zEDBodyTrackingManager.AvatarControlList.Values)
            {
                for (int i = 0; i < _bodyTrackAnalyzers.Count; i++)
                {
                    if (_bodyTrackAnalyzers[i].CheckPosture(item))
                    {
                        _bodyTrackAnalyzers[i].PostureDetected();
                        break;
                    }
                    else
                        _bodyTrackAnalyzers[i].PostureNotDetected();
                }
            }
        }
    }

    private bool IsBodyTrackingWorking()
    {
        bool hasPosturesToDetect = _bodyTrackAnalyzers != null && _bodyTrackAnalyzers.Count > 0;
        bool zedBodyTrackingInEnabled = _zEDBodyTrackingManager.AvatarControlList != null && _zEDBodyTrackingManager.AvatarControlList.Count > 0;

        return zedBodyTrackingInEnabled && hasPosturesToDetect;
    }

    private void OnDrawGizmosSelected()
    {
        if (_zEDBodyTrackingManager == null || _drawSkeleton)
            return;

        InitPose();

        if (IsBodyTrackingWorking())
        {

            foreach (SkeletonHandler item in _zEDBodyTrackingManager.AvatarControlList.Values)
            {

                DrawSkeleton(item);

                for (int i = 0; i < _bodyTrackAnalyzers.Count; i++)
                {
                    if (_bodyTrackAnalyzers[i].CheckPosture(item))
                        _bodyTrackAnalyzers[i].DrawGizmos(item);
                }
            }
        }

    }

    private void InitPose()
    {
        var spawner = Spawner.Instance;
        if (spawner == null)
            return;
        else if (spawner.Player == null)
            return;

        var playerController = spawner.Player.PlayerController;
        _bodyTrackAnalyzers = new List<IBodyTrackAnalyzer>()
        {
            new HuntingPosture(playerController, _config.Distance),
            new TurningRightPosture(playerController,_config.RightTurn),
            new TurningLeftPosture(playerController, _config.LeftTurn)
        };
    }

    private void DrawSkeleton(SkeletonHandler skeletonHandler)
    {


        foreach (var joint in skeletonHandler.currentJoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(joint, _radius);
        }

    }
}

public interface IBodyTrackAnalyzer
{
    event Action OnPostureDetected;
    bool CheckPosture(SkeletonHandler skeleton);
    void PostureDetected();
    public void PostureNotDetected();
    void DrawGizmos(SkeletonHandler skeleton);
}

public class TurningLeftPosture : IBodyTrackAnalyzer
{
    public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_34_PARTS.LEFT_HAND;
    public const int LEFT_SHOULDER = (int)sl.BODY_34_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_34_PARTS.RIGHT_HAND;
    public const int RIGHT_SHOULDER = (int)sl.BODY_34_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _rightArmAngle;
    private float _leftArmAngle;

    public TurningLeftPosture(PlayerController playerController, BodyTrackingConfigSO.TurningConfig turningConfig) : this(turningConfig.LeftArmAngle, turningConfig.RightArmAngle)
    {
        this.playerController = playerController;
    }

    public TurningLeftPosture(float leftArmAngle, float rightArmAngle)
    {
        _rightArmAngle = rightArmAngle;
        _leftArmAngle = leftArmAngle;
    }

    public bool CheckPosture(SkeletonHandler skeleton)
    {
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];

        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

        bool leftHandPose = leftAngle < _leftArmAngle;
        bool rightHandPose = rightAngle < _rightArmAngle;
        bool handAbove = leftShoulder.y < leftHand.y && rightShoulder.y >= rightHand.y;
#if UNITY_EDITOR
        Debug.Log($"{typeof(TurningLeftPosture).Name}  RESULT: { leftHandPose && rightHandPose}\n" +
                $"Right arm angle: {rightAngle}. Need to be under {_rightArmAngle}. Result = {rightHandPose}\n" +
                $"Left arm angle: " + leftAngle + $". Need to be under {_leftArmAngle}.Result = {leftHandPose}");
        // float angle = Vector3.Angle(rightHandToElbow, rightElbowToShoulder);
#endif
        if (leftHandPose && rightHandPose && handAbove)
            OnPostureDetected?.Invoke();
        return leftHandPose && rightHandPose;
    }

    public void DrawGizmos(SkeletonHandler skeleton)
    {
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];



        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

        bool leftHandPose = leftAngle < _leftArmAngle;
        bool rightHandPose = rightAngle < _rightArmAngle;

#if UNITY_EDITOR
        Debug.Log($"{typeof(TurningLeftPosture).Name}  RESULT: { leftHandPose && rightHandPose}\n" +
            $"Right arm angle: {rightAngle}. Need to be above {_rightArmAngle}. Result = {rightHandPose}\n" +
            $"Left arm angle: " + leftAngle + $". Need to be under {_leftArmAngle}.Result = {leftHandPose}");
        // float angle = Vector3.Angle(rightHandToElbow, rightElbowToShoulder);
#endif
        Gizmos.color = leftHandPose && rightHandPose ? Color.green : Color.red;
        Gizmos.DrawLine(leftHand, leftShoulder);





        float leftDistance = Vector3.Distance(leftHand, leftShoulder);
        float rightDistance = Vector3.Distance(rightHand, rightShoulder);

        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.Rotate(Vector3.left);
        OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {
        playerController.Rotate(Vector3.zero);
    }
}

public class TurningRightPosture : IBodyTrackAnalyzer
{
    public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_34_PARTS.LEFT_HAND;
    public const int LEFT_SHOULDER = (int)sl.BODY_34_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_34_PARTS.RIGHT_HAND;
    public const int RIGHT_SHOULDER = (int)sl.BODY_34_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _rightArmAngle;
    private float _leftArmAngle;

    public TurningRightPosture(PlayerController playerController, BodyTrackingConfigSO.TurningConfig turningConfig) : this(turningConfig.LeftArmAngle, turningConfig.RightArmAngle)
    {
        this.playerController = playerController;
    }
    public TurningRightPosture(float leftArmAngle, float rightArmAngle)
    {
        _rightArmAngle = rightArmAngle;
        _leftArmAngle = leftArmAngle;
    }

    public bool CheckPosture(SkeletonHandler skeleton)
    {
        //receiving the points from the skeleton
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];

        // creating the vector from the shoulder to the hand
        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        // calculating the angle of the shoulder to hand vector
        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;


        // left hand is above certain angle
        bool leftHandPose = leftAngle > _leftArmAngle;
        // right hand is under a certain angle
        bool rightHandPose = rightAngle > _rightArmAngle;
        bool handAbove = leftShoulder.y > leftHand.y && rightShoulder.y <= rightHand.y;
#if UNITY_EDITOR
        Debug.Log($"{typeof(TurningRightPosture).Name} RESULT: { leftHandPose && rightHandPose}\n" +
      $"Right arm angle: {rightAngle}. Need to be above {_rightArmAngle}. Result = {rightHandPose}\n" +
      $"Left arm angle: " + leftAngle + $". Need to be above {_leftArmAngle}.Result = {leftHandPose}");
        // float angle = Vector3.Angle(rightHandToElbow, rightElbowToShoulder);
#endif
        // posture is correct!
        if (leftHandPose && rightHandPose && handAbove)
            OnPostureDetected?.Invoke();
        return leftHandPose && rightHandPose;
    }

    public void DrawGizmos(SkeletonHandler skeleton)
    {
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];

        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

        bool leftHandPose = leftAngle > _leftArmAngle;
        bool rightHandPose = rightAngle > _rightArmAngle;
#if UNITY_EDITOR
        Debug.Log($"{typeof(TurningRightPosture).Name}- RESULT: { leftHandPose && rightHandPose}\n" +
      $"Right arm angle: {rightAngle}. Need to be above {_rightArmAngle}. Result = {rightHandPose}\n" +
      $"Left arm angle: " + leftAngle + $". Need to be above {_leftArmAngle}.Result = {leftHandPose}");
#endif
        Gizmos.color = leftHandPose && rightHandPose ? Color.green : Color.red;
        Gizmos.DrawLine(leftHand, leftShoulder);
        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.Rotate(Vector3.right);
        OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {
        playerController.Rotate(Vector3.zero);
    }
}



public class HuntingPosture : IBodyTrackAnalyzer
{
    public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_34_PARTS.LEFT_HAND;
    public const int LEFT_SHOULDER = (int)sl.BODY_34_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_34_PARTS.RIGHT_HAND;
    public const int RIGHT_SHOULDER = (int)sl.BODY_34_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _distance;



    public HuntingPosture(PlayerController playerController, float distance)
    {
        this.playerController = playerController;
        _distance = distance;
    }

    public bool CheckPosture(SkeletonHandler skeleton)
    {
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];

        float leftDistance = Vector3.Distance(leftHand, leftShoulder);
        float rightDistance = Vector3.Distance(rightHand, rightShoulder);

        bool isCorrectPosture = leftDistance < _distance && rightDistance < _distance;
        if (isCorrectPosture)
            OnPostureDetected?.Invoke();

        return isCorrectPosture;
    }

    public void DrawGizmos(SkeletonHandler skeleton)
    {
        Vector3 leftHand = skeleton.currentJoints[LEFT_HAND];
        Vector3 leftShoulder = skeleton.currentJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeleton.currentJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeleton.currentJoints[RIGHT_SHOULDER];

        float leftDistance = Vector3.Distance(leftHand, leftShoulder);
        float rightDistance = Vector3.Distance(rightHand, rightShoulder);
        bool isCorrectPosture = leftDistance < _distance && rightDistance < _distance;
#if UNITY_EDITOR
        Debug.Log($"{typeof(HuntingPosture).Name}- RESULT: {isCorrectPosture}\n" +
$"Distance between left hand and left Shoulder is {leftDistance} need to be under {_distance}\nDistance Between right Hand and right shoulder is {rightDistance} need to be under {_distance}");
#endif
        Gizmos.color = (isCorrectPosture) ? Color.green : Color.red;
        Gizmos.DrawLine(leftHand, leftShoulder);

        //   Gizmos.color = () ? Color.green : Color.red;
        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.HuntDown();
        OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {

    }
}