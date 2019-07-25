using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WebCamSource))]
public class EditorGUILayoutPopup : Editor
{
    string[] cams
    {
        get
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            List<string> opts = new List<string>();
            for (int i = 0; i < devices.Length; i++)
            {
                opts.Add(devices[i].name);
            }
            return opts.ToArray();
        }
    }
    public override void OnInspectorGUI()
    {
        WebCamSource source = (WebCamSource)target;
        EditorUtility.SetDirty(source);

        source.cam = EditorGUILayout.Popup("Available Webcams:", source.cam, cams);
        source.Resolution = (WebcamModes)EditorGUILayout.Popup("Resolutions:", (int)source.Resolution, System.Enum.GetNames(typeof(WebcamModes)));
    }
}