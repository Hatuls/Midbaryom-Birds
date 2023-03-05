using Midbaryom.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Midbaryom.Core
{
    public class AimAssists : MonoBehaviour
    {
        private const float _halfViewPoint = .5f;


        public event Action OnTargetReset;
        public event Action OnTargetDetected;
        public event Action<IEntity> OnTargetDetectedEntity;

        private List<IEntity> _allTargets = new List<IEntity>();

        [SerializeField]
        private UnityEngine.Camera _camera;

        [SerializeField]
        private int _layerMaskIndex;
        [SerializeField, Range(0, 100f)]
        private float _radius;
        [SerializeField, Range(0, 200f)]
        private float _rayDistance;


        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private WarnLogic _warningTargets;
        [SerializeField]
        private int _startWarningAnimalsAt = 2;

        [SerializeField]
        private TagSO[] _targetingTags;


        private bool _lockTarget;
        private Vector3 _middleScreenPoint;
        public bool HasTarget => Target != null;
        public IEntity Target { get; private set; }
        public IReadOnlyList<IEntity> AllActiveEntities => PoolManager.Instance.ActiveEntities;

        public Vector3 TargetDirection => HasTarget ? (Target.CurrentPosition - transform.position).normalized : Vector3.zero;

        public IReadOnlyList<IEntity> AllTargets { get => _allTargets; }
        public IWarningTargets WarningTargets { get => _warningTargets; }
        public Vector3 FacingDirection => (ScreenToWorldPoint() - transform.position).normalized;

        private void Start()
        {
            ///Set _targetingTags to the current targeting animals
            _middleScreenPoint = new Vector3(_halfViewPoint, _halfViewPoint, _camera.nearClipPlane);
        }
        private void Update()
        {
            if (_lockTarget)
                return;

            Scan(FacingDirection);
        }

        private Vector3 ScreenToWorldPoint()
        => _camera.ViewportToWorldPoint(Quaternion.Euler(_offset) * _middleScreenPoint);


        private void Scan(Vector3 facingDirection)
        {
            if (ShootRaycast(facingDirection, out RaycastHit raycastHit))
            {
                IEntity closestTarget = CheckDistance(raycastHit.point);
                if (closestTarget != null)
                { 
                    AssignTarget(closestTarget);
                    WarnOtherTarget();
                    return;
                }
            }

            ResetTarget();
        }




        private void AssignTarget(IEntity closestTarget)
        {
            if (Target == closestTarget)
                return;

            //   Target?.TargetBehaviour.UnTargeted();
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

                    if (!currentEntity.ContainOneOrMoreTags(_targetingTags))
                        continue;

                  //  if (!currentEntity.EntityTagSO.CanBeTargeted)
                  //      continue;

                    float currentsTargetDistance = GetDistance(currentEntity, middleScanPoint);

                    if (currentsTargetDistance < _radius)
                    {
                        if (closestTarget == null)
                            closestTarget = currentEntity;

                        else if (currentsTargetDistance < GetDistance(closestTarget, middleScanPoint))
                            closestTarget = currentEntity;

                        if(!_allTargets.Contains(currentEntity))
                        _allTargets.Add(currentEntity);
                    }
                    else
                    {
                       
                        if(_allTargets.Remove(currentEntity))
                            currentEntity.TargetBehaviour.UnTargeted();
                    }
                }
            }

            return closestTarget;

            float GetDistance(IEntity closestTarget, in Vector3 point)
                => Vector3.Distance(closestTarget.CurrentPosition, point);
        }

        internal void LockTarget()
        {
            _lockTarget = true;
        }

        public void ResetTarget()
        {
            //if (Target != null)
            //   Target.TargetBehaviour.UnTargeted();
            Target = null;
            _allTargets.Clear();
            if (OnTargetReset != null)
                OnTargetReset.Invoke();
        }

        internal void UnLockTarget()
        {
            _lockTarget = false;
        }

        private bool ShootRaycast(Vector3 direction, out RaycastHit raycastHit)
        {
            Ray rei = new Ray(transform.position, direction);
            return Physics.Raycast(rei, out raycastHit, _rayDistance, _layerMaskIndex);
        }



        private void WarnOtherTarget()
        {
            if (Target == null
                || _allTargets.Count < _startWarningAnimalsAt)
                return;


            //Getting all the other targets that are not the target we currently locked on
            var otherTargets = _allTargets.Where(x => x!=Target && WarningTargets.IsTargetWarned(transform.position, x));

            foreach (var alertedEnemy in otherTargets)
                alertedEnemy.TargetBehaviour.PotentiallyTarget();

        }







#if UNITY_EDITOR
        #region Editor:
        private void OnDrawGizmosSelected()
        {

            Gizmos.color = HasTarget ? Color.green : Color.red;
            Vector3 direction = ScreenToWorldPoint() - transform.position;

            Gizmos.DrawLine(transform.position, transform.position + direction * _rayDistance);
            if (ShootRaycast(direction.normalized, out RaycastHit hit))
                Gizmos.DrawWireSphere(hit.point, _radius);
            Gizmos.color = Color.blue;
            //     Gizmos.DrawLine(_camera.transform.position, hit.point);


         
        }

        private void OnDrawGizmos()
        {
            DrawTargetDotDirection();
        }
        private void DrawTargetDotDirection()
        {
            if (_allTargets == null || _allTargets.Count == 0)
                return;

            var otherTargets = _allTargets;
            foreach (var target in otherTargets)
            {
                IEntity entity = target;
                DrawTargetsFacingDirection(entity);
                DrawTargetsFacingTowardPlayerDirection(entity);
            }

            void DrawTargetsFacingDirection(IEntity entity)
            {
                Gizmos.color = Color.blue;
                Vector3 currentPosition = entity.CurrentPosition;
                Gizmos.DrawLine(currentPosition, currentPosition + entity.MovementHandler.CurrentFacingDirection * (transform.position-currentPosition).magnitude);
            }

            void DrawTargetsFacingTowardPlayerDirection(IEntity entity)
            {
                if (Target == null)
                    return;


                Gizmos.color = _warningTargets.IsOtherTargetFacingCurrentTarget(transform.position, entity) ? Color.green : Color.red;
                Vector3 currentPosition = entity.CurrentPosition;
                Vector3 facingDireciton = transform.position - currentPosition;
                Gizmos.DrawLine(currentPosition, currentPosition + facingDireciton);
                Gizmos.DrawWireSphere(currentPosition + facingDireciton, .01f);
            }


        }
        #endregion
#endif

    }


    public interface IWarningTargets
    {
        bool IsTargetWarned(Vector3 myself, IEntity otherTarget);
    }
    [System.Serializable]
    public class WarnLogic : IWarningTargets
    {
        [SerializeField, Range(0, 1), Tooltip("The angle the target need to face to be warned")]
        private float _preciseness;

        public bool IsTargetWarned(Vector3 myself, IEntity otherTarget)
        {
            return IsOtherTargetFacingCurrentTarget(myself, otherTarget);
        }

        public bool IsOtherTargetFacingCurrentTarget(Vector3 myself, IEntity otherTarget)
        {
            Vector3 targetsPosition = otherTarget.CurrentPosition;

            Vector3 targetsDirection = otherTarget.CurrentFacingDirection;

            Vector3 lhs = myself - targetsPosition;
            lhs.Normalize();
            float lookness = Vector3.Dot(lhs, targetsDirection);

            return lookness >= _preciseness;
        }


    }
}
