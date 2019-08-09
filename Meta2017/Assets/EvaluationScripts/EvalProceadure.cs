using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvalProceadure : MonoBehaviour {

	[Header("Config Before Running Test")]
	public ConditionType condition;
	public TaskType task;
	public int MaxRepetitions = 3; // for each task
	public string configFile = "config.txt";
	private string _resultsFolder;

	private int _repetition = 0;
	public int repetition { get { return _repetition; } }


	[Space(100)]
	[HideInInspector]
	public LAG lag;
	[HideInInspector]
	public EvalState evalState;



	void Start () {
		Application.runInBackground = true;
		_repetition = 0;
		lag = LAG.NONE;
		evalState = EvalState.NONE;

		_resultsFolder = Application.dataPath + "/Results";
        if (!Directory.Exists(_resultsFolder))
        {
            Directory.CreateDirectory(_resultsFolder);
            Debug.Log("Folder created: " + _resultsFolder);
        }
        else
        {
            print("Folder exists: " + _resultsFolder);
        }
	}

	void Update () {
		_lookForKeyEvents ();


		if (evalState == EvalState.IN_SESSION) {
			_send_updateMsgToBonsai ();
		}
	}

	public void loadConfig()
	{
		string config = Application.dataPath + "/" + configFile;

		condition = (ConditionType)Enum.Parse(enumType: typeof(ConditionType), value: ConfigProperties.load(config, "condition"));
		task = (TaskType)Enum.Parse(enumType: typeof(TaskType), value: ConfigProperties.load(config, "task"));
	}

	private void _lookForKeyEvents()
	{
		if (Input.GetKeyDown (KeyCode.S)) {
			_startTask ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			_endTask ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			loadConfig ();
		}
	}

	private string _resultsFile;
	private Log _log;
	private DateTime _lastTimeStamp;
	public DateTime LastTimeStamp {get { return _lastTimeStamp; } }

	private void _startTask()
	{
		if (evalState != EvalState.IN_SESSION && _repetition < MaxRepetitions) {
			print ("START");
			_repetition += 1;
			evalState = EvalState.IN_SESSION;

			if (_repetition == 1) {
				_resultsFile = _resultsFolder + Path.DirectorySeparatorChar + "Results_" + condition + "_" + task + "_" + DateTime.Now.ToString(@"hh\:mm\:ss") + ".txt";
				_log = new Log (_resultsFile);
			}

			_send_startMsgToBonsai ();
			_lastTimeStamp = DateTime.Now;
		}

	}

	private void _endTask()
	{
		if (evalState == EvalState.IN_SESSION) {

			if (_repetition < MaxRepetitions) {
				evalState = EvalState.PAUSE;
				print ("END TASK");

				_send_endMsgToBonsai ();
				_log.writeLine (condition, task, _repetition, DateTime.Now - _lastTimeStamp);

			} else {
				evalState = EvalState.NONE;

				_send_endMsgToBonsai ();
				_log.writeLine (condition, task, _repetition, DateTime.Now - _lastTimeStamp);

				_log.flush();

				print ("END SESSION");
			}
		}
	}

	void _send_startMsgToBonsai()
	{
		Debug.Log ("[TO BONSAI] START");
	}

	void _send_endMsgToBonsai()
	{
		Debug.Log ("[TO BONSAI] END");
	}

	void _send_updateMsgToBonsai()
	{
		Debug.Log ("[TO BONSAI] UPDATE");
	}
}

public class Log
{
	private string _file;
	private string _sep = "$";
	private List<string> _lines;


	public Log(string filename)
	{
		_file = filename;

		string header = "";

		header += "TIMESTAMP" + _sep;
		header += "CONDITION" + _sep;
		header += "TASK" + _sep;
		header += "REPETITION" + _sep;
		header += "ELAPSEDTIME" + _sep;


		_lines = new List<string>();
		_lines.Add(header);
		Debug.Log("created: " + filename);
	}

	private void _writeLine(string line)
	{
		File.AppendAllText(_file, line + Environment.NewLine);
	}

	public void writeLine(ConditionType condition, TaskType task, int repetition, TimeSpan elapsedTime)
	{
		string line = "";

		line += DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss") + _sep;
		line += condition.ToString () + _sep;
		line += task.ToString () + _sep;
		line += repetition + _sep;
		line += elapsedTime.TotalMilliseconds.ToString () + _sep;

		_lines.Add(line);
	}

	public void flush()
	{
		File.WriteAllLines(_file, _lines.ToArray());
	}
}


