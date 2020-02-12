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

    public StartStopVideo INFORM_BONSAI;

    public HUDMessage displayMessage;
    public WebCamQueue _webcamQueue;

    public int SessionTimeMinutes = 10;
    private float _sessionTimeMilliseconds = 0;
    public LAG lag = LAG.NONE;
    public ConditionType condition = ConditionType.HUD;
    public TaskType taskType = TaskType.Needle_Thread;
    public string configFile = "config.txt";
    private string _resultsFolder = "/Results";

    public OSCTunaReceiver tunas;

    public GUIStyle taskInfoStyle;

    public double displayStartTaskMessageTime = 1500;
    public double displayEndTaskMessageTime = 1000;
    public double displayEndEvaluationMessageTime = 10000;

    private string _calibrationPoseFilename;

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

        _calibrationPoseFilename = _resultsFolder + "/CalibrationPose-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";

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

        SessionTimeMinutes = int.Parse(ConfigProperties.load(config, "sessionTimeMinutes"));
        condition = (ConditionType)Enum.Parse(enumType: typeof(ConditionType), value: ConfigProperties.load(config, "condition"));
        taskType = (TaskType)Enum.Parse(enumType: typeof(TaskType), value: ConfigProperties.load(config, "task"));
    }


    public int taskCounter = 0;
    public DateTime sessionStartTime;
    public TimeSpan ellapsed;
    public string ellapsedDisplay = "";

    public bool sessionRunning = false;
    public bool duringTask = false;

    private LogData _log;
    private bool _canLogTunas = false;
    private LogTunas _logTunas;
    private DateTime taskStartTime;

    private void _startTask()
    {

        INFORM_BONSAI.start = 1;


        if (duringTask) return;

        if (taskCounter == 0)
        {
            sessionStartTime = DateTime.Now;
            sessionRunning = true;
            _log = new LogData(Application.dataPath + "/Results/Results_" + condition + "_" + taskType + ".txt");
        }

        if (sessionRunning)
        {
            taskCounter += 1;
            duringTask = true;
            taskStartTime = DateTime.Now;
            displayMessage.add("Task started...", Color.green, displayStartTaskMessageTime);

            _canLogTunas = true;
            _logTunas = new LogTunas(Application.dataPath + "/Results/Tunas_" + condition + "_" + taskType + "_" + "task" + taskCounter + ".txt");
            _logTunas.header(tunas.tunaData.Keys);
        }
    }

    private void _endTask()
    {
        if (!duringTask) return;
        duringTask = false;
        _log.Add(taskCounter, taskStartTime, (DateTime.Now - taskStartTime));

        displayMessage.add("Task ended...", Color.yellow, displayEndTaskMessageTime);

        _canLogTunas = false;
        _logTunas.flush();


        INFORM_BONSAI.start = 0;
    }

    void Update()
    {
        _lookForKeyEvents();

        if (sessionRunning)
        {
            ellapsed = DateTime.Now - sessionStartTime;
            ellapsedDisplay = " ";// (new TimeSpan(0, SessionTimeMinutes, 0) - ellapsed).ToString("c").Split('.')[0];


            if (_canLogTunas)
            {
                _logTunas.add(tunas.tunaData.Values);
            }


            if (ellapsed.TotalMilliseconds >= _sessionTimeMilliseconds)
            {
                Debug.Log("STAPH");
                displayMessage.add("Mission Accomplished", Color.red, displayEndEvaluationMessageTime);
                sessionRunning = false;
                _log.flush();
                _logTunas.flush();
            }
        }

        _webcamQueue.Delay = (uint)lag;
    }

    void OnGUI()
    {
        if (sessionRunning)
        {
            int left = Screen.width / 2;
            int top = 10;
            int step = 35;

            GUI.Label(new Rect(left,top, 500, step), ellapsedDisplay, taskInfoStyle);
            if (duringTask)
            {
                top += step;
                GUI.Label(new Rect(left, top, 500, step), "Task: " + taskCounter, taskInfoStyle);
            }
        }

        if (tunas.tunaIds.Count > 0) {
            if (GUI.Button(new Rect(0, Screen.height - 20, 200, 20), "Store calibration pose")) {
                _saveCalibrationPose();
            }
        }
    }

    private void _saveCalibrationPose() {
        List<string> lines = new List<string>();

        string line = "";
        line += "ID$";

        line += "accel.X$";
        line += "accel.Y$";
        line += "accel.Z$";

        line += "gyro.X$";
        line += "gyro.Y$";
        line += "gyro.Z$";

        line += "mag.X$";
        line += "mag.Y$";
        line += "mag.Z$";

        lines.Add(line);

        foreach (TunaInfo i in tunas.tunaData.Values) {
            line = "";
            line += i.ID + "$";
            line += i.accel.x + "$";
            line += i.accel.y + "$";
            line += i.accel.z + "$";
            line += i.gyro.x + "$";
            line += i.gyro.y + "$";
            line += i.gyro.z + "$";
            line += i.mag.x + "$";
            line += i.mag.y + "$";
            line += i.mag.z + "$";
            lines.Add(line);
        }

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(_calibrationPoseFilename)) {
            foreach (string l in lines) {
                file.WriteLine(l);
            }
            Debug.Log("file recorded: " + _calibrationPoseFilename);

        }
    }
}

public class LogData
{
    private string _filename;
    private List<string> _lines;
    private string _sep = "$";

    public LogData(string filename)
    {
        _filename = filename;
        _lines = new List<string>();

        string line = "TASK" + _sep + "START" + _sep + "TOTALTIME";
        _lines.Add(line);
    }

    public void Add(int task, DateTime start, TimeSpan totaltime)
    {
        string line = "";
        line += task + _sep;
        line += start.ToString("yyyy/MM/dd-HH:mm:ss") + _sep;
        line += totaltime.TotalMilliseconds + _sep;

        _lines.Add(line);
    }

    public void flush()
    {
        File.WriteAllLines(_filename, _lines.ToArray());
    }
}

public class LogTunas
{
    private string _filename;
    private List<string> _lines;
    private string _sep = "$";

    public LogTunas(string filename)
    {
        _filename = filename;
        _lines = new List<string>();
    }



    public void flush()
    {
        File.WriteAllLines(_filename, _lines.ToArray());
    }

    internal void add(Dictionary<string, TunaInfo>.ValueCollection values)
    {
        string line = "";
        foreach (TunaInfo i in values)
        {
            line += i.accel.x + _sep;
            line += i.accel.y + _sep;
            line += i.accel.z + _sep;
            line += i.gyro.x + _sep;
            line += i.gyro.y + _sep;
            line += i.gyro.z + _sep;
            line += i.mag.x + _sep;
            line += i.mag.y + _sep;
            line += i.mag.z + _sep;
        }
        _lines.Add(line);
    }

    internal void header(Dictionary<string, TunaInfo>.KeyCollection keys)
    {
        string line = "";
        foreach (string s in keys)
        {
            line += s + ".accel.x" + _sep;
            line += s + ".accel.y" + _sep;
            line += s + ".accel.z" + _sep;
            line += s + ".gyro.x" + _sep;
            line += s + ".gyro.y" + _sep;
            line += s + ".gyro.z" + _sep;
            line += s + ".mag.x" + _sep;
            line += s + ".mag.y" + _sep;
            line += s + ".mag.z" + _sep;
        }
        _lines.Add(line);
    }
}
