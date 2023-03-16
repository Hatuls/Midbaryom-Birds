using Midbaryom.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarcussEatingVisualController : MonoBehaviour
{
    [SerializeField]
    private Spawner _spawner;


    [SerializeField]
    private UnityEvent OnStartEatingCarcuss,OnFinishedEating;
    private IPlayer _player;
    private IState _eatState;
    private IEnumerator Start()
    {
        yield return null;
        _player = _spawner.Player.Transform.GetComponent<IPlayer>();
        _eatState =  _player.StateMachine.StateDictionary[StateType.Eat];
        _eatState.OnStateEnterEvent += ApplyEffect;
        _eatState.OnStateExitEvent += ReturnToNormal;
    }

    private void ApplyEffect()
    {
        OnStartEatingCarcuss?.Invoke();
    }

    private void ReturnToNormal()
    {
        OnFinishedEating?.Invoke();
    }
    private void OnDestroy()
    {
        _eatState.OnStateEnterEvent -= ApplyEffect;
        _eatState.OnStateExitEvent  -= ReturnToNormal;
    }
}
