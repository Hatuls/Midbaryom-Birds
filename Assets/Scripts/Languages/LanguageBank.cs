using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Languages/Bank")]
public class LanguageBank : ScriptableObject
{
    [SerializeField]
    private Sentence[] _sentences;

    public Sentence this[int index]
    {
        get => _sentences[index];
    }
    public string GetText(LanguageType type, int index)
    {
        return GetLanguage(this[index], type);
    }

    public string GetLanguage(Sentence sentence, LanguageType languageType)
    {
        switch (languageType)
        {
            case LanguageType.Hebrew:
                return sentence.Hebrew;
            case LanguageType.English:
                return sentence.English;
            case LanguageType.Arabic:
                return sentence.Arabic;
            default:
                throw new Exception("Language was not found");
        }
    }
    public void Init(Sentence[] sentences)
        => _sentences = sentences;
}


[Serializable]
public class Sentence
{
    [SerializeField]
    private string[] _sentences;

    public Sentence(string[] sentences)
    {
        _sentences = new string[sentences.Length];
        for (int i = 0; i < sentences.Length; i++)
        _sentences[i] = sentences[i].Replace('^', ','); 
    }
    public string Hebrew => _sentences[0];
    public string English => _sentences[1];
    public string Arabic => _sentences[2];

    public Sentence Copy()
    => new Sentence(_sentences);

    
}
