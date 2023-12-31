﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(CXAnimationCurve), true)]
public class CXAnimationCurveInspector : Editor 
{
	/// 禁用 系统 按钮 事件 
	[MenuItem("CONTEXT/CXAnimationCurve/Remove Component")]
	static void CONTEXT_RemoveComponent (MenuCommand command){Debug.Log("command \"Remove Component\" in CXAnimation is invalid");}
	[MenuItem("CONTEXT/CXAnimationCurve/Copy Component")]
	static void CONTEXT_CopyComponent (MenuCommand command){Debug.Log("command \"Copy Component\" in CXAnimation is is invalid");}
	[MenuItem("CONTEXT/CXAnimationCurve/Reset")]
	static void CONTEXT_Reset (MenuCommand command){Debug.Log("command \"Reset\" in CXAnimation is invalid");}
	[MenuItem("CONTEXT/CXAnimationCurve/Revert to Prefab")]
	static void CONTEXT_RevertToPrefab (MenuCommand command){Debug.Log("command \"Revert to Prefab\" in CXAnimation is invalid");}
	[MenuItem("CONTEXT/CXAnimationCurve/Move Up")]
	static void CONTEXT_MoveUp (MenuCommand command){Debug.Log("command \"Move Up\" in CXAnimation is invalid");}
	[MenuItem("CONTEXT/CXAnimationCurve/Move Down")]
	static void CONTEXT_MoveDown (MenuCommand command){Debug.Log("command \"Move Down\" in CXAnimation is invalid");}

	/// 禁用 系统 按钮 事件 
	[MenuItem("CONTEXT/CXAnimationCurve/Remove All CX Animation Curve If No Board")]
	static void CONTEXT_RemoveAllComponentIfNoBoard (MenuCommand command)
	{
		CXAnimationCurve curve = command.context as CXAnimationCurve;
		if (curve != null)
		{
			if (curve.gameObject.GetComponent<CXAnimationBoard>() == null)
			{
				EditorApplication.delayCall = delegate 
				{
					CXAnimationCurve[] curves = curve.gameObject.GetComponents<CXAnimationCurve>();
					foreach (CXAnimationCurve c in curves)
					{
						DestroyImmediate(c);
					}
				};
			}
		}
	}


	public override void OnInspectorGUI ()
	{
	}

}