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
	[CustomEditor(typeof(ImageNormalVE))]
	public class ImageNormalVEInspector : ImageVEInspector 
	{
		static ImageNormalVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, ImageNormalVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();
			ImageNormalVE _this = (ImageNormalVE)this.target;

			if (_this.Drawcall == null)
			{
				return;
			}
			_this.Pivot = EditorUILayout.Size2DField("Pivot", _this.Pivot, Vector2.zero);
		}
	}
}