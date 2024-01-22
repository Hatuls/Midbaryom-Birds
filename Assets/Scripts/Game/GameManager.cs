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
        [Header("TEMP")]
        public bool useDebugMessages;
        public Text leftL, rightL, aboveL;
        public Text leftR, rightR, aboveR;
        public Text midL, midR;
        public Canvas debugCanvas;
        public GameSessionConfig sessionConfig;
        public float maxRotationSpeed;
        public static bool isDuringTutorial;

        [Header("Loaded Data")]
        public float moveRight_RightArmMin, moveRight_RightArmMax;
        public float moveRight_LeftArmMin, moveRight_LeftArmMax;

        public float moveLeft_RightArmMax, moveLeft_RightArmMin;
        public float moveLeft_LeftArmMax, moveLeft_LeftArmMin;

        public float hunt_RightArmMax, hunt_RightArmMin;
        public float hunt_LeftArmMax, hunt_LeftArmMin;

        public float distanceTemp;



        public static event Action OnGameStarted;
        public static event Action OnTutorialStarted;
        public static event Action OnGameReset;

        [Header("Gameplay")]
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

        [Header("Counters")]
        [SerializeField] private float fileTimeWaitInactiveGampley;
        [SerializeField] private float currentTimeWaitInactiveGameplay;

        bool isRestartingGameplay;
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

            ResetResetGameplayTimer();

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
            {
                useDebugMessages = !useDebugMessages;
                debugCanvas.gameObject.SetActive(useDebugMessages);
            }

            CountDownInactiveResetGameplay();
        }

        private void CountDownInactiveResetGameplay()
        {
            if(currentTimeWaitInactiveGameplay > 0)
            {
                currentTimeWaitInactiveGameplay -= Time.deltaTime;
            }

            if (currentTimeWaitInactiveGameplay <= 0 && !isRestartingGameplay)
            {
                SoundManager.Instance.StopAllSounds();

                isRestartingGameplay = true;
                Debug.LogError(System.DateTime.Now.ToString() + "" + "Reloading scene 1 - inactivity");
                _screenTransitioner.StartExit(1);

                // restart scene to index 1
            }

        }
        public void ResetResetGameplayTimer()
        {
            isRestartingGameplay = false;
            currentTimeWaitInactiveGameplay = fileTimeWaitInactiveGampley;
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

                moveLeft_LeftArmMin = float.Parse(parameters[0]);
                moveLeft_LeftArmMax = float.Parse(parameters[1]);
                moveLeft_RightArmMin = float.Parse(parameters[2]);
                moveLeft_RightArmMax = float.Parse(parameters[3]);

                moveRight_LeftArmMin = float.Parse(parameters[4]);
                moveRight_LeftArmMax = float.Parse(parameters[5]);
                moveRight_RightArmMin = float.Parse(parameters[6]);
                moveRight_RightArmMax = float.Parse(parameters[7]);

                hunt_RightArmMin = float.Parse(parameters[8]);
                hunt_RightArmMax = float.Parse(parameters[9]);
                hunt_LeftArmMin = float.Parse(parameters[10]);
                hunt_LeftArmMax = float.Parse(parameters[11]);

                distanceTemp = float.Parse(parameters[12]);
            }
        }
        public void LoadGameSessionParameters()
        {
            if (File.Exists(Application.streamingAssetsPath + "\\Parameter.ini"))
            {
                string[] parameters = File.ReadAllLines(Application.streamingAssetsPath + "\\Parameter.ini");

                sessionConfig.SessionTime = float.Parse(parameters[13]);
                fileTimeWaitInactiveGampley = float.Parse(parameters[14]);
            }
        }
    }


}