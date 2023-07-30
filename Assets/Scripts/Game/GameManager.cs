using Midbaryom.Camera;
using Midbaryom.Core.Config;
using Midbaryom.Core.Tutorial;
using System;
using UnityEngine;
using System.IO;

using UnityEngine.UI; //TEMP



namespace Midbaryom.Core
{
    [DefaultExecutionOrder(-99999)]
    public class GameManager : MonoBehaviour
    {
        //TEMP
        public bool useDebugMessages;
        public Text leftL, rightL, aboveL;
        public Text leftR, rightR, aboveR;
        public Text midL, midR;
        public GameSessionConfig sessionConfig;
        public float leftRTemp, rightRTemp;
        public float leftLTemp, rightLTemp;
        public float distanceTemp;




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

        public UnityEngine.Camera mainCam;
        public UnityEngine.Camera zoomCam;

        public Animator eagleAnimator;
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

            LoadGameSessionParameters();
            LoadParameters();

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
            if (Input.GetKeyUp(KeyCode.DownArrow))
                useDebugMessages = !useDebugMessages;
        }
        private void OnDestroy()
        {
            _timerText.OnTimeEnded -= MoveToNextScene;
            _tutorialManager.OnTutorialCompeleted -= StartGame;
        }
        //#if UNITY_EDITOR
        //        [Header("Editor:")]
        //        public float Radius;
        //        private void OnDrawGizmosSelected()
        //        {
        //            Gizmos.color = Color.yellow;
        //            Gizmos.DrawSphere(HeightConfigSO.AnimalHeight.Height * Vector3.up, Radius);
        //            Gizmos.DrawSphere(HeightConfigSO.PlayerHeight.Height * Vector3.up, Radius);
        //            Gizmos.DrawSphere(HeightConfigSO.GroundHeight.Height * Vector3.up, Radius);
        //        }
        //#endif


        public void LoadParameters()
        {
            if (File.Exists(Application.streamingAssetsPath + "\\Parameter.ini"))
            {
                string[] parameters = File.ReadAllLines(Application.streamingAssetsPath + "\\Parameter.ini");

                rightLTemp = float.Parse(parameters[0]);
                leftLTemp = float.Parse(parameters[1]);
                rightRTemp = float.Parse(parameters[2]);
                leftRTemp = float.Parse(parameters[3]);
                distanceTemp = float.Parse(parameters[4]);
            }
        }
        public void LoadGameSessionParameters()
        {
            if (File.Exists(Application.streamingAssetsPath + "\\Parameter.ini"))
            {
                string[] parameters = File.ReadAllLines(Application.streamingAssetsPath + "\\Parameter.ini");

                sessionConfig.SessionTime = float.Parse(parameters[5]);
            }
        }
    }


}