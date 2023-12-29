using Midbaryom.Core;
using System;
using System.Collections.Generic;
using System.Threading;
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
        private EatenScaleEffect _eatenEffect;
        [SerializeField]
        private ShaderView _shaderView;
        public IAnimatorController AnimatorController => _animatorController;

        public Transform VisualTransform { get => _visualTransform; }

        public void Init(IEntity entity)
        {
            _animatorController.Init(entity);
            if (!_isPlayer)
            {
                _shaderView.Init();
                _eatenEffect.Init();
            }
        }
        private void Update()
        {
            if (_isPlayer)
            {
                return;
            }

            Terrain t = Terrain.activeTerrain;
            var worldPosition = transform.parent.position;
            var terrainPosition = worldPosition - t.transform.position;
            var td = t.terrainData;

            float x = (terrainPosition.x / td.size.x);
            float z = (terrainPosition.z / td.size.z);
            Vector3 normal = td.GetInterpolatedNormal(x, z);
            Vector3 f = Vector3.Cross(transform.parent.right, normal);

            var rot = Quaternion.LookRotation(f, normal);
            transform.rotation = rot;
        }


        private void LateUpdate()
        {
            _visualTransform.transform.localPosition = Vector3.zero;
        }
        private void OnEnable()
        {
            _shaderView.IgnoreResetEffect = false;
            _shaderView.DeActivate();
            _eatenEffect.Reset();
        }
        private void OnDestroy()
        {
            if (!_isPlayer)
            {
            _shaderView.Dispose();
            _eatenEffect.Dispose();
            }
    
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
            if (!IgnoreResetEffect)
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

    [Serializable]
    public class EatenScaleEffect : IDisposable
    {
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private TargetedBehaviour _targetBehaviour;
        [SerializeField]
        private AnimationCurve _scaleEase;
               [SerializeField]
        private float _duration = 2f;
        private CancellationTokenSource _cancellationTokenSource;

        private CancellationToken _cancellationToken;
        private bool _isFlag;
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _targetBehaviour.OnEaten -= ActivateRedEffect;
            _cancellationTokenSource.Dispose();
        }

        public void Init()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            _targetBehaviour.OnEaten += ActivateRedEffect;
        }
        public void Reset()
        {
                       _entity.Transform.localScale = Vector3.one;
            _isFlag = false;
        }
        private async void ActivateRedEffect()
        {
                      if (_isFlag)
                return;
            _isFlag = true;
  
            float _counter = 0;
            Transform transform = _entity.Transform;
            Vector3 scale = transform.localScale; 
            do
            {
                await System.Threading.Tasks.Task.Yield();
                if (_cancellationToken.IsCancellationRequested || transform == null)
                    return;

                _counter += Time.deltaTime;
                //transform.localScale = Vector3.Lerp(scale, Vector3.zero, _scaleEase.Evaluate( _counter / _duration));

            } while (_counter <= _duration);

        }
    }
}