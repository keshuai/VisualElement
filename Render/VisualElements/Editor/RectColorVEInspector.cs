﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;

namespace CXEditor
{
	[InitializeOnLoad]
	[CustomEditor(typeof(RectColorVE))]
	public class ColorEleInspector : RectVEInspector 
	{

		static ColorEleInspector ()
		{
			DrawcallInspector.AddSupportedType<View, RectColorVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();
			RectColorVE _this = (RectColorVE)target;

			if (_this.Drawcall == null)
			{
				return;
			}

			_this.Color = EditorUILayout.ColorField("color", _this.Color);
		}
	}
}
