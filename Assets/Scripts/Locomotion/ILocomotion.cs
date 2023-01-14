using System;
using UnityEngine;

namespace Midbaryom.Core
{
    /// <summary>
    /// Handle the movement of the entity
    /// </summary>
    public interface ILocomotion : ITrackable, IUpdateable
    {
        bool StopMovement { get; set; }
        void SetPosition(Vector3 position);
        void MoveTowards(Vector3 direction);

    }
    public interface IForwardDirection
    {
        Vector3 CurrentFacingDirection { get; }
    }
    public interface ITrackable : IForwardDirection
    {

        Vector3 CurrentPosition { get; }
    }

    public class Locomotion : ILocomotion
    {
        public event Func<Vector3> OnHeightRequested;
        public event Func<Vector3> OnFaceTowardDirectionRequested;

        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;
        private readonly IStat _movementStat;
        private bool _stopMovement;
        public bool StopMovement { get => _stopMovement; set => _stopMovement = value; }
        public Vector3 CurrentFacingDirection => _transform.forward;
        public Vector3 CurrentPosition => _transform.position;
        public Locomotion(Transform transform, Rigidbody rigidbody, bool stopMovement, IStat movementStat)
        {
            StopMovement = stopMovement;
            _movementStat = movementStat;
            _transform = transform;
            _rigidbody = rigidbody;
        }



        public void MoveTowards(Vector3 direction)
        {
            Vector3 nextPos = OnHeightRequested?.Invoke() ?? CurrentPosition;
            nextPos += _movementStat.Value * Time.deltaTime * direction;
            _transform.position = Vector3.MoveTowards(CurrentPosition, nextPos, Time.deltaTime);
            //    _transform.position = Vector3.MoveTowards(CurrentPosition, nextPos, Time.deltaTime);
        }

        public void Tick()
        {
            Vector3 direction = CurrentFacingDirection;
            MoveTowards(direction);
        }


        public void SetPosition(Vector3 position)
        => _transform.position = position;
    }


    public interface IRotator : IForwardDirection, IUpdateable
    {
        Vector3 NewDirection { get; }
        Quaternion StartingRotation();
        void AssignRotation(Vector3 direction);
    }
    public class Rotator : IRotator
    {
        protected readonly Transform _transform;
        protected readonly IStat _rotationSpeed;
        protected readonly Quaternion _startRotation;
        protected float _currentVelocity;
        protected bool _lockRotation;
        private Vector3 _direction;
        public Rotator(Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation)
        {
            _lockRotation = toLockRotation;
            _rotationSpeed = rotationSpeed;
            _transform = transform;
            _startRotation = startRotation;
        }

        public Vector3 NewDirection => _direction;
        public float RotationSpeed => _rotationSpeed.Value;
        public Vector3 CurrentFacingDirection => _transform.forward;

        public void AssignRotation(Vector3 direction)
        {
            _direction = direction;
            _direction.Normalize();
        }

        public void Tick()
        {
            if (CanRotate())
                RotateTowards();
        }
        protected virtual void RotateTowards()
        {

            float currentYRotation = _transform.eulerAngles.y;
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            float relativeAngle = targetAngle + currentYRotation;
            float smoothAngle = Mathf.SmoothDampAngle(currentYRotation, relativeAngle, ref _currentVelocity, RotationSpeed);

            _transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
        protected virtual bool CanRotate()
        {
            return (!_lockRotation && NewDirection.magnitude > 0);
        }


        public void Lock() => _lockRotation = true;
        public void UnLock() => _lockRotation = false;

        public Quaternion StartingRotation() => _startRotation;
    }
}