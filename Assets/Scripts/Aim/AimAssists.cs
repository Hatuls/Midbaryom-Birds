using Midbaryom.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public class AimAssists : MonoBehaviour
    {
        private const float _halfViewPoint = .5f;


        public event Action OnTargetReset;
        public event Action OnTargetDetected;
        public event Action<IEntity> OnTargetDetectedEntity;


        private PoolManager _poolManager;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private int _layerMaskIndex;
        [SerializeField, Range(0, 100f)]
        private float _radius;
        [SerializeField, Range(0, 200f)]
        private float _maxDistance;


        [SerializeField]
        private Vector3 _offset ;
        public bool HasTarget { get; private set; }
        public IEntity Target { get; private set; }

        public IReadOnlyList<IEntity> AllActiveEntities => _poolManager.ActiveEntities;

        private void Start()
        {
            _poolManager = PoolManager.Instance;
        }
        private void Update()
        {
            Vector3 worldPoint = ScreenToWorldPoint();
            Vector3 direction = worldPoint - transform.position;
            Scan(direction.normalized);
        }

        private Vector3 ScreenToWorldPoint()
        => _camera.ViewportToWorldPoint(Quaternion.Euler(_offset) *new Vector3(_halfViewPoint, _halfViewPoint, _camera.nearClipPlane));


        private void Scan(Vector3 facingDirection)
        {
            if (ShootRaycast(facingDirection, out RaycastHit raycastHit))
            {
                IEntity closestTarget = CheckDistance(raycastHit.point);
                if (closestTarget == null)
                    ResetTarget();
                else
                    AssignTarget(closestTarget);
            }
            else
                ResetTarget();
        }

        private void AssignTarget(IEntity closestTarget)
        {
            Target = closestTarget;
            OnTargetDetected?.Invoke();
            OnTargetDetectedEntity?.Invoke(Target);
        }

        private IEntity CheckDistance(in Vector3 middleScanPoint)
        {
            IEntity currentEntity = null;
            IEntity closestTarget = null;
            IReadOnlyList<IEntity> allActiveEntities = AllActiveEntities;
            int count = allActiveEntities.Count;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    currentEntity = allActiveEntities[i];
                    float currentsTargetDistance = GetDistance(currentEntity, middleScanPoint);

                    if (currentsTargetDistance < _maxDistance)
                    {
                        if (closestTarget == null)
                            closestTarget = currentEntity;

                        else if (currentsTargetDistance < GetDistance(closestTarget, middleScanPoint))
                            closestTarget = currentEntity;
                    }
                }
            }

            return closestTarget;

            float GetDistance(IEntity closestTarget, in Vector3 point)
                => Vector3.Distance(closestTarget.CurrentPosition, point);
        }

        public void ResetTarget()
        {
            Target = null;
            HasTarget = false;
            if (OnTargetReset != null)
                OnTargetReset.Invoke();
        }

        private bool ShootRaycast(Vector3 direction, out RaycastHit raycastHit)
        {
            Ray rei = new Ray(transform.position, direction);
            return Physics.Raycast(rei, out raycastHit, _maxDistance, _layerMaskIndex);
        }
        private void OnDrawGizmosSelected()
        {

            Gizmos.color = HasTarget ? Color.green : Color.red;
            Vector3 direction = ScreenToWorldPoint() - transform.position;
            
            Gizmos.DrawLine(transform.position, transform.position + direction*_maxDistance);
          if( ShootRaycast(direction.normalized,out RaycastHit hit))
            Gizmos.DrawWireSphere(hit.point, _radius);
            Gizmos.color = Color.blue;
       //     Gizmos.DrawLine(_camera.transform.position, hit.point);
        }
    }
}
