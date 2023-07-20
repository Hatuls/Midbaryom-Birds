using System;
using UnityEngine;
namespace ZED.Tracking
{
    public enum StatusType
    {
        NotOperating,
        Starting,
        Finished,
        OnGoing
    }
    public interface IBaseIntervalCondition<T> : IBaseMotionCondition<T>
    {
        StatusType Status { get; }
        int FrameInterval { get; }
        int ID { get; }
        bool IsPassFrameInterval();
        void NextFrame(float deltaTime);
        void Reset();
        void ResetFrameCounter();
    }

    [Serializable]
    public abstract class BaseIntervalCondition<T> : IBaseIntervalCondition<T>
    {
        [SerializeField]
        private int _frameCounter;
        [SerializeField]
        protected T _deltaData;
        [SerializeField]
        private StatusType _status;

        public int ID { get; }
        public int FrameInterval { get; private set; }
        public StatusType Status { get => _status; protected set => _status = value; }

        public BaseIntervalCondition(int id, int frameInterval)
        {
            ID = id;
            FrameInterval = frameInterval;
            Status = StatusType.NotOperating;
        }
        public abstract bool StartingMotion(T data);
        public abstract bool OnGoingCondition(T data);
        public abstract bool EndCondition(T data);

        public virtual void NextFrame(float deltaTime)
        {
            _frameCounter++;

            //if (IsOnGoing)
            //    OnConditionTrue?.Invoke();
        }
        public virtual void Reset()
        {
            ResetFrameCounter();
            //  IsOnGoing = false;

            Status = StatusType.NotOperating;
        }
        public virtual void ResetFrameCounter() => _frameCounter = -1;
        public bool IsPassFrameInterval() => _frameCounter >= FrameInterval;
        public virtual void UpdateData(T data) => _deltaData = data;
    }
 
}

public class ZedsIDSheet
{
    public const int HEAD = (int)sl.BODY_34_PARTS.HEAD;
    public const int NECK = (int)sl.BODY_34_PARTS.NECK;

    public const int LEFT_WRIST = (int)sl.BODY_34_PARTS.LEFT_WRIST;
    public const int LEFT_ELBOW = (int)sl.BODY_34_PARTS.LEFT_ELBOW;
    public const int LEFT_SHOULDER = (int)sl.BODY_34_PARTS.LEFT_SHOULDER;

    public const int RIGHT_WRIST = (int)sl.BODY_34_PARTS.RIGHT_WRIST;
    public const int RIGHT_SHOULDER = (int)sl.BODY_34_PARTS.RIGHT_SHOULDER;

    public const int CHEST = (int)sl.BODY_34_PARTS.CHEST_SPINE;
    public const int PELVIS = (int)sl.BODY_34_PARTS.PELVIS;
}