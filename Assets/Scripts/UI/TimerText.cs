using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private const string FORMAT = "00:00";
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private float _sessionTime =90f;
    float _counter = 0;

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
        int roundedTime = Mathf.RoundToInt(time);
        int clampedTime = Mathf.Max(roundedTime, 0);
        _text.text = clampedTime.ToString(FORMAT);
    }
}
