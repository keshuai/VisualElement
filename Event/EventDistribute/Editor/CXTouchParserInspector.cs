﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CXTouchParser))]
public class CXTouchParserInspector : Editor 
{
	CXTouchParser _this;
	public override bool RequiresConstantRepaint ()
	{
		return true;
	}

	void Awake ()
	{
		_this = (CXTouchParser)this.target;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();


		EditorGUILayout.LabelField("Current Touch Count: "+ _this.TouchCount);
		for (int i = 0; i < _this.TouchCount; ++i)
		{
			this.ShowTouch(_this.GetCurrentTouch(i));
		}

	}

	private void ShowTouch (CXTouch touch)
	{
		if (touch == null)
		{
			EditorGUILayout.LabelField("Out of touch count");
		}
		else
		{
			EditorGUILayout.LabelField("Finger id: " +touch.FingerId);

			CXTouchEvent focusEvent = touch.FocusEvent;
			if (focusEvent != null)
			{
				Object o = focusEvent as Object;
				if (o != null)
				{
					EditorGUILayout.LabelField("FocusEvent");
					EditorGUILayout.ObjectField(o.name, o, typeof(Object), true);
				}
			}

			List<CXTouchEvent> eventList = EasyReflect.GetField<List<CXTouchEvent>>(touch, "EventList");
			if (eventList != null)
			{
				foreach (CXTouchEvent e in eventList)
				{
					Object o = e as Object;
					if (o != null)
					{
						EditorGUILayout.ObjectField(o.name, o, typeof(Object), true);
					}
				}
			}
		}
	}
}