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
        private bool _isPlayer;
        [SerializeField]
        private Transform _visualTransform;
        [SerializeField]
        private AnimatorController _animatorController;
        [SerializeField]
        private ShaderView _shaderView;
        public IAnimatorController AnimatorController => _animatorController;

        public Transform VisualTransform { get => _visualTransform; }

        public void Init(IEntity entity)
        {
            _animatorController.Init(entity);
            if (!_isPlayer)
                _shaderView.Init();

        }

        private void LateUpdate()
        {
            _visualTransform.transform.localPosition = Vector3.zero;
        }
        private void OnEnable()
        {
            _shaderView.IgnoreResetEffect = false;
            _shaderView.DeActivate();
        }
        private void OnDestroy()
        {
            if (!_isPlayer)
                _shaderView?.Dispose();
        }

#if UNITY_EDITOR
        [ContextMenu("Assign Data")]
        private void Assign()
        {
            _shaderView?.Assign(this);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
    [Serializable]
    public class ShaderView : IDisposable
    {


        [SerializeField]
        private GlowingShaderSO _glowingShaderSO;
        [SerializeField]
        private List<Renderer> _meshRenderers;
        [SerializeField]
        private TargetedBehaviour _targetedBehaviour;

        public bool IgnoreResetEffect { get; set; } = false;

        private List<Material> _glowingShadersMaterial = new List<Material>();
        public void Init()
        {
            for (int i = 0; i < _meshRenderers.Count; i++)
            {
                var current = _meshRenderers[i];
                var mats = current.materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].HasProperty(GlowingShaderSO.SHADER_ACTIVATION_REFERENCE))
                        _glowingShadersMaterial.Add(mats[j]);
                }
            }
            _targetedBehaviour.OnEaten += ActivateRedEffect;
            _targetedBehaviour.OnUnTargeted += DeActivate;
            _targetedBehaviour.OnCurrentlyTargeted += ActivateWhiteEffect;
        }
        public void ActivateRedEffect()
        {
            IgnoreResetEffect = true;
          _glowingShaderSO.ApplyEffect(_glowingShadersMaterial, _glowingShaderSO.RedEffect);
        }

        public void ActivateWhiteEffect()
       => _glowingShaderSO.ApplyEffect(_glowingShadersMaterial, _glowingShaderSO.WhiteEffect);


        public void DeActivate()
        {
            if(!IgnoreResetEffect)
            _glowingShaderSO?.RemoveEffect(_glowingShadersMaterial);
        }


        public void Dispose()
        {
            DeActivate();
            _targetedBehaviour.OnEaten -= ActivateRedEffect;
            _targetedBehaviour.OnUnTargeted -= DeActivate;
            _targetedBehaviour.OnCurrentlyTargeted -= ActivateWhiteEffect;
        }
#if UNITY_EDITOR
        public void Assign(VisualHandler visualHandler)
        {
            var parent = visualHandler.transform.parent;
            _targetedBehaviour = parent.GetComponent<TargetedBehaviour>();
            _meshRenderers = new List<Renderer>();
            foreach (var renderer in parent.GetComponentsInChildren<Renderer>())
            {
                _meshRenderers.Add(renderer);
            }
        }
#endif

    }


}