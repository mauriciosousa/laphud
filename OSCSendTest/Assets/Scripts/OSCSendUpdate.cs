using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using System;

public class OSCSendUpdate : MonoBehaviour {

    [Header("OSC Settings")]
    public OSCTransmitter Transmitter;

    public bool sending = false;

    [Header("Send data")]
    public Vector3 Accelerometer;
    public Vector3 Gyroscope;
    public Vector3 Magnetometer;

    
    void Start () {

        Application.runInBackground = true;
		
	}
	
	void Update () {

        if (sending)
        {
            Accelerometer = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
            Gyroscope = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
            Magnetometer = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));


            for (int i = 0; i < 15; i++)
            {

                var message = new OSCMessage("/tuna" + i);

                message.AddValue(OSCValue.Float(Accelerometer.x));
                message.AddValue(OSCValue.Float(Accelerometer.y));
                message.AddValue(OSCValue.Float(Accelerometer.z));

                message.AddValue(OSCValue.Float(Gyroscope.x));
                message.AddValue(OSCValue.Float(Gyroscope.y));
                message.AddValue(OSCValue.Float(Gyroscope.z));

                message.AddValue(OSCValue.Float(Magnetometer.x));
                message.AddValue(OSCValue.Float(Magnetometer.y));
                message.AddValue(OSCValue.Float(Magnetometer.z));

                Transmitter.Send(message);

            }

        }
	}


}
