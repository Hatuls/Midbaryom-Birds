using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LanguageToolEditor :EditorWindow
{

    private LanguageType _languageType;
    [MenuItem("Tools/Languages/Load")]
     public static void AssignLanguages()
    {
        LoadLanguage();
        GetWindow<LanguageToolEditor>();
    }

    private static void LoadLanguage()
    {
        TextAsset textAsset = Resources.Load("Languages/LanguageCSV") as TextAsset;
        if (textAsset == null)
            throw new Exception("LanguageCSV was not found at path!\nPath:Resources/Languages/LanguageCSV");

        string csv = textAsset.text;
        string[] rows = csv.Replace("\r", "").Split('\n');

        BankSenteces[] bankSenteces = new BankSenteces[3]
        {
            new BankSenteces(),
            new BankSenteces(),
            new BankSenteces()
        };

        for (int i = 1; i < rows.Length; i++)
        {
            string[] coulmns = rows[i].Split(',');


            for (int j = 0; j < bankSenteces.Length; j++)
            {
                bankSenteces[j]._sentences.Add(coulmns[j + 1]);
            }
        }




    }

    private void OnGUI()
    {
        GUILayout.Label("This is a Langauge", EditorStyles.boldLabel);
        Enum x  = EditorGUILayout.EnumPopup(_languageType);
        _languageType = (LanguageType)x;
       // Debug.Log(_languageType);
    }
}

public class BankSenteces
{
    public List<string> _sentences;
    public BankSenteces()
    {
        _sentences = new List<string>();
    }
}