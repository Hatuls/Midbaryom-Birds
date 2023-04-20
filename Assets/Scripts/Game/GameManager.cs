using Midbaryom.Camera;
using Midbaryom.Core.Config;
using Midbaryom.Core.Tutorial;
using System;
using UnityEngine;
namespace Midbaryom.Core
{
    [DefaultExecutionOrder(-99999)]
    public class GameManager : MonoBehaviour
    {
        public static event Action OnGameStarted;
        public static event Action OnTutorialStarted;
        public static event Action OnGameReset;


        private static GameManager _instance;
        public static GameManager Instance => _instance;
  
        [SerializeField]
        private ScreenTransitioner _screenTransitioner;
        [SerializeField]
        private TutorialManager _tutorialManager;
        [SerializeField]
        private TimerText _timerText;

        public CameraRotationSO HuntDown, HuntUp;
        public HeightConfigSO HeightConfigSO;
        public SpawnConfigSO _spawnConfig;
        private void Awake()
        {
            if (_instance == null || _instance == this)
                _instance = this;
            else if (_instance != this)
                Destroy(this.gameObject);
        }
        private void Start()
        {
            if (PlayerScore.Instance != null)
                PlayerScore.Instance.ResetScores();
         
            _timerText.OnTimeEnded += MoveToNextScene;
            _tutorialManager.OnTutorialCompeleted += StartGame;
        }
        public void StartGame()
        {
            OnGameStarted?.Invoke();
        }
        private void MoveToNextScene()
        {
    
            const int END_GAME_SCENE_INDEX = 3;
            _screenTransitioner.StartExit(END_GAME_SCENE_INDEX);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
        private void OnDestroy()
        {
            _timerText.OnTimeEnded -= MoveToNextScene;
            _tutorialManager.OnTutorialCompeleted -= StartGame;
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