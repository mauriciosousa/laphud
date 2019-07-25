using System;
using UnityEngine;

public abstract class VideoSource : MonoBehaviour
{
    public abstract int width { get; }
    public abstract int height { get; }
    public abstract bool isPlaying { get; }
    public abstract bool didUpdateThisFrame { get; }

    public abstract Texture texture { get; }
}
