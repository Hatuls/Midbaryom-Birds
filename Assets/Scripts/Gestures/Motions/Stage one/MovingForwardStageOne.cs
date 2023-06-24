using System;
using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Zeds/Motions/Stage One/Move Forward")]
    public class MovingForwardStageOne : BaseMotionCondition
    {
        [SerializeField]
        private float _minDeltaMovement;
        private MovingForwardMotionStageOne _moveForward;
        public override IBaseIntervalCondition<SkeletonHandler> MotionCondition
        {
            get
            {
                _moveForward = new MovingForwardMotionStageOne(ID, FrameIntervals, _minDeltaMovement);
                return _moveForward;
            }
        }


    }

    [Serializable]
    public class MovingForwardMotionStageOne : BaseIntervalCondition<SkeletonHandler>
    {
        private IBaseMotionCondition<SkeletonHandler> _leftToRight;
        private IBaseMotionCondition<SkeletonHandler> _rightToLeft;
        private IBaseMotionCondition<SkeletonHandler> _current;

        public MovingForwardMotionStageOne(int id, int frameIntervals, float minDeltaMovement) : base(id, frameIntervals)
        {
            _leftToRight = new MoveForwardStageOneLeftToRightMotion(minDeltaMovement);
            _rightToLeft = new MoveForwardStageOneRightToLeftMotion(minDeltaMovement);
        }
        public override bool EndCondition(SkeletonHandler data)
        {
            bool isTrue = _current.EndCondition(data);
            Status = isTrue ? StatusType.Finished : Status;

            return Status == StatusType.Finished;
        }



        public override bool OnGoingCondition(SkeletonHandler data)
        {
            bool isLeft = _leftToRight.OnGoingCondition(data);
            bool isRight = false;
            if (isLeft)
                _current = _leftToRight;
            else
            {
                isRight = _rightToLeft.OnGoingCondition(data);
                if (isRight)
                    _current = _rightToLeft;

                if (isLeft && isRight)
                    throw new Exception("Cannot detect both motions at the same time!");
            }
            

            bool isTrue = isLeft || isRight;
            if (isTrue == false)
                Debug.Log("!");
            Status = isTrue ? StatusType.OnGoing : StatusType.NotOperating;
            //    Debug.Log($"OnGoing Condition {IsOnGoing}");
            return Status == StatusType.OnGoing;
        }

        public override bool StartingMotion(SkeletonHandler data)
        {
            bool isLeft = _leftToRight.StartingMotion(data);
            bool isRight = false;
            if (isLeft)
                _current = _leftToRight;
            else
            {
                isRight = _rightToLeft.StartingMotion(data);
                if (isRight)
                    _current = _rightToLeft;

                if (isLeft && isRight)
                    throw new Exception("Cannot detect both motions at the same time!");
            }


            bool isTrue = isLeft || isRight;

            Status = isTrue ? StatusType.Starting : StatusType.NotOperating;

            return Status == StatusType.Starting;
        }
        public override void UpdateData(SkeletonHandler data)
        {
            base.UpdateData(data);
            _leftToRight.UpdateData(data);
            _rightToLeft.UpdateData(data);      
        }
    }



    public class MoveForwardStageOneLeftToRightMotion : IBaseMotionCondition<SkeletonHandler>
    {
        private readonly float _movementDeltaDistance;
        private Vector3 _leftWrist;
        private Vector3 _rightWrist;


        public MoveForwardStageOneLeftToRightMotion(float movementDeltaDistance)
        {
            _movementDeltaDistance = movementDeltaDistance;
        }


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];
            Vector3 spine = currentJoints[ZedsIDSheet.CHEST];


            return leftWrist.y < spine.y || rightWrist.y < spine.y ;
    
        

            //bool isLeftWristIsPassedSpine = leftWrist.x >= spine.x || Mathf.Abs(leftWrist.x - spine.x) < _spineDistance;
            //bool isRightWristIsPassedSpine = rightWrist.x > spine.x;
            //return isLeftWristIsPassedSpine && isRightWristIsPassedSpine;
        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool LeftHandMoved = leftWrist.x > _leftWrist.x && rightWrist.x >= _rightWrist.x;
            bool RightHandMoved = leftWrist.x >= _leftWrist.x && rightWrist.x > _rightWrist.x;
            bool isMovedAbit = Mathf.Abs(_leftWrist.x - leftWrist.x) >= _movementDeltaDistance || Mathf.Abs(_rightWrist.x - rightWrist.x) >= _movementDeltaDistance;
            Debug.Log($"A: {Mathf.Abs(_leftWrist.x - leftWrist.x)}\nB: {Mathf.Abs(_rightWrist.x - rightWrist.x)} ");

            return (RightHandMoved || LeftHandMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 spine = currentJoints[ZedsIDSheet.CHEST];
            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isLeftHandIsOnLeftSideOfSpine = leftWrist.x < spine.x;
            bool isRightHandIsOnLeftSideOfSpine = rightWrist.x < spine.x;
            bool isAboveChestHeight = leftWrist.y >= spine.y && rightWrist.y >= spine.y;

            bool isTrue = isLeftHandIsOnLeftSideOfSpine && isRightHandIsOnLeftSideOfSpine && isAboveChestHeight;
            return isTrue;
        }

        public void UpdateData(SkeletonHandler data)
        {
            _leftWrist = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
            _rightWrist = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];
        }
    }
    public class MoveForwardStageOneRightToLeftMotion : IBaseMotionCondition<SkeletonHandler>
    {
 
        private readonly float _movementDeltaDistance;
        private Vector3 _leftWrist;
        private Vector3 _rightWrist;


        public MoveForwardStageOneRightToLeftMotion(float movementDeltaDistance)
        {
            _movementDeltaDistance = movementDeltaDistance;
        }


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];
            Vector3 spine = currentJoints[ZedsIDSheet.CHEST];


            return leftWrist.y < spine.y || rightWrist.y < spine.y;

            //bool isLeftWristIsPassedSpine = leftWrist.x <= spine.x || Mathf.Abs(leftWrist.x - spine.x) < _spineDistance;
            //bool isRightWristIsPassedSpine = rightWrist.x < spine.x;

            //return isLeftWristIsPassedSpine && isRightWristIsPassedSpine;

        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool LeftHandMoved = leftWrist.x < _leftWrist.x && rightWrist.x <= _rightWrist.x;
            bool RightHandMoved = leftWrist.x <= _leftWrist.x && rightWrist.x < _rightWrist.x;
            bool isMovedAbit = Mathf.Abs(_leftWrist.x - leftWrist.x) >= _movementDeltaDistance || Mathf.Abs(_rightWrist.x - rightWrist.x) >= _movementDeltaDistance;


            return (RightHandMoved || LeftHandMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 spine = currentJoints[ZedsIDSheet.CHEST];
            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isLeftHandIsOnLeftSideOfSpine = leftWrist.x > spine.x;
            bool isRightHandIsOnLeftSideOfSpine = rightWrist.x > spine.x;
            bool isAboveChestHeight = leftWrist.y >= spine.y && rightWrist.y >= spine.y;

            bool isTrue = isLeftHandIsOnLeftSideOfSpine && isRightHandIsOnLeftSideOfSpine && isAboveChestHeight;
            return isTrue;
        }

        public void UpdateData(SkeletonHandler data)
        {
            _leftWrist = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
            _rightWrist = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];
        }
    }
}