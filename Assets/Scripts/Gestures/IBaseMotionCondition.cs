namespace ZED.Tracking
{
    public interface IBaseMotionCondition<T>
    {
        /// <summary>
        /// Ending Motion Condition
        /// if true it will signal that this motion is finished correctly
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool EndCondition(T data);
        /// <summary>
        /// On Going condition
        /// checks if the motion is still on going
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool OnGoingCondition(T data);
        /// <summary>
        /// The starting pose to start detect the motion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool StartingMotion(T data);
        /// <summary>
        /// Updates the delta data
        /// </summary>
        /// <param name="data"></param>
        void UpdateData(T data);
    }
}