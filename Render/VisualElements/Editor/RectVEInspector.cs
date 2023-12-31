﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
namespace CXEditor
{
	[CustomEditor(typeof(RectVE))]
	public class RectVEInspector : VEleInspector 
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			RectVE _this = (RectVE)target;
			if (_this.Drawcall == null)
			{
				return;
			}


			bool fit = false;

			Vector2 size = new Vector2(_this.Width, _this.Height);

			ImageNormalVE imageElement = _this as ImageNormalVE;
			if (imageElement == null)
			{
				size = EditorUILayout.Size2DField("W/H", size);
			}
			else
			{
				size = EditorUILayout.Size2DField("W/H", size, ref fit);
			}
			if (fit)
			{
				imageElement.FitImageSize();
			}
			else
			{
				_this.Width = size.x;
				_this.Height = size.y;
			}
		}
	}
}