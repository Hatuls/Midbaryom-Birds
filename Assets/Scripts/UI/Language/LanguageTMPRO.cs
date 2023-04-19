using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageTMPRO : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]
    private Text _text;
    private void Awake()
    {
        if (ApplicationManager.Instance == null)
            gameObject.SetActive(false);

        var currentLangauge = ApplicationManager.Instance?.LanguageSettings?.LanguageType ?? LanguageType.English;
        ChangeAllignments(currentLangauge);
    }

    private void ChangeAllignments(LanguageType currentLangauge)
    {
        if (_textMeshProUGUI == null)
            return;

        switch (currentLangauge)
        {
            case LanguageType.Hebrew:
            case LanguageType.Arabic:
                //_textMeshProUGUI.alignment = TextAlignmentOptions.Midline;
                _textMeshProUGUI.isRightToLeftText = true;
                break;
            case LanguageType.English:
                _textMeshProUGUI.isRightToLeftText = false;
                //_textMeshProUGUI.alignment= TextAlignmentOptions.Midline;
                break;
            default:
                break;
        }
    }

    public void SetText(int index)
    {
        string text = ApplicationManager.Instance?.LanguageSettings?.GetText(index);
        if (_textMeshProUGUI != null)
            _textMeshProUGUI.text = text;
        else if(_text != null)
        {
            string result = text;

         var language =    ApplicationManager.Instance?.LanguageSettings?.LanguageType ?? LanguageType.English;
            if (language != LanguageType.English)
                result = Reverse(text);
            _text.text = result;
        }
    }

    private string Reverse(string text)
    {
        string s = string.Empty;
        foreach (var letter in text.Reverse())
            s = string.Concat(s,letter);
        return s;
    }
}
