using UnityEngine;

public class EagleUILanguage : MonoBehaviour
{
    [SerializeField]
    private LanguageTMPRO _eagleInfo;
    [SerializeField]
    private LanguageTMPRO _eagleName;
    [SerializeField]
    private LanguageTMPRO _huntedLike;
    [SerializeField]
    private LanguageTMPRO eagleEats;

    [SerializeField]
    private ScoreAnalyzer _scoreAnalyzer;

    private void Awake()
    {
        _scoreAnalyzer.OnEagleResultFound += ShowTexts;
    }
    private void OnDestroy()
    {
        _scoreAnalyzer.OnEagleResultFound -= ShowTexts;
        
    }
    private void ShowTexts(EagleTypeSO obj)
    {
        const int HUNTED_LIKE_TEXT = 18;
        _huntedLike.SetText(HUNTED_LIKE_TEXT);
        _eagleInfo.SetText(obj.InfoIndex);
        _eagleName.SetText(obj.EagleNameIndex);


        if(obj._eagleName == "House sparrow")
        {
            eagleEats.gameObject.SetActive(false);
        }
        else
        {
            eagleEats.gameObject.SetActive(true);

            string text = ApplicationManager.Instance?.LanguageSettings?.GetText(obj.EagleNameIndex);
            switch (ApplicationManager.Instance.LanguageSettings.LanguageType)
            {
                case LanguageType.Hebrew:
                    eagleEats.rtlText.text = text + "  אוכל";
                    break;
                case LanguageType.English:
                    eagleEats.rtlText.text = text + "  feeds on";
                    break;
                case LanguageType.Arabic:
                    eagleEats.rtlText.text = text + "  يأكل";
                    break;
                default:
                    break;
            }

            eagleEats.ChangeFont(ApplicationManager.Instance.LanguageSettings.LanguageType, eagleEats.rtlText, null);
        }
    }


#if UNITY_EDITOR
    [SerializeField]
    private EagleTypeSO _eagle;
    [ContextMenu("Test Eagle Texts")]
    private void ShowTexts() => ShowTexts(_eagle);
#endif
}
