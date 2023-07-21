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


        private void Update()
        {
            Debug.Log(StateMachine.CurrentState.ToString());
        }
        public virtual void Tick()
        {
            _playerStateMachine.Tick();
        }
    }




   
}