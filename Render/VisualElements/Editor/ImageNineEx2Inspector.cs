﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
namespace CXEditor
{
	[CustomEditor(typeof(ImageNineEx2))]
	public class ImageNineEx2Inspector : Editor 
	{
		ImageNineEx2 _this;

		void Awake ()
		{
			_this = (ImageNineEx2)this.target;
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			if (GUILayout.Button("open editor"))
			{
				ImageNineEx2Editor.Show(_this);
			}
		}
	}
}