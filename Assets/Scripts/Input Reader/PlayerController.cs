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
            _player.HuntDown();
        }

        private void EndHunt(InputAction.CallbackContext obj)
        {
            _player.HuntUp();
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
            _entity.Rotator.AssignRotation(new Vector3(value.x, 0, value.y));
        }


        ~PlayerController()
        {
            foreach (var input in InputActions)
                input.Disable();
        }

    }

}