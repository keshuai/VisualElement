﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EasyReflectTest))]
public class EasyReflectTestInspector : Editor 
{
	EasyReflectTest _this;

	void Awake ()
	{
		_this = (EasyReflectTest)this.target;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUILayout.Button("StaticMethod"))
		{
			EasyReflect.CallStaticMethod<EasyReflectTest>("StaticMethod");
		}

		if (GUILayout.Button("StaticMethodBool"))
		{
			bool v = EasyReflect.CallStaticMethod<bool, EasyReflectTest>("StaticMethodBool");
			Debug.Log( "get " + v);
		}

		if (GUILayout.Button("Method"))
		{
			EasyReflect.CallMethod(_this, "Method");
		}

		if (GUILayout.Button("MethodBool"))
		{
			bool v = EasyReflect.CallMethod<bool>(_this, "MethodBool");
			Debug.Log( "get instance " + v);
		}

		if (GUILayout.Button("Filed"))
		{
			EasyReflect.SetField(_this, "Filed", true);
			Debug.Log("SetField");

			bool v = EasyReflect.GetField<bool>(_this, "Filed");
			Debug.Log( "GetField " + v);
		}

		if (GUILayout.Button("StaticFiled"))
		{
			EasyReflect.SetStaticField(_this.GetType(), "StaticFiled", true);

			bool v = (bool)EasyReflect.GetStaticField(_this.GetType(), "StaticFiled");
			Debug.Log( "get StaticFiled" + v);
		}

		if (GUILayout.Button("Property"))
		{
			EasyReflect.SetProperty(_this, "Property", true);

			bool v = (bool)EasyReflect.GetProperty(_this, "Property");
			Debug.Log( "GetProperty" + v);
		}

		if (GUILayout.Button("StaticProperty"))
		{
			EasyReflect.SetStaticProperty(_this.GetType(), "StaticProperty", true);

			bool v = (bool)EasyReflect.GetStaticProperty(_this.GetType(), "StaticProperty");
			Debug.Log( "get StaticProperty" + v);
		}

	}
}