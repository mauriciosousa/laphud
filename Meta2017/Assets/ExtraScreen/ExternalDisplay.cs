using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
//using System.Management;

public class ExternalDisplay : MonoBehaviour {

    [SerializeField]
    public int DisplayNum = 0;

	// Use this for initialization
	void Start () {

        Vector2Int p1, p2;
        Debug.Log("Detected " + System.Windows.Forms.Screen.AllScreens.Length + " displays. If you believe this number is incorrect, please restart Unity and try again.");
        for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[i];
            char[] name = screen.DeviceName.ToCharArray();
            string val = "";
            foreach (char c in name)
                val += char.GetNumericValue(c).ToString() + ", ";
            Debug.Log("Screen " + i + ", name: " + val + ", position: " + screen.Bounds);
        }

        if (System.Windows.Forms.Screen.AllScreens.Length < 2)
        {
            p1 = new Vector2Int(0, 0);
            p2 = new Vector2Int(320, 500);
        }
        else
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[1];
            p1 = new Vector2Int(screen.Bounds.X, screen.Bounds.Y + 2);
            p2 = new Vector2Int(screen.Bounds.Width, screen.Bounds.Height);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
