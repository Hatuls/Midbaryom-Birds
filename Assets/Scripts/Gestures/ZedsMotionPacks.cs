using UnityEngine;
namespace ZED.Tracking
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Zeds/Motions/New Motion Pack")]
    public class ZedsMotionPacks : ScriptableObject, IMotionPack
    {
        [SerializeField]
        private BaseMotionCondition[] _motions;
        public BaseMotionCondition[] Motions => _motions;
    }

    public interface IMotionPack
    {
        BaseMotionCondition[] Motions { get; }
    }

    public static class ZedsMotionPacksExtentions
    {
        public static void Add(this TrackingMotionManagerMB m,int id, ZedsMotionPacks zedsMotionPacks)
        {
            BaseMotionCondition[] motions = zedsMotionPacks.Motions;
            for (int i = 0; i < motions.Length; i++)
                m.Add(id,motions[i].MotionCondition);

        }
        public static void Remove(this TrackingMotionManagerMB m,int id, ZedsMotionPacks zedsMotionPacks)
        {
            BaseMotionCondition[] motions = zedsMotionPacks.Motions;
            for (int i = 0; i < motions.Length; i++)
                m.Remove(id,motions[i].MotionCondition);

        }

        public static void SetNewMotionsPack(this TrackingMotionManagerMB m,int id , IMotionPack motionPack)
        {
            m.RemoveAll(id);
            foreach (var item in motionPack.Motions)
                m.Add(id,item.MotionCondition);

        }
    }
}