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
	[CustomEditor(typeof(ImageNineScaleVE))]
	public class ImageNineScaleVEInspector : ImageVEInspector 
	{
		static ImageNineScaleVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, ImageNineScaleVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();


			ImageNineScaleVE _this = (ImageNineScaleVE)this.target;
			if (_this.Drawcall == null)
			{
				return;
			}
		}
	}
}