using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitioner _screenTransitioner;
    private int _nextScene = 2; // game scene



    private void Start()
    {
        Screen.SetResolution(ApplicationManager.SCREEN_RESOLUTION, ApplicationManager.SCREEN_RESOLUTION, true);
    }
    public void Play()
    {
        _screenTransitioner.StartExit(_nextScene);
    }
}
