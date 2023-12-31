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
	[CustomEditor(typeof(ImageFourColorVE))]
	public class ImageFourColorVEInspector : ImageVEInspector 
	{
		static ImageFourColorVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, ImageFourColorVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			ImageFourColorVE _this = (ImageFourColorVE)this.target;
			if (_this.Drawcall == null)
			{
				return;
			}

			_this.TopLeftColor = EditorUILayout.ColorField("Top Left", _this.TopLeftColor);
			_this.TopRightColor = EditorUILayout.ColorField("Top Right", _this.TopRightColor);
			_this.BottomLeftColor = EditorUILayout.ColorField("Bottom Left", _this.BottomLeftColor);
			_this.BottomRightColor = EditorUILayout.ColorField("Bottom Right", _this.BottomRightColor);
		}
	}
}