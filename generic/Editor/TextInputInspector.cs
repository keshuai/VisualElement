using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using CX;

namespace CXEditor
{
	[CustomEditor(typeof(TextInput))]
	public class TextInputInspector : Editor 
	{
		TextInput _this;

		private void Awake()
		{
			_this = this.target as TextInput;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			_this.CursorFlickerOnceTime = EditorUILayout.FloadField("CursorFlickerOnceTime", _this.CursorFlickerOnceTime);
			EditorUILayout.Size2DField("Selection", new Vector2(_this.Selection.start, _this.Selection.length));
			_this.Size = EditorUILayout.Size2DField("W/H", _this.Size);
			_this.OnInputEdit = EditorUILayout.BoolField("On Input Edit", _this.OnInputEdit);
			_this.secure = EditorUILayout.BoolField("Secure", _this.secure);
			_this.Text = EditorUILayout.TextArea("Text", _this.Text);
		}
	}
}