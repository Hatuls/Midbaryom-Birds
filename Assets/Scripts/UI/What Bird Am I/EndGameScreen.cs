using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    const int LANGUAGE_BUILD_INDEX = 1;
    const int GAME_BUILD_INDEX = 2;

    private SceneHandler _sceneHandler;
    private void Awake()
    {
        _sceneHandler = FindObjectOfType<SceneHandler>();
    }
    public void SelectLanguage() => MoveToScene(LANGUAGE_BUILD_INDEX);
    public void PlayAgain() => MoveToScene(GAME_BUILD_INDEX);

    private void MoveToScene(int index)
    {
        _sceneHandler.LoadSceneAdditive(index, true);
    }
}