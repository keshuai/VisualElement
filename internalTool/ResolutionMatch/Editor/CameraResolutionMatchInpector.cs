﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraResolutionMatch))]
public class CameraResolutionMatchInpector : Editor 
{
	CameraResolutionMatch _this;

	void Awake ()
	{
		_this = (CameraResolutionMatch)this.target;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (Application.isPlaying && PrefabUtility.GetPrefabType(_this) != PrefabType.Prefab)
		{
			_this.ScreenWidth = EditorGUILayout.IntField("Screen Width", _this.ScreenWidth);
			_this.ScreenHeight = EditorGUILayout.IntField("Screen Height", _this.ScreenHeight);
		}

		_this.Type = (CameraResolutionMatch.MatchType)EditorGUILayout.EnumPopup("Match Type", _this.Type);
		_this.Width = EditorGUILayout.IntField("Match Width", _this.Width);
		_this.Height = EditorGUILayout.IntField("Match Height", _this.Height);

		if (!_this.GetComponent<Camera>().orthographic)
		{
			_this.Distance = EditorGUILayout.FloatField("Match Distance", _this.Distance);
		}


		EditorGUILayout.FloatField ("Scale Rate X", _this.MatchScaleX);
		EditorGUILayout.FloatField ("Scale Rate Y", _this.MatchScaleY);

		if (GUILayout.Button("Apply", GUILayout.Height(50)))
		{
			_this.Apply();
		}
	}
}