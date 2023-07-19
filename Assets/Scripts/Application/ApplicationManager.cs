using System.Collections;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;

    public const int SCREEN_RESOLUTION_WIDTH = 1920;
    public const int SCREEN_RESOLUTION_HEIGHT = 1080;
    [SerializeField]
    private LanguageBank _languageBank;


    public static ApplicationManager Instance => _instance;
    public LangaugeSettings LanguageSettings { get; private set; }
    [SerializeField]
    private void OnEnable()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        _instance = this;
        LanguageSettings = new LangaugeSettings(_languageBank);
        Screen.SetResolution(SCREEN_RESOLUTION_WIDTH, SCREEN_RESOLUTION_HEIGHT, true);
        PlayerPrefs.Save();
    }
    private IEnumerator Start()
    {
        yield return null;
        Screen.SetResolution(SCREEN_RESOLUTION_WIDTH, SCREEN_RESOLUTION_HEIGHT, true);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetLanguage(LanguageType language)
        => LanguageSettings.SetLanguage(language);
}
public enum LanguageType
{
    Hebrew = 0,
    English = 1,
    Arabic = 2,
}
public class LangaugeSettings
{
    public LanguageBank LanguageBank { get; private set; }
    public LanguageType LanguageType { get; private set; }

    public LangaugeSettings(LanguageBank languageBank)
    {
        LanguageBank = languageBank;
    }
    public void SetLanguage(LanguageType language)
    {
        LanguageType = language;
    }
    public string GetText(int index)
    {
        try
        {
            return LanguageBank.GetText(LanguageType, index);

        }
        catch (System.Exception e)
        {

            throw new System.Exception($"Language index was not found\nKey: {LanguageType}\nValue: {index}\n{e.Message}");
        }
    }
    public string GetText(int index, LanguageType languageType)
    {
        try
        {
            return LanguageBank.GetText(languageType, index);

        }
        catch (System.Exception e)
        {

            throw new System.Exception($"Language index was not found\nKey: {LanguageType}\nValue: {index}\n{e.Message}");
        }
    }
}