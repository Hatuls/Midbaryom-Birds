using Midbaryom.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    [SerializeField]
    private Entity _entity;
    private IStateMachine _stateMachine;
    private AIBrain _aIBrain;
    public IEnumerable<IUpdateable> Updateables
    {
        get
        {
            yield return _stateMachine;
            yield return _aIBrain;
        }
    }
    private void Start()
    {
        BaseState[] AIStates = new BaseState[]
        {
            new AIIdleState(_entity),
            new AIMoveState(_entity),
        };
        _stateMachine = new StateMachine(StateType.Idle, AIStates);
        _aIBrain = new AIBrain(_stateMachine);
    }
    private void Update()
    {
        foreach (var item in Updateables)
            item.Tick();
    }
}
public class AIBrain : IUpdateable
{
    private readonly IStateMachine _stateMachine;
    private readonly Vector2 _durationToMove = new Vector2(1f,2f);
    private readonly Vector2 _durationToIdle = new Vector2(2f,5f);
    private float _counter;
    private float _duration;
    public AIBrain(IStateMachine state)
    {
        _stateMachine = state;
        Reset();
    }

    private void Reset()
    {
        _counter = 0;

        var currentState = _stateMachine.CurrentStateType;
        if (currentState == StateType.Idle)
            _duration = RND(_durationToMove);
        else if(currentState == StateType.Run)
            _duration = RND(_durationToIdle);
    }
    public void Tick()
    {
        _counter += Time.deltaTime;
        if (_counter >= _duration)
            ChangeState();
    }

    private void ChangeState()
    {
        var currentState = _stateMachine.CurrentStateType;
        if (currentState == StateType.Idle)
            _stateMachine.ChangeState(StateType.Run);
        else if (currentState == StateType.Run)
            _stateMachine.ChangeState(StateType.Idle);

        Reset();
    }

    private float RND(Vector2 v)
        => UnityEngine.Random.Range(v.x, v.y);
}