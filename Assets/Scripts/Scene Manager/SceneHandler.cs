using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneHandler
{
    event Action OnSceneLoaded;
    event Action OnSceneUnloaded;
    event Action OnSceneHandlerDestroyed;

    void LoadSceneAdditive(int sceneIndex, bool unloadPrevious);
    void LoadSceneSingle(int sceneIndex);
}

public class SceneHandler : MonoBehaviour, ISceneHandler
{
    /// <summary>
    /// Both single and additive
    /// </summary>
    public event Action OnSceneLoaded;
    public event Action OnSceneUnloaded;
    public event Action OnSceneHandlerDestroyed;

    private const float SCENE_LOADED_OFFSET = 0.9f;

    protected HashSet<int> _activeScenes = new HashSet<int>();
    protected Coroutine _coroutine;
    private int _previousSceneBuildIndex = -1;


    public int[] GetActiveScenes()
    {
        int[] _copyScene = new int[_activeScenes.Count];
        int _counter = -1;

        foreach (int activeScene in _activeScenes)
        {
            _counter++;
            _copyScene[_counter] = activeScene;
        }

        return _copyScene;
    }


    public void LoadSceneSingle(int sceneIndex)
    {
        StartCoroutine(LoadSceneSingleCoroutine(sceneIndex));
    }
    public void LoadSceneAdditive(int sceneIndex)
    {
        LoadSceneAdditive(sceneIndex, false);
    }
    public void LoadSceneAdditive(int sceneIndex, bool unloadPrevious)
    {
        if (_coroutine != null)
            return;

        _coroutine = StartCoroutine(LoadSceneAdditiveCoroutine(sceneIndex, unloadPrevious));
    }

    public void UnloadScene(int sceneIndex)
    {
        if (_activeScenes.Contains(sceneIndex))
            StartCoroutine(UnLoadScene(sceneIndex));

    }

    #region Loading Coroutines
    protected IEnumerator LoadSceneSingleCoroutine(int nextSceneIndex)
    {
        OnSceneUnloaded?.Invoke();
        OnSceneLoaded?.Invoke();
        yield return SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Single);
    }
    protected IEnumerator LoadSceneAdditiveCoroutine(int nextSceneIndex, bool unloadPrevious)
    {

        if (unloadPrevious)
        {
            yield return UnLoadScene(_previousSceneBuildIndex);
            yield return null;
        }

        yield return LoadSceneAdditiveCoroutine(nextSceneIndex);

        _activeScenes.Add(nextSceneIndex);
        _previousSceneBuildIndex = nextSceneIndex;
        _coroutine = null;
    }
    protected IEnumerator LoadSceneAdditiveCoroutine(int addedScene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(addedScene, LoadSceneMode.Additive);

        asyncOperation.allowSceneActivation = false;

        do
        {
            yield return null;

            if (asyncOperation.progress >= SCENE_LOADED_OFFSET)
                asyncOperation.allowSceneActivation = true;
        }
        while (!asyncOperation.isDone);

        yield return null;


        OnSceneLoaded?.Invoke();
    }
    protected IEnumerator UnLoadScene(int index)
    {
        //Notifying relevant listeners that scene will be Unloaded
        OnSceneUnloaded?.Invoke();

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(index);

        do
        {
            yield return null;
        }
        while (!asyncOperation.isDone);

        _activeScenes.Remove(index);

      yield return Resources.UnloadUnusedAssets();
    }
    #endregion


    #region MonoBehaviour

    protected virtual void OnDestroy()
    {
        if (OnSceneHandlerDestroyed != null)
            OnSceneHandlerDestroyed.Invoke();
    }
    #endregion

}
