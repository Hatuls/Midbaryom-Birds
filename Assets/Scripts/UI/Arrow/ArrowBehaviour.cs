using Midbaryom.Core;
using System;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _arrow;
    [SerializeField]
    private Camera _camera;
    private IEntity _target;
    private IPlayer _player;
    [SerializeField]
    private float _rotationModifier;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _smallOffset;

    public bool IsActive;
    private void Awake()
    {
        Close();
    }
    public void Open()
    {
        _arrow.gameObject.SetActive(true);
     
    }

    private void Update()
    {
        if (_target == null || _player == null || !IsActive)
            return;

        var playerFace = _player.Entity.CurrentFacingDirection  ;
        var playerPos = _player.Entity.CurrentPosition  ;
        var targetPos = _target.CurrentPosition;
        playerPos.y = 0;
        targetPos.y = 0;

        Vector3 toPosition = (targetPos - playerPos);
        float angle = AngleBetweenVector2(playerFace, toPosition) + _rotationModifier;
     

        //float zDifference =playerFace.magnitude;


        //float angle = Mathf.Atan2(dirTowardsTarget.z,dirTowardsTarget.x) * Mathf.Rad2Deg + _rotationModifier;

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);


        _arrow.rotation = Quaternion.Slerp(_arrow.rotation, q,Time.deltaTime * _rotationSpeed);



        bool isOnScreen = PointInCameraView(_target.CurrentPosition);

        if (isOnScreen)
            Close();
        else
            Open();

        float AngleBetweenVector2(Vector3 vec1, Vector3 vec2)
        {
            var angle = Vector3.Angle(vec1, vec2); // calculate angle
            return angle * -Mathf.Sign(Vector3.Cross(vec1, vec2).y);
        }
    }

    public void Close()
    {
        if (ReferenceEquals(_arrow.gameObject, null) == false) ;
        _arrow.gameObject.SetActive(false);
    }

    internal void PointTowards(IPlayer player, IEntity mob)
    {
        _player = player;
        _target = mob;
    }


    bool PointInCameraView(Vector3 point)
    {
        Vector3 viewport = _camera.WorldToViewportPoint(point);
        bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
        bool inFrontOfCamera = viewport.z > 0;

        RaycastHit depthCheck;
        bool objectBlockingPoint = false;

        Vector3 directionBetween = point - _camera.transform.position;
        directionBetween = directionBetween.normalized;

        float distance = Vector3.Distance(_camera.transform.position, point);

        if (Physics.Raycast(_camera.transform.position, directionBetween, out depthCheck, distance + _smallOffset))
        {
            if (depthCheck.point != point)
            {
                objectBlockingPoint = true;
            }
        }

        return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
    }

    bool Is01(float a)
    => a > 0 && a < 1;
}
