using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LanguageToolEditor : EditorWindow
{
    private static BankSenteces[] _bankSenteces;
    private LanguageType _languageType;
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
    private static BankSenteces[] LoadLanguage()
    {
        TextAsset textAsset = Resources.Load("Languages/LanguageCSV") as TextAsset;
        if (textAsset == null)
            throw new Exception("LanguageCSV was not found at path!\nPath:Resources/Languages/LanguageCSV");

        string csv = textAsset.text;
        string[] rows = csv.Replace("\r", "").Split('\n');

        BankSenteces[] sentences = new BankSenteces[rows.Length-2];
        

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
        
            sentences[i-1] = new BankSenteces(i-1,coulmns);
        }

        return sentences;


    }
    
    private void OnGUI()
    {
        GUILayout.Label("This is a Langauge", EditorStyles.boldLabel);

        if(GUILayout.Button("Refresh"))
            _bankSenteces = LoadLanguage();

        if (_bankSenteces == null)
            return;

       EditorGUILayout.IntField("Size", _bankSenteces.Length);

        for (int i = 0; i < _bankSenteces.Length; i++)
        {
            BankSenteces dialogue = _bankSenteces[i];
            EditorGUILayout.BeginVertical();
            dialogue.GUI();
            EditorGUILayout.EndVertical();
        }      

        
        // Debug.Log(_languageType);
    }
}
[Serializable]
public class BankSenteces
{
    [SerializeField]
    private int _index;
    [SerializeField]
    private string[] _sentences;
    public BankSenteces(int index, params string[] sentences)
    {
        _index = index;
        _sentences = new string[sentences.Length];
        for (int i = 0; i < _sentences.Length; i++)
            _sentences[i] = sentences[i].Replace('^',',');
        
    }
    public int Index => _index;
    public string Context => _sentences[0];
    public string Hebrew => _sentences[1];
    public string English => _sentences[2];
    public string Arabic => _sentences[3];

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