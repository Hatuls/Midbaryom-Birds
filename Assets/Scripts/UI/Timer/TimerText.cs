using Midbaryom.Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    public event Action OnTimeEnded;

    private const string FORMAT = "{0:00}:{1:00}";
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private GameSessionConfig _sessionTime ;
    float _counter = 0;

    [SerializeField]
    private Image _img;
    [SerializeField]
    private bool isTimeDepleted;
    private void Awake()
    {
        _panel.SetActive(false);
        gameObject.SetActive(false);
        GameManager.OnGameStarted += StartTimer;
    }
    private void OnDestroy()
    {

        GameManager.OnGameStarted -= StartTimer;
    }
    private void StartTimer()
    {
        _panel.SetActive(true);
        gameObject.SetActive(true);
        ResetTimer();
        StartCoroutine(CountDown());
    }

    private void ResetTimer()
    {
        isTimeDepleted = false;
        _counter = _sessionTime.SessionTime;
        SetText(_counter);
    }

    private IEnumerator CountDown()
    {
   
        do
        {
            yield return null;
            _counter -= Time.deltaTime;
            _img.fillAmount = 1 - (_counter / _sessionTime.SessionTime);
            SetText(_counter);
        } while (_counter > 0);
    }

    private void SetText(float time)
    {
        int roundedTime = Mathf.FloorToInt(time);
        int clampedTime = Mathf.Max(roundedTime, 0);


        int minutes = clampedTime / 60;
        int seconds = clampedTime % 60;

        _text.text = string.Format(FORMAT, minutes, seconds);

        //Time finished
        if (!isTimeDepleted && clampedTime == 0)
        {
            isTimeDepleted = true;
            OnTimeEnded?.Invoke();

            SoundManager.Instance.StopAllSounds();
            SoundManager.Instance.CallPlaySound(sounds.EndGameUIAppear);
        }

    }
}
