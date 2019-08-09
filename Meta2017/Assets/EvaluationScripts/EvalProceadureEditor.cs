using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EvalProceadure))]
public class EvalProceadureEditor : Editor {

	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();


		EvalProceadure myScript = (EvalProceadure)target;

		GUILayout.Space (10);

		if (myScript.repetition == 0) {
			EditorGUILayout.LabelField ("CONFIG SETTINGS:");

			if (GUILayout.Button ("Retrieve Conditions From Config", GUILayout.Width (200))) {
				myScript.loadConfig ();
			}

			GUILayout.Space (10);
		}
		EditorGUILayout.LabelField ("STATUS VARS:");

		if (myScript.repetition > 0) {
			EditorGUILayout.LabelField ("Repetition:", myScript.repetition.ToString ());
		}

		if (myScript.evalState == EvalState.IN_SESSION) {
			EditorGUILayout.LabelField ("Elapsed Time:", (DateTime.Now - myScript.LastTimeStamp).TotalSeconds.ToString());
		}

		EditorGUILayout.LabelField ("LAG:", myScript.lag.ToString());
		EditorGUILayout.LabelField ("Eval State:", myScript.evalState.ToString());


	}

}
