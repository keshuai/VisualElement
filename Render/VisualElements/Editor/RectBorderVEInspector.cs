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
	[CustomEditor(typeof(RectBorderVE))]
	public class RectBorderVEInspector : RectVEInspector 
	{

		static RectBorderVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, RectBorderVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();
			RectBorderVE _this = (RectBorderVE)target;

			if (_this.Drawcall == null)
			{
				return;
			}
			
			bool toDefault = false;

			Vector2 size = new Vector2(_this.BorderWidth, _this.BorderHeight);
			size = EditorUILayout.Size2DField("Border W/H", size, ref toDefault, "2");
			
			if (toDefault)
			{
				_this.BorderWidth = 2;
				_this.BorderHeight = 2;
			}
			else
			{
				_this.BorderWidth = size.x;
				_this.BorderHeight = size.y;
			}

			_this.Color = EditorUILayout.ColorField("color", _this.Color);
		}
	}
}
