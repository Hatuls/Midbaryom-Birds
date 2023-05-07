using Midbaryom.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BirdEyeTutorial : BaseEyeTutorial
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private AnimationCurve _curve;

    [SerializeField]
    private float _duration;
    [SerializeField]
    private float _startingFOV = 60f;
    [SerializeField]
    private float _endingFOV = 10f;

    [SerializeField]
    private GameObject[] _objectsToClose;
    [SerializeField]
    private GameObject[] _objectsToOpen;
    [SerializeField]
    private GameObject[] _objectsToOpenAtStart;
    [SerializeField]
    private GameObject[] _objectsToCloseAtStart;
    [SerializeField]
    private Player _player;


    [SerializeField]
    private AnimationCurve _innerEyeCurve;

    [SerializeField]
    private float _innerEyeFadeInDuration;

    [SerializeField]
    private CanvasGroup _innerBirdEye;

    [SerializeField]
    private Image _innerCircle;


    protected override void SetVisuals()
    {
        StartCoroutine(CameraTransition());
        Array.ForEach(_objectsToCloseAtStart, x => x.SetActive(false));
        Array.ForEach(_objectsToOpenAtStart, x => x.SetActive(true));
        _player.PlayerController.SetInputBehaviour(new NoInputBehaviour());
        base.SetVisuals();
    }

    private IEnumerator CameraTransition()
    {
        float counter = 0;
        _camera.usePhysicalProperties = false;
        while (counter <_duration)
        {
            _camera.fieldOfView = Mathf.Lerp(_startingFOV, _endingFOV, _curve.Evaluate(counter / _duration));
            yield return null;
            counter += Time.deltaTime;
        }
        _camera.usePhysicalProperties = true;
        _camera.fieldOfView = _endingFOV;
    }

    protected override void RemoveVisuals()
    {
        Array.ForEach(_objectsToClose, x => x.SetActive(false));
        StartCoroutine(InnerEyeTranstiion());
        Array.ForEach(_objectsToOpen, x => x.SetActive(true));
        base.RemoveVisuals();
    }

    private IEnumerator InnerEyeTranstiion()
    {
        _innerBirdEye.alpha = 0;
        var imageColor = _innerCircle.color;
        imageColor.a = 0;
        _innerCircle.color = imageColor;
        float counter = 0;


        while (counter< _innerEyeFadeInDuration)
        {
            yield return null;
            counter += Time.deltaTime;
            _innerBirdEye.alpha = _innerEyeCurve.Evaluate(counter / _innerEyeFadeInDuration);
            imageColor.a = _innerEyeCurve.Evaluate(counter / _innerEyeFadeInDuration);
            _innerCircle.color = imageColor;
        }
        imageColor.a = 1;
        _innerBirdEye.alpha = 1;
        _innerCircle.color = imageColor;
    }
}