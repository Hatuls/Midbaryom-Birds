using System;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

namespace Midbaryom.Core
{
    /// <summary>
    /// Handle the movement of the entity
    /// </summary>
    public interface ILocomotion : ITrackable, IUpdateable
    {
        event Action<float> OnMove;
        bool AutoPilot { get; set; }
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
        Transform Transform { get; }
        Vector3 CurrentPosition { get; }
    }

    public class Locomotion : ILocomotion
    {
        public event Func<float> OnHeightRequested;
        public event Func<Vector3> OnFaceTowardDirectionRequested;
        public event Action<float> OnMove;
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;
        private readonly IStat _movementStat;
        private bool _stopMovement;
        public bool StopMovement { get => _stopMovement; set => _stopMovement = value; }
        public Vector3 CurrentFacingDirection => Transform.forward.normalized;
        public Vector3 CurrentPosition => Transform.position;

        public bool AutoPilot { get; set; }

        public Transform Transform => _transform;

        public Locomotion(Transform transform, Rigidbody rigidbody, bool stopMovement, IStat movementStat)
        {
            AutoPilot = false;
            StopMovement = stopMovement;
            _movementStat = movementStat;
            _transform = transform;
            _rigidbody = rigidbody;
        }



        public void MoveTowards(Vector3 direction)
        {
            Vector3 nextPos = CurrentPosition;
            nextPos +=  direction;
       
            nextPos.y = OnHeightRequested?.Invoke() ?? CurrentPosition.y;
            float movementSpeed = _movementStat.Value;
            Vector3 finalPosition = Vector3.MoveTowards(CurrentPosition, nextPos, movementSpeed * Time.deltaTime);
            if (!finalPosition.IsNan())
            SetPosition(finalPosition);
            OnMove?.Invoke(movementSpeed);
            //    _transform.position = Vector3.MoveTowards(CurrentPosition, nextPos, Time.deltaTime);
        }

        public void Tick()
        {
            if (StopMovement)
            {
                OnMove?.Invoke(0);
                return;
            }


            Vector3 direction = CurrentFacingDirection;
            MoveTowards(direction);
        }


        public void SetPosition(Vector3 position)
        => _transform.position = position;
    }


    public interface IRotator : IForwardDirection, IUpdateable
    {
        event Action<float> OnFaceDirection;
        bool StopRotation { get; set; }
        Vector3 NewDirection { get; }
        Quaternion StartingRotation();
        void SetRotation(Quaternion quaternion);
        void AssignRotation(Vector3 direction);
    }
    public class Rotator : IRotator
    {
        private static int  _layerMask = 1 << LayerMask.NameToLayer("TransparentFX");

        private readonly TerrainData _terrainData;
        public event Action<float> OnFaceDirection;
        protected readonly Transform _transform;
        protected readonly IStat _rotationSpeed;
        protected readonly Quaternion _startRotation;
        protected float _currentVelocity;
        protected bool _isAffectedByTerrainFace;
        protected bool _lockRotation;
        protected Vector3 _direction;
        protected float _counter;
        protected float _lerpDuration;
        protected AnimationCurve _curve;
        protected int _rotationCounter;
        public Rotator(Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation, bool isAffectedByTerrainFace)
        {
            _lockRotation = toLockRotation;
            _rotationSpeed = rotationSpeed;
            _transform = transform;
            _startRotation = startRotation;
            _counter = 0;
            _lerpDuration = rotationSpeed.Value;
            _curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
            _isAffectedByTerrainFace = isAffectedByTerrainFace;

            Terrain terrain = Terrain.activeTerrain;
            _terrainData = terrain.terrainData;
        }
        public Rotator(Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation, AnimationCurve rotationCurve, bool isAffectedByTerrainFace )
        {
            _lockRotation = toLockRotation;
            _rotationSpeed = rotationSpeed;
            _transform = transform;
            _startRotation = startRotation;
            _counter = 0;
            _lerpDuration = rotationSpeed.Value;
            _curve = rotationCurve;
            _isAffectedByTerrainFace = isAffectedByTerrainFace;

            Terrain terrain = Terrain.activeTerrain;
            _terrainData = terrain.terrainData;
        }

