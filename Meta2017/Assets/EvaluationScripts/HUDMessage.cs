using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class HUDMessage : MonoBehaviour {

    private TextMesh _text;

    void Start()
    {
    }

    public void display(string message, Color color)
    {
        if (_text == null) _text = GetComponent<TextMesh>();

        _text.text = message;
        _text.color = color;
    }

    public void clear()
    {
        _text.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            display("lololol", Color.green);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            clear();
        }
    }
}
