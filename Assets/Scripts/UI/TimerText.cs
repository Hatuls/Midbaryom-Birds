using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private const string Format = "00:00";
    [SerializeField]
    private TextMeshProUGUI _text;

    float _counter = 0;

    private void Start()
    {
        _counter = 0;   
    }

    void Update()
    {
        _counter += Time.deltaTime;
        SetText(_counter);
    }

    private void SetText(float time)
    {
        int roundedTime = Mathf.RoundToInt(time);
        _text.text =roundedTime.ToString(Format);
    }
}
