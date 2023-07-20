using System.Threading;
using UnityEngine;
namespace ZED.Tracking
{
    public abstract class BaseTrackingMotion<T> : MonoBehaviour
    {
        private Thread _thread;
        private float _deltaTime;
        private bool _isGameOn;


        public abstract BaseTrackingInstance<T> Tracker { get; }


        protected virtual void Update()
        {
            _deltaTime = Time.deltaTime;

            for (int j = 0; j < Tracker.Observers.Count; j++)
                Tracker.Observers[j].Tick();
        }
        protected virtual void Awake()
        {
            _isGameOn = true;
            _thread = new Thread(new ThreadStart(Loop));
            _thread.Start();
        }
        protected virtual void OnDestroy()
        {
            _isGameOn = false;
            _thread.Abort();
        }
        private void Loop()
        {
            Debug.Log("Loop starting");
            try
            {
                while (_isGameOn)
                {
                    Tracker.Tick(_deltaTime);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
                throw e;
            }
            Debug.Log("Loop Ended");
        }
        public void Add(params IBaseIntervalCondition<T>[] motions)
        {
            for (int i = 0; i < motions.Length; i++)
                Add(motions[i]);

        }

        public void Remove(params IBaseIntervalCondition<T>[] motions)
        {
            for (int i = 0; i < motions.Length; i++)
                Remove(motions[i]);
        }
        public void Add(IBaseIntervalCondition<T> motion)
        {
            Tracker.AddIntervals(motion);
            if (TryGetObserver(motion.ID, out var observer))
                observer.AssignCondition(motion);
            else
                Tracker.Observers.Add(new MotionObserver<T>(motion.ID, motion));
        }

        public void Remove(IBaseIntervalCondition<T> motion)
        {
            Tracker.RemoveIntervals(motion);
            if (TryGetObserver(motion.ID, out var observer))
                observer.AssignCondition(null);
        }
        public void RemoveAll()
        {
            Tracker.RemoveAll();
        }
        public bool TryGetObserver(int motionID, out MotionObserver<T> motion)
        {
            var observers = Tracker.Observers;
            int count = observers.Count;
            motion = null;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (observers[i].ID == motionID)
                    {
                        motion = observers[i];
                        return true;
                    }
                }
            }
            return false;
        }

        public void TryGetOrCreateEmptyObserver(int motionID, out MotionObserver<T> motion)
        {
            if (!TryGetObserver(motionID, out motion))
            {
                motion = new MotionObserver<T>(motionID, null);
                Tracker.Observers.Add(motion);
            }
        }
    }
}