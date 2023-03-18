using UnityEngine;

public class PlayButton : MonoBehaviour
{
    SceneHandler _sceneHandler;
    void Start()
    {
        _sceneHandler = FindObjectOfType<SceneHandler>();
    }

    public void Play()
    {
        _sceneHandler.LoadSceneAdditive(2, true);

    }
}
