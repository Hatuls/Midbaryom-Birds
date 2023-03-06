using UnityEngine;
using UnityEngine.AI;
using System;
namespace Midbaryom.Core
{

    public class TargetedBehaviour : MonoBehaviour, ITargetBehaviour
    {
        public event Action OnPotentiallyTargeted;
        public event Action OnEaten;
        public event Action OnUnTargeted;
        public event Action OnCurrentlyTargeted;

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
            _entity.VisualHandler.AnimatorController.SetBool("isDead", false);
        }
        /// <summary>
        /// Called the moment the bird holds the prey
        /// </summary>
        public void Eaten()
        {
            _entity.MovementHandler.StopMovement = true;
            _entity.Rotator.StopRotation = true;

            StopAgentMovement();

            _entity.VisualHandler.AnimatorController.SetBool("isDead", true);

            _rb.isKinematic = true;

            OnEaten?.Invoke();
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

        public void CurrentTarget()
        {
            OnCurrentlyTargeted?.Invoke();
        }
    }

    public interface ITargetBehaviour
    {
        event Action OnPotentiallyTargeted;
        event Action OnCurrentlyTargeted;
        event Action OnEaten;
        event Action OnUnTargeted;
        /// <summary>
        /// When the prey is under the bird legs
        /// </summary>
        void Eaten();
        /// <summary>
        /// When the animal is in the scan distance
        /// </summary>
        void PotentiallyTarget();
        /// <summary>
        /// when the animal is not a target anymore
        /// </summary>
        void UnTargeted();
        /// <summary>
        /// the animal is being targeted
        /// </summary>
        void CurrentTarget();
    }
}