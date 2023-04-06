using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;
    private const int FIRST_SCENE_INDEX = 1;
    [SerializeField]
    private SceneHandler _sceneHandler;
    private LanguageType _languageType;
    public static ApplicationManager Instance => _instance;

    public LanguageType LanguageType => _languageType;

    private void OnEnable()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Awake()
    {
        _instance = this;
        _languageType = LanguageType.Hebrew;
        _sceneHandler.LoadSceneAdditive(FIRST_SCENE_INDEX);
    }
    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetLanguage(LanguageType language)
    {
        _languageType = language;
    }
}
public enum LanguageType
{
Hebrew=0,
English=1,
Arabic=2,
}