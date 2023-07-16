using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Timer/New Game Session Config")]
public class GameSessionConfig : ScriptableObject
{
    public float SessionTime = 90f;

    public bool SkipTutorial;
}
