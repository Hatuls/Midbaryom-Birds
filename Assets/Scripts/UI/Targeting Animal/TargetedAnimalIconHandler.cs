using Midbaryom.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetedAnimalIconHandler : MonoBehaviour
{

    [SerializeField]
    private Spawner _spawner;
    [SerializeField]
    private TargetedAnimalIcon[] _targetedAnimalIcons;

    [SerializeField]
    private TargetGroup[] _allEntities;

    private EntityTagSO _entityTag;

    private AimAssists _aimAssists;

    

    private IEnumerator Start()
    {
        ResetAll();
        yield return null;
        _aimAssists = _spawner.PlayerEntity.Transform.GetComponent<AimAssists>();
        _aimAssists.OnTargetDetectedEntity += Target;
        _aimAssists.OnTargetReset += ResetAll;
    }
    private void OnDestroy()
    {
        _aimAssists.OnTargetDetectedEntity -= Target;
        _aimAssists.OnTargetReset          -= ResetAll;
    }

    public void ResetAll()
    {
        _entityTag = null;
        for (int i = 0; i < _targetedAnimalIcons.Length; i++)
            _targetedAnimalIcons[i].UnTargeted();
    }
    public void Target(IEntity entity)
    {
        if (entity == null)
            return;

        EntityTagSO entityTag = entity.EntityTagSO;

        if (_entityTag == entityTag)
            return;

        ResetAll();
        GetIcon(entity).Targeted();

        _entityTag = entityTag;
    }

    public TargetedAnimalIcon GetIcon(IEntity entityTag)
    {
        for (int i = 0; i < _targetedAnimalIcons.Length; i++)
        {
            if (_targetedAnimalIcons[i].TargetType.ContainOneOrMoreTags(entityTag.Tags))
                return _targetedAnimalIcons[i];
        }
        throw new System.Exception("Entity Tag was not found!\n"+ entityTag.EntityTagSO.name);
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


[System.Serializable]
public class TargetedAnimalIcon
{
    [SerializeField]
    private GameObject _targetedImageObject;
    [SerializeField]
    private GameObject _untargetedImageObject;
    [SerializeField]
    private TargetGroup _entityTag;
    public TargetGroup TargetType => _entityTag;
    private int[] _animalesCounts = new int[7];
    public void Targeted()
    {
        //_untargetedImageObject.SetActive(false);
        //_targetedImageObject.SetActive(true);
    }

    public void UnTargeted()
    {
        //_untargetedImageObject.SetActive(true);
        //_targetedImageObject.SetActive(false);
    }

    public void Hide()
    {
        ResetCount(_targetedImageObject.transform.parent.name, _targetedImageObject.transform.parent.Find("Not Targeted Icon").gameObject);
        _targetedImageObject.transform.parent.gameObject.SetActive(false);
    }
    public void Show()
    {
        UpdateCount(_targetedImageObject.transform.parent.name, _targetedImageObject.transform.parent.Find("Not Targeted Icon").gameObject);
        _targetedImageObject.transform.parent.gameObject.SetActive(true);
    }

    public void Init(GameObject gameObject, TargetGroup entityTag)
    {
        var parentTransform = gameObject.transform;
        _untargetedImageObject = parentTransform.GetChild(0).gameObject;
        _targetedImageObject = parentTransform.GetChild(1).gameObject;
        _entityTag = entityTag;
    }

    private void UpdateCount(string TargetedParentName, GameObject TragetGameobject)
    {
        switch(TargetedParentName)
        {
            case "Animal Icon - Mice":
            {
                    _animalesCounts[0]++;
                    if(_animalesCounts[0] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if(_animalesCounts[0] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
            }
            case "Animal Icon - Turtle":
                {
                    _animalesCounts[1]++;
                    if (_animalesCounts[1] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if (_animalesCounts[1] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
            case "Animal Icon - Snake":
                {
                    _animalesCounts[2]++;
                    if (_animalesCounts[2] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if (_animalesCounts[2] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
            case "Animal Icon - Gecko":
                {
                    _animalesCounts[3]++;
                    if (_animalesCounts[3] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if (_animalesCounts[3] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
            case "Animal Icon - Rabbit Alive":
                {
                    _animalesCounts[4]++;
                    if (_animalesCounts[4] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);
                   

                    else if (_animalesCounts[4] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
            case "Animal Icon - Dead Rabbit":
                {
                    _animalesCounts[5]++;
                    if (_animalesCounts[5] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if (_animalesCounts[5] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
            case "Animal Icon - Beetle":
                {
                    _animalesCounts[6]++;
                    if (_animalesCounts[6] == 2)
                        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(true);

                    else if (_animalesCounts[6] == 3)
                        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(true);

                    break;
                }
        }
    }

    private void ResetCount(string TargetedParentName, GameObject TragetGameobject)
    {
        switch (TargetedParentName)
        {
            case "Animal Icon - Mice":
                {
                    _animalesCounts[0] = 0;
                    break;
                }
            case "Animal Icon - Turtle":
                {
                    _animalesCounts[1] = 0;
                    break;
                }
            case "Animal Icon - Snake":
                {
                    _animalesCounts[2] = 0;
                    break;
                }
            case "Animal Icon - Gecko":
                {
                    _animalesCounts[3] = 0;
                    break;
                }
            case "Animal Icon - Rabbit Alive":
                {
                    _animalesCounts[4] = 0;
                    break;
                }
            case "Animal Icon - Dead Rabbit":
                {
                    _animalesCounts[5] = 0;
                    break;
                }
            case "Animal Icon - Beetle":
                {
                    _animalesCounts[6] = 0;
                    break;
                }
        }
        TragetGameobject.transform.Find("Number 2").gameObject.SetActive(false);
        TragetGameobject.transform.Find("Number 3").gameObject.SetActive(false);
    }
}

[System.Serializable]
public class TargetGroup : ITaggable
{
#if UNITY_EDITOR
    public string GroupName;
#endif
    public TagSO[] TagsCollection;

    public IEnumerable<TagSO> Tags => TagsCollection;
}