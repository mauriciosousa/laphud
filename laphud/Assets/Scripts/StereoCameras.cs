using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoCameras : MonoBehaviour {

    public List<GameObject> quads;

	void Start () {

        quads = new List<GameObject>();

        WebCamDevice[] devices = WebCamTexture.devices;

        print("# devices: " + devices.Length);

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name.Contains("USB"))
            {
                print("" + i + " " + devices[i].name);
                GameObject quad = _getQuad();
                quad.name = "quad (" + devices[i].name + ")";
                quads.Add(quad);

                WebCamTexture t = new WebCamTexture();
                t.deviceName = devices[i].name;
                Renderer renderer = quad.GetComponent<Renderer>();
                renderer.material.mainTexture = t;
                t.Play();
            }
        }
	}

    private GameObject _getQuad()
    {
        GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        q.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        q.transform.localScale = new Vector3(1.6f, 0.9f, 1);

        return q;
    }
}
