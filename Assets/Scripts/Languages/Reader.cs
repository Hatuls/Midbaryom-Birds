using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reader : MonoBehaviour
{
    [SerializeField]
    private string _path;
    [SerializeField]
    private LanguageResource[] _languageResources;

    private Dictionary<LanguageType, LanguageResource> _languageTextsDict;
    private void Awake()
    {
        _languageTextsDict = new Dictionary<LanguageType, LanguageResource>(_languageResources.Length);
        Load();
    }

    private void Load()
    {
        foreach (var item in _languageResources)
        {
            _languageTextsDict.Add(item.Language, item);
            StartCoroutine(LoadTextAsset(item));
        }
    }

    private IEnumerator LoadTextAsset(LanguageResource item)
    {
      var yieldInsturctions =  Resources.LoadAsync<TextAsset>(_path + item.FileName);
        yield return yieldInsturctions;

        if (yieldInsturctions.asset != null)
            item.TextAsset = yieldInsturctions.asset as TextAsset;
    }

    public TextAsset GetText(LanguageType languageType)
        => _languageTextsDict[languageType].TextAsset;
}
[System.Serializable]
public class LanguageResource
{
    public LanguageType Language;
    public TextAsset TextAsset;
    public string FileName;
}