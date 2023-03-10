using UnityEngine;

namespace Midbaryom.Core
{
    public abstract class BirdStateMachine : MonoBehaviour, IUpdateable
    {
        [SerializeField]
        protected Player _player;

        protected StateMachine _playerStateMachine;

        public IStateMachine StateMachine => _playerStateMachine;

        public abstract void InitStateMachine();

        public virtual void Tick()
        {
            _playerStateMachine.Tick();
        }
    }




   
}