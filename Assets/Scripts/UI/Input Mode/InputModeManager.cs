using System;
using System.Management;
using System.Threading.Tasks;
using UnityEngine;
using static BodyTrackingConfigSO;

public class InputModeManager : MonoBehaviour
{
    private const int FIRST_SCENE_INDEX = 1;

    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private BodyTrackingConfigSO _bodyTrackingConfigSO;

    [SerializeField]
    private ZEDManager _zedManager;
    [SerializeField]
    private SceneHandler _sceneHandler;
    void Start()
    {
        _canvas.SetActive(true);
        _camera.gameObject.SetActive(true);
    }


    public  void SetCameraInputMode()
    {

       // if ()
       try {

            _canvas.SetActive(false);
            _bodyTrackingConfigSO.SetInput(InputMode.Camera);
            _camera.gameObject.SetActive(false);
            _zedManager = Instantiate(_zedManager);
            _zedManager.OnBodyTrackingInitialized += LoadNextScene;
            Screen.SetResolution(ApplicationManager.SCREEN_RESOLUTION, ApplicationManager.SCREEN_RESOLUTION, true);
        } catch (Exception e)
        {
            throw e;
        }

      

    }
    public void SetKeyboardInputMode()
    {
        _bodyTrackingConfigSO.SetInput(InputMode.Keyboard);
        _canvas.SetActive(false);
        _zedManager = null;
        _camera.gameObject.SetActive(false);
        LoadNextScene();
    }

    private void OnDestroy()
    {
        if (_zedManager != null)
            _zedManager.OnBodyTrackingInitialized += LoadNextScene;
    }

    private void LoadNextScene()
    {
        _sceneHandler.LoadSceneAdditive(FIRST_SCENE_INDEX);
        Screen.SetResolution(ApplicationManager.SCREEN_RESOLUTION, ApplicationManager.SCREEN_RESOLUTION, true);
    }

 
}