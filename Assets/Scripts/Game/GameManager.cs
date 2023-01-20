using Midbaryom.Camera;
using Midbaryom.Core.Config;
using UnityEngine;
namespace Midbaryom.Core
{
    [DefaultExecutionOrder(-99999)]
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        private void Awake()
        {
            if (_instance == null || _instance == this)
                _instance = this;
            else if (_instance != this)
                Destroy(this.gameObject);
        }


        public CameraRotationSO HuntDown, HuntUp;
        public HeightConfigSO HeightConfigSO;
        public SpawnConfigSO _spawnConfig;
#if UNITY_EDITOR
        [Header("Editor:")]
        public float Radius;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(HeightConfigSO.AnimalHeight.Height * Vector3.up, Radius);
            Gizmos.DrawSphere(HeightConfigSO.PlayerHeight.Height * Vector3.up, Radius);
            Gizmos.DrawSphere(HeightConfigSO.GroundHeight.Height * Vector3.up, Radius);
        }
#endif
    }


}