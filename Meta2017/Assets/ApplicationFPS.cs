using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationFPS : MonoBehaviour {

    public Text appFPSText;
    public string appFPSLabel = "Application: ";
    float previousTime;
    bool first = true;

    private float timeSinceLast = 0, FPS = 0;
    private float cummulativeFPS = 0;
    private float totalFrames = 0;
    private float averageFPS
    {
        get
        {
            return totalFrames > 0 ? cummulativeFPS / totalFrames : 0;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (first)
        {
            first = false;
            previousTime = Time.time;
        }
        else
        {
            timeSinceLast = Time.time - previousTime;
            FPS = 1f / timeSinceLast;
            cummulativeFPS += FPS;
            totalFrames++;
            previousTime = Time.time;
        }

        appFPSText.text = appFPSLabel + ((int)averageFPS).ToString();
	}
}
