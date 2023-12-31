﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor (typeof (CXTransformValueCenter))]
public class CXTransformValueCenterInspector : CXEditorInspector 
{
	CXTransformValueCenter _this;
	private List<CXTransformValueCurve> AnimationCurveList;

	void Awake ()
	{
		_this = this.target as CXTransformValueCenter;
		AnimationCurveList = EasyReflect.GetField<List<CXTransformValueCurve>>(_this, "AnimationCurveList");
	}
	public override bool RequiresConstantRepaint ()
	{
		return true;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		EditorGUILayout.LabelField( "Curve Count: " + _this.CurveCount );

		if (this.AnimationCurveList != null)
		{
			CXTransformValueCurve curve;
			UnityEngine.Object attachObject;
			Type type;
			string curveTypeName;
			AnimationCurve u3dCurve;
			for (int i = 0; i < this.AnimationCurveList.Count; ++i)
			{
				curve = this.AnimationCurveList[i];
				if (curve != null)
				{
					attachObject = curve.AttachObject;
					type = attachObject == null? null : attachObject.GetType();
					curveTypeName = curve.GetType().Name;

					u3dCurve = EasyReflect.GetAnyField<AnimationCurve>(curve);

					EditorGUILayout.LabelField("====== " + i + " ======");

					EditorGUILayout.LabelField(curveTypeName);
					EditorGUILayout.ObjectField(attachObject, type,true);
					if (u3dCurve!= null)
					{
						CXEditorInspector.CurveField(u3dCurve);
					}

					GUILayout.Space(10f);
				}

			}

		}

	}
}