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
	[CustomEditor(typeof(ImageNineExtVE))]
	public class ImageNineExtVEInspector : ImageVEInspector 
	{
		static ImageNineExtVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, ImageNineExtVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			ImageNineExtVE _this = (ImageNineExtVE)this.target;
			if (_this.Drawcall == null)
			{
				return;
			}

			_this.Color = EditorUILayout.ColorField("color", _this.Color);
			_this.Hor = EditorUILayout.IntFieldPlus("Hor", _this.Hor);
			_this.Ver = EditorUILayout.IntFieldPlus("Ver", _this.Ver);
		}
	}
}