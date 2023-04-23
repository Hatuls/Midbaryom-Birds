public class LanguageTMPROStaticText : LanguageTMPRO
{
    public LanguageType _languageType;
    public int _textIndex;

    private void Awake()
    {
        SetText(_textIndex);
    }
    
    public override void SetText(int index)
    {
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