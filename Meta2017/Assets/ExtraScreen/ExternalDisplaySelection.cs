using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;


[CustomEditor(typeof(ExternalDisplay))]
public class EditorGUIExternalDisplaySelector : Editor
{
    private System.Windows.Forms.Screen screen = null;
    private WebcamPreview window = null;
    string[] displays
    {
        get
        {
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            List<string> opts = new List<string>();
            for (int i = 0; i < screens.Length; i++)
            {
                string str = "Display :" + i;
                   if (screens[i].Primary) str += " (Primary)";
                if (screens[i].Bounds.Contains(System.Windows.Forms.Control.MousePosition)) str += "(Current)";
                opts.Add(str);
            }
            return opts.ToArray();
        }
    }
    public override void OnInspectorGUI()
    {

        ExternalDisplay source = (ExternalDisplay)target;
        EditorUtility.SetDirty(source);

        EditorGUILayout.HelpBox("The display where the mouse pointer is located will be denoted as (Current) in the following dropdown list.", MessageType.Info, true);

        source.DisplayNum = EditorGUILayout.Popup("Available Displays:", source.DisplayNum, displays);
        GUILayout.Label("");
        if (GUILayout.Button("Show External Window"))
        {
            if (window == null)
            {
                window = (WebcamPreview)EditorWindow.GetWindow(typeof(WebcamPreview), true, "Webcam Preview");
                window.init(source.DisplayNum, true);
            }
            else
                window.Show();
        }
        if (GUILayout.Button("Hide External Window"))
        {
            if (window != null)
                window.Close();
        }
    }
}