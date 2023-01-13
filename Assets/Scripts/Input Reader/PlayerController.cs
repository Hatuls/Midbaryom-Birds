using Midbaryom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Midbaryom.Inputs
{

    public class PlayerController : MonoBehaviour
    {
        public event Action<Vector2> OnMove;
        private IEntity _entity;



        private BirdInputAction _birdInputAction;
        private InputAction _huntInputAction;
        private InputAction _movementInputAction;





        public IEnumerable<InputAction> InputActions
        {
            get
            {
                yield return _movementInputAction;
                yield return _huntInputAction;
            }
        }
        private void Awake()
        {
            _entity = GetComponent<IEntity>();
            _birdInputAction = new BirdInputAction();
        }

        private void OnEnable()
        {
            _movementInputAction = _birdInputAction.Player.Move;
     
            _huntInputAction = _birdInputAction.Player.Hunt;

            foreach (var input in InputActions)
                input.Enable();
        }

        private void OnDisable()
        {
            foreach (var input in InputActions)
                input.Disable();
        }


        private void Update()
        {
            Vector2 value = _movementInputAction.ReadValue<Vector2>();
            _entity.Rotator.AssignRotation(new Vector3(value.x, 0, value.y));
        }

    }

}