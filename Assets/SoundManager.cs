using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum sounds
{
    StartOfGame,
    WindForToutorial,
    MoveRightTutorial,
    MoveLeftTutorial,
    Nesika,
    StartFlyingInGame,
    MoveRightInGame,
    MoveLeftInGame,
    Grab,
    EndGameUIAppear

}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<AudioSource> allAudioSources;

    Dictionary<sounds, AudioSource> audioSources;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(sounds)).Length; i++)
        {
            audioSources.Add((sounds)i, allAudioSources[i]);
        }
    }

    public IEnumerator PlaySound(sounds sound)
    {
        audioSources[sound].gameObject.SetActive(true);

        yield return new WaitForSeconds(audioSources[sound].clip.length);

        audioSources[sound].gameObject.SetActive(false);
    }
}
