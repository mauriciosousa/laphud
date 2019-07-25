using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDistance : MonoBehaviour {

    public GameObject leftEye, rightEye, distanceIndicator;
    public float distance = 0.0f;
    public float distanceStep = 0.005f;
    public float maxDistance = 2.0f, minDistance = -2.0f;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKey(KeyCode.LeftControl) && rightEye != null && leftEye != null)
        {
            if (Input.GetMouseButtonDown(2)) // CTRL+MOUSEWHEEL-CLICK = RESET DISTANCE
            {
                distance = 0.0f;
            }
            else // CTRL+MOUSEWHEEL-SCROLL = VARY DISTANCE BETWEEN EYES
            {
                float b = Input.GetAxis("Mouse ScrollWheel");
                if (b > 0) distance = distance + distanceStep > maxDistance ? maxDistance : distance + distanceStep;
                if (b < 0) distance = distance - distanceStep < minDistance ? minDistance : distance - distanceStep;
            }
            leftEye.transform.localPosition = Vector3.zero + Vector3.right * distance * leftEye.transform.lossyScale.x;
            rightEye.transform.localPosition = Vector3.zero - Vector3.right * distance * rightEye.transform.lossyScale.x;
            if (distanceIndicator != null)
                distanceIndicator.SetActive(distance == 0.0f);
        }
    }
}
