using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField]
    private LanguageType _languageType;
    public void Click()
    {
        ApplicationManager.Instance.SetLanguage(_languageType);

        SoundManager.Instance.CallPlaySound(sounds.StartOfGame);
    }
}
