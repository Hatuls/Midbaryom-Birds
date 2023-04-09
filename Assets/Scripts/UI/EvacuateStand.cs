using System;
using System.Collections;
using UnityEngine;

public class EvacuateStand : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitioner _screenTransitioner;
    [SerializeField]
    private LanguageTMPRO _popUp;

    [SerializeField]
    private int _evacuateTextIndex;
    [SerializeField]
    private float _popupEtranceDuration = .2f;
    [SerializeField]
    private AnimationCurve _curve;

    [SerializeField]
    private float _delayTillPopUp = 5f;
    [SerializeField]
    private float _delayTillReturnToLanguageScene = 4f;

    private void Start()
    {
        _popUp.gameObject.SetActive(false);
        StartCoroutine(EndGameScreen());
    }

    private IEnumerator EndGameScreen()
    {
        yield return new WaitForSeconds(_delayTillPopUp);
        yield return ShowPopUp();
      var  _sceneHandler = FindObjectOfType<SceneHandler>();
        yield return new WaitForSeconds(_delayTillReturnToLanguageScene);
        _screenTransitioner.StartExit(1);
    }
  
    private IEnumerator ShowPopUp()
    {
        var transform = _popUp.transform ;
        transform.localScale = Vector3.zero;
        _popUp.SetText(_evacuateTextIndex);
        _popUp.gameObject.SetActive(true);

        float counter = 0;
        do
        {
            yield return null;
            counter += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero,Vector3.one, _curve.Evaluate( counter /_popupEtranceDuration));
        } while (counter<=_popupEtranceDuration);

    }
}
