using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;
    private const int FIRST_SCENE_INDEX = 1;
    private const int SCREEN_RESOLUTION = 1376;
    [SerializeField]
    private LanguageBank _languageBank;
    [SerializeField]
    private SceneHandler _sceneHandler;
    public static ApplicationManager Instance => _instance;
    public LangaugeSettings LanguageSettings { get; private set; }

    private void OnEnable()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Awake()
    {
        Screen.SetResolution(SCREEN_RESOLUTION, SCREEN_RESOLUTION, true);
        _instance = this;
        LanguageSettings = new LangaugeSettings(_languageBank);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
    }
    private void Start()
    {
        _sceneHandler.LoadSceneAdditive(FIRST_SCENE_INDEX);
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
        => LanguageBank.GetText(LanguageType, index);
}