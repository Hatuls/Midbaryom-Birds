using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public enum HeightType
    {
        Ground,
        Animal,
        Player,
    }

    public interface IHeightHandler : IUpdateable
    {
        void ChangeState(HeightType heightType);
        HeightConfigSO HeightConfigSO { get; }
    }
    public class HeightHandler : IHeightHandler
    {
        protected readonly Locomotion _locomotion;
        protected readonly Dictionary<HeightType, HeightState> _states;
        private HeightType _currentHeight;
        private float _currentFloat;
        public HeightConfigSO HeightConfigSO { get; }
        public HeightHandler(Locomotion locomotion, Transform transform, HeightType startingState, IPlayer player) : this(locomotion, startingState) // player
        {
            _states = new Dictionary<HeightType, HeightState>()
            {
                {HeightType.Animal,new HuntHeightState(transform, HeightConfigSO.AnimalHeight,player) },
                {HeightType.Ground,new HeightState(transform, HeightConfigSO.GroundHeight) },
                {HeightType.Player,new HeightState(transform, HeightConfigSO.PlayerHeight) },
            };
            SetStartPosition(transform, HeightConfigSO);
            RegisterToEvents();

        }

        public HeightHandler(Locomotion locomotion, Transform transform, HeightType startingState) : this(locomotion, startingState) // animal 
        {
            _states = new Dictionary<HeightType, HeightState>()
            {
                {HeightType.Animal,new HeightState(transform, HeightConfigSO.AnimalHeight) },
                {HeightType.Ground,new HeightState(transform, HeightConfigSO.GroundHeight) },
                {HeightType.Player,new HeightState(transform, HeightConfigSO.PlayerHeight) },
            };

            SetStartPosition(transform, HeightConfigSO);
            RegisterToEvents();
        }

        private void RegisterToEvents()
        {
            _locomotion.OnHeightRequested += GetHeightValue;

            foreach (var item in _states)
                item.Value.OnHeightNeeded += SetHeightValue;
        }

        private HeightHandler(Locomotion locomotion, HeightType startingState)
        {
            _locomotion = locomotion;
            _currentHeight = startingState;

            HeightConfigSO = GameManager.Instance.HeightConfigSO;
        }
        void SetStartPosition(Transform transform, HeightConfigSO heightConfigSO)
        {
            Vector3 v = transform.position;
            switch (_currentHeight)
            {
                case HeightType.Ground:
                    v.y = heightConfigSO.GroundHeight.Height;
                    break;
                case HeightType.Animal:
                    v.y = heightConfigSO.AnimalHeight.Height;
                    break;
                case HeightType.Player:
                    v.y = heightConfigSO.PlayerHeight.Height;
                    break;
                default:
                    break;
            }
            transform.position = v;
        }

        ~HeightHandler()
        {
            foreach (var item in _states)
                item.Value.OnHeightNeeded -= SetHeightValue;
            _locomotion.OnHeightRequested -= GetHeightValue;
        }
        public IState CurrentState => GetState(_currentHeight);

        private IState GetState(HeightType heightType)
        {
            if (_states.TryGetValue(heightType, out HeightState state))
                return state;

            throw new System.Exception($"HeightHandler: State was not found: {heightType}");
        }
        private void SetHeightValue(float val) => _currentFloat = val;
        private float GetHeightValue() => _currentFloat;
        public void ChangeState(HeightType heightType)
        {
            if (heightType == _currentHeight)
                return;
            CurrentState.OnStateExit();
            _currentHeight = heightType;
            CurrentState.OnStateEnter();
        }

        public void Tick()
   => CurrentState.OnStateTick();
    }



    public class HeightState : IState
    {
        public event Action<float> OnHeightNeeded;
        public event Action OnStateEnterEvent;
        public event Action OnStateExitEvent;
        public event Action OnStateTickEvent;

        protected readonly Transform Transform;
        protected readonly HeightTransition HeightTransition;
        protected float _counter;
        public HeightState(Transform transform, HeightTransition heightTransition)
        {
            Transform = transform;
            HeightTransition = heightTransition;
        }
        protected float GetHeightValue() => HeightTransition.Evaluate(_counter);
        public virtual void OnStateEnter()
        {
            _counter = 0;
            OnStateEnterEvent?.Invoke();
        }

        public virtual void OnStateExit()
        {
            OnStateExitEvent?.Invoke();
        }


        public virtual void OnStateTick()
        {
            float currentHeight = Transform.position.y;
            float remainDistance = HeightTransition.Height - currentHeight;

            _counter += Time.deltaTime;
            currentHeight += GetHeightValue() * remainDistance;
            InvokeHeightNeeded(currentHeight);
            OnStateTickEvent?.Invoke();
        }

        protected void InvokeHeightNeeded(float val) => OnHeightNeeded?.Invoke(val);
    }

    public class HuntHeightState : HeightState
    {
        private readonly IStat _movementSpeedStat;
        private readonly AimAssists aimAssists;
        private float _time;
        public HuntHeightState(Transform transform, HeightTransition heightTransition, IPlayer player) : base(transform, heightTransition)
        {
            this.aimAssists = player.AimAssists;
            _movementSpeedStat = player.Entity.StatHandler[StatType.DiveYSpeed];
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();


            if (aimAssists.HasTarget)
                CalculateTime();
        }


        public override void OnStateTick()
        {
            float currentHeight = Transform.position.y;
            if (aimAssists.HasTarget)
            {
                float remainDistance = aimAssists.Target.CurrentPosition.y - currentHeight; // maybe addOffset?
                _counter += Time.deltaTime;


                currentHeight += HeightTransition.Evaluate(_counter, _time) * remainDistance;
#if UNITY_EDITOR
               // Debug.Log($"Current Height: {Transform.position.y}\nHas Target: {aimAssists.HasTarget}\nTarget's Y: {aimAssists.Target.CurrentPosition.y}\nRemain Height: {aimAssists.Target.CurrentPosition.y - currentHeight}\nNext Height: { currentHeight}");
#endif
            }
            else
                CalculateTime();
        
            InvokeHeightNeeded(currentHeight);
        }

        private void CalculateTime()
        {
            if (aimAssists.HasTarget == false)
                return;
            float currentHeight = Transform.position.y;
            float remainDistance = aimAssists.Target.CurrentPosition.y - currentHeight;
            _time = Mathf.Abs(remainDistance) / _movementSpeedStat.Value;
            Debug.LogWarning(System.DateTime.Now.ToString() + "" + $"My Height: {currentHeight}\nDistance: {remainDistance}\nSpeed: {_movementSpeedStat.Value}\nEstimate Time: {_time}");
        }
    }
}