using Midbaryom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerScore : MonoBehaviour
{
    public static PlayerScore Instance { get; private set; } = null;
    private Dictionary<TagSO, AnimalScore> _animalScore;

    [SerializeField]
    private TagSO[] _gameEntites;

    public IReadOnlyDictionary<TagSO, AnimalScore> AnimalScoreDictionary => _animalScore;
 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);



    }
    private void Start()
    {
        var l = _gameEntites.ToList().ToArray();
        _gameEntites = l.ToArray();
        _animalScore = new Dictionary<TagSO, AnimalScore>();
        for (int i = 0; i < _gameEntites.Length; i++)
            _animalScore.Add(_gameEntites[i], new AnimalScore());

        ResetScores();
    }

   
    public void ResetScores()
    {
        AnimalScore.TotalAmount = 0;
        foreach (var animal in AnimalScoreDictionary.Values)
            animal.Reset();
    }

    public void Add(ITaggable taggable)
    {
        bool found = false;
        foreach (var tag in AnimalScoreDictionary)
        {
            if (taggable.ContainTag(tag.Key))
            {
                tag.Value.AddOneToCount();
                found = true;
                break;
            }
        }

        if (found)
            AnimalScore.TotalAmount++;
        else
            Debug.LogError("Tag was not found!");
    }
    public float[] GetKillFeedPrecentages()
    {
        float[] precentages = new float[AnimalScoreDictionary.Count];
        int i = -1;
        foreach (var tag in AnimalScoreDictionary.Values)
        {
            i++;
            precentages[i] = AnimalScore.TotalAmount ==0 ? 0 : tag.Precentage;
        }
        return precentages;
    }

}
[Serializable]
public class AnimalScore
{
    public static int TotalAmount =0;
    private int _amount = 0;

    public int Amount { get => _amount; }
    public float Precentage => Amount / TotalAmount;
    public void AddOneToCount() => _amount++;
    public void Reset() => _amount = 0;
}
