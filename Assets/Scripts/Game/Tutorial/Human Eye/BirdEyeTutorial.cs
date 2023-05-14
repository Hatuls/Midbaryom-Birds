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
    private RawImage _tutorialBirdEyeImage;
    [SerializeField]
    private float _birdEyeFadeOutTransition=1f;
    [SerializeField]
    private AnimationCurve _birdEyeFadeOutCurve;

    [SerializeField]
    private float _birdEyeFadeInTransition = 1f;
    [SerializeField]
    private AnimationCurve _birdEyeFadeInCurve;

    [SerializeField]
    private Image _innerCircle;

    [SerializeField]
    private Spawner _spawner;
    [SerializeField]
    private EntityTagSO _entityTagSO;

    [SerializeField]
    private float _delayBeforeFirstFade;

    [SerializeField]
    private float _delayBetweenFades;
    protected override void SetVisuals()
    {
        StartCoroutine(CameraTransition());
        Array.ForEach(_objectsToCloseAtStart, x => x.SetActive(false));
        Array.ForEach(_objectsToOpenAtStart, x => x.SetActive(true));
        _player.PlayerController.SetInputBehaviour(new NoInputBehaviour());

        SpawnRabbit();
        base.SetVisuals();
    }

    private void SpawnRabbit()
    {
   
        //   _player.Entity.Rotator.AssignRotation(Vector3.zero);
        Vector3 dir = _player.AimAssists.FacingDirection;
        Ray rei = new Ray(_player.Entity.CurrentPosition, dir);
        Physics.Raycast(rei, out RaycastHit hit);
        _spawner.SpawnEntity(_entityTagSO, hit.point);
    }

    private IEnumerator CameraTransition()
    {
     //  yield return CameraBlendTransition(); 
        yield return BirdFadeInTransition();
  //      yield return new WaitForSeconds(_delayBeforeFirstFade);
  //   //   yield return BirdEyeFadeOutTransition();
  //    //  yield return new WaitForSeconds(_delayBetweenFades);
    }
    private IEnumerator CameraBlendTransition()
    {
        float counter = 0;
        var c = _tutorialBirdEyeImage.color;
        _camera.usePhysicalProperties = false;
        while (counter < _duration)
        {
            yield return null;
            counter += Time.deltaTime;

            c.a = _birdEyeFadeOutCurve.Evaluate(counter / _birdEyeFadeOutTransition);
            _tutorialBirdEyeImage.color = c;


            _camera.fieldOfView = Mathf.Lerp(_startingFOV, _endingFOV, _curve.Evaluate(counter / _duration));
        }
        _camera.usePhysicalProperties = true;
        _camera.fieldOfView = _endingFOV;
    }
    private IEnumerator BirdFadeInTransition()
    {
        float counter = 0;
        var c = _tutorialBirdEyeImage.color;
        while (counter < _birdEyeFadeInTransition)
        {
            yield return null;

            counter += Time.deltaTime;
            c.a = _birdEyeFadeInCurve.Evaluate(counter / _birdEyeFadeInTransition);
            _tutorialBirdEyeImage.color = c;
        }
    }
    private IEnumerator BirdEyeFadeOutTransition()
    {
        float counter = 0;
        var c = _tutorialBirdEyeImage.color;
        while (counter < _birdEyeFadeOutTransition)
        {
            yield return null;
            counter += Time.deltaTime;
            c.a = _birdEyeFadeOutCurve.Evaluate(counter / _birdEyeFadeOutTransition);
            _tutorialBirdEyeImage.color = c;
        }
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