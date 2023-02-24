using UnityEngine;
using UnityEngine.AI;
using System;
namespace Midbaryom.Core
{

    public class TargetedBehaviour : MonoBehaviour, ITargetBehaviour
    {
        public event Action OnPotentiallyTargeted;
        public event Action OnTargeted;
        public event Action OnUnTargeted;

        [SerializeField]
        private Entity _entity;

        [SerializeField]
        private Rigidbody _rb;
        [SerializeField]
        private NavMeshAgent _agent;

        private bool _isPotentiallyTargeted;
        public void PotentiallyTarget()
        {
            Debug.Log("Potential Target: " + gameObject.name);
            if (!_isPotentiallyTargeted && OnPotentiallyTargeted != null)
                OnPotentiallyTargeted.Invoke();
            _isPotentiallyTargeted = true;
        }
        private void OnEnable()
        {
            _entity.VisualHandler.AnimatorController.Animator.SetBool("isDead", false);
        }
        /// <summary>
        /// Called the moment the bird holds the prey
        /// </summary>
        public void Targeted()
        {
            _entity.MovementHandler.StopMovement = true;
            _entity.Rotator.StopRotation = true;

            StopAgentMovement();

            _entity.VisualHandler.AnimatorController.Animator.SetBool("isDead", true);

            _rb.isKinematic = true;

            OnTargeted?.Invoke();
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
            _isPotentiallyTargeted = false;
            OnUnTargeted?.Invoke();
        }
    }

    public interface ITargetBehaviour
    {
        event Action OnPotentiallyTargeted;
        event Action OnTargeted;
        event Action OnUnTargeted;
        void Targeted();
        void PotentiallyTarget();
        void UnTargeted();
    }
}