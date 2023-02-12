using UnityEngine;
using UnityEngine.AI;

namespace Midbaryom.Core
{

    public class TargetedBehaviour : MonoBehaviour, ITargetBehaviour
    {
        [SerializeField]
        private Entity _entity;

        [SerializeField]
        private Rigidbody _rb;
        [SerializeField]
        private NavMeshAgent _agent;
        public void PotentiallyTarget()
        {
            Debug.Log("Potential Target: " + gameObject.name);
        }
        private void OnEnable()
        {
            _entity.VisualHandler.AnimatorController.Animator.SetBool("isDead", false);
        }
        public void Targeted()
        {
            _entity.MovementHandler.StopMovement = true;
            _entity.Rotator.StopRotation = true;

            StopAgentMovement();

            _entity.VisualHandler.AnimatorController.Animator.SetBool("isDead", true);

            _rb.isKinematic = true;
        }

        private void StopAgentMovement()
        {
            if (_agent.isActiveAndEnabled)
            {
                _agent.isStopped = true;
                _agent.enabled = false;
            }
        }

        public void UnTargeted()
        {
            Debug.Log("Not Target: " + gameObject.name);
        }
    }

    public interface ITargetBehaviour
    {
        void Targeted();
        void PotentiallyTarget();
        void UnTargeted();
    }
}