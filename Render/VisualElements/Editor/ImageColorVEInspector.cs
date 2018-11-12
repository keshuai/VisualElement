/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;

namespace CXEditor
{
	[InitializeOnLoad]
	[CustomEditor(typeof(ImageColorVE))][CanEditMultipleObjects]
	public class ImageColorVEInspector : ImageVEInspector 
	{
		static ImageColorVEInspector ()
		{
			DrawcallInspector.AddSupportedType<View, ImageColorVE>();
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			ImageColorVE _this = (ImageColorVE)this.target;

			if (_this.Drawcall == null)
			{
				return;
			}

			_this.Color = EditorUILayout.ColorField("Color", _this.Color);
		}
	}
}