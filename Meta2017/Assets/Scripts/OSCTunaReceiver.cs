using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using System;

public class TunaInfo
{
    public Vector3 accel;
    public Vector3 gyro;
    public Vector3 mag;

    public TunaInfo()
    {
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

    void Start()
    {
        Application.runInBackground = true;

        string tunaAddress;
        tunaData = new Dictionary<string, TunaInfo>();
        for (int i = 0; i < tunaIds.Count; i++)
        {
            tunaAddress = "/tuna" + tunaIds[i];
            tunaData[tunaAddress] = new TunaInfo();
            Receiver.Bind(tunaAddress, ReceivedMessage);

            if (i == 0) _firstTuna = tunaAddress;
        }
    }

    private void ReceivedMessage (OSCMessage message) {

        if (tunaData.ContainsKey(message.Address))
        {
            tunaData[message.Address].accel.x = message.Values[0].FloatValue;
            tunaData[message.Address].accel.y = message.Values[1].FloatValue;
            tunaData[message.Address].accel.z = message.Values[2].FloatValue;

            tunaData[message.Address].gyro.x = message.Values[3].FloatValue;
            tunaData[message.Address].gyro.y = message.Values[4].FloatValue;
            tunaData[message.Address].gyro.z = message.Values[5].FloatValue;

            tunaData[message.Address].mag.x = message.Values[6].FloatValue;
            tunaData[message.Address].mag.y = message.Values[7].FloatValue;
            tunaData[message.Address].mag.z = message.Values[8].FloatValue;
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
    }
}
