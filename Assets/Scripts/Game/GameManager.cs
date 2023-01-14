using Midbaryom.Camera;
using Midbaryom.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }


    public CameraRotationSO HuntDown, HuntUp;
    public HeightConfigSO HeightConfigSO;

#if UNITY_EDITOR
    [Header("Editor:")]
    public float Radius;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(HeightConfigSO.AnimalHeight * Vector3.up, Radius);
        Gizmos.DrawSphere(HeightConfigSO.PlayerHeight * Vector3.up, Radius);
        Gizmos.DrawSphere(HeightConfigSO.GroundHeight * Vector3.up, Radius);
    }
#endif
}
