using UnityEngine;
namespace Midbaryom.Core
{

    public class TargetedBehaviour : MonoBehaviour, ITargetBehaviour
    {
        [SerializeField]
        private Entity _entity;

       
        public void Targeted()
        {
            Debug.Log("Target: " + gameObject.name);
        }

        public void UnTargeted()
        {
            Debug.Log("Not Target: " + gameObject.name);
        }
    }

    public interface ITargetBehaviour
    {
        void Targeted();

        void UnTargeted();
    }
}