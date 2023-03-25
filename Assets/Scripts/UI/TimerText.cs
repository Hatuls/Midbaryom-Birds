using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    public event Action OnTimeEnded;

    private const string FORMAT = "{0:00}:{1:00}";
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private float _sessionTime =90f;
    float _counter = 0;

    [SerializeField]
    private bool isTimeDepleted;
    private void Start()
    {
        _counter = _sessionTime;   
    }

    void LateUpdate()
    {
        _counter -= Time.deltaTime;
        SetText(_counter);
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
        }
        
    }
}
