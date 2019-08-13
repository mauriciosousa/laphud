using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HUDInfo
{
    private string _message;
    private Color _color;
    private double _milliseconds;
    public DateTime initTime;
    public HUDInfo(string message, Color color, double milliseconds)
    {
        Message = message;
        Color = color;
        Milliseconds = milliseconds;
        initTime = DateTime.Now;
        Debug.Log("new " + message);
    }

    public string Message
    {
        get
        {
            return _message;
        }

        set
        {
            _message = value;
        }
    }

    public Color Color
    {
        get
        {
            return _color;
        }

        set
        {
            _color = value;
        }
    }

    public double Milliseconds
    {
        get
        {
            return _milliseconds;
        }

        set
        {
            _milliseconds = value;
        }
    }
}

public class HUDMessage : MonoBehaviour {

    private List<HUDInfo> _messages;

    private HUDInfo displaying = null;

    void Start()
    {
        _messages = new List<HUDInfo>();
    }

    public void add(string m, Color c, double milliseconds)
    {
        _messages.Add(new HUDInfo(m, c, milliseconds));
    }

    private void Update()
    {
        if (_messages.Count > 0)
        {
            if (displaying == null)
            {
                displaying = _messages[0];
                _messages.RemoveAt(0);


                print("trying to display: " + displaying.Message);
                GetComponent<TextMesh>().text = displaying.Message;
                GetComponent<TextMesh>().color = displaying.Color;
            }
            else
            {
                _clear();
            }
        }
        else if (displaying != null) 
        {
            _clear();
        }
    }

    private void _clear()
    {

        if ((DateTime.Now - displaying.initTime).TotalMilliseconds > displaying.Milliseconds)
        {
            GetComponent<TextMesh>().text = "";
            displaying = null;
        }
    }
}
