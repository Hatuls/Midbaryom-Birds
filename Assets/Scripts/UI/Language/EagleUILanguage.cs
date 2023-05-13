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
    }


#if UNITY_EDITOR
    [SerializeField]
    private EagleTypeSO _eagle;
    [ContextMenu("Test Eagle Texts")]
    private void ShowTexts() => ShowTexts(_eagle);
#endif
}
