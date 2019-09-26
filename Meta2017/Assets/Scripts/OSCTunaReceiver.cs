using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using System;

public class TunaInfo
{
    private readonly string _id;
    public string ID => _id;

    public Vector3 accel;
    public Vector3 gyro;
    public Vector3 mag;

    public TunaInfo(string id)
    {
        _id = id;
        accel = Vector3.zero;
        gyro = Vector3.zero;
        mag = Vector3.zero;
    }
}

public class OSCTunaReceiver : MonoBehaviour {


    [Header("OSC Settings")]
    public OSCReceiver Receiver;

    public List<string> tunaIds;
    public Dictionary<string, TunaInfo> tunaData;

    private string _firstTuna;
    private DateTime _lastUpdate;
    public double updateRate = 0;

    public GUIStyle s;
    public bool displayTunas;

    public Transform tuna11;

    public bool recordingGUI;

    void Start()
    {
        Application.runInBackground = true;

        string tunaAddress;
        tunaData = new Dictionary<string, TunaInfo>();
        for (int i = 0; i < tunaIds.Count; i++)
        {
            tunaAddress = "/tuna" + tunaIds[i];
            tunaData.Add(tunaAddress, new TunaInfo(tunaIds[i]));
            Debug.Log("add: " + tunaAddress);
            Receiver.Bind(tunaAddress, ReceivedMessage);

            if (i == 0) _firstTuna = tunaAddress;
        }
    }

    #region Smoothing //Pedro Belchior - para estabilizar os valores dos imus

    [Range(1, 200)]
    public int maxQueueSize = 100; private Queue<Vector3> smoothingGyro = new Queue<Vector3>();
    private Vector3 gyroSum = Vector3.zero;

    private void AddToSmoothingQueue(Vector3 tmp, int maxQueueSize) {

        smoothingGyro.Enqueue(tmp);
        gyroSum += tmp;

        while (smoothingGyro.Count > maxQueueSize) {
            gyroSum -= smoothingGyro.Dequeue();
        }
    }

    private Vector3 GetSmoothedValue() {
        int div = smoothingGyro.Count;
        if (div < 1) div = 1;
        return gyroSum / div;
    }

    #endregion

    private void Update() {
        if (recording && tunaData.ContainsKey("/tuna17")) {
            TunaInfo t = tunaData["/tuna17"];
            _lines.Add("" + DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss:fff") + sep + t.accel.x + sep + t.accel.y + sep + t.accel.z + sep + t.gyro.x + sep + t.gyro.y + sep + t.gyro.z + sep + t.mag.x + sep + t.mag.y + sep + t.mag.z);

        }
    }

    void LateUpdate() {

        Vector3 tmp = tunaData["/tuna17"].gyro;
        AddToSmoothingQueue(tmp, maxQueueSize);

        Quaternion q = Quaternion.Euler(GetSmoothedValue());
        //tuna11.rotation = q;

        tuna11.LookAt(tuna11.position + GetSmoothedValue());

        Debug.DrawRay(tuna11.position, GetSmoothedValue());
        Debug.DrawRay(tuna11.position, transform.forward, Color.blue);
        Debug.DrawRay(tuna11.position, transform.right, Color.red);
        Debug.DrawRay(tuna11.position, transform.up, Color.green);

    }

    private void ReceivedMessage (OSCMessage message) {

        //Debug.Log(message.Address);

        if (tunaData.ContainsKey(message.Address))
        {


            //Debug.Log("1: "+ tunaData[message.Address].accel.x + "   " + message.Values[0].Value);


            tunaData[message.Address].accel.x =    (float)message.Values[0].DoubleValue;
            tunaData[message.Address].accel.y =    (float) message.Values[1].DoubleValue;
            tunaData[message.Address].accel.z =    (float) message.Values[2].DoubleValue;
                                                                             
            tunaData[message.Address].gyro.x =     (float) message.Values[3].DoubleValue;
            tunaData[message.Address].gyro.y =     (float) message.Values[4].DoubleValue;
            tunaData[message.Address].gyro.z =     (float) message.Values[5].DoubleValue;
                                                                             
            tunaData[message.Address].mag.x =      (float) message.Values[6].DoubleValue;
            tunaData[message.Address].mag.y =      (float) message.Values[7].DoubleValue;
            tunaData[message.Address].mag.z =      (float)message.Values[8].DoubleValue;

            //Debug.Log("2: " + tunaData[message.Address].accel.x);

        }

        if (message.Address == _firstTuna)
        {
            if (_lastUpdate == null) _lastUpdate = DateTime.Now;
            else
            {
                updateRate = (DateTime.Now - _lastUpdate).TotalMilliseconds;
                _lastUpdate = DateTime.Now;
            }
        }
    }

    private bool recording = false;
    private string filename = "";
    private List<string> _lines;
    private string sep = "$";
    private void startRecording() {

        if (tunaData.Count > 0 && !recording) {

            filename = Application.dataPath + "/Recordings/Recording" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
                recording = true;
                _lines = new List<string>();
        }
    }

    private void endRecording() {

        if (!recording) return;

        recording = false;

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename)) {
            foreach (string line in _lines) {
                    file.WriteLine(line);
            }
            Debug.Log("file recorded: " + filename);

        }
    }


    private void OnGUI()
    {
        if (displayTunas)
        {
            int left = Screen.width - 200;
            int top = 10;
            int step = 10;


            foreach (KeyValuePair<string, TunaInfo> t in tunaData)
            {
                GUI.Label(new Rect(left, top, 500, step), t.Key, s); top += step;
                GUI.Label(new Rect(left + 10, top, 500, step), "accel: " + t.Value.accel.ToString(), s); top += step;
                GUI.Label(new Rect(left + 10, top, 500, step), "gyro: " + t.Value.gyro.ToString(), s); top += step;
                GUI.Label(new Rect(left + 10, top, 500, step), "mag: " + t.Value.mag.ToString(), s); top += step;
            }
        }

        if (recordingGUI) {
            if (!recording) {
                if (GUI.Button(new Rect(10, 10, 100, 100), "Start Recording")) {
                    startRecording();
                }
            }
            else {
                if (GUI.Button(new Rect(10, 10, 100, 100), "End Recording")) {
                    endRecording();
                }
            }
            
         }
    }
}
