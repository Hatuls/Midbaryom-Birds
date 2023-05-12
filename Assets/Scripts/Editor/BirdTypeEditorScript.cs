using Midbaryom.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
             { 0, 0, 0, 0, 0, 0, 0 },
             { 60, 30, 10, 0, 0, 0, 0 },
             { 10, 0, 0, 60, 30, 0, 0 },
             { 30, 60, 10, 0, 0, 0, 0 },
             { 0, 10, 30, 0, 0, 60, 0 },
             { 0, 0, 0, 0, 0, 0, 100 },
            
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
        // דרור
        //חיוואי נחשים,
        //עיט זהוב,
        //עקב עיטי,
        //בז מצוי,
        //נשר מקראי
        string[] configsNames = new string[]
        {
            "House sparrow",
            "Short-toed snake eagle",
            "Golden eagle",
            "Long-legged buzzard",
            "Common kestrel",
            "Eurasian griffon vulture",
        };

        int[] InfoIndexes = new int[]
        {
            24,
            15,
            16,
            17,
            14,
            13,
        };

        int[] NameIndexes = new int[]
         {
            23,
            7,
            8,
            9,
            11,
            10,
         };


        string path = $"Assets/Resources/Config/Eagles Data/";
       // Debug.Log(path);
        List<EagleTypeSO> eagleTypeSOs = new List<EagleTypeSO>();


      string[] imagesGUIDs =  AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/Art/Images/Eagles" });
        List<string> paths = new List<string>();
        for (int i = 0; i < imagesGUIDs.Length; i++)
            paths.Add(AssetDatabase.GUIDToAssetPath(imagesGUIDs[i]));
        

        for (int i = 0; i < configsNames.Length; i++)
        {

            string EagleName = configsNames[i];
            string relevatImagePath = paths.FirstOrDefault(x => x.Contains(EagleName));
            Sprite image = null;
            if (string.IsNullOrEmpty(relevatImagePath) == false)
                image = AssetDatabase.LoadAssetAtPath(relevatImagePath, typeof(Sprite)) as Sprite;

            var eagleType = ScriptableObject.CreateInstance<EagleTypeSO>();
            eagleType.SetRawInfo(EagleName, image,NameIndexes[i],InfoIndexes[i]);// add image here
            eagleTypeSOs.Add(eagleType);
            eagleType.Order = i;
            AssetDatabase.CreateAsset(eagleType, path + $"{EagleName}.asset");
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
