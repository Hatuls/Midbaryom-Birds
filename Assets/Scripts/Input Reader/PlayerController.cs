using Midbaryom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Midbaryom.Inputs
{

    public class PlayerController : IUpdateable
    {
        public event Action<Vector2> OnMove;
        private readonly IEntity _entity;
        private readonly Player _player;



        private BirdInputAction _birdInputAction;

        private InputAction _huntInputAction;
        private InputAction _movementInputAction;
        private Vector3 _previousInput;
        private float _counter;

        public PlayerController(Player player,IEntity entity)
        {
            _player = player;
            _entity = entity;
            _birdInputAction = new BirdInputAction();

            _movementInputAction = _birdInputAction.Player.Move;

            _huntInputAction = _birdInputAction.Player.Hunt;
            _huntInputAction.started += StartHundDown;
            _huntInputAction.canceled += EndHunt;
            foreach (var input in InputActions)
                input.Enable();
        }

        private void StartHundDown(InputAction.CallbackContext obj)
        {
            if(_player.AimAssists.HasTarget)
            _player.StateMachine.ChangeState(StateType.Dive);
        }

        private void EndHunt(InputAction.CallbackContext obj)
        {
            _player.StateMachine.ChangeState(StateType.Recover);
        }

        public IEnumerable<InputAction> InputActions
        {
            get
            {
                yield return _movementInputAction;
                yield return _huntInputAction;
            }
        }



        public void Tick()
        {
            Rotation();
        }



        private void Rotation()
        {
            Vector2 value = _movementInputAction.ReadValue<Vector2>();
            Vector3 result = new Vector3(value.x, 0, value.y);

            _entity.Rotator.AssignRotation(result);
        }




        ~PlayerController()
        {
            foreach (var input in InputActions)
                input.Disable();
        }

    }

}