using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {


    private bool follow = false;
    public float scale = 0.2f;
    public bool flipUD = false;
    public bool flipLR = false;
    public float distance = 1.0f;
    public float distanceStep = 0.1f;
    public float scrollWheel = 0.0f;
    public float minDistance = 0.3f;
    public float maxDistance = 2f;
    private float mousewheel = 0.0f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) 
        {
            follow = !follow;
        }
        if (follow)
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                mousewheel = Input.GetAxis("Mouse ScrollWheel");
                if (mousewheel < 0) distance = (distance + distanceStep > maxDistance) ? maxDistance : distance + distanceStep;
                if (mousewheel > 0) distance = (distance - distanceStep < minDistance) ? minDistance : distance - distanceStep;
                if (mousewheel != 0.0f)
                {
                    transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
                    transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 2, new Vector3(0, flipUD ? -1 : 1, 0));
                    transform.localScale = new Vector3(flipLR ? -scale : scale, scale, 1);
                }
            }
        }
    }
}
