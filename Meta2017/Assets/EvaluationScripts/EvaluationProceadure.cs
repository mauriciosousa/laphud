using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LAG
{
    LAG_5 = 5,
    LAG_10 = 10,
    LAG_15 = 15,
    LAG_20 = 20,
    LAG_30 = 30,
    LAG_60 = 60,
    NONE = 0
}

public enum EvalState
{
    IN_SESSION,
    PAUSE,
    NONE
}

public enum ConditionType
{
    BL,
    HUD
}

public enum TaskType
{
    Needle_Thread,
    Simple_Suture,
    Precision_Cutting
}


public class EvaluationProceadure : MonoBehaviour {

    public HUDMessage displayMessage;
    public WebCamQueue _webcamQueue;

    public float SessionTimeMinutes = 10;
    private float _sessionTimeMilliseconds = 0;
    public LAG lag = LAG.NONE;
    public ConditionType condition = ConditionType.HUD;
    public TaskType taskType = TaskType.Needle_Thread;
    public string configFile = "config.txt";
    private string _resultsFolder = "/Results";


    [Space(10)]
    public string startMessageText = "";
    public Color startMessageColor = Color.green;




	void Start () {
        Application.runInBackground = true;

        _resultsFolder = Application.dataPath + _resultsFolder;
        if (!Directory.Exists(_resultsFolder))
        {
            Directory.CreateDirectory(_resultsFolder);
            Debug.Log("Folder created: " + _resultsFolder);
        }
        else
        {
            print("Folder exists: " + _resultsFolder);
        }

        _loadConfig();
        _sessionTimeMilliseconds = SessionTimeMinutes * 60000; 
    }

    private void _lookForKeyEvents()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _startTask();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _endTask();
        }
    }

    private void _loadConfig()
    {
        string config = Application.dataPath + "/" + configFile;

        SessionTimeMinutes = float.Parse(ConfigProperties.load(config, "sessionTimeMinutes"));
        condition = (ConditionType)Enum.Parse(enumType: typeof(ConditionType), value: ConfigProperties.load(config, "condition"));
        taskType = (TaskType)Enum.Parse(enumType: typeof(TaskType), value: ConfigProperties.load(config, "task"));
    }


    public int taskCounter = 0;
    public DateTime sessionStartTime;
    public TimeSpan ellapsed;
    public string ellapsedDisplay = "";

    public bool sessionRunning = false;

    private void _startTask()
    {
        if (taskCounter == 0)
        {
            sessionStartTime = DateTime.Now;
            sessionRunning = true;
        }

        if (sessionRunning)
        {
            taskCounter += 1;
        }
    }

    private void _endTask()
    {


    }

    void Update()
    {
        _lookForKeyEvents();

        if (sessionRunning)
        {
            ellapsed = DateTime.Now - sessionStartTime;
            ellapsedDisplay = ellapsed.ToString("c");

            if (ellapsed.TotalMilliseconds >= _sessionTimeMilliseconds)
            {
                Debug.Log("STAPH");
                sessionRunning = false;
            }


            _webcamQueue.Delay = (uint) lag;
            
        }
    }

    void OnGUI()
    {
        if (sessionRunning)
        {
            GUI.Label(new Rect(Screen.width - 120, 10, 120, 30), ellapsedDisplay);
        }
    }
}
