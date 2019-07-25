using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class Stack : MonoBehaviour {

    [Range(1,100)]
    public int stackSize = 1;
    private LinkedList<RenderTexture> textureStack;
    private LinkedListNode<RenderTexture> outputNode;
    private LinkedListNode<RenderTexture> inputNode;
    // TO-DO: Abstrack the input and output types, 
    //        and make them Lists for multiple inputs and outputs
    public VideoPlayer inputPlayer;
    public List<Material> outputMaterials;
    public List<Material> unlaggedMaterials;



    public int lastFrame = -1;

    public float frameRate = 0;

    private bool isStackFilled = false;

    void initStack(int stackSize)
    {
        textureStack = new LinkedList<RenderTexture>();
        for (int i = 0; i < stackSize; i++)
        {
            RenderTexture texture = new RenderTexture(1280, 720, 24);
            textureStack.AddFirst(texture);
        }
        
    }

	// Use this for initialization
	void Start () {
        initStack(stackSize);
        inputNode = textureStack.First;
        inputPlayer.targetTexture = inputNode.Value;
        outputNode = textureStack.First;
        inputPlayer.Play();
        foreach (Material mat in unlaggedMaterials)
        {
            mat.SetTexture("_MainTex", inputNode.Value);
        }
    }

	// Update is called once per frame
	void Update () {
        if (inputPlayer.isPrepared)
            frameRate = inputPlayer.frameRate;

        if (inputPlayer.frame != lastFrame)
        {
            if (inputPlayer.frame > 0)
            {
                foreach (Material mat in unlaggedMaterials)
                {
                    mat.SetTexture("_MainTex", inputNode.Value);
                }
            }

            //Debug.Log(inputPlayer.frame);

            lastFrame = (int)inputPlayer.frame;
            inputNode = (inputNode.Next == null) ? textureStack.First : inputNode.Next;
            inputPlayer.targetTexture = inputNode.Value;

            if (isStackFilled)
            {
                foreach (Material mat in outputMaterials)
                {
                    mat.SetTexture("_MainTex", outputNode.Value);
                }
                outputNode = (outputNode.Next == null) ? textureStack.First : outputNode.Next;
            }
            else
            {
                if (inputNode == textureStack.Last.Previous || stackSize < 2)
                {
                    isStackFilled = true;
                }
            }
            
        }
    }
}
