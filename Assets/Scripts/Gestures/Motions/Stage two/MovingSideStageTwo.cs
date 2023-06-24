using System;
using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu (menuName = "ScriptableObjects/Zeds/Motions/Stage Two/Move Left")]
    public class MovingSideStageTwo : BaseMotionCondition
    {
        public float MinAngle;
        public float MaxAngle;
        public float Duration;
        public float Distance;
        private MovingSideStageTwoMotion _movingLeft;
        public override IBaseIntervalCondition<SkeletonHandler> MotionCondition
        {
            get
            {
                _movingLeft = new MovingSideStageTwoMotion(ID, FrameIntervals, Duration, MinAngle, Distance, MaxAngle);
                return _movingLeft;
            }
        }


    }


    [Serializable]
    public class MovingSideStageTwoMotion : BaseIntervalCondition<SkeletonHandler>
    {
        Vector3 _startingHeadPose;
        Vector3 _startingPelvisPose;

        private float _timeCounter;

        private readonly float _distance;
        private readonly float _duration;
        private readonly float _minAngle;
        private readonly float _maxAngle;

        public MovingSideStageTwoMotion(int id, int frameIntervals, float duration, float startingAngle, float distance, float maxAngle) : base(id, frameIntervals)
        {
            _duration = duration;
            _minAngle = startingAngle;
            _distance = distance;
            _maxAngle = maxAngle;
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
            bool isTimeComplete = _timeCounter >= _duration;

            if (!isTimeComplete)
                return false;

            bool isTrue = DetectPosture(data);//&&StayedInPlace(data);
                Status = isTrue ? StatusType.Finished : Status;
   

            return Status == StatusType.Finished;
        }

        private bool StayedInPlace(SkeletonHandler data)
        {
            Vector3 head = data.currentJoints[ZedsIDSheet.HEAD];
            Vector3 pelvis = data.currentJoints[ZedsIDSheet.PELVIS];

            float poseDistance = _distance;
            float pelvisDistance = Vector3.Distance(_startingPelvisPose, pelvis);
            float headDistance = Vector3.Distance(_startingHeadPose, head);
            return headDistance <= poseDistance && pelvisDistance <= poseDistance;
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
                _startingHeadPose = data.currentJoints[ZedsIDSheet.HEAD];
                _startingPelvisPose = data.currentJoints[ZedsIDSheet.PELVIS];
            }
            return Status == StatusType.Starting;
        }

        private  bool DetectPosture(SkeletonHandler data )
        {
            if (data == null)
                return false;
            Vector3[] currentJoints = data.currentJoints;
            Vector3 HEAD = currentJoints[ZedsIDSheet.HEAD];
            Vector3 pelvis = currentJoints[ZedsIDSheet.PELVIS];

            Vector3 neckToPelvis = HEAD - pelvis;
            float tan = Mathf.Atan2(neckToPelvis.y, neckToPelvis.x) * Mathf.Rad2Deg;


          //  Debug.DrawLine(pelvis, neck);
          //  UnityEditor.Handles.DrawLine(neck, pelvis);
          //  UnityEditor.Handles.Label(neckToPelvis / 2f, $"Angle: {angle}");
            bool isTrue = tan >= _minAngle && tan <= _maxAngle;
            Debug.Log($"Angle: <a>{tan}</a> Result: {isTrue}\nMinAngle: {_minAngle} MaxAngle: {_maxAngle}");
            return isTrue;
        }
    }
}