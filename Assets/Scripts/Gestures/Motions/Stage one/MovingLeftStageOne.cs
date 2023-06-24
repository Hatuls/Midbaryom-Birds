using System;
using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu (menuName = "ScriptableObjects/Zeds/Motions/Stage One/Move Left")]
    public class MovingLeftStageOne : BaseMotionCondition
    {
        [SerializeField]
        private MoveConfig _config;
        private MovingLeft _movingLeft;
        public override IBaseIntervalCondition<SkeletonHandler> MotionCondition
        {
            get
            {
                _movingLeft = new MovingLeft(ID, FrameIntervals, _config);
                return _movingLeft;
            }
        }


    }
    [System.Serializable]
    public class MoveConfig
    {
     
        public float Distance;
        public float Duration;
    }


    [Serializable]
    public class MovingLeft : BaseIntervalCondition<SkeletonHandler>
    {
        Vector3 _startingLeftWrist;
        Vector3 _startingRightWrist;

        private float _timeCounter;

        private readonly MoveConfig _config;

        public MovingLeft(int id, int frameIntervals, MoveConfig config) : base(id, frameIntervals)
        {
            _config = config;
        }
        public override void Reset()
        {
            base.Reset();

            _timeCounter = 0;
        }
        public override void NextFrame(float deltaTime)
        {
            base.NextFrame(deltaTime);
            _timeCounter += deltaTime;
        }
        public override bool EndCondition(SkeletonHandler data)
        {
            try
            {
                bool isTrue = DetectPosture(data) && _timeCounter >= _config.Duration && StayedInPlace(data);
                Status = isTrue ? StatusType.Finished : Status;
            }
            catch (Exception e)
            {

                Debug.LogError($"Skeleton - {data == null}\nConfig: {_config == null}");
                throw e;
            }




            return Status == StatusType.Finished;
        }

        private bool StayedInPlace(SkeletonHandler data)
        {
            Vector3 leftWrist = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];

            float poseDistance = _config.Distance;
            float rightDistance = Vector3.Distance(_startingRightWrist, rightWrist);
            float leftDistance = Vector3.Distance(_startingLeftWrist, leftWrist);
            return leftDistance <= poseDistance && rightDistance <= poseDistance;
        }

        public override bool OnGoingCondition(SkeletonHandler data)
        {
            bool isTrue = DetectPosture(data);

            Status = isTrue ? StatusType.OnGoing : StatusType.NotOperating;
            //    Debug.Log($"OnGoing Condition {IsOnGoing}");
            return Status == StatusType.OnGoing;
        }

        public override bool StartingMotion(SkeletonHandler data)
        {
            bool isTrue = DetectPosture(data);

            Status = isTrue ? StatusType.Starting : StatusType.NotOperating;
            //   Debug.Log($"Start Condition {IsOnGoing}");
            if (isTrue)
            {
                _startingLeftWrist = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
                _startingRightWrist = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];
            }
            return Status == StatusType.Starting;
        }

        private static bool DetectPosture(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 spine = currentJoints[ZedsIDSheet.CHEST];
            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isLeftHandIsOnLeftSideOfSpine = leftWrist.x >= spine.x;
            bool isRightHandIsOnLeftSideOfSpine = rightWrist.x >= spine.x;
            bool isAboveChestHeight = leftWrist.y >= spine.y && rightWrist.y >= spine.y;

            bool isTrue = isLeftHandIsOnLeftSideOfSpine && isRightHandIsOnLeftSideOfSpine && isAboveChestHeight;
            return isTrue;
        }
    }
}