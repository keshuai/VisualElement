﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;


public class EasyReflectTest : MonoBehaviour 
{

	static void StaticMethod ()
	{
		Debug.Log("StaticMethod called");
	}

	static bool StaticMethodBool ()
	{
		Debug.Log("StaticMethodBool called");
		return true;
	}

	void Method ()
	{
		Debug.Log("Method called");
	}

	bool MethodBool ()
	{
		Debug.Log("MethodBool called");
		return true;
	}

	bool Filed;

	static bool StaticFiled;

	bool Property
	{
		get 
		{
			Debug.Log("Get Property");
			return true;
		}
		set
		{
			Debug.Log("Set Property");
		}
	}

	static bool StaticProperty
	{
		get 
		{
			Debug.Log("Get StaticProperty");
			return true;
		}
		set
		{
			Debug.Log("Set StaticProperty");
		}
	}

	// 开始
	void Start () 
	{
	
	}
	
	// 每帧更新
	void Update () 
	{
	
	}
}
#endif