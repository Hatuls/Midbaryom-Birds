using Midbaryom.Core;
using Midbaryom.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    private bool _gameStarted;


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
        _gameStarted = false;
        yield return null;
        yield return null;
        _gameStarted = true;

        InitPose();
    }

    private void Update()
    {
        if (_zEDBodyTrackingManager == null || !_gameStarted)
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
    //event Action OnPostureDetected;
    bool CheckPosture(SkeletonHandler skeleton);
    void PostureDetected();
    public void PostureNotDetected();
    void DrawGizmos(SkeletonHandler skeleton);
}

public class TurningLeftPosture : IBodyTrackAnalyzer
{

    //public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_38_PARTS.LEFT_WRIST;
    public const int LEFT_SHOULDER = (int)sl.BODY_38_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_38_PARTS.RIGHT_WRIST;
    public const int RIGHT_SHOULDER = (int)sl.BODY_38_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _leftArmAngle;
    private float _rightArmAngle;
    private float tempDistanceEmpty;

    public TurningLeftPosture(PlayerController playerController, BodyTrackingConfigSO.TurningConfig turningConfig) : this(turningConfig.LeftArmAngle, turningConfig.RightArmAngle)
    {
        this.playerController = playerController;
    }

    public TurningLeftPosture(float leftArmAngle, float rightArmAngle)
    {
        //_rightArmAngle = rightArmAngle;
        //_leftArmAngle = leftArmAngle;

        _rightArmAngle = GameManager.Instance.rightLTemp;
        _leftArmAngle = GameManager.Instance.leftLTemp;
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

        bool handAbove = rightShoulder.z < rightHand.z;

        if(GameManager.Instance.useDebugMessages)
        {
            GameManager.Instance.leftL.text = "Left L: " + leftAngle.ToString();
            GameManager.Instance.rightL.text = "Right L: " + rightAngle.ToString();
            GameManager.Instance.aboveL.text = handAbove == true ? "Above L: Yes" : "Above L: No";
        }

        //Debug.Log("Left L: " + leftAngle);
        //Debug.Log("right L: " + rightAngle);
        //Debug.Log("Above L: " + handAbove);

        //if (leftHandPose && rightHandPose && handAbove)
        //OnPostureDetected?.Invoke();
        return leftAngle > -_leftArmAngle && leftAngle < _leftArmAngle && handAbove;
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
        //bool rightHandPose = rightAngle < _rightArmAngle;
        bool handAbove = leftShoulder.y > leftHand.y && rightShoulder.y < rightHand.y;
//#if UNITY_EDITOR
//        Debug.Log($"{typeof(TurningLeftPosture).Name}  RESULT: {leftHandPose && rightHandPose && handAbove}\n" +
//                $"Right arm angle: {rightAngle}. Need to be under {_rightArmAngle}. Result = {rightHandPose}\n" +
//                $"Left arm angle: " + leftAngle + $". Need to be under {_leftArmAngle}.Result = {leftHandPose}\n" +
//        $"Left arm need to be under left shoulder - shoulder ({leftShoulder.y}), hand ({leftHand.y}), RESULT - {leftShoulder.y > leftHand.y}\n" +
//        $"Right arm need to be above right shoulder - shoulder ({rightShoulder.y}), hand ({rightHand.y}), RESULT - {rightShoulder.y < rightHand.y}");
//#endif
//        Gizmos.color = leftHandPose && handAbove && rightHandPose ? Color.green : Color.red;
//        Gizmos.DrawLine(leftHand, leftShoulder);





        float leftDistance = Vector3.Distance(leftHand, leftShoulder);
        float rightDistance = Vector3.Distance(rightHand, rightShoulder);

        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.Rotate(Vector3.left / 2);
        //OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {
        playerController.Rotate(Vector3.zero);
    }
}

