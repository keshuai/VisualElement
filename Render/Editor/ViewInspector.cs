/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Collections.Generic;

namespace CXEditor
{
	[CustomEditor(typeof(View))]
	public class ViewInspector : DrawcallInspector
	{
		View _this;

		CXInspectorLayoutColorFrame m_AssetsFrame = new CXInspectorLayoutColorFrame("Assets");
		CXInspectorLayoutColorFrame[] m_FrameArray;
		CXInspectorLayoutColorFrame[] frameArray
		{
			get
			{
				if (m_FrameArray == null)
				{
					m_FrameArray = new CXInspectorLayoutColorFrame[View.assetMaxCount];
					for (int i = 0; i < View.assetMaxCount; ++i)
					{
						m_FrameArray[i] = new CXInspectorLayoutColorFrame("Asset " + i);
						m_FrameArray[i].NoSwitch = true;
					}
				}

				return m_FrameArray;
			}
		}

		// button +
		//protected GUIContent m_IconToolbarPlus;

		//List<System.Type> m_SupportedTypeList = new List<System.Type>() { typeof(ViewAsset), typeof(ViewAsset) };

		protected override void Awake ()
		{
			base.Awake();
			_this = (View)this.target;

			//m_IconToolbarPlus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"));// "+"
		}

		public override void OnInspectorGUI()
		{
			this.InspectorAssets();
			base.OnInspectorGUI();
		}
		
		private void InspectorAssets ()
		{
			if (m_AssetsFrame.Begin())
			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				this.InspercAsset(i);
			}
			m_AssetsFrame.End();
		}

		private void InspercAsset(int index)
		{
			ViewAsset asset = _this.GetAsset(index);
			CXInspectorLayoutColorFrame frame = frameArray[index];
			if (asset == null) frame.title = "Asset " + index + " Empty NULL";
			else if (asset.assetType == ViewAssetType.Empty) frame.title = "Asset " + index + " [Empty]";
			else if (asset.assetType == ViewAssetType.ImageAtlas) frame.title = "Asset " + index + " [Image Atlas]";
			else if (asset.assetType == ViewAssetType.TTF) frame.title = "Asset " + index + " [TTF Font]";
			else frame.title = "Asset " + index + " Unknown";
			frame.Begin();
			if (asset == null)
			{
				this.DrawAddButton(index);
			}
			
			
			asset.assetType = (ViewAssetType)EditorUILayout.EnumPopupField("Asset Type", asset.assetType);
			
			if (asset.assetType == ViewAssetType.ImageAtlas)
			{
				System.Action<Object> selectAtlas = delegate (Object atlas)
				{
					_this.SetImageAtlas(index, atlas as ImageAtlas);
				};
				_this.SetImageAtlas(index, EditorUILayout.ImageAtlasField("Image Atlas", asset.GetImageAtlas(), selectAtlas));
			}
			else if (asset.assetType == ViewAssetType.TTF)
			{
				_this.SetTTF(index, (Font)EditorUILayout.ObjectField("ttf Font", asset.GetTTFFont(), typeof(Font)));
			}
			else if (asset.assetType == ViewAssetType.Empty)
			{
				EditorGUILayout.HelpBox("Asset Empty", MessageType.Info);
			}
			
			frame.End();
		}

		private void DrawAddButton(int index)
		{
			EditorGUILayout.HelpBox("Click + to Add Image or Font", MessageType.Info);

			// 计算添加按钮（x, y, width, hight）
			Rect rect = GUILayoutUtility.GetRect(m_IconToolbarPlus, GUI.skin.button);
			//const float addButonWidth = 150f;
			//rect.x = rect.x + (rect.width - addButonWidth) / 2;
			//rect.width = addButonWidth;

			// 添加项按钮
			//if (GUI.Button(rect, m_IconToolbarPlus))
			{
				
			}
			
			float space = 20;
			float width = (rect.width - space * 3) / 2;
			Rect rect_image = new Rect(rect.x + space, rect.y, width, rect.height);
			Rect rect_font = new Rect(rect_image.x + width + space, rect.y, width, rect.height);
			
			if (GUI.Button(rect_image, "image"))
			{ 
				_this.GetAsset(index).assetType = ViewAssetType.ImageAtlas;
			}
			if (GUI.Button(rect_font, "font"))
			{
				_this.GetAsset(index).assetType = ViewAssetType.TTF;
			}
		}

		void SelectAtIndex(System.Type t)
		{
			//m_AddType = t;
		}
		private void InsperctEmpty(int index)
		{
			this.DrawAddButton(index);
		}

	}
}