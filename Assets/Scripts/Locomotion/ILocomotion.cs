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
    }

    public interface ITrackable
    {
        Vector3 CurrentFacingDirection { get; }
        Vector3 CurrentPosition { get; }
    }

    public class Locomotion : ILocomotion
    {
        public event Func<Vector3> OnHeightRequested;

        private readonly Transform _transform;
        private readonly IStat _movementStat;
        private bool _stopMovement;

        public Locomotion(Transform transform,bool stopMovement , IStat movementStat)
        {
            StopMovement = stopMovement;
            _movementStat = movementStat;
            _transform = transform;
        }

        public bool StopMovement { get => _stopMovement; set => _stopMovement = value; }
        public Vector3 CurrentFacingDirection => _transform.forward;
        public Vector3 CurrentPosition => _transform.position;

        public void MoveTowards(Vector3 direction)
        {
            Vector3 nextPos = OnHeightRequested?.Invoke() ?? CurrentPosition;
            nextPos += _movementStat.Value * direction;
            _transform.position = Vector3.MoveTowards(CurrentPosition, nextPos, Time.deltaTime);
        }

        public void SetPosition(Vector3 position)
        => _transform.position = position;
    }
}