public class TurningRightPosture : IBodyTrackAnalyzer
{
    //public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_38_PARTS.LEFT_WRIST;
    public const int LEFT_SHOULDER = (int)sl.BODY_38_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_38_PARTS.RIGHT_WRIST;
    public const int RIGHT_SHOULDER = (int)sl.BODY_38_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _rightArmAngle;
    private float _leftArmAngle;
    private float tempDistanceEmpty;

    public TurningRightPosture(PlayerController playerController, BodyTrackingConfigSO.TurningConfig turningConfig) : this(turningConfig.LeftArmAngle, turningConfig.RightArmAngle)
    {
        this.playerController = playerController;
    }
    public TurningRightPosture(float leftArmAngle, float rightArmAngle)
    {
        _rightArmAngle = GameManager.Instance.rightRTemp;
        _leftArmAngle = GameManager.Instance.leftRTemp;

        //_rightArmAngle = rightArmAngle;
        //_leftArmAngle = leftArmAngle;
    }

    public bool CheckPosture(SkeletonHandler skeleton)
    {
        //receiving the points from the skeleton
        Vector3[] skeletonJoints = skeleton.currentJoints;
        Vector3 leftHand = skeletonJoints[LEFT_HAND];
        Vector3 leftShoulder = skeletonJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeletonJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeletonJoints[RIGHT_SHOULDER];

        // creating the vector from the shoulder to the hand
        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        // calculating the angle of the shoulder to hand vector
        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

        bool handAbove = leftShoulder.z < leftHand.z;

        if (GameManager.Instance.useDebugMessages)
        {
            GameManager.Instance.leftR.text = "Left R: " + leftAngle.ToString();
            GameManager.Instance.rightR.text = "Right R: " + rightAngle.ToString();
            GameManager.Instance.aboveR.text = handAbove == true ? "Above R: Yes" : "Above R: No";
        }

        //Debug.Log("Left R: " + leftAngle);
        //Debug.Log("right R: " + rightAngle);
        //Debug.Log("Above R: " + handAbove);


        // posture is correct!
        //if (leftHandPose && rightHandPose && handAbove)
        //    OnPostureDetected?.Invoke();
        return rightAngle > -_rightArmAngle && rightAngle < _rightArmAngle && handAbove;
    }

