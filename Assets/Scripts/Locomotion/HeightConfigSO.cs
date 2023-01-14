using UnityEngine;

namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Config/New Height Config")]
    public class HeightConfigSO : ScriptableObject
    {
        public float PlayerHeight, GroundHeight, AnimalHeight;
    }
}