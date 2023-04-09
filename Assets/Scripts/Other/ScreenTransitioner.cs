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
    private BlackScreenEffect _exitEffect;
    [SerializeField]
    private BlackScreenEffect _entranceEffect;


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
        _img.gameObject.SetActive(true);
        StartCoroutine(EffectCoroutine(_exitEffect,FinishExit));

        void FinishExit()
        {
            _sceneHandler.LoadSceneAdditive(nextScene, true);
        }
    }
    private IEnumerator EffectCoroutine(BlackScreenEffect effect, Action OnComplete = null)
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
public class BlackScreenEffect
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