using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextReverser : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    [SerializeField]
    private string _Text;
    [SerializeField]
    private bool _toReverse;

    private void OnValidate()
    {
        string result = string.Empty;
        if (_toReverse)
        {

            foreach (char letter in _Text.Reverse())
                result += letter;
        }
        else
            result = _Text;

        _text.text = result;
    }
}
