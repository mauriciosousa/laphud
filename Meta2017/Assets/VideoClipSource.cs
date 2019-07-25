using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoClipSource : VideoSource {
    public override int width
    {
        get
        {
            if (video.isPrepared)
                return video.texture.width;
            else
                return 0;
        }
    }
    public override int height
    {
        get
        {
            if (video.isPrepared)
                return video.texture.height;
            else
                return 0;
        }
    }
    public override bool isPlaying
    {
        get
        {
            return video.isPlaying;
        }
    }
    public override bool didUpdateThisFrame
    {
        get
        {
            return video.frame > lastFrame;
        }
    }
    public override Texture texture
    {
        get
        {
            if (video.isPrepared)
                return video.targetTexture;
            else
                return null;
        }
    }

    private VideoPlayer video;
    private long lastFrame = -1;

    // Use this for initialization
    void Start () {
        video = GetComponent<VideoPlayer>();
        video.Play();
	}

    private void LateUpdate()
    {
        if (video.frame > lastFrame)
            lastFrame = video.frame;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
