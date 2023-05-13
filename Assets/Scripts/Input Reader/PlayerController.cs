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
        private readonly BodyTrackingConfigSO _bodyTrackingConfigSO;


        private BirdInputAction _birdInputAction;
        private IInputHandler<Vector2> _movementInputHandler;
        private InputAction _huntInputAction;
        private InputAction _movementInputAction;
        private Vector3 _previousInput;
        private float _counter;
        public bool LockInputs { get; set; }
        public bool LockHuntInput { get; set; }
        public bool CanCancelHunt { get; set; }
        public PlayerController(Player player, IEntity entity, BodyTrackingConfigSO bodyTrackingConfigSO)
        {
            _player = player;
            _entity = entity;
            _birdInputAction = new BirdInputAction();

            _movementInputAction = _birdInputAction.Player.Move;
            LockInputs = false;
            _huntInputAction = _birdInputAction.Player.Hunt;
            _huntInputAction.started += StartHundDown;
            _huntInputAction.canceled += EndHunt;
            foreach (var input in InputActions)
                input.Enable();

            ResetToDefault();
            _bodyTrackingConfigSO = bodyTrackingConfigSO;
        }

        private void StartHundDown(InputAction.CallbackContext obj)
        {
            if(!LockHuntInput)
            HuntDown();
        }

        public void HuntDown()
        {
            if (_player.AimAssists.HasTarget)
                _player.StateMachine.ChangeState(StateType.Dive);
        }

        private void EndHunt(InputAction.CallbackContext obj)
        {
            if(CanCancelHunt)
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
            if (LockInputs == false && _bodyTrackingConfigSO.InputModeType == BodyTrackingConfigSO.InputMode.Keyboard)
                Rotation();
        }



        private void Rotation()
        {
            Vector2 value = _movementInputAction.ReadValue<Vector2>();
            _movementInputHandler.Handle(value);
        }
        public void Rotate(Vector3 direction)
        {
            if (LockInputs)
                return;

            _movementInputHandler.Handle(direction);
        }
        public void SetInputBehaviour(IInputHandler<Vector2> inputBehaviour)
        {
            _movementInputHandler = inputBehaviour;
        }
        public void ResetToDefault() => SetInputBehaviour(new RegularInputs(_entity));

        ~PlayerController()
        {
            foreach (var input in InputActions)
                input.Disable();
        }

    }

}

public interface IInputHandler<T>
{
    void Handle(T input);
}

public class RegularInputs : IInputHandler<Vector2>
{
    private readonly IEntity _entity;
    public RegularInputs(IEntity entity)
    {
        _entity = entity;
    }
    public void Handle(Vector2 input)
    {
        Vector3 result = new Vector3(input.x, 0, 0);

        _entity.Rotator.AssignRotation(result);
    }
}
public class OnlyRightInputEnabled : IInputHandler<Vector2>
{
    public event Action OnRight;
    private readonly IEntity _entity;

    public OnlyRightInputEnabled(IEntity entity)
    {
        _entity = entity;
    }

    public void Handle(Vector2 input)
    {
        Vector3 result = new Vector3(input.x, 0, 0);
        bool _isLeft = input.x >= 0;
        if (_isLeft)
        {
            _entity.Rotator.AssignRotation(result);
            OnRight?.Invoke();
        }
    }
}
public class OnlyLeftInputEnabled : IInputHandler<Vector2>
{
    public event Action OnLeft;
    private readonly IEntity _entity;

    public OnlyLeftInputEnabled(IEntity entity)
    {
        _entity = entity;
    }

    public void Handle(Vector2 input)
    {
        Vector3 result = new Vector3(input.x, 0, 0);
        bool _isLeft = input.x <= 0;

        if (_isLeft)
            _entity.Rotator.AssignRotation(result);
        if (input.x < 0)
            OnLeft?.Invoke();
        
    }
}

public class NoInputBehaviour : IInputHandler<Vector2>
{

    public void Handle(Vector2 input)
    {
      

    }
}