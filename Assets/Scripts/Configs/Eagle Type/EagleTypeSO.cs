using Midbaryom.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName ="ScriptableObjects/Eagles/New Eagle Data")]
public class EagleTypeSO : ScriptableObject
{
    [SerializeField]
    private string _eagleName;
    [SerializeField]
    private Sprite _eagleImage;
    [SerializeField]
    private DietData[] _diets;


    public Sprite Image => _eagleImage;
    public string Name => _eagleName;

    public IReadOnlyList<DietData> DietDatas => _diets;
    public float GetPrecentage(TagSO tagSO)
    {
        if(tagSO == null || _diets == null || _diets.Length == 0)
        {
            Debug.LogError("Initalization was not set correct");
            return -1;
        }

        int length = _diets.Length;
        for (int i = 0; i < length; i++)
        {
            if (_diets[i].DietType == tagSO)
                return _diets[i].Precentage;
        }

        Debug.LogWarning("Type was not found?");
        return -1;
    }
}

[System.Serializable]
public class DietData
{
    public TagSO DietType;
    public float Precentage;
}
