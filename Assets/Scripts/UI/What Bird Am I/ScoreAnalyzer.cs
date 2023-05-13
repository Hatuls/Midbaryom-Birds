using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ScoreAnalyzer : MonoBehaviour
{
    public event Action<EagleTypeSO> OnEagleResultFound;
    [SerializeField]
    private EagleTypeSO[] _eagleTypeSOs;

   
    private void Start()
    {
        LoadEagles();
        if (PlayerScore.Instance != null)
        {
            OnEagleResultFound?.Invoke(AnalyzeScore());
        }
    }

    private EagleTypeSO AnalyzeScore()
    {
        if (AnimalScore.TotalAmount == 0)
            return _eagleTypeSOs.First(x=>x.Name.Contains("sparrow"));


        float[] sample = PlayerScore.Instance.GetKillFeedPrecentages();
        float[] result = { 0, 0, 0, 0, 0, 0, 0 };
        int categoryCount = PlayerScore.Instance.AnimalScoreDictionary.Count;
        List<float> currentPrecentage = new List<float>(categoryCount);

        for (int i = 1; i < _eagleTypeSOs.Length; i++)
        {
            currentPrecentage.Clear();
            currentPrecentage.Capacity = categoryCount;

            foreach (var precentage in _eagleTypeSOs[i].DietPrecentages)
                currentPrecentage.Add(precentage);

            for (int j = 0; j < categoryCount; j++)
            {
                float k = sample[j] - currentPrecentage[ j];
                if (k < 0) result[i] += sample[j];
                if (k >= 0) result[i] += currentPrecentage [j];
            }
        }

        int max = 0;
        for (int i = 0; i < _eagleTypeSOs.Length; i++)
        {
           // Console.WriteLine(result[i]);
            if (result[i] > result[max])
                max = i;
        }

        return _eagleTypeSOs[max];
    }
    [ContextMenu("Load Eagles From Resources")]
    private void LoadEagles()
    {
        var sortedList = Resources.LoadAll<EagleTypeSO>("Config/Eagles Data")?.ToList();
        sortedList?.Sort(new EagleOrderSorter());
        _eagleTypeSOs = sortedList?.ToArray();
        if (_eagleTypeSOs == null || _eagleTypeSOs.Length == 0)
            Debug.LogError("Eagles Data were not found");
    }

    public class EagleOrderSorter : IComparer<EagleTypeSO>
    {
        public int Compare(EagleTypeSO x, EagleTypeSO y)
        {
            if (x.Order < y.Order)
                return -1;
            else if (x.Order == y.Order)
                return 0;
            else
                return 1;
        }
    }
}
