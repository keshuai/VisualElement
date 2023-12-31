﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using CX;

namespace CXEditor
{
	[InitializeOnLoad]
	[CustomEditor(typeof(Label))]
	public class LabelInspector : RectVEInspector 
	{
		static LabelInspector ()
		{
			DrawcallInspector.AddSupportedType<View, Label>();
		}
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			Label _this = (Label)this.target;

			EditorUILayout.BoolPair("Lock size", "Width", ref _this.lockWidth, "Height", ref _this.lockHeight);
			//_this.lockWidth = EditorUILayout.BoolField("Lock Width", _this.lockWidth);
			//_this.lockHeight = EditorUILayout.BoolField("Lock Height", _this.lockHeight);

			EditorUILayout.EmptyLine();

			_this.spacingX = EditorUILayout.IntFieldPlus("Spacing X", _this.spacingX);
			_this.spacingY = EditorUILayout.IntFieldPlus("Spacing Y", _this.spacingY);

			_this.alignmentX = (LabelAlignmentX)EditorUILayout.EnumPopupField("Alignment X", _this.alignmentX);
			_this.alignmentY = (LabelAlignmentY)EditorUILayout.EnumPopupField("Alignment Y", _this.alignmentY);

			EditorUILayout.EmptyLine();

			_this.color = EditorUILayout.ColorField("Color", _this.color);
			_this.effect = (LabelEffect)EditorUILayout.EnumPopupField("Effect", _this.effect);
			if (_this.effect != LabelEffect.None)
			{
				_this.effectColor = EditorUILayout.ColorField("Effect Color", _this.effectColor);
			}
			if (_this.effect == LabelEffect.Shadow || _this.effect == LabelEffect.Outline)
			{
				_this.effectOffset = EditorUILayout.Vector2Field("Effect Offset", _this.effectOffset);
			}
				
			EditorUILayout.EmptyLine();

			View view = _this.Drawcall as View;
			if (view == null)
			{
				EditorGUILayout.HelpBox("No View", MessageType.Info);
			}
			else
			{
				List<Font> fontList = view.fontSelectList;
				if (fontList.Count == 0)
				{
					EditorGUILayout.HelpBox("View no font to select", MessageType.Info);
				}
				else
				{
					EditorUILayout.FontField("TTF", _this.TTFFont, fontList, SelectFont);
				}
			}
			_this.fontStyle = (FontStyle)EditorUILayout.EnumPopupField("Font Style", _this.fontStyle);
			_this.fontSize = EditorUILayout.IntFieldPlus("FontSize", _this.fontSize);
			_this.Text = EditorUILayout.TextArea("Text", _this.Text);

			//EditorUtility.SetDirty(_this);
		}

		void SelectFont (Object o)
		{
			Label _this = (Label)this.target;
			_this.TTFFont = o as Font;
		}
	}
}