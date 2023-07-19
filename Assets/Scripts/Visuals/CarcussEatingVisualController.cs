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
    private EntityTagSO _deadRabbit;
    [SerializeField]
    private float _zoomedInFOV;
    private float _defaultZoomedInFOV;
    [SerializeField]
    private Camera _humanEyeCamera;
    [SerializeField]
    private Camera _zoomedCamera;
    [SerializeField]
    private UnityEvent OnStartEatingCarcuss,OnFinishedEating;
    private IPlayer _player;
    //private IState _eatState;
    [SerializeField]
    private FloatTransitionEffect _startEffect,_endEffect;

    //private int _defaultCullingMaskBit;

    private IEnumerator Start()
    {
        //_defaultCullingMaskBit = _humanEyeCamera.cullingMask;
        _defaultZoomedInFOV = _zoomedCamera.fieldOfView;
        yield return null;
        _player = _spawner.Player;
        IReadOnlyDictionary<StateType, IState> statesMachine = _player.StateMachine.StateDictionary;
        var diveState = statesMachine[StateType.Dive] as PlayerDiveState;
        var recoverState = statesMachine[StateType.Recover];
        recoverState.OnStateEnterEvent += ReturnToNormal;
        diveState.OnStateEnterEvent += ApplyEffectOnlyOnDive;
        diveState.OnCloseToTarget += DisablePossProcessing;
        //_eatState = statesMachine[StateType.Eat];
        // _eatState.OnStateEnterEvent += ApplyEffect;
        //   _eatState.OnStateExitEvent += ReturnToNormal;
    }

    private void ApplyEffect()
    {
        StartCoroutine(FOVTransitionByHeight(_startEffect));
       // _zoomedCamera.fieldOfView = _zoomedInFOV;
       // OnStartEatingCarcuss?.Invoke();
    }
    private IEnumerator FOVTransition(BaseTransitionEffect<float> effect)
    {
        float duration = effect.Duration;
        AnimationCurve curve = effect.Curve;
        float counter = 0;

        while (counter < duration)
        {
            yield return null;
            counter += Time.deltaTime;
            float result = Mathf.Lerp(effect.StartResult, effect.EndResult, curve.Evaluate(counter / duration)
                );
            _zoomedCamera.fieldOfView = result;
        }
    }
    private IEnumerator FOVTransitionByHeight(BaseTransitionEffect<float> effect)
    {
        float duration = effect.Duration;
        AnimationCurve curve = effect.Curve;
  
        
            IEntity player = _player.Entity;
            IEntity target = _player.TargetHandler.Target;
        float startingHeight = player.CurrentPosition.y;



        while (Vector3.Distance(target.CurrentPosition, player.CurrentPosition) >= PlayerDiveState.minDistanceToStartCatchingAnimation)
        {
            yield return null;
            float x = target.CurrentPosition.y;
            float SHDistance = Mathf.Abs(startingHeight - x);
            float CurrentDistance = Mathf.Abs(x - player.CurrentPosition.y);
            float result = Mathf.Lerp(effect.StartResult, effect.EndResult,
                curve.Evaluate(1 - Mathf.Clamp01((CurrentDistance - PlayerDiveState.minDistanceToStartCatchingAnimation ) / SHDistance ))//  target.CurrentPosition.y / player.CurrentPosition.y)
                );
       //    Debug.Log($"Target's Height {x}\n starting Height {startingHeight}\nStarting distance {SHDistance}\nCurrent Distance from target{CurrentDistance}\n evaluation {1 - Mathf.Clamp01((CurrentDistance - PlayerDiveState.minDistanceToStartCatchingAnimation )/ SHDistance )}");
            _zoomedCamera.fieldOfView = result;
        }
        //    Debug.Log("Finished Diving");
    }
    private void EnablePossProcessing() => OnStartEatingCarcuss?.Invoke();
    private void DisablePossProcessing() => OnFinishedEating?.Invoke();
    private void ReturnToNormal()
    {
        StartCoroutine(FOVTransition(_endEffect));
        EnablePossProcessing();
        ResetCameraMask();
    }

    private void ResetCameraMask()
    {
        //_humanEyeCamera.cullingMask = _defaultCullingMaskBit;
    }

    private void OnDestroy()
    {
     // _eatState.OnStateEnterEvent -= ApplyEffect;
        //_eatState.OnStateExitEvent  -= ReturnToNormal;
        IReadOnlyDictionary<StateType, IState> statesMachine = _player.StateMachine.StateDictionary;
        var diveState = statesMachine[StateType.Dive] as PlayerDiveState;
        var recoverState = statesMachine[StateType.Recover];
        recoverState.OnStateEnterEvent -= ReturnToNormal;
        diveState.OnStateEnterEvent      -= ApplyEffectOnlyOnDive;
        diveState.OnCloseToTarget -= DisablePossProcessing;
    }

    private void ApplyEffectOnlyOnDive()
    {
        // Un mask
        MaskHumanEye();

        ApplyEffect();
    }

    private void MaskHumanEye()
    {
        //_humanEyeCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Animal"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Debug.Log(_humanEyeCamera.cullingMask);
    }
}
