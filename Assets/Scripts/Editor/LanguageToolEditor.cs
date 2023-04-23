using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LanguageToolEditor : EditorWindow
{
    private static BankSentences[] _bankSenteces;
    private LanguageType _languageType;
    private Vector2 _scrollPos;
    [MenuItem("Tools/Languages/Load")]
    public static void AssignLanguages()
    {
        _bankSenteces = LoadLanguage();
        GetWindow<LanguageToolEditor>();
    }
    private void OnDisable()
    {
        _bankSenteces = null;
        Resources.UnloadUnusedAssets();
    }
    private static BankSentences[] LoadLanguage()
    {
        TextAsset textAsset = Resources.Load("Languages/LanguageCSV") as TextAsset;
        if (textAsset == null)
            throw new Exception("LanguageCSV was not found at path!\nPath:Resources/Languages/LanguageCSV");

        string csv = textAsset.text;
        string[] rows = csv.Replace("\r", "").Split('\n');

        BankSentences[] sentences = new BankSentences[rows.Length-2];
        

        for (int i = 1; i < rows.Length-1; i++)
        {
            

            string[] coulmns = rows[i].Split(',');
            string checkThisLine = coulmns[0];
            if (string.IsNullOrEmpty(checkThisLine))
            {

                checkThisLine = sentences[i - 2].Context;
            if (string.IsNullOrEmpty(checkThisLine))
                throw new Exception("Delete line " + (i + 1));

                coulmns[0] = checkThisLine;
            }
        
            sentences[i-1] = new BankSentences(i-1,coulmns);
        }

        return sentences;


    }
    
    private void OnGUI()
    {
        GUILayout.Label("This is a Langauge", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Refresh"))
            _bankSenteces = LoadLanguage();

        if (_bankSenteces == null)
            return;


        if (GUILayout.Button("Create Assets"))
            CreateAssets();

        EditorGUILayout.EndHorizontal();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
       EditorGUILayout.IntField("Size", _bankSenteces.Length);

        for (int i = 0; i < _bankSenteces.Length; i++)
        {
            BankSentences dialogue = _bankSenteces[i];
            EditorGUILayout.BeginVertical();
            dialogue.GUI();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        // Debug.Log(_languageType);
    }

    private void CreateAssets()
    {
        if (_bankSenteces == null)
            _bankSenteces = LoadLanguage();

        if (_bankSenteces == null)
            return;

        var bankSentences = ScriptableObject.CreateInstance<LanguageBank>();
        int length = _bankSenteces.Length;
        Sentence[] sentences = new Sentence[length];

        for (int i = 0; i < length; i++)
            sentences[i] = _bankSenteces[i].Sentence;

        bankSentences.Init(sentences);
        string path = $"Assets/Resources/Languages/";
        AssetDatabase.CreateAsset(bankSentences, path + $"LanguageSO.asset");
        AssetDatabase.SaveAssets();
    }
}

[Serializable]
public class BankSentences
{
    [SerializeField]
    private int _index;
    [SerializeField]
    private string _context;
    [SerializeField]
    private Sentence _sentence;
    public BankSentences(int index, params string[] sentences)
    {
        _index = index;

        _context = sentences[0].Replace('^',',');

        string[] translates = new string[sentences.Length - 1];
        for (int i = 1; i < sentences.Length; i++)
            translates[i - 1] = sentences[i];
        _sentence = new Sentence(translates);
    }
    public int Index => _index;
    public string Context => _context;
    public string Hebrew => _sentence.Hebrew;
    public string English => _sentence.English;
    public string Arabic => _sentence.Arabic;

    public Sentence Sentence => _sentence.Copy();
    public string Text(LanguageType languageType)
    {
        switch (languageType)
        {
            case LanguageType.Hebrew:
                return Hebrew;
            case LanguageType.Arabic:
                return Arabic;
            case LanguageType.English:
                return English;
            default:
                break;
        }
        throw new Exception("Language type was not found?\nInput: " + languageType.ToString());
    }
    public bool IsFocuses;
    public void GUI()
    {
        GUILayout.BeginHorizontal();

        IsFocuses = EditorGUILayout.BeginFoldoutHeaderGroup(IsFocuses, "Sentences");

        string text = string.Empty;
        if (IsFocuses)
        {
            GUILayout.BeginVertical();

                GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            text = string.Concat(Text(LanguageType.Hebrew).Reverse());
            EditorGUILayout.TextArea(text, new GUIStyle() { alignment = TextAnchor.MiddleRight });

            EditorGUILayout.TextArea("|", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
            text = string.Concat(Text(LanguageType.Arabic).Reverse());
            EditorGUILayout.TextArea(text, new GUIStyle() { alignment = TextAnchor.MiddleRight });
            EditorGUILayout.TextArea("|", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.TextArea(Text(LanguageType.English), new GUIStyle() { alignment = TextAnchor.MiddleLeft });

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        text = string.Concat(Context.Reverse());
        EditorGUILayout.TextArea(text, new GUIStyle() {
            border = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleRight }); ;
      //  EditorGUILayout.LabelField(text, new GUIStyle() { alignment = TextAnchor.MiddleRight });
        EditorGUILayout.IntField(_index);
   
   

         EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndHorizontal();
        if(IsFocuses)
            GUILayout.Space(20);
    }
}