using Midbaryom.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetParticle : MonoBehaviour
{
    public TagSO PlayerTag;
    [SerializeField]
    private ParticleSystem _particleSystem;

    private AimAssists _aimAssists;
    private ITrackable _target;

    [SerializeField]
    private ScaleInput[] ScaleInputs;

    private ScaleInput _scaleInput;
    private bool IsTargetOn => _target != null;
    ITrackable _player;
    private void Start()
    {
        IEntity entity = Spawner.Instance.AllEntities.First(x => x.ContainTag(PlayerTag));
        _player = entity;
        _aimAssists = entity.Transform.gameObject.GetComponent<AimAssists>();

        _aimAssists.OnTargetDetectedEntity += SetTarget;
        _aimAssists.OnTargetReset += ResetTarget;
        _particleSystem.Stop();
    }
    private void OnDestroy()
    {

        _aimAssists.OnTargetDetectedEntity -= SetTarget;
        _aimAssists.OnTargetReset -= ResetTarget;
    }
    private void ResetTarget()
    {
        _target = null;
        _particleSystem.Stop();
    }

    private void SetTarget(IEntity obj)
    {
        if (obj == null || obj == _target)
            return;

        _scaleInput = ScaleInput(obj.EntityTagSO);

        if (_scaleInput.IsTargetable)
        {
            _target = obj;
            var main = _particleSystem.main;
            main.startSize = _scaleInput.Scale;
            _particleSystem.Play();
        }
        else
        {
            ResetTarget();
        }
    }


    private void LateUpdate()
    {
        if (IsTargetOn)
        {
            transform.position = _scaleInput.AssignVisual(_target, _player);
        }
    }

    private ScaleInput ScaleInput(EntityTagSO entityTagSO)
    => ScaleInputs.First(x => x.EntityTagSO == entityTagSO);

#if UNITY_EDITOR
    [ContextMenu("Assign Entities")]
    private void AssignEntities()
    {
        var tags = Resources.LoadAll<EntityTagSO>("Entities");
        List<ScaleInput> si = new List<ScaleInput>();
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == PlayerTag)
                continue;
            var x = new ScaleInput();
            x.EntityTagSO = tags[i];
            si.Add(x);
        }
        ScaleInputs = si.ToArray();
    }
#endif

}


[System.Serializable]
public class ScaleInput
{
    [SerializeField] EntityTagSO TagSO;
    [SerializeField] float _offsetScalar = 1f;
    [SerializeField] float _scale = 1;
    public bool IsTargetable = true;
    public EntityTagSO EntityTagSO { get => TagSO; set => TagSO = value; }

    public float Scale { get => _scale; }

    public Vector3 AssignVisual(ITrackable trackable, ITrackable player)
    {
        Vector3 currentPosition = trackable.CurrentPosition;
        Vector3 directionToPlayer = (player.CurrentPosition - currentPosition).normalized;
        Vector3 result = directionToPlayer * _offsetScalar;
        return trackable.CurrentPosition + result;
    }
}

