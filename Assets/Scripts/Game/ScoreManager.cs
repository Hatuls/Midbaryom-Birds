using Midbaryom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<EntityTagSO, AnimalScore> _animalScore;

    [SerializeField]
    private EntityTagSO[] _gameEntites;


    public int TotalScoreAmount
    {

        get
        {
            int amount = 0;
            foreach (var animal in _animalScore.Values)
            {
                amount += animal.Amount;
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < _gameEntites.Length; i++)
            _animalScore.Add(_gameEntites[i], new AnimalScore());

    }


}


public class User
{
}
[Serializable]
public class AnimalScore
{
    private int _amount = 0;

    public int Amount { get => _amount; }

    public void AddOneToCount() => _amount++;
}
