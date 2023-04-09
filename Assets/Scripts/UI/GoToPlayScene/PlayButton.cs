using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private ScreenTransitioner _screenTransitioner;
    private int _nextScene = 2; // game scene
    public void Play()
    {
        _screenTransitioner.StartExit(_nextScene);
    }
}
