using Midbaryom.Core.Tutorial;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseEyeTutorial : MonoBehaviour
{
    [SerializeField]
    private int _textIndex;
    [SerializeField]
    private Sprite _icon;


    [SerializeField]
    private BaseTask _task;

    [SerializeField]
    private GameObject _parent;
    [SerializeField]
    private Image _img;

    [SerializeField]
    private LanguageTMPRO _text;


    protected virtual void Awake()
    {
        _task.OnTaskStarted += SetVisuals;
        _task.OnComplete += RemoveVisuals;
    }

    protected virtual void OnDestroy()
    {
        _task.OnTaskStarted -= SetVisuals;
        _task.OnComplete -= RemoveVisuals;
    }

    protected virtual void RemoveVisuals()
    {
        _parent.SetActive(false);
    }

    protected virtual void SetVisuals()
    {
        _text.SetText(_textIndex);
        _img.sprite = _icon;

        _parent.SetActive(true);
    }
}

public class HumanEyeTutorial : BaseEyeTutorial
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
