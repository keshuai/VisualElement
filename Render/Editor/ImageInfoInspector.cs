﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;

namespace CXEditor
{

	[CustomEditor(typeof(ImageInfo))]
	public class ImageInfoInspector : Editor 
	{
		ImageInfo _this;
		CXInspectorLayoutColorFrame m_BaseInfoFrame = new CXInspectorLayoutColorFrame();
		CXInspectorLayoutColorFrame m_TrimInfoFrame = new CXInspectorLayoutColorFrame();
		CXInspectorLayoutColorFrame m_NineInfoFrame = new CXInspectorLayoutColorFrame();

		void NotifyImageInfoChanged ()
		{
			ImageVE[] imageElements = GameObject.FindObjectsOfType<ImageVE>();
			foreach(ImageVE element in imageElements)
			{
				if (element.ImageInfo == _this)
				{
					//element.OnEditorUVChanged();
					System.Reflection.MethodInfo methodInfo = typeof(ImageVE).GetMethod("OnEditorUVChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
					methodInfo.Invoke(element, null);
				}
			}
		}

		void Awake ()
		{
			_this = (ImageInfo)this.target;
			m_BaseInfoFrame.title = "Base Info (Read Only)";
			m_TrimInfoFrame.title = "Trim Info (Read Only)";
			m_NineInfoFrame.title = "Nine Info";
			m_BaseInfoFrame.NoSwitch = true;
			m_TrimInfoFrame.NoSwitch = true;
			m_NineInfoFrame.NoSwitch = true;
		}
		public override void OnInspectorGUI ()
		{
			if (_this.atlas == null)
			{
				EditorGUILayout.HelpBox("No Atlas, fixed it in image atlas.", MessageType.Error);
				return;
			}

			EditorUILayout.SetLabelWidth();

			m_BaseInfoFrame.Begin();
			EditorUILayout.ImageAtlasField("Atlas", _this.atlas);
			EditorUILayout.IntField("ID", _this.id);
			EditorUILayout.Size2DField("Size", new Vector2(_this.width, _this.height));
			m_BaseInfoFrame.End();

			m_TrimInfoFrame.Begin();
			EditorUILayout.IntField("Left", _this.trimLft);
			EditorUILayout.IntField("Right", _this.trimRgt);
			EditorUILayout.IntField("Top", _this.trimTop);
			EditorUILayout.IntField("Bottom", _this.trimBtm);
			m_TrimInfoFrame.End();

			Color defaultColor = GUI.backgroundColor;
			m_NineInfoFrame.Begin();
			{
				int visualWidth = _this.width - _this.trimLft - _this.trimRgt;
				int visualHeight = _this.height - _this.trimTop - _this.trimBtm;

				int nineLft, nineRgt, nineTop, nineBtm;

				GUI.backgroundColor = ImageInfoPreview.s_LftLineColor;
				nineLft = Mathf.Clamp(EditorUILayout.IntFieldPlus("Left", _this.nineLft), 0, visualWidth);

				GUI.backgroundColor = ImageInfoPreview.s_RgtLineColor;
				nineRgt = Mathf.Clamp(EditorUILayout.IntFieldPlus("Right", _this.nineRgt), 0, visualWidth);

				GUI.backgroundColor = ImageInfoPreview.s_TopLineColor;
				nineTop = Mathf.Clamp(EditorUILayout.IntFieldPlus("Top", _this.nineTop), 0, visualHeight);

				GUI.backgroundColor = ImageInfoPreview.s_BtmLineColor;
				nineBtm = Mathf.Clamp(EditorUILayout.IntFieldPlus("Bottom", _this.nineBtm), 0, visualHeight);
			
				if (_this.nineLft != nineLft || _this.nineRgt != nineRgt || _this.nineTop != nineTop || _this.nineBtm != nineBtm)
				{
					_this.nineLft = nineLft;
					_this.nineRgt = nineRgt;
					_this.nineTop = nineTop;
					_this.nineBtm = nineBtm;

					this.NotifyImageInfoChanged();
				}
			}
			m_NineInfoFrame.End();
			GUI.backgroundColor = defaultColor;
		}
		public override bool HasPreviewGUI ()
		{
			return _this.atlas != null;
		}

		public override void OnInteractivePreviewGUI (Rect r, GUIStyle background)
		{
			ImageInfoPreview.DrawPreview(_this, r);
		}
	}
}