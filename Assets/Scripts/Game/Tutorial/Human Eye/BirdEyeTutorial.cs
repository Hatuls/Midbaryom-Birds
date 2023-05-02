using System;
using System.Collections;
using UnityEngine;

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


    protected override void SetVisuals()
    {
        StartCoroutine(CameraTransition());
        Array.ForEach(_objectsToOpenAtStart, x => x.SetActive(true));
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
        Array.ForEach(_objectsToOpen, x => x.SetActive(true));
        base.RemoveVisuals();
    }
}