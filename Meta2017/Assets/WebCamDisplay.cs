using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamDisplay : MonoBehaviour {

    public WebCamSource source;
    private Renderer rendererComponent;


	// Use this for initialization
	void Start () {
        rendererComponent = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (source.texture != null &&
            source.isPlaying &&
            source.didUpdateThisFrame)
        {
            rendererComponent.material.mainTexture = source.texture;
        }
	}
}
