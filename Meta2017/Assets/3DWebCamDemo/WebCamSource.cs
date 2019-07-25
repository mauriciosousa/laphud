using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//public class textureQueue
//{
//    private Queue<RenderTexture> texturePool;
//    public Queue<RenderTexture> queue = new Queue<RenderTexture>();
//    private int width, height;
//    public int _size = 0;
//    public int size
//    {
//        get
//        {
//            return _size;
//        }
//        set
//        {
//            //if (value != _size)
//            //{
//                if (value < 0)
//                    _size = 0;
//                else
//                    _size = value;
//            //}
//            //else
//            //{
//            //    frameCounter = 0;
//            //}
//        }
//    }
//    public bool isCleared
//    {
//        get
//        {
//            return queue.Count == 0;
//        }
//    }

//    public int sizeIncr = 1, framesIncr = 20;
//    public int sizeDecr = 1, framesDecr = 20;
//    public int frameCounter = 0;

//    public void Clear()
//    {
//        while (queue.Count > 0)
//        {
//            texturePool.Enqueue(queue.Dequeue());
//        }
//    }

//    public textureQueue(int width, int height, int poolSize = 30)
//    {
//        this.width = width;
//        this.height = height;

//        int size = (poolSize < 1) ? 1 : poolSize;
//        texturePool = new Queue<RenderTexture>(size);
//        for (int i = 0; i < size; i++)
//        {
//            RenderTexture r = new RenderTexture(width, height, 0);
//            r.Create();
//            texturePool.Enqueue(r);
//        }
//    }

//    private RenderTexture getPooledTexture()
//    {
//        if (texturePool.Count > 0)
//        {
//            return texturePool.Dequeue();
//        }
//        else
//        {
//            RenderTexture r = new RenderTexture(width, height, 0);
//            r.Create();
//            return r;
//        }
//    }

//    public RenderTexture Get()
//    {
//        int s = size;
//        if (queue.Count == s)
//        {
//            frameCounter = 0;
//            return queue.Dequeue();
//        }
//        else
//        {
//            if (queue.Count > s)
//            {
//                RenderTexture t = queue.Dequeue();
//                if (queue.Count > s && frameCounter % framesDecr == 0)
//                {
//                    for (int i = 0; i < sizeDecr; i++)
//                    {
//                        texturePool.Enqueue(t);
//                        t = queue.Dequeue();
//                        if (queue.Count == s) break;
//                    }
//                }
//                texturePool.Enqueue(t);
//                return t;
//            }
//            else
//            //if (queue.Count < s)
//            {
//                if (frameCounter % framesIncr == 0)
//                {
//                    return queue.Peek();
//                }
//                else
//                {
//                    return queue.Dequeue();
//                }
//            }
//        }
//    }

//    public void Add(Texture t)
//    {
//        frameCounter++;
//        if (queue.Count > _size)
//        {
//            RenderTexture pooledTexture = getPooledTexture();
//            Graphics.CopyTexture(t, pooledTexture);
//            queue.Enqueue(pooledTexture);
//        }
//        else
//        {
//            RenderTexture pooledTexture = getPooledTexture();
//            Graphics.CopyTexture(t, pooledTexture);
//            queue.Enqueue(pooledTexture);
//        }
//    }
//}
[System.Serializable]
public enum WebcamModes
{
    _320x240x120 = 0,
    _320x240x60,
    _320x240x30,
    _640x480x120,
    _640x480x60,
    _640x480x30,
    _800x600x120,
    _800x600x60,
    _800x600x30,
    _1280x720x60,
    _1280x720x30,
    _1280x1024x60,
    _1280x1024x30,
    _1920x1080x60,
    _1920x1080x30
}
[System.Serializable]
public enum Webcams
{
    Lenovo_EasyCamera = 0,
    HD_USB_Camera_1 = 1
}

public class WebCamSource : VideoSource
{
    public override int width
    {
        get
        {
            if (isPlaying)
                return texture.width;
            else
                return 0;
        }
    }
    public override int height
    {
        get
        {
            if (isPlaying)
                return texture.height;
            else
                return 0;
        }
    }
    public override bool isPlaying
    {
        get
        {
            return webcam.isPlaying;
        }
    }
    public override bool didUpdateThisFrame
    {
        get
        {
            return webcam.didUpdateThisFrame;
        }
    }
    public override Texture texture
    {
        get
        {
            return webcam;
        }
    }

    public int cam = 0;
    [SerializeField]
    public WebcamModes Resolution = 0;
    //public List<GameObject> targets = new List<GameObject>();
    //public Webcams Camera = Webcams.HD_USB_Camera_1;
    //public List<string> Devices = new List<string>();
    //public UnityEngine.UI.Text text;
    //Texture2D output;
    //Color32[] data;
    //private float ratio;
    private WebCamTexture webcam;

    private bool initialized = false;
    private void Debug_AvailableDevices()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Available Devices (" + devices.Length.ToString() + "): ");
        foreach (WebCamDevice device in devices)
        {
            //Devices.Add(device.name);
            Debug.Log(device.name);
        }
    }
    private void Debug_CopyTextureSupport()
    {
        CopyTextureSupport info = SystemInfo.copyTextureSupport;
        Debug.Log("CopyTextureSupport: " + info);
    }
    private void setWebcamParameters()
    {
        if (webcam == null || webcam.isPlaying)
        {
            return;
        }

        string[] parts = Resolution.ToString().Split('x');
        int width, height, framerate;
        if (int.TryParse(parts[0].Substring(1), out width) &&
            int.TryParse(parts[1], out height) &&
            int.TryParse(parts[2], out framerate))
        {
            webcam.requestedWidth = width;
            webcam.requestedHeight = height;
            webcam.requestedFPS = framerate;
        }
    }
    private void Quit(string msg)
    {
        Debug.Log(msg);
        Application.Quit();
    }
    IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
        #if (UNITY_EDITOR)
            Debug_AvailableDevices();
            Debug_CopyTextureSupport();
        #endif
            WebCamDevice[] devices = WebCamTexture.devices;
            int len = devices.Length;
            if (len < 1)
                Quit("No webcams detected, quitting application");
            Application.targetFrameRate = 300;
            webcam = new WebCamTexture();
            webcam.deviceName = devices[cam < len ? cam : 0].name;
            setWebcamParameters();
            webcam.Play();
            initialized = true;
        }
        else
            Quit("No permission to use webcam, quitting application.");
    }
}