using Midbaryom.Camera;
using Midbaryom.Core.Config;
using System;
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

            _sceneHandler = FindObjectOfType<SceneHandler>();
        }
        [SerializeField]
        private TimerText _timerText;

        public CameraRotationSO HuntDown, HuntUp;
        public HeightConfigSO HeightConfigSO;
        public SpawnConfigSO _spawnConfig;

        private SceneHandler _sceneHandler;
        private void Start()
        {
            if (PlayerScore.Instance != null)
                PlayerScore.Instance.ResetScores();
         
            _timerText.OnTimeEnded += MoveToNextScene;
        }

        private void MoveToNextScene()
        {
            if (_sceneHandler == null)
                return;
            const int END_GAME_SCENE_INDEX = 3;

            _sceneHandler.LoadSceneAdditive(END_GAME_SCENE_INDEX, true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
        private void OnDestroy()
        {
            _timerText.OnTimeEnded -= MoveToNextScene;
        }
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