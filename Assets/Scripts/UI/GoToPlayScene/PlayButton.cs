using Midbaryom.Core;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitioner _screenTransitioner;

    [SerializeField] private GameObject _accessibleStation;
    [SerializeField] private GameObject _regularStation;

    private int _nextScene = 2; // game scene



    private void Start()
    {
        Screen.SetResolution(ApplicationManager.SCREEN_RESOLUTION_WIDTH, ApplicationManager.SCREEN_RESOLUTION_HEIGHT, true);
        if(ApplicationManager.Instance.IsAccessibleStation)
        {
            _accessibleStation.SetActive(true);
            _regularStation.SetActive(false);
        }

        else
        {
            _accessibleStation.SetActive(false);
            _regularStation.SetActive(true);
        }

    }
    public void Play()
    {
        _screenTransitioner.StartExit(_nextScene);
    }
}
