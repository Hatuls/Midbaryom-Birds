
using System.Collections.Generic;
using UnityEngine;
namespace ZED.Tracking
{
    public class TrackingSkeletonHandler : BaseTrackingInstance<SkeletonHandler>
    {
        public SkeletonHandler SkeletonHandler { get; set; }

        protected override SkeletonHandler GetCurrentFramesData()
        {
            return SkeletonHandler;
        }
    }


    public class TrackingMotionManagerMB : BaseTrackingMotion<SkeletonHandler>
    {
        private TrackingSkeletonHandler _tracker;

        [SerializeField]
        private ZEDBodyTrackingManager _zEDBodyTrackingManager;

        public override BaseTrackingInstance<SkeletonHandler> Tracker => _tracker;

        protected override  void Awake()
        {
            _tracker = new TrackingSkeletonHandler();
            base.Awake();
        }
        protected override void Update()
        {
        
            if (_zEDBodyTrackingManager == null || !IsBodyTrackingWorking())
                return;

            _tracker.SkeletonHandler = _zEDBodyTrackingManager.AvatarControlList[0];

            base.Update();
        }
        
        private bool IsBodyTrackingWorking()
        {
            bool zedBodyTrackingInEnabled = _zEDBodyTrackingManager.AvatarControlList != null && _zEDBodyTrackingManager.AvatarControlList.Count > 0;

            return zedBodyTrackingInEnabled ;
        }
    }
}