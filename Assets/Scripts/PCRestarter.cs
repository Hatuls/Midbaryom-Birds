using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

public class PCRestarter : MonoBehaviour
{
    public static PCRestarter instance;
    [SerializeField] private float fileTimeWaitRestartPC;
    [SerializeField] private float currentTimeWaitRestartPC;
    bool isRestartingPC;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadGameSessionParameters();

        ResetResetPCTimer();
    }

    private void Update()
    {
        CountDownResetPC();
    }

    [ContextMenu("Check Works PC Restart")]
    public IEnumerator RestartPC()
    {
        // start the child process
        Process process = new Process();

        if (File.Exists(Application.streamingAssetsPath + "\\CloseZed.bat"))
        {
            Process.Start(Application.streamingAssetsPath + "\\CloseZed.bat");
        }

        yield return new WaitForSeconds(5f);

        if (File.Exists(Application.streamingAssetsPath + "\\RestartCommand.lnk"))
        {
            Process.Start(Application.streamingAssetsPath + "\\RestartCommand.lnk");
        }
    }

    private void CountDownResetPC()
    {
        if (currentTimeWaitRestartPC > 0)
        {
            currentTimeWaitRestartPC -= Time.deltaTime;
        }

        if (currentTimeWaitRestartPC <= 0 && !isRestartingPC)
        {
            UnityEngine.Debug.LogError("Restart PC");
            isRestartingPC = true;
            StartCoroutine(RestartPC());
            //do restart pc here by external file.
        }
    }

    public void ResetResetPCTimer()
    {
        isRestartingPC = false;
        currentTimeWaitRestartPC = fileTimeWaitRestartPC;
    }


    public void LoadGameSessionParameters()
    {
        if (File.Exists(Application.streamingAssetsPath + "\\Parameter.ini"))
        {
            string[] parameters = File.ReadAllLines(Application.streamingAssetsPath + "\\Parameter.ini");

            fileTimeWaitRestartPC = float.Parse(parameters[15]);
        }
    }
}