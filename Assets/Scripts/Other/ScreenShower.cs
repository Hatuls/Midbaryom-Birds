using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenShower : MonoBehaviour
{


    private TextMeshProUGUI _Text;


    private void Start()
    {
        _Text = GetComponent<TextMeshProUGUI>();
        _Text.text = "Screen: " + Screen.width + " X " + Screen.height;
    }
}
