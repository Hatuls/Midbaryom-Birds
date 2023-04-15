using System;
using TMPro;
using UnityEngine;
public class LanguageTMPRO : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI;
    private void Awake()
    {
        var currentLangauge = ApplicationManager.Instance?.LanguageSettings?.LanguageType ?? LanguageType.English;
        ChangeAllignments(currentLangauge);
    }

    private void ChangeAllignments(LanguageType currentLangauge)
    {
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
        _textMeshProUGUI.text = ApplicationManager.Instance.LanguageSettings.GetText(index);
    }
}