    public void DrawGizmos(SkeletonHandler skeleton)
    {
        Vector3[] skeletonJoints = skeleton.currentJoints;
        Vector3 leftHand = skeletonJoints[LEFT_HAND];
        Vector3 leftShoulder = skeletonJoints[LEFT_SHOULDER];

        Vector3 rightHand = skeletonJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeletonJoints[RIGHT_SHOULDER];

        Vector3 leftShoulderToHand = leftHand - leftShoulder;
        Vector3 rightShoulderToHand = rightHand - rightShoulder;

        float rightAngle = Mathf.Sin(rightShoulderToHand.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
        float leftAngle = Mathf.Sin(leftShoulderToHand.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

        bool leftHandPose = leftAngle > _leftArmAngle;
        bool rightHandPose = rightAngle > _rightArmAngle;
        bool handAbove = leftShoulder.y > leftHand.y && rightShoulder.y <= rightHand.y;

        Gizmos.color = leftHandPose && handAbove && rightHandPose ? Color.green : Color.red;
        Gizmos.DrawLine(leftHand, leftShoulder);
        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.Rotate(Vector3.right / 2);
        //OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {
        playerController.Rotate(Vector3.zero);
    }
}



public class HuntingPosture : IBodyTrackAnalyzer
{
    //public event Action OnPostureDetected;
    public const int LEFT_HAND = (int)sl.BODY_38_PARTS.LEFT_WRIST;
    public const int LEFT_ELBOW = (int)sl.BODY_38_PARTS.LEFT_ELBOW;
    public const int LEFT_SHOULDER = (int)sl.BODY_38_PARTS.LEFT_SHOULDER;

    public const int RIGHT_HAND = (int)sl.BODY_38_PARTS.RIGHT_WRIST;
    public const int RIGHT_SHOULDER = (int)sl.BODY_38_PARTS.RIGHT_SHOULDER;
    private readonly PlayerController playerController;
    private float _distance;



    public HuntingPosture(PlayerController playerController, float distance)
    {
        this.playerController = playerController;
        //_distance = distance;

        _distance = GameManager.Instance.distanceTemp;

    }

    public bool CheckPosture(SkeletonHandler skeleton)
    {
        Vector3[] skeletonJoints = skeleton.currentJoints;

        Vector3 leftHand = skeletonJoints[LEFT_HAND];
        Vector3 leftShoulder = skeletonJoints[LEFT_SHOULDER];


        Vector3 rightHand = skeletonJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeletonJoints[RIGHT_SHOULDER];


        float leftXDistance = Mathf.Abs(leftHand.x - leftShoulder.x);
        float rightXDistance = Mathf.Abs(rightHand.x - rightShoulder.x);


        if (GameManager.Instance.useDebugMessages)
        {
            GameManager.Instance.midL.text = "Mid L: " + leftXDistance.ToString();
            GameManager.Instance.midR.text = "Mid R: " + rightXDistance.ToString();
        }

        //float armLength = Vector3.Distance(leftShoulder, skeletonJoints[LEFT_ELBOW]); // calculating the length of the arm
        //float left = Vector3.Distance(leftHand, leftShoulder);
        //float right = Vector3.Distance(rightHand, rightShoulder);
        //ClearLog();

        //Debug.Log("arm Length: " + armLength);
        //Debug.Log("Dist L: " + left);
        //Debug.Log("Dist R: " + right);

        //bool isHandsCloseToShoulder = left <= armLength && right <= armLength;



        bool isCorrectPosture = IsHandsCloseToShoulderOnXAxis(leftXDistance, rightXDistance) /*&& isHandsCloseToShoulder*/;

        if(isCorrectPosture)
        {
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Not Correct");
        }


        return isCorrectPosture;
  
        bool IsHandsCloseToShoulderOnXAxis(float leftDistance, float rightDistance)
        {


            return leftDistance <= _distance && rightDistance <= _distance;
        }

    }
    //public void ClearLog()
    //{
    //    var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
    //    var type = assembly.GetType("UnityEditor.LogEntries");
    //    var method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}

    public void DrawGizmos(SkeletonHandler skeleton)
    {
        Vector3[] skeletonJoints = skeleton.currentJoints;

        Vector3 leftHand = skeletonJoints[LEFT_HAND];
        Vector3 leftShoulder = skeletonJoints[LEFT_SHOULDER];


        Vector3 rightHand = skeletonJoints[RIGHT_HAND];
        Vector3 rightShoulder = skeletonJoints[RIGHT_SHOULDER];

        float leftXDistance = Mathf.Abs(leftHand.x - leftShoulder.x);
        float rightXDistance = Mathf.Abs(rightHand.x - rightShoulder.x);
        bool isXClose = leftXDistance <= _distance && rightXDistance <= _distance;

        float armLength = Vector3.Distance(leftHand, skeletonJoints[LEFT_ELBOW]); // calculating the length of the arm
        float left = Vector3.Distance(leftHand, leftShoulder);
        float right = Vector3.Distance(rightHand, rightShoulder);
        bool isHandsCloseToShoulder = left <= armLength && right <= armLength;


        bool isCorrectPosture = isXClose && isHandsCloseToShoulder;
        //if (isCorrectPosture)
        //    OnPostureDetected?.Invoke();


        Gizmos.color = (isCorrectPosture) ? Color.green : Color.red;
        Gizmos.DrawLine(leftHand, leftShoulder);

        //   Gizmos.color = () ? Color.green : Color.red;
        Gizmos.DrawLine(rightHand, rightShoulder);
    }

    public void PostureDetected()
    {
        playerController.HuntDown();
        //OnPostureDetected?.Invoke();
    }

    public void PostureNotDetected()
    {

    }

}