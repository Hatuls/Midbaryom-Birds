using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace ZED.Tracking
{
    public abstract class BaseTrackingMotion<T> : MonoBehaviour
    {
        private Thread _thread;
        private float _deltaTime;
        private bool _isGameOn;

 
        public abstract TrackingMotionManager<T>[] TrackingMotionManager { get; }


        protected virtual void Update()
        {
            _deltaTime = Time.deltaTime;

            for (int i = 0; i < TrackingMotionManager.Length; i++)
            {
                for (int j = 0; j < TrackingMotionManager[i].Observers.Count; j++)
                TrackingMotionManager[i].Observers[j].Tick();

            }
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
                    for (int i = 0; i < TrackingMotionManager.Length; i++)
                    {
                        TrackingMotionManager[i].Tick(_deltaTime);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
                throw e;
            }
            Debug.Log("Loop Ended");
        }
        public void Add(int id, params IBaseIntervalCondition<T>[] motions)
        {
            for (int i = 0; i < motions.Length; i++)
                Add(id, motions[i]);

        }

        public void Remove(int id, params IBaseIntervalCondition<T>[] motions)
        {
            for (int i = 0; i < motions.Length; i++)
                Remove(id, motions[i]);
        }
        public void Add(int id, IBaseIntervalCondition<T> motion)
        {
            TrackingMotionManager[id].AddIntervals(motion);
            if (TryGetObserver(id, motion.ID, out var observer))
                observer.AssignCondition(motion);
            else
                TrackingMotionManager[id].Observers.Add(new MotionObserver<T>(motion.ID, motion));
        }

        public void Remove(int id, int motionID, IBaseIntervalCondition<T> motion)
        {
            TrackingMotionManager[id].RemoveIntervals( motion);
            if (TryGetObserver(id, motion.ID, out var observer))
                observer.AssignCondition(null);
        }
        public void RemoveAll(int id)
        {
            TrackingMotionManager[id].RemoveAll();
        }
        public bool TryGetObserver(int id,int motionID, out MotionObserver<T> motion)
        {
            var observers = TrackingMotionManager[id].Observers;
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

        public void TryGetOrCreateEmptyObserver(int id, int motionID , out MotionObserver<T> motion)
        {
            if (!TryGetObserver(id, motionID, out motion))
            {
                motion = new MotionObserver<T>(id);
                TrackingMotionManager[id].Observers.Add(motion);
            }
        }
    }
}