using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accessory class that holds a list of ints and implements a Next and Reset method.
/// Used to simulate an easing function for the queues.
/// </summary>
public class QueueGetList
{
    #region Private Variables

    private List<int> _queueGetList = new List<int>();
    private readonly int Peek = 0;
    private int idx = 0;

    #endregion
    
    #region Public Variables
    public int Count
    {
        get
        {
            return (_queueGetList == null) ? -1 : _queueGetList.Count;
        }
    }
    public int this[int i]
    {
        get { return _queueGetList[i]; }
        set { _queueGetList[i] = value; }
    }
    public int Next
    {
        get
        {
            if (Count < 1)
            {
                return Peek;
            }
            else
            {
                int i = idx;
                idx = ++idx % Count;
                return _queueGetList[i];
            }
        }
    }

    #endregion

    #region Public Methods
    public void Reset()
    {
        idx = 0;
    }
    public QueueGetList(List<int> list)
    {
        _queueGetList = list;
    }
    public QueueGetList(int[] arr)
    {
        _queueGetList = new List<int>(arr);
    }
    public override string ToString()
    {
        string str = "";
        foreach (int q in _queueGetList)
            str += q + ",";
        return str;
    }

    #endregion
}

/// <summary>
/// Texture Queue. Gets frames from a Webcam or Video source, retains those frames in a 
/// queue, and sets them as the main texture of a given Renderer's material.
/// Varying the rate at which frames are witheld in the queue can simulate video lag.
/// </summary>
public class WebCamQueue : MonoBehaviour {

    #region Public Variables

    public List<int> DelayIncreaseSequence;
    public List<int> DelayDecreaseSequence;
    public uint Delay, RenderQueueSize, WaitingQueueSize;
    public VideoSource Source;
    public Renderer Target;

    public bool mirror = false;

    #endregion

    #region Private Variables

