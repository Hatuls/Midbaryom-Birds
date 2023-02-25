using Midbaryom.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetVisual : MonoBehaviour
{
    [SerializeField]
    private Entity _entity;
    [SerializeField]
    private TargetedBehaviour _targetBehaviour;
    private void Awake()
    {
        _targetBehaviour.OnTargeted += RedSignal;
    }

    private void RedSignal()
    {

    }

    private void OnDestroy()
    {
        _targetBehaviour.OnTargeted -= RedSignal;
    }
    private void OnEnable()
    {
        
    }


}
