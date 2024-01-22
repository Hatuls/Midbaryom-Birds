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
            Debug.Log(System.DateTime.Now.ToString() + "" + "Potential Target: " + gameObject.name);
            if (!_isPotentiallyTargeted && OnPotentiallyTargeted != null)
                OnPotentiallyTargeted.Invoke();
            _isPotentiallyTargeted = true;
        }
        private void OnEnable()
        {
            //_entity.VisualHandler.AnimatorController.SetTrigger("isDead");
            //_rb.isKinematic = false;
            //_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        /// <summary>
        /// Called the moment the bird holds the prey
        /// </summary>
        public void Eaten()
        {
            int LayerIgnore = LayerMask.NameToLayer("Animal");
            var newMask = GameManager.Instance.mainCam.cullingMask & ~(1 << LayerIgnore);
            GameManager.Instance.mainCam.cullingMask = newMask;

            _entity.MovementHandler.StopMovement = true;
            _entity.Rotator.StopRotation = true;

            StopAgentMovement();

            _entity.VisualHandler.AnimatorController.SetBool("isDead", true);

            _rb.isKinematic = true;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            OnEaten?.Invoke();
        }

        private void StopAgentMovement()
        {
            if (_agent.isActiveAndEnabled)
            {
                if (_agent.isOnNavMesh)
                _agent.isStopped = true;
                _agent.enabled = false;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set Targets Hunting Position")]
        private void SetPosition()
        {

            var entityTag = _entity.EntityTagSO;

            entityTag.HoldingOffset.PositionOffset = transform.localPosition;
            entityTag.HoldingOffset.RotaionOffset = transform.localRotation;

        }
#endif
        public void UnTargeted()
        {
     //       Debug.Log("Not Target: " + gameObject.name);
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