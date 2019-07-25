using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WebcamFPS : MonoBehaviour {

    public VideoSource source;
    public int FPS;

    public Text AverageFPSText, CurrentFPSText;
    public string AverageFPSLabel = "Average FPS: ", 
                  CurrentFPSLabel = "Current FPS: ";

    public float averageFPS
    {
        get
        {
            return (TotalUpdates > 0) ? FPSSum / TotalUpdates : 0;
        }
    }
    public float currentFPS
    {
        get
        {
            return _currentFPS;
        }
    }

    private float FPSSum = 0;
    private uint TotalUpdates = 0;
    private float _currentFPS = 0;
    private bool firstFrame = true;

    private DateTime lastDatetime;

    public void DisplayAverage()
    {
        AverageFPSText.text = AverageFPSLabel + ((int)averageFPS).ToString();
    }

    public void DisplayCurrent()
    {
        CurrentFPSText.text = CurrentFPSLabel + ((int)currentFPS).ToString();
    }

    public void Reset()
    {
        FPSSum = 0;
        TotalUpdates = 0;
        _currentFPS = 0;
        firstFrame = true;
    }

    // Update is called once per frame
    void Update () {
		if (source.texture != null &&
            source.isPlaying &&
            source.didUpdateThisFrame)
        {
            DateTime now = DateTime.Now;
            if (firstFrame)
            {
                firstFrame = false;
            }
            else
            {
                //float Delta = Time.time - LastUpdate;
                //_currentFPS = Mathf.Round(1f / Delta);

                double dateTimeFPS = 1000.0 / now.Subtract(lastDatetime).TotalMilliseconds;
                _currentFPS = (float)dateTimeFPS;

                FPSSum += _currentFPS;
                TotalUpdates++;

            }
            //LastUpdate = Time.time;
            lastDatetime = now;
            DisplayAverage();
            DisplayCurrent();

        }
    }
}
