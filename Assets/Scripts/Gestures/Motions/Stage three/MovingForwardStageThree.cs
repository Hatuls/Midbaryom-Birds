using System;
using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Zeds/Motions/Stage Three/Move Forward")]
    public class MovingForwardStageThree : BaseMotionCondition
    {
        [SerializeField]
        private float _minDeltaMovement;
        private MovingForwardMotionStageThree _moveForward;
        public override IBaseIntervalCondition<SkeletonHandler> MotionCondition
        {
            get
            {
                _moveForward = new MovingForwardMotionStageThree(ID, FrameIntervals, _minDeltaMovement);
                return _moveForward;
            }
        }


    }
    [Serializable]
    public class MovingForwardMotionStageThree : BaseIntervalCondition<SkeletonHandler>
    {
        private IBaseMotionCondition<SkeletonHandler> _upToDown;
        private IBaseMotionCondition<SkeletonHandler> _downToUp;
        private IBaseMotionCondition<SkeletonHandler> _current;

        public MovingForwardMotionStageThree(int id, int frameIntervals, float minDeltaMovement) : base(id, frameIntervals)
        {
            _upToDown = new MoveForwardStageThreeUpToDownMotion(minDeltaMovement);
            _downToUp = new MoveForwardStageThreeDownToUpMotion(minDeltaMovement);
        }
        public override bool EndCondition(SkeletonHandler data)
        {
            bool isTrue = _current.EndCondition(data);
            Status = isTrue ? StatusType.Finished : Status;

            return Status == StatusType.Finished;
        }



        public override bool OnGoingCondition(SkeletonHandler data)
        {
            bool isLeft = _upToDown.OnGoingCondition(data);
            bool isRight = false;
            if (isLeft)
                _current = _upToDown;
            else
            {
                isRight = _downToUp.OnGoingCondition(data);
                if (isRight)
                    _current = _downToUp;

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
            bool isLeft = _upToDown.StartingMotion(data);
            bool isRight = false;
            if (isLeft)
                _current = _upToDown;
            else
            {
                isRight = _downToUp.StartingMotion(data);
                if (isRight)
                    _current = _downToUp;

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
            _upToDown.UpdateData(data);
            _downToUp.UpdateData(data);
        }
    }



    public class MoveForwardStageThreeUpToDownMotion : IBaseMotionCondition<SkeletonHandler>
    {
        private readonly float _movementDeltaDistance;
        private Vector3 _leftWristDelta;
        private Vector3 _rightWristDelta;


        public MoveForwardStageThreeUpToDownMotion(float movementDeltaDistance)
        {
            _movementDeltaDistance = movementDeltaDistance;
        }


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];
            Vector3 neck = currentJoints[ZedsIDSheet.NECK];

            bool isMovedAbit = Mathf.Abs(_leftWristDelta.y - leftWrist.y) >= _movementDeltaDistance || Mathf.Abs(_rightWristDelta.y - rightWrist.y) >= _movementDeltaDistance;
            return leftWrist.y < neck.y || rightWrist.y < neck.y || !isMovedAbit;



            //bool isLeftWristIsPassedSpine = leftWrist.x >= spine.x || Mathf.Abs(leftWrist.x - spine.x) < _spineDistance;
            //bool isRightWristIsPassedSpine = rightWrist.x > spine.x;
            //return isLeftWristIsPassedSpine && isRightWristIsPassedSpine;
        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool LeftHandMoved = leftWrist.y < _leftWristDelta.y && rightWrist.y <= _rightWristDelta.y;
            bool RightHandMoved = leftWrist.y <= _leftWristDelta.y && rightWrist.y < _rightWristDelta.y;


            bool isMovedAbit = Mathf.Abs(_leftWristDelta.y - leftWrist.y) >= _movementDeltaDistance || Mathf.Abs(_rightWristDelta.y - rightWrist.y) >= _movementDeltaDistance;
            Debug.Log($"A: {Mathf.Abs(_leftWristDelta.y - leftWrist.y)}\nB: {Mathf.Abs(_rightWristDelta.y - rightWrist.y)} ");

            return (RightHandMoved || LeftHandMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
          
            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isLeftHandIsAboveHead = leftWrist.y > head.y;
            bool isRightHandIsAboveHead = rightWrist.y > head.y;

            bool isTrue = isLeftHandIsAboveHead && isRightHandIsAboveHead;

            return isTrue;
        }

        public void UpdateData(SkeletonHandler data)
        {
            _leftWristDelta = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
            _rightWristDelta = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];
        }
    }
    public class MoveForwardStageThreeDownToUpMotion : IBaseMotionCondition<SkeletonHandler>
    {

        private readonly float _movementDeltaDistance;
        private Vector3 _leftWristDelta;
        private Vector3 _rightWristDelta;


        public MoveForwardStageThreeDownToUpMotion(float movementDeltaDistance)
        {
            _movementDeltaDistance = movementDeltaDistance;
        }


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isMovedAbit = Mathf.Abs(_leftWristDelta.y - leftWrist.y) >= _movementDeltaDistance || Mathf.Abs(_rightWristDelta.y - rightWrist.y) >= _movementDeltaDistance;
            return !isMovedAbit;

            //bool isLeftWristIsPassedSpine = leftWrist.x <= spine.x || Mathf.Abs(leftWrist.x - spine.x) < _spineDistance;
            //bool isRightWristIsPassedSpine = rightWrist.x < spine.x;

            //return isLeftWristIsPassedSpine && isRightWristIsPassedSpine;

        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool LeftHandMoved = leftWrist.y > _leftWristDelta.y && rightWrist.y >= _rightWristDelta.y;
            bool RightHandMoved = leftWrist.y >= _leftWristDelta.y && rightWrist.y > _rightWristDelta.y;


            bool isMovedAbit = Mathf.Abs(_leftWristDelta.y - leftWrist.y) >= _movementDeltaDistance || Mathf.Abs(_rightWristDelta.y - rightWrist.y) >= _movementDeltaDistance;
            Debug.Log($"A: {Mathf.Abs(_leftWristDelta.y - leftWrist.y)}\nB: {Mathf.Abs(_rightWristDelta.y - rightWrist.y)} ");

            return (RightHandMoved || LeftHandMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];

            Vector3 leftWrist = currentJoints[ZedsIDSheet.LEFT_WRIST];
            Vector3 rightWrist = currentJoints[ZedsIDSheet.RIGHT_WRIST];

            bool isLeftHandIsBetweenHeadAndChest = leftWrist.y <= head.y && leftWrist.y >= chest.y;
            bool isRightHandIsAboveHeadAndChest = rightWrist.y <= head.y && leftWrist.y >= chest.y;

            bool isTrue = isLeftHandIsBetweenHeadAndChest && isRightHandIsAboveHeadAndChest;

            return isTrue;
        }

        public void UpdateData(SkeletonHandler data)
        {
            _leftWristDelta = data.currentJoints[ZedsIDSheet.LEFT_WRIST];
            _rightWristDelta = data.currentJoints[ZedsIDSheet.RIGHT_WRIST];
        }
    }
}