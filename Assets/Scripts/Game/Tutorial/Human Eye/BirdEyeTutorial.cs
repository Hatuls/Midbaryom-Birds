using System;
using UnityEngine;

public class BirdEyeTutorial : BaseEyeTutorial
{
    [SerializeField]
    private GameObject[] _objectsToClose;
    protected override void SetVisuals()
    {
        Array.ForEach(_objectsToClose, x => x.SetActive(false));
        base.SetVisuals();
    }
    protected override void RemoveVisuals()
    {
        Array.ForEach(_objectsToClose, x => x.SetActive(true));
        base.RemoveVisuals();
    }
}