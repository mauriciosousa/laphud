using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebcamPreview : EditorWindow
{
    public bool mirror = false;
    public GameObject source;

    private bool hasPlayed = false;
    private bool windowPositioned = false;
    private Vector2Int p1, p2;
    private Texture webcamTex;

    static WebcamPreview window;

    [MenuItem("VIMMI/Webcam")]
    static void ShowWindow()
    {
        window = (WebcamPreview)EditorWindow.GetWindow(typeof(WebcamPreview));        
        window.init();
    }

    void setWindowPosition(int x, int y, int w, int h)
    {
        Rect r = position;
        r.x = x;
        r.y = y;
        r.width = w + 2;
        r.height = h + 2;
        position = r;
        minSize = new Vector2(position.width, position.height);
        maxSize = minSize * 2;
    }

    public void init(int display = 0, bool fullscreen = false)
    {
        if (display > System.Windows.Forms.Screen.AllScreens.Length)
        {
            display = 0;
            fullscreen = false;
        }

        source = GameObject.Find("WebcamDisplay");
        if (fullscreen)
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[display];
            p1 = new Vector2Int(screen.Bounds.X, screen.Bounds.Y + 2);
            p2 = new Vector2Int(screen.Bounds.Width, screen.Bounds.Height);
        }
        else
        {
            p1 = new Vector2Int(0, 0);
            p2 = new Vector2Int(320, 500);
        }

        setWindowPosition(p1.x, p1.y, p2.x, p2.y);
    }

    void OnGUI()
    {

        if (window == null)
        {
            window = (WebcamPreview)EditorWindow.GetWindow(typeof(WebcamPreview));
        }
        else
        {
            setWindowPosition(p1.x, p1.y, p2.x, p2.y);
        }

        // SHOW CAM FEED
        if (webcamTex != null) 
        {
            if (!windowPositioned)
            {
                setWindowPosition(p1.x, p1.y, p2.x, p2.y);
                windowPositioned = true;
            }
            GUI.DrawTexture(new Rect(mirror?position.width:0, 0, position.width * (mirror?-1:1), position.height), webcamTex);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close Window", GUILayout.Width(300)))
            {
                Close();
            }
        }
        // SHOW GUI
        else
        {
            EditorGUILayout.LabelField("Mirror the webcam output");
            mirror = EditorGUILayout.Toggle(mirror);
            EditorGUILayout.LabelField("\n");
            this.source = (GameObject) EditorGUILayout.ObjectField("Webcam Quad", this.source, typeof(GameObject), true, GUILayout.Width(300));
            EditorGUILayout.LabelField("\n");
            p1 = EditorGUILayout.Vector2IntField("Set the position:", p1, GUILayout.Width(300));
            //p2 = EditorGUILayout.Vector2IntField("Set the size:", p2);
            if (GUILayout.Button("Accept new position", GUILayout.Width(300)))
            {
                setWindowPosition(p1.x, p1.y, p2.x, p2.y);
            }

            EditorGUILayout.LabelField("\n");
            if (GUILayout.Button("Close Window", GUILayout.Width(300)))
            {
                Close();
            }
        }
    }


    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            hasPlayed = true;
            if (source != null)
            {
                if (webcamTex == null)
                    //webcamTex = GameObject.Find("webcamsource").GetComponent<webcamsource>().webcamTex;
                    webcamTex = source.GetComponent<MeshRenderer>().material.mainTexture;
                else
                {
                    Repaint();
                }
            }
        }
        //else
        //{
        //    if (hasPlayed)
        //    {
        //        windowPositioned = false;
        //    }
        //}
    }
}