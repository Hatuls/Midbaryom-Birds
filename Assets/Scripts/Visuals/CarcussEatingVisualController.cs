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
        recoverState.OnStateEnterEvent += () => StartCoroutine(ReturnToNormal());
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
    private IEnumerator ReturnToNormal()
    {
        //all hard coded = problem!


        yield return new WaitForSeconds(4);
        EnablePossProcessing();
        ResetCameraMask();

        GameManager.Instance.eagleAnimator.SetTrigger("ReturnToBaseState");

        yield return new WaitForEndOfFrame();
        GameManager.Instance.eagleAnimator.ResetTrigger("ReturnToBaseState");

        int LayerIgnore2 = LayerMask.NameToLayer("Bird Body");
        var newMask2 = GameManager.Instance.zoomCam.cullingMask & ~(1 << LayerIgnore2);
        GameManager.Instance.zoomCam.cullingMask = newMask2;

        int LayerIgnore3 = LayerMask.NameToLayer("Animal");
        var newMask3 = GameManager.Instance.zoomCam.cullingMask & ~(1 << LayerIgnore3);
        GameManager.Instance.zoomCam.cullingMask = newMask3;

        yield return (StartCoroutine(FOVTransition(_endEffect)));

        int LayerStopIgnore = LayerMask.NameToLayer("Animal");
        var newMask = GameManager.Instance.mainCam.cullingMask | (1 << LayerStopIgnore);
        GameManager.Instance.mainCam.cullingMask = newMask;

        int LayerStopIgnore2 = LayerMask.NameToLayer("Bird Body");
        var newMask4 = GameManager.Instance.zoomCam.cullingMask | (1 << LayerStopIgnore2);
        GameManager.Instance.zoomCam.cullingMask = newMask4;

        int LayerStopIgnore3 = LayerMask.NameToLayer("Animal");
        var newMask5 = GameManager.Instance.zoomCam.cullingMask | (1 << LayerStopIgnore3);
        GameManager.Instance.zoomCam.cullingMask = newMask5;


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
        recoverState.OnStateEnterEvent -= () => StartCoroutine(ReturnToNormal());
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
