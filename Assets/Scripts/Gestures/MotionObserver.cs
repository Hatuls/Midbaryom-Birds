using System;
namespace ZED.Tracking
{

    public interface IUpdateable
    {
        void Tick();
    }
    public class MotionObserver<T> : IUpdateable
    {
        public readonly int ID;
        // called on each changed of state
        public event Action OnComplete;
        public event Action OnCancel;
        public event Action OnStart;
        public event Action OnGoing;

        //Called each frame
        public event Action OnActive;
        public event Action OnNotActive;


        private StatusType _currentStatus;
        private IBaseIntervalCondition<T> _motion;

        public MotionObserver(int id):this (id, null){ }
    
        public MotionObserver(int id, IBaseIntervalCondition<T> baseIntervalCondition)
        {
            ID = id;
            AssignCondition(baseIntervalCondition);
        }
        public void AssignCondition(IBaseIntervalCondition<T> baseIntervalCondition)
        {
            _currentStatus = StatusType.NotOperating;
            _motion = baseIntervalCondition;
        }
        public void Tick()
        {
            if (_motion == null)
                return;

            if (_motion.Status == StatusType.NotOperating)
                OnNotActive?.Invoke();
            else
                OnActive?.Invoke();

            if (_currentStatus == StatusType.OnGoing && _motion.Status == StatusType.OnGoing)
            {
                OnGoing?.Invoke();
                return;
            }

            if (_currentStatus == _motion.Status )
                return;

            _currentStatus = _motion.Status;

            switch (_currentStatus)
            {
                case StatusType.OnGoing:
                    OnGoing?.Invoke();
                    break;
                case StatusType.NotOperating:
                    OnCancel?.Invoke();
                    break;
                case StatusType.Starting:
                    OnStart?.Invoke();
                    break;
                case StatusType.Finished:
                    OnComplete?.Invoke();
                    break;
                default:
                    throw new Exception("Status type is not defined?\nInput: " + _currentStatus.ToString());
            }
        }
    }
}