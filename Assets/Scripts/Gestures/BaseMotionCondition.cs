using UnityEngine;
namespace ZED.Tracking
{
 

    public abstract class BaseMotionCondition : ScriptableObject
    {
        public int ID;
        [Tooltip("The amount of frames the condition will check if the motion is still on going")]
        public int FrameIntervals;
        public abstract IBaseIntervalCondition<SkeletonHandler> MotionCondition { get; }
    }



}