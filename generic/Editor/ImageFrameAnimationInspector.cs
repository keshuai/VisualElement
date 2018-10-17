using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CX;
namespace CXEditor
{

	[CustomEditor(typeof(ImageFrameAnimation))]
	public class ImageFrameAnimationInspector : Editor 
	{
		ImageFrameAnimation _this;

		private void Awake()
		{
			_this = this.target as ImageFrameAnimation;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			_this.onceTime = EditorUILayout.FloadField("Once time", _this.onceTime);
		}
	}
}
