using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum sounds
{
    StartOfGame,
    WindForToutorial,
    MoveRightTutorial,
    MoveLeftTutorial,
    tslila,
    StartFlyingInGame,
    MoveRightInGame,
    MoveLeftInGame,
    Grab,
    GrabDeadAnimal,
    EndGameUIAppear,
    LockingOnPrey

}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public List<AudioSource> allAudioSources;

    Dictionary<sounds, AudioSource> audioSources;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        audioSources = new Dictionary<sounds, AudioSource>();

        for (int i = 0; i < System.Enum.GetValues(typeof(sounds)).Length; i++)
        {
            audioSources.Add((sounds)i, allAudioSources[i]);
        }
    }

    public void CallPlaySound(sounds sound)
    {
        StartCoroutine(PlaySound(sound));
    }
    private IEnumerator PlaySound(sounds sound)
    {
        if (audioSources[sound].gameObject.activeInHierarchy) yield break;

        audioSources[sound].gameObject.SetActive(true);

        yield return new WaitForSeconds(audioSources[sound].clip.length);

        if(!audioSources[sound].loop)
        {
            audioSources[sound].gameObject.SetActive(false);
        }
    }

    public void StopSound(sounds sound)
    {
        audioSources[sound].gameObject.SetActive(false);
    }

    public void StopAllSounds()
    {
        foreach (var pair in audioSources)
        {
            pair.Value.gameObject.SetActive(false);
        }
    }

    public bool isSoundPlaying(sounds sound)
    {
        return audioSources[sound].gameObject.activeInHierarchy;
    }
}
