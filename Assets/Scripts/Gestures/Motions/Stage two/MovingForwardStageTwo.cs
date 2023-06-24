using System;
using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Zeds/Motions/Stage Two/Move Forward")]
    public class MovingForwardStageTwo : BaseMotionCondition
    {
        [SerializeField]
        private float _minHeadMovementDelta;
        [SerializeField]
        private float _minChestMovementDelta;
        private MovingForwardMotionStageTwo _moveForward;
        public override IBaseIntervalCondition<SkeletonHandler> MotionCondition
        {
            get
            {
                _moveForward = new MovingForwardMotionStageTwo(ID, FrameIntervals, _minHeadMovementDelta, _minChestMovementDelta);
                return _moveForward;
            }
        }


    }
    [Serializable]
    public class MovingForwardMotionStageTwo : BaseIntervalCondition<SkeletonHandler>
    {
        private IBaseMotionCondition<SkeletonHandler> _leftToRight;
        private IBaseMotionCondition<SkeletonHandler> _rightToLeft;
        private IBaseMotionCondition<SkeletonHandler> _current;

        public MovingForwardMotionStageTwo(int id, int frameIntervals, float headMovementDelta,float chestMovementDelta) : base(id, frameIntervals)
        {
            _leftToRight = new MoveForwardStageTwoLeftToRightMotion(headMovementDelta, chestMovementDelta);
            _rightToLeft = new MoveForwardStageTwoRightToLeftMotion(headMovementDelta, chestMovementDelta);
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



    public class MoveForwardStageTwoLeftToRightMotion : IBaseMotionCondition<SkeletonHandler>
    {
        private readonly float _headMovementDeltaDistance;
        private readonly float _chestMovementDeltaDistance;
        private Vector3 _headDelta;
        private Vector3 _chestDelta;
        //private Vector3 _pelvisDelta;


        public MoveForwardStageTwoLeftToRightMotion(float headMovementDeltaDistance, float chestMovementDeltaDistance)
        {
            _headMovementDeltaDistance = headMovementDeltaDistance;
            _chestMovementDeltaDistance = chestMovementDeltaDistance;
        }


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];


            bool isMovedAbit = Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance || Mathf.Abs(_chestDelta.x - chest.x) >= _chestMovementDeltaDistance;

            return !isMovedAbit;
        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];

            bool headMoved = head.x > _headDelta.x && chest.x >= _chestDelta.x;
            bool chestmoved = head.x >= _headDelta.x && chest.x > _chestDelta.x;


            bool isMovedAbit = Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance || Mathf.Abs(_chestDelta.x - chest.x) >= _headMovementDeltaDistance;
    //        Debug.Log($"A: {Mathf.Abs(_headDelta.x - head.x)} Result:<a>{Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance}</a>\nB: {Mathf.Abs(_chestDelta.x - chest.x)} Result:<a>{Mathf.Abs(_chestDelta.x - chest.x) >= _headMovementDeltaDistance}</a>");

            return (chestmoved || headMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];
            Vector3 pelvis = currentJoints[ZedsIDSheet.PELVIS];
            
            bool isHeadIsLeftToChest = Mathf.Abs(chest.x - pelvis.x) < 0.01f;
            bool isChestLeftToPelvis = Mathf.Abs(chest.x - head.x) < 0.03f;

            bool isTrue = isHeadIsLeftToChest && isChestLeftToPelvis;
         
           // Debug.LogWarning($"Starting Distance Of chest: {Mathf.Abs(chest.x - pelvis.x)}\nStating Distance Of Head: {Mathf.Abs(chest.x - head.x)}");
            return isTrue;
        }

        public void UpdateData(SkeletonHandler data)
        {
            _headDelta = data.currentJoints[ZedsIDSheet.HEAD];
            _chestDelta = data.currentJoints[ZedsIDSheet.CHEST];
        }
    }
    public class MoveForwardStageTwoRightToLeftMotion : IBaseMotionCondition<SkeletonHandler>
    {

        private readonly float _headMovementDeltaDistance;
        private readonly float _chestMovementDeltaDistance;
        private Vector3 _headDelta;
        private Vector3 _chestDelta;
        //private Vector3 _pelvisDelta;
        public MoveForwardStageTwoRightToLeftMotion(float headMovementDeltaDistance, float chestMovementDeltaDistance)
        {
            _headMovementDeltaDistance = headMovementDeltaDistance;
            _chestMovementDeltaDistance = chestMovementDeltaDistance;
        }
   


        public bool EndCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];


            bool isMovedAbit = Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance || Mathf.Abs(_chestDelta.x - chest.x) >= _chestMovementDeltaDistance;

            return !isMovedAbit;

        }

        public bool OnGoingCondition(SkeletonHandler data)
        {
            Vector3[] currentJoints = data.currentJoints;

            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];

            bool headMoved = head.x < _headDelta.x && chest.x <= _chestDelta.x;
            bool chestmoved = head.x <= _headDelta.x && chest.x < _chestDelta.x;


            bool isMovedAbit = Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance || Mathf.Abs(_chestDelta.x - chest.x) >= _headMovementDeltaDistance;
         
        //    Debug.Log($"A: {Mathf.Abs(_headDelta.x - head.x)} Result:<a>{Mathf.Abs(_headDelta.x - head.x) >= _headMovementDeltaDistance}</a>\nB: {Mathf.Abs(_chestDelta.x - chest.x)} Result:<a>{Mathf.Abs(_chestDelta.x - chest.x) >= _headMovementDeltaDistance}</a>");
            return (chestmoved || headMoved) && isMovedAbit;
        }

        public bool StartingMotion(SkeletonHandler data)
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            Vector3 chest = currentJoints[ZedsIDSheet.CHEST];
            Vector3 pelvis = currentJoints[ZedsIDSheet.PELVIS];

            bool isHeadIsLeftToChest = Mathf.Abs(chest.x - pelvis.x) < 0.01f;
            bool isChestLeftToPelvis = Mathf.Abs(chest.x - head.x) < 0.03f;

            bool isTrue = isHeadIsLeftToChest && isChestLeftToPelvis;

         //   Debug.LogWarning($"Starting Distance Of chest: {Mathf.Abs(chest.x - pelvis.x)}\nStating Distance Of Head: {Mathf.Abs(chest.x - head.x)}");
            return isTrue;
            //if (data == null)
            //    return false;
            //Vector3[] currentJoints = data.currentJoints;
            //Vector3 head = currentJoints[ZedsIDSheet.HEAD];
            //Vector3 chest = currentJoints[ZedsIDSheet.CHEST];
            //Vector3 pelvis = currentJoints[ZedsIDSheet.PELVIS];

            //bool isHeadIsRightToChest = head.x > chest.x;
            //bool isChestRightToPelvis = chest.x > pelvis.x;

            //bool isTrue = isHeadIsRightToChest && isChestRightToPelvis;

            //return isTrue;
        }


        public void UpdateData(SkeletonHandler data)
        {
            _headDelta = data.currentJoints[ZedsIDSheet.HEAD];
            _chestDelta = data.currentJoints[ZedsIDSheet.CHEST];
        }
    }
}