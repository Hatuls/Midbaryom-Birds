
using System.Collections.Generic;
using UnityEngine;
namespace ZED.Tracking
{
    public class TrackingTestManager : TrackingMotionManager<SkeletonHandler>
    {
        public SkeletonHandler SkeletonHandler { get; set; }

        protected override SkeletonHandler GetCurrentFramesData()
        {
            return SkeletonHandler;
        }

    }


    public class TrackingMotionManagerMB : BaseTrackingMotion<SkeletonHandler>
    {
        private TrackingTestManager[] _trackingTestManager;

        [SerializeField]
        private ZEDBodyTrackingManager _zEDBodyTrackingManager;

        public override TrackingMotionManager<SkeletonHandler>[] TrackingMotionManager => _trackingTestManager;

        public Dictionary<int, TrackingTestManager> SkeletonDictionary = new Dictionary<int, TrackingTestManager>();
        protected override  void Awake()
        {
            _trackingTestManager = new TrackingTestManager[4];
            for (int i = 0; i < _trackingTestManager.Length; i++)
            {
                _trackingTestManager[i] = new TrackingTestManager();
            }
            base.Awake();
        }
        protected override void Update()
        {
        
            if (_zEDBodyTrackingManager == null || !IsBodyTrackingWorking())
                return;


            foreach (KeyValuePair<int, SkeletonHandler> item in _zEDBodyTrackingManager.AvatarControlList)
            {
                if (!SkeletonDictionary.TryGetValue(item.Key, out var motionManager))
                {
                    motionManager = _trackingTestManager[SkeletonDictionary.Count];
                    SkeletonDictionary.Add(item.Key, motionManager);
                }

                motionManager.SkeletonHandler = item.Value;
            }


            base.Update();
        }
        
        private bool IsBodyTrackingWorking()
        {
            bool zedBodyTrackingInEnabled = _zEDBodyTrackingManager.AvatarControlList != null && _zEDBodyTrackingManager.AvatarControlList.Count > 0;

            return zedBodyTrackingInEnabled ;
        }
    }
}