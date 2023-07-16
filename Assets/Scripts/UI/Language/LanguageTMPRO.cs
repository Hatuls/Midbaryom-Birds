using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageTMPRO : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]
    protected Text _text;
    protected virtual void Awake()
    {
        if (ApplicationManager.Instance == null)
            gameObject.SetActive(false);

        var currentLangauge = ApplicationManager.Instance?.LanguageSettings?.LanguageType ?? LanguageType.English;
        ChangeAllignments(currentLangauge);
    }

    protected void ChangeAllignments(LanguageType currentLangauge)
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

    public virtual void SetText(int index)
    {
        string text = ApplicationManager.Instance?.LanguageSettings?.GetText(index);
        if (_textMeshProUGUI != null)
            _textMeshProUGUI.text = text;
        else if(_text != null)
        {
            string result = text;
         var language =    ApplicationManager.Instance?.LanguageSettings?.LanguageType ?? LanguageType.English;
            _text.lineSpacing = language == LanguageType.English ? 1 : -1;
            if (language != LanguageType.English)
            {
                result = ReverseLetters(text);
            }
            _text.text = result;
        }
    }

    private string ReverseWordsOrder(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        string[] s = text.Split(' ');
        string result = string.Empty;
        const string space = " ";
        for (int i = s.Length - 1; i >= 0; i--)
        {
            result = string.Concat(result, space, s[i]);
        }
        return result;
    }

    protected string ReverseLetters(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        string s = string.Empty;
        foreach (var letter in text.Reverse())
            s = string.Concat(s,letter);

        return s;
    }
}
