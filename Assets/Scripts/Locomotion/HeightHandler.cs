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
        void SetState(HeightType heightType);
    }
    public class HeightHandler : IHeightHandler
    {
        private readonly Locomotion _locomotion;
        private readonly Dictionary<HeightType, IState> _states;
        private HeightType _currentHeight;
        private float _currentFloat;

        public HeightHandler(Locomotion locomotion,Transform transform,HeightType startingState)
        {
            _locomotion = locomotion;
            _currentHeight = startingState;

            HeightConfigSO heightConfigSO = GameManager.Instance.HeightConfigSO;

            _states = new Dictionary<HeightType, IState>()
            {
                {HeightType.Animal,new HeightState(transform, heightConfigSO.AnimalHeight) },
                {HeightType.Ground,new HeightState(transform, heightConfigSO.GroundHeight) },
                {HeightType.Player,new HeightState(transform, heightConfigSO.PlayerHeight) },
            };
            SetStartPosition(transform, heightConfigSO);

            _locomotion.OnHeightRequested += GetHeightValue;
            HeightState.OnHeightNeeded += SetHeightValue;
           
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
        }


        ~HeightHandler()
        {
            HeightState.OnHeightNeeded -= SetHeightValue;
            _locomotion.OnHeightRequested -= GetHeightValue;
        }
        public IState CurrentState => GetState(_currentHeight);

        private IState GetState(HeightType heightType)
        {
            if (_states.TryGetValue(heightType, out IState state))
                return state;

            throw new System.Exception($"HeightHandler: State was not found: {heightType}");
        }
        private void SetHeightValue(float val) => _currentFloat = val;
        private float GetHeightValue() => _currentFloat;
        public void SetState(HeightType heightType)
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
        public static event Action<float> OnHeightNeeded;
        protected readonly Transform Transform;
        protected readonly HeightTransition HeightTransition;
        protected static float _counter;
        public HeightState(Transform transform,HeightTransition heightTransition)
        {
            Transform = transform;
            HeightTransition = heightTransition;
        }
        protected float GetHeightValue() => HeightTransition.Evaluate(_counter);
        public void OnStateEnter()
        {
            _counter = 0;
        }

        public void OnStateExit()
        {

        }
 

        public void OnStateTick()
        {
            float currentHeight = Transform.position.y;
            float remainDistance = HeightTransition.Height - currentHeight;

            currentHeight += GetHeightValue() * remainDistance;
            OnHeightNeeded?.Invoke(currentHeight);
            _counter += Time.deltaTime;
        }
    }


}