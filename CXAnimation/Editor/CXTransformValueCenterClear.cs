using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CXTransformValueCenterClear 
{
	static CXTransformValueCenterClear()
	{
		Debug.Log("+ OnUnityPlayModeChanged");
		EditorApplication.hierarchyChanged += hierarchyChanged;
		EditorApplication.pauseStateChanged += pauseStateChanged;
		EditorApplication.playModeStateChanged += playModeStateChanged;
		EditorApplication.projectChanged += projectChanged;
		EditorApplication.update += OnEditorUpdate;
	}
	private static void hierarchyChanged ()
	{
		Debug.Log("hierarchyChanged");
	}
	private static void pauseStateChanged (PauseState s)
	{
		Debug.Log("pauseStateChanged: " + s);
	}
	private static void playModeStateChanged(PlayModeStateChange s)
	{
		Debug.Log("playModeStateChanged: " + s);
		if (EditorApplication.isCompiling)
		{
			Debug.Log("isCompiling");



		}
	}
	private static void projectChanged ()
	{
		Debug.Log("projectChanged");
	}



    static void OnEditorUpdate()
    {
		if(EditorApplication.isPlaying && EditorApplication.isCompiling)// && EditorApplication.isPlaying)
        {
			Debug.Log("isCompiling");
			EditorApplication.isPaused = true;

            //UnityEngine.Object.DestroyImmediate(CXTransformValueCenter.Instance);
        }
    }
}
