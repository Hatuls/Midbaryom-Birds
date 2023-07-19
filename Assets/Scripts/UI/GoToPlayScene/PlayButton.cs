using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitioner _screenTransitioner;
    private int _nextScene = 2; // game scene



    private void Start()
    {
        Screen.SetResolution(ApplicationManager.SCREEN_RESOLUTION_WIDTH, ApplicationManager.SCREEN_RESOLUTION_HEIGHT, true);
    }
    public void Play()
    {
        _screenTransitioner.StartExit(_nextScene);
    }
}
