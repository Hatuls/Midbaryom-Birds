using System;
using UnityEngine;

namespace Midbaryom.Core
{
    /// <summary>
    /// Handle the movement of the entity
    /// </summary>
    public interface ILocomotion : ITrackable
    {
        bool StopMovement { get; set; }
        void SetPosition(Vector3 position);
        void MoveTowards(Vector3 direction);
        void FixedUpdateTick();
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

        public Locomotion(Transform transform, Rigidbody rigidbody,bool stopMovement , IStat movementStat)
        {
            StopMovement = stopMovement;
            _movementStat = movementStat;
            _transform = transform;
            _rigidbody = rigidbody;
        }

        public bool StopMovement { get => _stopMovement; set => _stopMovement = value; }
        public Vector3 CurrentFacingDirection => _transform.forward;
        public Vector3 CurrentPosition => _transform.position;

        public void MoveTowards(Vector3 direction)
        {
            Vector3 nextPos = OnHeightRequested?.Invoke() ?? CurrentPosition;
            nextPos += _movementStat.Value *Time.deltaTime* direction;
            _rigidbody.MovePosition(nextPos);
        //    _transform.position = Vector3.MoveTowards(CurrentPosition, nextPos, Time.deltaTime);
        }

        public void FixedUpdateTick()
        {
            Vector3 direction = CurrentFacingDirection;
            MoveTowards(direction);
        }


        public void SetPosition(Vector3 position)
        => _transform.position = position;
    }


    public interface IRotator : IForwardDirection
    {
        Vector3 NewRotation { get; }

        void AssignRotation(Vector3 direction);
        void RotationTick();
    }
    public class Rotator : IRotator
    {
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;
        private readonly IStat _rotationSpeed;
        private Vector3 _direction;
        public Rotator(Transform transform, Rigidbody rigidbody, IStat rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
            _transform = transform;
            _rigidbody = rigidbody;
        }

        public Vector3 NewRotation => _direction;
        public float RotationSpeed => _rotationSpeed.Value;
        public Vector3 CurrentFacingDirection => _transform.forward;

        public void AssignRotation(Vector3 direction)
        {
           _direction = direction ;
            _direction.Normalize();
        }

        public void RotationTick()
        {
            Quaternion toRotation = Quaternion.LookRotation(NewRotation, Vector3.up);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, toRotation, Time.deltaTime * RotationSpeed);
            // Need to add Rotation
           // Debug.Log("Add Rotation");
        }
    }
}