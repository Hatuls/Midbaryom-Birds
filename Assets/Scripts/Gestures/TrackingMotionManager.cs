using System.Collections.Generic;
using System.Threading;

namespace ZED.Tracking
{
    public abstract class TrackingMotionManager<T>
    {
        private List<IBaseIntervalCondition<T>> _startingBehaviours = new List<IBaseIntervalCondition<T>>();
        private List<IBaseIntervalCondition<T>> _onGoingBehaviours = new List<IBaseIntervalCondition<T>>();

        protected abstract T GetCurrentFramesData();

        protected List<MotionObserver<T>> _observers;

        public TrackingMotionManager()
        {
            _observers = new List<MotionObserver<T>>();
        }
        public List<MotionObserver<T>> Observers => _observers;
        public IReadOnlyList<IBaseIntervalCondition<T>> StartingBehaviour => _startingBehaviours;
        public IReadOnlyList<IBaseIntervalCondition<T>> OnGoingBehaviours => _onGoingBehaviours;

        public void AddIntervals(params IBaseIntervalCondition<T>[] baseIntervalConditions)
        {
            lock (_startingBehaviours)
                _startingBehaviours.AddRange(baseIntervalConditions);
        }
        public void RemoveIntervals(params IBaseIntervalCondition<T>[] baseIntervalConditions)
        {
            for (int i = 0; i < baseIntervalConditions.Length; i++)
            {
                IBaseIntervalCondition<T> item = baseIntervalConditions[i];
                lock (_startingBehaviours)
                    _startingBehaviours.Remove(item);

                lock (_onGoingBehaviours)
                    _onGoingBehaviours.Remove(item);
            }
        }
        public virtual void Tick(float deltaTime)
        {
            T position = GetCurrentFramesData();
            CheckOngoingMotions(deltaTime, position);
            Thread.Sleep(System.TimeSpan.FromSeconds(deltaTime));
            CheckStartingMotions(position);
        }

        private void CheckOngoingMotions(float deltaTime, T data)
        {
            lock (_onGoingBehaviours)
            {
                int count = _onGoingBehaviours.Count;
                if (count == 0)
                    return;

                //IBaseIntervalCondition<T> behaviour = null;
                //     Parallel.ForEach(_onGoingBehaviours, CheckCondition);
                for (int i = count - 1; i >= 0; i--)
                {
                    CheckCondition(_onGoingBehaviours[i]);
                }

            }
            void CheckCondition(IBaseIntervalCondition<T> behaviour)
            {
                if (behaviour.Status == StatusType.Finished)
                {
                    ReturnToStartTrackingList(behaviour);
                    return;
                }
                // behaviour = _onGoingBehaviours[i];
                //waiting for the next interval to check this motion
                if (!behaviour.IsPassFrameInterval())
                {
                    behaviour.NextFrame(deltaTime);
                    return;
                }
                // reseting the frame counter
                behaviour.ResetFrameCounter();

                //checking if the motion finished
                if (!behaviour.EndCondition(data))
                {
                    //checking if its still in progress
                    if (behaviour.OnGoingCondition(data))
                        behaviour.UpdateData(data);
                    //if its not reached his end goal and is not in progress then we need to wait for it to start
                    else
                        ReturnToStartTrackingList(behaviour);
                }

            }
            void ReturnToStartTrackingList(IBaseIntervalCondition<T> behaviour)
            {
                behaviour.Reset();
                _onGoingBehaviours.Remove(behaviour);

                lock (_startingBehaviours)
                    _startingBehaviours.Add(behaviour);
            }
        }

        internal void RemoveAll()
        {
            lock (_startingBehaviours)
                _startingBehaviours.Clear();

            lock (_onGoingBehaviours)
                _onGoingBehaviours.Clear();
        }

        private void CheckStartingMotions(T data)
        {
            lock (_startingBehaviours)
            {
                int count = _startingBehaviours.Count;
                if (count == 0)
                    return;
                //Parallel.ForEach(_startingBehaviours, CheckStartingCondition);

                for (int i = count - 1; i >= 0; i--)
                {
                    CheckStartingCondition(_startingBehaviours[i]);
                }
            }

            void CheckStartingCondition(IBaseIntervalCondition<T> behaviour)
            {
                if (behaviour.Status == StatusType.Starting)
                {
                    _onGoingBehaviours.Add(behaviour);
                    _startingBehaviours.Remove(behaviour);
                }
                else if (behaviour.StartingMotion(data))
                {
                    behaviour.UpdateData(data);
                }
                else
                    behaviour.Reset();
            }
        }
    }


}