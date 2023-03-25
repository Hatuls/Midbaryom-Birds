using Midbaryom.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class BirdTypeEditorScript
{
    [MenuItem("Tools/Create Birds Diet Configs")]
    public static void AddConfigs()
    {
        // load all tags
        List<TagSO> tagsFound = LoadTags();
        Assert.IsTrue(tagsFound.Count > 0, "Tags were not found!");

        //create eagle type so
        List<EagleTypeSO> configs = CreateConfigs();
        Assert.IsTrue(configs.Count > 0, "Configs were not created!");

        // add the precentages
        AddDiet(configs, tagsFound);
        //save
        for (int i = 0; i < configs.Count; i++)
        {
            AssetDatabase.SaveAssetIfDirty(configs[i]);
        }

    }

    private static void AddDiet(List<EagleTypeSO> configs, List<TagSO> tagsFound)
    {
        //"Snake","Reptiles","Mouse","Rabbit","Bird","Insects","Dead Target"
        int[,] diets = new int[,]
        {
             { 60, 30, 10, 0, 0, 0, 0 },
             { 10, 0, 0, 60, 30, 0, 0 },
             { 30, 60, 10, 0, 0, 0, 0 },
             { 0, 10, 30, 0, 0, 60, 0 },
             { 0, 0, 0, 0, 0, 0, 100 }
        };

        for (int i = 0; i < configs.Count; i++)
        {
            for (int j = 0; j < tagsFound.Count; j++)
            {
                int currentPRecentage = diets[i, j];
                configs[i].Add(tagsFound[j], currentPRecentage);
            }
            AssetDatabase.SaveAssetIfDirty(configs[i]);
        }
        AssetDatabase.SaveAssets();
    }

    private static List<EagleTypeSO> CreateConfigs()
    {
        //חיוואי נחשים,
        //עיט זהוב,
        //עקב עיטי,
        //בז מצוי,
        //נשר מקראי

        string[] configsNames = new string[]
        {
            "Short-toed snake eagle",
            "Golden Eagle",
            "Long-legged buzzard",
            "Common kestrel",
            "Eurasian griffon vulture",
        };
        string path = $"Assets/Resources/Config/Eagles Data/";
       // Debug.Log(path);
        List<EagleTypeSO> eagleTypeSOs = new List<EagleTypeSO>();

        for (int i = 0; i < configsNames.Length; i++)
        {
            var eagleType = ScriptableObject.CreateInstance<EagleTypeSO>();
            eagleType.SetRawInfo(configsNames[i], null);// add image here
            eagleTypeSOs.Add(eagleType);
            eagleType.Order = i;
            AssetDatabase.CreateAsset(eagleType, path + $"{configsNames[i]}.asset");
            AssetDatabase.SaveAssetIfDirty(eagleType);
        }

        AssetDatabase.SaveAssets();
        return eagleTypeSOs;
    }

    private static List<TagSO> LoadTags()
    {
        var results = AssetDatabase.FindAssets("t:TagSO", new string[] { "Assets" });
        List<TagSO> tags = new List<TagSO>();
        string[] requestsTags = new string[]
        {
            "Snake",
            "Reptiles",
            "Mouse",
            "Rabbit",
            "Bird",
            "Insects",
            "Dead Target",
        };


        for (int i = 0; i < requestsTags.Length; i++)
        {
            foreach (var item in results)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                var currentTag = AssetDatabase.LoadAssetAtPath(path, typeof(TagSO)) as TagSO;

                if (requestsTags[i] == currentTag.name)
                {
                    tags.Add(currentTag);
                    break;
                }
            }
        }

        return tags;
    }
}
