using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    private const int FIRST_SCENE_INDEX = 1;
    [SerializeField]
    private SceneHandler _sceneHandler;

    private void OnEnable()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Awake()
    {
        _sceneHandler.LoadSceneAdditive(FIRST_SCENE_INDEX);
    }

}