    private static int WaitingQueuePreallocation = 120;
    public bool WaitingQueueInitialized = false,
                 passingThrough = false;
    private Queue<RenderTexture> RQ = new Queue<RenderTexture>(WaitingQueuePreallocation),
                                 WQ = new Queue<RenderTexture>(WaitingQueuePreallocation);
    private QueueGetList incrList, decrList, noneList;
    private bool sourceIsReady
    {
        get
        {
            return Source != null && Source.texture != null && Source.isPlaying;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initialize the Waiting Queue
    /// </summary>
    /// <returns></returns>
    private void InitializeWaitingQueue()
    {
        if (WaitingQueueInitialized || !sourceIsReady) return;
        WQ = PreAllocateQueue(WQ, Source.width, Source.height, 0, WaitingQueuePreallocation);
        WaitingQueueInitialized = true;
    }
    /// <summary>
    /// Pre-allocate a given number of Render Textures into a given Render Texture Queue
    /// </summary>
    /// <param name="rtq">A Render Texture Queue.</param>
    /// <param name="width">Render Texture width.</param>
    /// <param name="height">Render Texture height.</param>
    /// <param name="depth">Render Texture depth (0, 16 or 24). Default = 0</param>
    /// <param name="nallocs">Number of Render Textures to create. Default = 60.</param>
    /// <returns>The pre-allocated Render Texture Queue.</returns>
    private Queue<RenderTexture> PreAllocateQueue(Queue<RenderTexture> rtq, int width, int height, int depth = 0, int nallocs = 60)
    {
        if (rtq == null)
            rtq = new Queue<RenderTexture>(nallocs);

        for (int i = 0; i < nallocs; i++)
        {
            RenderTexture rt = new RenderTexture(Source.width, Source.height, 0);
            rt.Create();
            rtq.Enqueue(rt);
        }

        return rtq;
    }
    /// <summary>
    /// Pushes a Texture to the Render Queue.
    /// </summary>
    /// <param name="tex">Texture to be pushed to the Render Queue</param>
    /// <param name="width">Texture width</param>
    /// <param name="height">Texture height</param>
    /// <param name="depth">Texture depth (0, 16 or 24), Default 0</param>
    /// <returns></returns>
    private RenderTexture PushRenderQueue(Texture tex, int width, int height, int depth = 0)
    {
        RenderTexture rt = WQ.Count > 0 ? WQ.Dequeue() : new RenderTexture(width, height, depth);
        if (!rt.IsCreated()) rt.Create();
        Graphics.CopyTexture(tex, rt);
        RQ.Enqueue(rt);
        return rt;
    }
    /// <summary>
    /// Pop the Render Queue a given number of times
    /// </summary>
    /// <param name="npops">The number of times to pop the render queue. 0 = Peek.</param>
    /// <returns>The last RenderTexture popped from the Render Queue.</returns>
    private RenderTexture PopRenderQueue(int npops)
    {
        RenderTexture rt = RQ.Peek();
        for (int i = 0; i < npops; i++)
        {
            WQ.Enqueue(rt = RQ.Dequeue());
            if (RQ.Count == Delay)
            {
                if (Delay == 0)
                    passingThrough = false;
                break;
            }
        }
        return rt;
    }
    /// <summary>
    /// Pop the RenderQueue according to the given QueueGetList
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private RenderTexture Next(QueueGetList list)
    {
        return PopRenderQueue(list.Next);
    }
    /// <summary>
    /// Checks if a given integer List is null or empty, and constructs a fall-back List of length len consisting of 1's and ending with finalElement.
    /// </summary>
    /// <param name="def">A List of integers</param>
    /// <param name="len">The length for the fall-back list.</param>
    /// <param name="finalElement">The final element to be put on the fall-back list.</param>
    /// <returns>Returns def if def is neither null nor empty, or returns a new integer List of length len.</returns>
    private List<int> Sequence(List<int> def, int len, int finalElement)
    {
        if (def == null || def.Count < 1)
        {
            List<int> l = new List<int>(len);
            for (int i = 0; i < len - 1; i++) l.Add(1);
            l.Add(finalElement);
            return l;
        }
        else
        {
            return def;
        }
    }
    /// <summary>
    /// Display the current source texture directly on the display without passing through the Render Queue
    /// </summary>
    private void DisplaySourceTexture()
    {
        if (!passingThrough)
        {
            Target.material.mainTexture = Source.texture;
            passingThrough = true;
        }
    }
    /// <summary>
    /// Add the current source texture to the back of the Render Queue and display the texture at the top of the Render Queue.
    /// </summary>
    private void DisplayQueueTexture()
    {
        PushRenderQueue(Source.texture, Source.width, Source.height);
        Target.material.mainTexture = (RQ.Count == Delay + 1) ? Next(noneList) :
                                      (RQ.Count <= Delay) ? Next(incrList) : Next(decrList);
        if (RQ.Count == Delay)
        {
            incrList.Reset();
            decrList.Reset();
        }
    }
    /// <summary>
    /// Set the Targets dimensions to match the Source's image ratio
    /// </summary>
    private void setDisplayRatio()
    {
        if (sourceIsReady && Source.height > 0)
        {
            float ratio = (float)Source.width / (float)Source.height;
            Target.transform.localScale = new Vector3( mirror ? -ratio : ratio, transform.localScale.y, transform.localScale.z);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adjust Delay based on User Inputs
    /// </summary>
    public void HandleInputs()
    {
        // Increase Delay
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Delay = Delay < 30 ? Delay + 1 : 30;
            passingThrough = false;
        }

        // Decrease Delay
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Delay = Delay > 0 ? Delay - 1 : 0;
        }
    }



    #endregion

    #region Unity Methods

    void Start()
    {
        noneList = new QueueGetList(Sequence(null, 1, 1));
        decrList = new QueueGetList(Sequence(DelayDecreaseSequence, 10, 2));
        incrList = new QueueGetList(Sequence(DelayIncreaseSequence, 10, 0));
    }
    void Update () {
		if (!sourceIsReady)
        {
            return;
        }
        else
        {
            HandleInputs();
            InitializeWaitingQueue();
            if (Source.didUpdateThisFrame)
            {
                setDisplayRatio();
                if (Delay == 0 && RQ.Count == 0)
                {
                    DisplaySourceTexture();
                }
                else
                {
                    DisplayQueueTexture();
                }
            #if (UNITY_EDITOR)
                RenderQueueSize = (uint)RQ.Count;
                WaitingQueueSize = (uint)WQ.Count;
            #endif
            }
        }
	}

    #endregion
}
