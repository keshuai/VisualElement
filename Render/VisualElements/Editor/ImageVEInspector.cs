﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Collections.Generic;


namespace CXEditor
{
	[CustomEditor(typeof(ImageVE))]
	public class ImageVEInspector : RectVEInspector 
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			ImageVE _this = (ImageVE)this.target;
			if (_this.Drawcall == null)
			{
				return;
			}

			if (_this.ImageDrawCall == null)
			{
				EditorGUILayout.HelpBox("ImageVE Must Has AtlasDrawcall", MessageType.Warning);
				return;
			}

			this.DrawImageInfoFiled();

			/*

			if (_this.ImageDrawCall.Atlas == null )
			{


				if (_this.ImageInfo == null)
				{
					EditorGUILayout.TextField("ImageInfo(no atlas)", "");
				}
				else
				{
					EditorGUILayout.TextField("ImageInfo(no atlas)", _this.ImageInfo.name);
				}
			}
			else
			{
				string[] iamgeNameArray = _this.ImageDrawCall.Atlas.iamgeNameArray;
				if (iamgeNameArray.Length == 0)
				{
					if (_this.ImageInfo == null)
					{
						EditorGUILayout.TextField("ImageInfo(empty atlas)", "");
					}
					else
					{
						EditorGUILayout.TextField("ImageInfo(empty atlas)", _this.ImageInfo.name);
					}
				}
				else
				{
					//_this.ImageInfo  = CXEditorInspector.PopupField<ImageInfo>("ImageInfo", _this.ImageInfo, _this.ImageDrawCall.Atlas.ImageInfoArray, iamgeNameArray);
					this.DrawImageInfoFiled(_this.ImageInfo);
				}
			}
			*/
		}

		void SelectImageInfo (ImageInfo imageInfo)
		{
			ImageVE _this = (ImageVE)this.target;
			_this.ImageInfo = imageInfo;
		}

		void DrawImageInfoFiled ()
		{
			ImageVE _this = (ImageVE)this.target;
			ImageInfo imageInfo = _this.ImageInfo;

			List<ImageInfo> imageInfoLsit = new List<ImageInfo>();

			View drawcall = _this.Drawcall as View;
			if (drawcall != null)
			{
				imageInfoLsit.AddRange(drawcall.imageInfoList);
			}

			_this.ImageInfo = EditorUILayout.ImageInfoField("ImageInfo", imageInfo, imageInfoLsit, SelectImageInfo);
		}
	}


}