        public Vector3 NewDirection => _direction;
        public float RotationSpeed => _rotationSpeed.Value;
        public Vector3 CurrentFacingDirection => _transform.forward;
        public bool StopRotation { get => _lockRotation; set => _lockRotation = value; }
        public void AssignRotation(Vector3 direction)
        {
            _direction = direction;
            //Debug.LogError(direction);
            _direction.Normalize();
        }

        public void Tick()
        {
            if (!CanRotate())
                return;
            else if (NewDirection.magnitude > 0)
                RotateTowards();
            else
                ResetLerp();
        }
        protected virtual void RotateTowards()
        {
            //if (_rotationCounter % 3 == 0)
            //{
            //    Debug.Log("rotation");
            //    float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            //    float currentYRotation = _transform.eulerAngles.y;

            //    _counter += Time.deltaTime;


            //    float relativeAngle = targetAngle + currentYRotation;
            //    float rotationLerpValue = _curve.Evaluate((_counter * RotationSpeed) / 100f);
            //    //  Debug.Log(rotationLerpValue);
            //    //float smoothAngle = Mathf.SmoothDampAngle(currentYRotation, relativeAngle, ref _currentVelocity, rotationLerpValue);
            //    Quaternion lerpDirection = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, relativeAngle, 0), rotationLerpValue);
            //    SetRotation(lerpDirection);

            //    OnFaceDirection?.Invoke(relativeAngle);
            //}
            //Debug.Log("not rotation");
            //_rotationCounter++;

            float counterClamped = Mathf.Min(_counter, GameManager.Instance.maxRotationSpeed);
            Debug.Log("Counter: " + counterClamped);

            float curve = _curve.Evaluate(counterClamped / 100f);
            Debug.Log("curve: " + curve);

            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Debug.Log("targetAngle: " + targetAngle);

            var yRotation = _transform.eulerAngles.y;
            Debug.Log("yRotation: " + yRotation);

            var relativeAngle = yRotation + targetAngle;
            Debug.Log("relativeAngle: " + relativeAngle);




            var rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, relativeAngle, 0), Time.deltaTime * RotationSpeed * curve);
            Debug.Log("rotation: " + rotation);


            SetRotation(rotation);
            OnFaceDirection?.Invoke(relativeAngle);

            _counter += Time.deltaTime;
        }
        protected virtual bool CanRotate()
        {
            return (!StopRotation);
        }

        public virtual void SetRotation(Quaternion rotation) 
        { 
            _transform.rotation = rotation;

            if (!_isAffectedByTerrainFace)
                return;


            //RaycastHit hit;
            //var rei = new Ray(_transform.position + Vector3.up*5f, Vector3.down);


           
            //if (!Physics.Raycast(rei, out hit, 1000f, _layerMask))
            //{
            //    Debug.Log("Not hitting anything!");
            //    return;
            //}

            //if (hit.collider == null)
            //    return;

            //// Assuming you have already done a raycast and have hitInfo with the hitNormal
            //Vector3 hitNormal = hit.normal;

            //// Get the rotation that aligns the forward axis of your object with the hit normal
            //Quaternion relativeRotationToGround = Quaternion.FromToRotation(_transform.up, hitNormal);

            //// Apply the rotation to your object's transform
            //_transform.rotation = relativeRotationToGround;

        }
        public void Lock() => _lockRotation = true;
        public void UnLock() => _lockRotation = false;
        public void ResetLerp() => _counter = 0;
        public Quaternion StartingRotation() => _startRotation;
    }
}

public static class VectorHelper
{
    public static bool IsNan(this Vector3 vector3)
        => vector3.x.IsNan() || vector3.y.IsNan() || vector3.z.IsNan();

        public static bool IsNan(this float v) => float.IsNaN(v);
}