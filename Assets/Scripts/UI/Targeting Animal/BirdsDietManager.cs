using Midbaryom.Core;
using UnityEngine;

public class BirdsDietManager : MonoBehaviour
{
    [SerializeField]
    private TargetedAnimalIcon[] _targetedAnimalIcons;

    [SerializeField]
    private TargetGroup[] _allEntities;


    [SerializeField]
    private ScoreAnalyzer _scoreAnalyzer;

    private void Awake()
    {
        _scoreAnalyzer.OnEagleResultFound += AssignEagleDiet;
        ResetAll();
    }
    private void OnDestroy()
    {
        _scoreAnalyzer.OnEagleResultFound -= AssignEagleDiet;
    }
    private void ResetAll()
    {
        for (int i = 0; i < _targetedAnimalIcons.Length; i++)
            _targetedAnimalIcons[i].UnTargeted();
    }
    public void AssignEagleDiet(EagleTypeSO eagleTypeSO)
    {
        ResetAll();
        foreach (var entity in eagleTypeSO.DietDatas)
        {
            if(entity.Precentage > 0)
            {
                GetIcon(entity.DietType)?.Targeted();
            }
            else
            {
                GetIcon(entity.DietType)?.Hide();
            }
        }
    }
    private TargetedAnimalIcon GetIcon(TagSO entityTag)
    {
        for (int i = 0; i < _targetedAnimalIcons.Length; i++)
        {
            if (_targetedAnimalIcons[i].TargetType.ContainTag(entityTag))
                return _targetedAnimalIcons[i];
        }
        Debug.LogWarning("Entity Tag was not found!\n" + entityTag.name);
        return null;
    }

    #region Editor
    [ContextMenu("Assign")]
    private void AssignTargetedAnimal()
    {
        int entitiesCount = _allEntities.Length;
        _targetedAnimalIcons = new TargetedAnimalIcon[entitiesCount];
        for (int i = 0; i < entitiesCount; i++)
        {
            _targetedAnimalIcons[i] = new TargetedAnimalIcon();
            _targetedAnimalIcons[i].Init(transform.GetChild(i).gameObject, _allEntities[i]);
        }
    }

    #endregion

}
