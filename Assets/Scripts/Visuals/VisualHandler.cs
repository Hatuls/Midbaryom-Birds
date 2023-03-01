using Midbaryom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Visual
{
    public interface IVisualHandler
    {
        IAnimatorController AnimatorController { get; }
        Transform VisualTransform { get; }
        void Init(IEntity entity);
    }

    public class VisualHandler : MonoBehaviour, IVisualHandler
    {
        [SerializeField]
        private Transform _visualTransform;
        [SerializeField]
        private AnimatorController _animatorController;
        [SerializeField]
        private EatenEffect _eatenEffect;
        public IAnimatorController AnimatorController => _animatorController;

        public Transform VisualTransform { get => _visualTransform; }

        public void Init(IEntity entity)
        {
            _animatorController.Init(entity);
            _eatenEffect.Init();
        }

        private void LateUpdate()
        {
            _visualTransform.transform.localPosition = Vector3.zero;
        }
        private void OnEnable()
        {
            _eatenEffect?.DeActivate();
        }
        private void OnDestroy()
        {
            _eatenEffect?.Dispose();
        }

        [ContextMenu("Assign Data")]
        private void Assign()
        {
            _eatenEffect?.Assign(this);
        }
    }
    [Serializable]
    public class EatenEffect : IDisposable
    {
        private const string PROPERTY_NAME = "IsEaten";
        [SerializeField]
        private Renderer[] _meshRenderers;
        [SerializeField]
        private TargetedBehaviour _targetedBehaviour;

        private List<Material> _redEffectMaterials = new List<Material>();
        public void Init()
        {
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                var current = _meshRenderers[i];
                var mats = current.materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].HasProperty(PROPERTY_NAME))
                        _redEffectMaterials.Add(mats[j]);
                }
            }
            _targetedBehaviour.OnTargeted += Activate;
        }
        public void Activate()
        => SetEatenProperty(1);




        public void DeActivate()
        => SetEatenProperty(0);


        private void SetEatenProperty(int val)
        {
            if(_redEffectMaterials.Count >0)
            for (int i = 0; i < _redEffectMaterials.Count; i++)
            {
                _redEffectMaterials[i].SetFloat(PROPERTY_NAME, val);
            }
        }

        public void Dispose()
        {
            DeActivate();
            _targetedBehaviour.OnTargeted -= Activate;
        }

        public void Assign(VisualHandler visualHandler)
        {
            var parent = visualHandler.transform.parent;
            _targetedBehaviour = parent.GetComponent<TargetedBehaviour>();
            _meshRenderers = parent.GetComponentsInChildren<Renderer>();
        }
    }



    [Serializable]
    public class AnimatorController : IAnimatorController, IBehaviour
    {

        [SerializeField]
        private Animator _animator;
        public Animator Animator => _animator;
        private IRotator _rotator;
        private ILocomotion _locomotion;
        private IStat _movmenetSpeed;

        internal void Init(IEntity entity)
        {
            _movmenetSpeed = entity.StatHandler[StatType.MovementSpeed];
            _locomotion = entity.MovementHandler;
            _rotator = entity.Rotator;
            _rotator.OnFaceDirection += AssignRotation;
            _locomotion.OnMove += AssignMovementSpeed;
            entity.DestroyHandler.AddBehaviour(this);
        }

        private void AssignRotation(float angle)
        {
            // Animator.SetFloat(RotationHash, -angle);
        }

        private void AssignMovementSpeed(float speed)
        {
            //float startSpeed = _movmenetSpeed.StartValue;
            //speed /= startSpeed * 2;
            //Animator.SetFloat("Forward", speed);

        }

        public void ApplyBehaviour()
        {
            _rotator.OnFaceDirection -= AssignRotation;
            _locomotion.OnMove -= AssignMovementSpeed;
        }
    }


}