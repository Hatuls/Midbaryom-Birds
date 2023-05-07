public class LanguageTMPROStaticText : LanguageTMPRO
{
    public LanguageType _languageType;
    public int _textIndex;
    public bool UseCurrentSelectedLanguage;
    protected override void Awake()
    {
        SetText(_textIndex);
  

    }
    
    public override void SetText(int index)
    {
        if (UseCurrentSelectedLanguage && ApplicationManager.Instance != null && ApplicationManager.Instance.LanguageSettings != null)
            _languageType = ApplicationManager.Instance.LanguageSettings.LanguageType;
        ChangeAllignments(_languageType);
        string text = ApplicationManager.Instance?.LanguageSettings?.GetText(index,_languageType);
        if (_textMeshProUGUI != null)
            _textMeshProUGUI.text = text;
        else if (_text != null)
        {
            string result = text;

         
            if (_languageType != LanguageType.English)
                result = Reverse(text);
            _text.text = result;
        }
    }
}