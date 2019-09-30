using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using extOSC;

public class Tuio : MonoBehaviour {


	public OSCReceiver Receiver;
	private string tuioAddress = "/tuio/2Dcur";


	private Dictionary<int, TuioCursor> cursors;
	private List<TuioCursor> updatedCursors;
	private List<int> addedCursors;
	private List<int> removedCursors;

	public TuioCursor[] Cursors
	{
		get { 
			TuioCursor[] c = new TuioCursor[cursors.Values.Count];
			cursors.Values.CopyTo (c, 0);
			return c;
		}
	}

	public int FrameNumber { get; private set; }

	void Start () {
	
		Receiver.Bind (tuioAddress, ReceiveMessage);

		cursors = new Dictionary<int, TuioCursor> ();

		updatedCursors = new List<TuioCursor> ();
		addedCursors = new List<int> ();
		removedCursors = new List<int> ();

	}
		
	void Update()
	{


	}

	private void ReceiveMessage(OSCMessage message)
	{
		string command = message.Values [0].StringValue;

		if (command == "set") {

			if (message.Values.Count < 7)
				return;

			var id = message.Values [1].IntValue;
			var xPos = message.Values [2].FloatValue;
			var yPos = message.Values [3].FloatValue;
			var velocityX = message.Values [4].FloatValue;
			var velocityY = message.Values [5].FloatValue;
			var acceleration = message.Values [6].FloatValue;

			TuioCursor cursor;
			if (!cursors.TryGetValue (id, out cursor))
				cursor = new TuioCursor (id);
			cursor.Update (xPos, yPos, velocityX, velocityY, acceleration);
			updatedCursors.Add (cursor);
		
		} else if (command == "alive") {
		
			var total = message.Values.Count;
			for (var i = 1; i < total; i++) {
				addedCursors.Add (message.Values [i].IntValue);
			}
			foreach (KeyValuePair<int, TuioCursor> value in cursors) {
				if (!addedCursors.Contains (value.Key)) {
					removedCursors.Add (value.Key);
				}
				addedCursors.Remove (value.Key);
			}
		
		} else if (command == "fseq") {

			if (message.Values.Count < 2) return;

			FrameNumber = message.Values[1].IntValue;
			var count = updatedCursors.Count;
			for (var i = 0; i < count; i++)
			{
				var updatedCursor = updatedCursors[i];
				if (addedCursors.Contains(updatedCursor.Id) && !cursors.ContainsKey(updatedCursor.Id))
				{
					cursors.Add(updatedCursor.Id, updatedCursor);

					//Debug.Log ("Cursor Added");
					//if (CursorAdded != null) CursorAdded(this, new TuioCursorEventArgs(updatedCursor));
				}
				else
				{
					//if (CursorUpdated != null) CursorUpdated(this, new TuioCursorEventArgs(updatedCursor));
					//Debug.Log ("Cursor Updated");
				}
			}
			count = removedCursors.Count;
			for (var i = 0; i < count; i++)
			{
				var cursorId = removedCursors[i];
				TuioCursor cursor = cursors[cursorId];
				cursors.Remove(cursorId);

				//Debug.Log ("Cursor Removed");
				//if (CursorRemoved != null) CursorRemoved(this, new TuioCursorEventArgs(cursor));
			}

			addedCursors.Clear();
			removedCursors.Clear();
			updatedCursors.Clear();
		}
	}

	public bool showDebug;
	void OnGUI()
	{
		if (showDebug) {
		
			int left = 10;
			int top = 10;
			int step = 25;

			GUI.Label ( new Rect(left, top, 500, 25), "# Cursors: " + cursors.Count); top += 2 * step;

			foreach (TuioCursor t in cursors.Values) {
			
				string line = "" + t.Id;
				line += "  " + t.X;
				line += "  " + t.Y; 


				GUI.Label (new Rect (left, top, 500, 25), line); top += step;

			
			}
		}
	}
}
	
