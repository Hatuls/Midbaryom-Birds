using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenTransitioner : MonoBehaviour
{
    public UnityEvent OnSceneEnter;

    private SceneHandler _sceneHandler;
    [SerializeField]
    private Image _img;

    [SerializeField]
    private TransitionEffect _exitEffect;
    [SerializeField]
    private TransitionEffect _entranceEffect;


    private void Awake()
    {
        _sceneHandler = FindObjectOfType<SceneHandler>();
        StartEnterance();
    }

    private void StartEnterance()
    {
        _img.gameObject.SetActive(true);
        StartCoroutine(EffectCoroutine(_entranceEffect, FinishEntrance));

        void FinishEntrance()
        {
            OnSceneEnter?.Invoke();
            _img.gameObject.SetActive(false);
        }
    }
    public void StartExit(int nextScene)
    {
        if(nextScene == 2)
        {
            //Do reset 1 hour counter here
            PCRestarter.instance.ResetResetPCTimer();
        }

        _img.gameObject.SetActive(true);
        StartCoroutine(EffectCoroutine(_exitEffect, () => StartCoroutine(FinishExit())));

        IEnumerator FinishExit()
        {
            yield return new WaitForSeconds(4);
            _sceneHandler.LoadSceneAdditive(nextScene, true);
        }
    }
    private IEnumerator EffectCoroutine(TransitionEffect effect, Action OnComplete = null)
    {
        float duration = effect.Duration;
        var curve = effect.Curve;
        float counter = 0;
        _img.color = effect.StartColor;
        do
        {
            yield return null;
            counter += Time.deltaTime;
            float precentage = counter / duration;

            _img.color = Color.Lerp(effect.StartColor, effect.EndColor, curve.Evaluate(precentage));

        } while (counter < duration);

        _img.color = effect.EndColor;
        yield return null;
        OnComplete?.Invoke();
    }
}
[Serializable]
public class TransitionEffect
{
    [SerializeField]
    private Color _startColor;
    [SerializeField]
    private Color _endColor;

    [SerializeField]
    private float _duration;
    [SerializeField]
    private AnimationCurve _curve;
    public float Duration => _duration;
    public AnimationCurve Curve => _curve;

    public Color EndColor { get => _endColor;}
    public Color StartColor { get => _startColor; }
}
[Serializable]
public class FloatTransitionEffect : BaseTransitionEffect<float>
{
    [SerializeField]
    private float _startValue;
    [SerializeField]
    private float _endValue;

    public override float StartResult => _startValue;
    public override float EndResult => _endValue;
}
[Serializable]
public abstract class BaseTransitionEffect<T>
{
    [SerializeField]
    private float _duration;
    [SerializeField]
    private AnimationCurve _curve;

    public abstract T StartResult { get; }
    public abstract T EndResult { get; }
    public float Duration => _duration;
    public AnimationCurve Curve => _curve;
}