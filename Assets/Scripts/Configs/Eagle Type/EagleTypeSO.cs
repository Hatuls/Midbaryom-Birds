using Midbaryom.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Eagles/New Eagle Data")]
[Serializable]
public class EagleTypeSO : ScriptableObject, IComparable<EagleTypeSO>
{
    [SerializeField]
    public string _eagleName;
    [SerializeField]
    private Sprite _eagleImage;
    [SerializeField]
    private DietData[] _diets;

    [Tooltip("The index of the information that presetented about this eagle at the end screen")]
    public int InfoIndex;
    [Tooltip("The Eagle's name index\nwith this you can translate the name to the relevant language")]
    public int EagleNameIndex;

    public int Order;

    public Sprite Image => _eagleImage;
    public string Name => _eagleName;
    public IEnumerable<float> DietPrecentages
    {
        get
        {
            for (int i = 0; i < _diets.Length; i++)
                yield return _diets[i].Precentage;
        }
    }
    public IReadOnlyList<DietData> DietDatas => _diets;
    public float GetPrecentage(TagSO tagSO)
    {
        if (tagSO == null || _diets == null || _diets.Length == 0)
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

    public int CompareTo(EagleTypeSO other)
    {
        if (other.Order == Order)
            return 0;
        else if (Order > other.Order)
            return +1;
        else
            return -1;
    }
#if UNITY_EDITOR
    public void SetRawInfo(string name, Sprite img, int nameIndex,int infoIndex)
    {
        _eagleName = name;
        _eagleImage = img;
        EagleNameIndex = nameIndex;
        InfoIndex = infoIndex;
        if (_diets == null)
            _diets = new DietData[0];
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    public void Add(TagSO tagSO, float precentage)
    {
      List<DietData> dietDatas = _diets.ToList();
        var instance = new DietData() { DietType = tagSO, Precentage = precentage };
        dietDatas.Add(instance);
        _diets = dietDatas.ToArray();
    
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
}

[Serializable]
public class DietData
{
    [SerializeField]
    public TagSO DietType;
    [SerializeField]
    public float Precentage;
}
