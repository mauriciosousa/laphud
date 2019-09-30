using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GesturesType
{
	PINCH, 
	DRAG,
	TILT,
	NONE
}

public class TouchGestures : MonoBehaviour {

	public Tuio tuio;
	public GesturesType gesture;

	public Transform obj;

	private Vector2 _lastDrag;
	public float dragFactor = 1f;

	private float _lastPinchDistance;
	//public float pinchFactor = 1f;

	private Vector2 _lastTilt;
	public float tiltFactor = 1f;

	void Start () {

		gesture = GesturesType.NONE;
		_lastDrag = Vector2.zero;
		_lastPinchDistance = 0f;
	}




	void Update () {

		int numberOfTouches = tuio.Cursors.Length;

		if (numberOfTouches == 1) {

			Vector2 touch = new Vector2(tuio.Cursors [0].X, tuio.Cursors [0].Y);

			if (gesture != GesturesType.DRAG) {

				_lastDrag = touch;
				gesture = GesturesType.DRAG;
			
			} else {

				float d = Vector2.Distance (touch, _lastDrag);
				Vector2 dir = touch - _lastDrag;
				dir.Normalize();

				obj.localPosition = obj.localPosition + obj.parent.forward * d * dragFactor * -dir.y;

				_lastDrag = touch;
			}



		} else if (numberOfTouches == 2) {
		
			Vector2 touch0 = new Vector2(tuio.Cursors [0].X, tuio.Cursors [0].Y);
			Vector2 touch1 = new Vector2(tuio.Cursors [1].X, tuio.Cursors [1].Y);

			float distance = Vector2.Distance (touch0, touch1);

			if (gesture != GesturesType.PINCH) {

				_lastPinchDistance = distance;
				gesture = GesturesType.PINCH;

			} else {
		
				float d = distance / _lastPinchDistance;
				obj.localScale = obj.localScale * d;
			
				_lastPinchDistance = distance;
			}
				

		} else if (numberOfTouches == 3){

			Vector2 touch0 = new Vector2(tuio.Cursors [0].X, tuio.Cursors [0].Y);
			Vector2 touch1 = new Vector2(tuio.Cursors [1].X, tuio.Cursors [1].Y);
			Vector2 touch2 = new Vector2(tuio.Cursors [2].X, tuio.Cursors [2].Y);

			Vector2 touch = (touch0 + touch1 + touch2) / 3;

			if (gesture != GesturesType.TILT) {

				_lastTilt = touch;
				gesture = GesturesType.TILT;

			} else {

				float d = Vector2.Distance (touch, _lastTilt);
				Vector2 dir = touch - _lastTilt;
				dir.Normalize();

				//obj.localPosition = obj.localPosition + obj.forward * d * dragFactor * -dir.y;

				obj.Rotate (d * -dir.y * tiltFactor, 0, 0);

				_lastTilt = touch;
			}

		} else {
			
			gesture = GesturesType.NONE;

		}
			
	}

	public bool showDebug;
	public GUIStyle font;
	void OnGUI()
	{
		if (gesture != GesturesType.NONE) {
		
			GUI.Label (new Rect (10, Screen.height - 100, 500, 30), "" + gesture, font);
		
		}
	}
}
