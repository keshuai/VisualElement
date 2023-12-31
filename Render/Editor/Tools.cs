﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using CX;
using Object = UnityEngine.Object;

namespace CXEditor
{
	public class Tools 
	{
		public static string AssetsParentPath
		{
			get
			{
				return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
			}
		}

		public static bool IsPrefab (UnityEngine.Object o)
		{
			return PrefabUtility.GetPrefabType(o) == PrefabType.Prefab;
		}
		public static void Destroy (UnityEngine.Object o)
		{
			if (IsPrefab(o))
			{
				UnityEngine.Object.DestroyImmediate(o, true);
			}
			else
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(o);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(o);
				}
			}
		}

		public static T GetWizardWindowInstance<T> () where T : ScriptableWizard
		{
			return GetWizardWindowInstance<T>(typeof(T).Name);
		}

		public static T GetWizardWindowInstance<T> (string windowTitle) where T : ScriptableWizard
		{
			T[] allThisWindow = Resources.FindObjectsOfTypeAll (typeof(T)) as T[];
			if (allThisWindow == null || allThisWindow.Length == 0)
			{
				return ScriptableWizard.DisplayWizard<T>(windowTitle);
			}
			else
			{
				allThisWindow[0].Focus();
				return allThisWindow[0];
			}
		}
	}

	/// 复制粘贴的值的存放处 
	public static class ValueBuffer
	{
		public static UnityEngine.Object Buffer_Object = null;
		public static UnityEngine.Color Buffer_Color = Color.white;
	}


	public static class EditorUI
	{
		static Texture2D s_WhiteTexture;
		public static Texture2D WhiteTexture
		{
			get
			{
				if (s_WhiteTexture == null)
				{
					Texture2D tex = new Texture2D(1,1);
					tex.SetPixels(new Color[]{Color.white});
					tex.Apply();

					s_WhiteTexture = tex;
				}

				return s_WhiteTexture;
			}

		}

		public static void DrawColor (Rect position, Color col)
		{
			DrawTexture(position, WhiteTexture, col);
		}

		public static void DrawTexture (Rect position, Texture tex, Color col)
		{
			Graphics.DrawTexture(position, tex, new Rect(0, 0, 1, 1), 0, 0, 0, 0, ESUI.View.GetDrawColor(col), null, -1);
		}
		public static void DrawTexture (Rect position, Texture tex, Rect uv, Color col)
		{
			Graphics.DrawTexture(position, tex, uv, 0, 0, 0, 0, ESUI.View.GetDrawColor(col), null, -1);
		}

		/// 绘制水平方向的线 
		public static void DrawHorLine (float x1, float x2, float y0, float lineWidth, Color color)
		{
			if (x1 == x2 || lineWidth == 0)
			{
				return;
			}

			if (x1 > x2)
			{
				float t = x1;
				x1 = x2;
				x2 = t;
			}

			Texture2D tex = new Texture2D(1,1);
			tex.SetPixels(new Color[]{color});
			tex.Apply();

			Rect position = new Rect(x1, y0 - lineWidth / 2, x2 - x1, lineWidth);
			GUI.DrawTexture(position, tex);

			UnityEngine.Object.DestroyImmediate(tex);
		}

		/// 绘制水平方向的线 
		public static void DrawVerLine (float y1, float y2, float x0, float lineWidth, Color color)
		{
			if (y1 == y2 || lineWidth == 0)
			{
				return;
			}

			if (y1 > y2)
			{
				float t = y1;
				y1 = y2;
				y2 = t;
			}

			Texture2D tex = new Texture2D(1,1);
			tex.SetPixels(new Color[]{color});
			tex.Apply();

			Rect position = new Rect(x0 - lineWidth / 2, y1, lineWidth, y2 - y1);
			GUI.DrawTexture(position, tex);

			UnityEngine.Object.DestroyImmediate(tex);
		}
	}

	///  
	/// 用于编辑器监视面板的值的显示和修改
	public class EditorUILayout
	{
		static GUIContent s_GetRect_GUIConent = new GUIContent();
		static GUIStyle s_GetRect_Style;
		static GUIContent GetRect_GUIConent { get { return s_GetRect_GUIConent; }}
		static GUIStyle GetRect_Style { get { if (s_GetRect_Style == null) {s_GetRect_Style = GUI.skin.button; } return s_GetRect_Style; }}

		public static GUIContent ToolbarPlus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"));
		public static GUIContent ToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));

		static GUIStyle _LabelMiddelStyle;
		public static GUIStyle LabelMiddleStyle 
		{
			get
			{
				if (_LabelMiddelStyle == null)
				{
					GUIStyle style = new GUIStyle(GUI.skin.label); 
					style.alignment = TextAnchor.MiddleCenter;
					_LabelMiddelStyle = style;
				}

				return _LabelMiddelStyle;
			}
		}

		public static GUIStyle NumberFiledStyle { get { return EditorStyles.numberField; } }

		[InitializeOnLoadMethod]
		static void Init ()
		{
			s_GetRect_GUIConent = new GUIContent();
		}

		public static Rect GetLine()
		{
			return GUILayoutUtility.GetRect(GetRect_GUIConent, GetRect_Style);
		}

		public static Rect GetLineWithHeight(float height)
		{
			return GUILayoutUtility.GetRect(0, int.MaxValue, height, height, GetRect_Style);
		}

		public static Rect GetNumberLine (params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.GetControlRect (true, 16, NumberFiledStyle, options);
			EasyReflect.SetStaticField<EditorGUILayout>("s_LastRect", position);
			return position;
		}

		public static Rect GetLineWithStyle(GUIStyle style)
		{
			return GUILayoutUtility.GetRect(GetRect_GUIConent, style);
		}
			

		public const float labelWidth = 120;
		public static void SetLabelWidth ()
		{
			EditorGUIUtility.labelWidth = labelWidth;
		}
		public static void SetLabelWidth (float labelWidth)
		{
			EditorGUIUtility.labelWidth = labelWidth;
		}
	
		public static string TextField (string label, string value)
		{
			Rect rect = EditorUILayout.GetLine();
			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
			GUI.Label(labelRect, label);
			return EditorGUI.TextField(valueRect, value);
		}
		public static string TextArea (string label, string value)
		{
			Rect rect = EditorUILayout.GetLineWithHeight(100);
			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
			GUI.Label(labelRect, label);
			return EditorGUI.TextArea(valueRect, value);
		}

		public static Object ObjectField (string label, Object value, System.Type objectType)
		{
			Rect rect = EditorUILayout.GetLine();
			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
			GUI.Label(labelRect, label);
			return EditorGUI.ObjectField(valueRect, value, objectType, true);
		}

		public static Texture TextureField (string label, Texture value)
		{
			return (Texture)EditorGUILayout.ObjectField(label, value, typeof(Texture), true);
		}

		public static Vector3 Trans_Field (string label, Vector3 value, Vector3 defaultValue)
		{
			const float PRSWidth = 20;
			const float PRSSpace = 30;

			Rect rect = EditorUILayout.GetLine();

			Rect buttonRect = new Rect(rect.x, rect.y, PRSWidth, rect.height);
			Rect vRect = new Rect(rect.x + PRSWidth + PRSSpace, rect.y, rect.width - PRSWidth - PRSSpace, rect.height);
	
			if (GUI.Button(buttonRect, label))
			{
				value = defaultValue;
			}
		
			return EditorGUI.Vector3Field(vRect, "", value);
		}

		public static Vector2 Size2DField (string label, Vector2 value, Vector2 defaultValue)
		{
			Rect rect = EditorUILayout.GetLine();

			//float labelSpace = 30;
			float valueSpace = 10;
			float resetButtonWidth = 44;
			float sizeWidth = rect.width - labelWidth - resetButtonWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, sizeWidth, rect.height);
			Rect btnRect = new Rect(valueRect.x + sizeWidth + valueSpace, rect.y, resetButtonWidth, rect.height);

			GUI.Label(labelRect, label);
			value = EditorGUI.Vector2Field(valueRect, "", value);
			if (GUI.Button(btnRect, "Reset"))
			{
				value = defaultValue;
			}

			return value;
		}

		public static Vector2 Size2DField (string label, Vector2 value, ref bool fit)
		{
			return Size2DField(label, value, ref fit, "fit");
		}

		public static Vector2 Size2DField (string label, Vector2 value, ref bool fit, string fitLabel)
		{
			Rect rect = EditorUILayout.GetLine();

			//float labelSpace = 30;
			float valueSpace = 10;
			float resetButtonWidth = 44;
			float sizeWidth = rect.width - labelWidth - resetButtonWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, sizeWidth, rect.height);
			Rect btnRect = new Rect(valueRect.x + sizeWidth + valueSpace, rect.y, resetButtonWidth, rect.height);

			GUI.Label(labelRect, label);
			value = EditorGUI.Vector2Field(valueRect, "", value);
			if (GUI.Button(btnRect, fitLabel))
			{
				fit = true;
			}

			return value;
		}

		public static Vector2 Size2DField (string label, Vector2 value)
		{
			Rect rect = EditorUILayout.GetLine();

			//float labelSpace = 30;
			float valueSpace = 10;
			float sizeWidth = rect.width - labelWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, sizeWidth, rect.height);

			GUI.Label(labelRect, label);
			value = EditorGUI.Vector2Field(valueRect, "", value);
	
			return value;
		}

		public static void BoolPair (string label, string label1, ref bool value1, string label2, ref bool value2)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueWidth = 20;
			float restWidth = (rect.width - labelWidth - valueWidth - valueWidth) / 2;
			float label1Width = restWidth;
			float label2Width = restWidth;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect value1Rect = new Rect(labelRect.x + labelRect.width, rect.y, valueWidth, rect.height);
			Rect label1Rect = new Rect(value1Rect.x + value1Rect.width, rect.y, label1Width, rect.height);
			Rect value2Rect = new Rect(label1Rect.x + label1Rect.width, rect.y, valueWidth, rect.height);
			Rect label2Rect = new Rect(value2Rect.x + value2Rect.width, rect.y, label2Width, rect.height);


			GUI.Label(labelRect, label);
			GUI.Label(label1Rect, label1);
			GUI.Label(label2Rect, label2);

			value1 = EditorGUI.Toggle(value1Rect, value1);
			value2 = EditorGUI.Toggle(value2Rect, value2);
		}

		public static float RangeField (string label, float value, float min, float max)
		{
			Rect rect = EditorUILayout.GetLine();

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect rangeRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

			GUI.Label(labelRect, label);
			return EditorGUI.Slider(rangeRect, value, min, max);
		}
		
		public static Vector2 RangeField (string label, Vector2 range)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueWidth = 40;
			float restWidth = (rect.width - labelWidth - valueWidth - valueWidth) / 2;
			float label1Width = restWidth;
			float label2Width = restWidth;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect value1Rect = new Rect(labelRect.x + labelRect.width, rect.y, valueWidth, rect.height);
			Rect label1Rect = new Rect(value1Rect.x + value1Rect.width, rect.y, label1Width, rect.height);
			Rect value2Rect = new Rect(label1Rect.x + label1Rect.width, rect.y, valueWidth, rect.height);
			Rect label2Rect = new Rect(value2Rect.x + value2Rect.width, rect.y, label2Width, rect.height);


			GUI.Label(labelRect, label);
			GUI.Label(value1Rect, "min");
			GUI.Label(value2Rect, "max");

			range.x = EditorGUI.FloatField(label1Rect, range.x);
			range.y = EditorGUI.FloatField(label2Rect, range.y);

			return range;
		}

		public static float FloadField (string label, float value)
		{
			SetLabelWidth(labelWidth);
			return EditorGUILayout.FloatField(label, value);
			/*
			Rect rect = EditorUILayout.GetLine();

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

			GUI.Label(labelRect, label);
			return EditorGUI.FloatField(valueRect ,value);
			*/
		}

		public static void EmptyLine ()
		{
			GetLine();
		}

		public static int IntField (string label, int value)
		{
			SetLabelWidth(labelWidth);
			Rect rect = GetLine();
			return EditorGUI.IntField (rect, label, value);
		}

		public static int IntFieldPlus (string label, int value)
		{
			SetLabelWidth(labelWidth);

			Rect rect = GetLine();
			const float buttonSpace = 5;
			float valueWidth = rect.width / 2;
			float buttonWidth = (rect.width - valueWidth - buttonSpace * 2) / 2;

			Rect valueRect = new Rect(rect.x, rect.y, valueWidth, rect.height);
			Rect buttonRect1 = new Rect(valueRect.x + valueWidth + buttonSpace, rect.y, buttonWidth, rect.height);
			Rect buttonRect2 = new Rect(buttonRect1.x + buttonWidth + buttonSpace, rect.y, buttonWidth, rect.height);

			value = EditorGUI.IntField (valueRect, label, value, NumberFiledStyle);

			if (GUI.Button(buttonRect1, ToolbarMinus))
			{
				--value;
			}
			if (GUI.Button(buttonRect2, ToolbarPlus))
			{
				++value;
			}
			return value;
		}
		
		public static void IntFieldPlus2 (string label, string value, Action subAction, Action plusAction)
		{
			const float valueWidth = 80;

			Rect rect = EditorUILayout.GetLine();
			float buttonWidth = (rect.width - labelWidth - valueWidth) / 2;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect moveBackRect = new Rect(labelRect.x + labelWidth, rect.y, buttonWidth, rect.height);
			Rect depthRect = new Rect(moveBackRect.x + moveBackRect.width, rect.y, valueWidth, rect.height);
			Rect moveFrontRect = new Rect(depthRect.x + depthRect.width, rect.y, buttonWidth, rect.height);

			GUI.Label(labelRect, label);
			if (GUI.Button(moveBackRect, new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"))))
			{
				if (subAction != null) subAction();
			}
			GUI.Label(depthRect, value, EditorUILayout.LabelMiddleStyle);
			if (GUI.Button(moveFrontRect, new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"))))
			{
				if (plusAction != null) plusAction();
			};
		}

		public static Vector2 Vector2Field (string label, Vector2 value)
		{
			SetLabelWidth(labelWidth);
			Rect rect = GetLine();
			return EditorGUI.Vector2Field (rect, label, value);
		}
	

		public static Color ColorField (string label, Color value)
		{
			const float btnWidth = 20;
			const float btnSpace = 10;

			Rect rect = EditorUILayout.GetLine();

			float valueWidth = rect.width - labelWidth - btnSpace - btnSpace - btnWidth - btnWidth;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, valueWidth, rect.height);
			Rect cBtnRect = new Rect(valueRect.x + valueWidth + btnSpace, rect.y, btnWidth, rect.height);
			Rect vBtnRect = new Rect(cBtnRect.x + btnWidth + btnSpace, rect.y, btnWidth, rect.height);

			GUI.Label(labelRect, label);
			value = EditorGUI.ColorField(valueRect, value);

			Color defaultColor = GUI.backgroundColor;

			GUI.backgroundColor = value;
			if (GUI.Button(cBtnRect, "C"))
			{
				ValueBuffer.Buffer_Color = value;
			}

			GUI.backgroundColor = ValueBuffer.Buffer_Color;
			if (GUI.Button(vBtnRect, "V"))
			{
				value = ValueBuffer.Buffer_Color;
			}

			GUI.backgroundColor = defaultColor;
			return value;
		}

		public static bool BoolField (string label, bool value)
		{
			Rect rect = EditorUILayout.GetLine();

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

			GUI.Label(labelRect, label);
			return EditorGUI.Toggle(valueRect, value);
		}

		public static string SearchField (string searchFilter)
		{
			const string style = "SearchTextField";
			const string btnStyle = "SearchCancelButton";
			const float btnWidth = 18f;

			Rect rect = EditorUILayout.GetLineWithStyle(style);

			Rect searchRect = new Rect(rect.x, rect.y, rect.width - btnWidth, rect.height);
			Rect btnRect = new Rect(rect.x + searchRect.width, rect.y, btnWidth, rect.height);

			string newFilter = EditorGUI.TextField(searchRect, "", searchFilter, style);

			if (GUI.Button(btnRect, "", btnStyle))
			{
				newFilter = "";
			}

			return newFilter;
		}

		public static bool SearchFieldChanged (ref string searchFilter)
		{
			const string style = "SearchTextField";
			const string btnStyle = "SearchCancelButton";
			const float btnWidth = 18f;

			Rect rect = EditorUILayout.GetLineWithStyle(style);

			Rect searchRect = new Rect(rect.x, rect.y, rect.width - btnWidth, rect.height);
			Rect btnRect = new Rect(rect.x + searchRect.width, rect.y, btnWidth, rect.height);

			string newFilter = EditorGUI.TextField(searchRect, "", searchFilter, style);

			if (GUI.Button(btnRect, "", btnStyle))
			{
				newFilter = "";
				GUIUtility.keyboardControl = 0;
			}

			bool changed = false;
			if (string.IsNullOrEmpty(searchFilter))
			{
				changed = !string.IsNullOrEmpty(newFilter);
			}
			else
			{
				changed = !searchFilter.Equals(newFilter);
			}

			if (changed)
			{
				searchFilter = newFilter;
			}
			return changed;
		}

		public static Font FontField (string label, Font font, List<Font> fontList, System.Action<Object> selectFont)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueSpace = 10;
			float buttonWidth = 44;
			float valueWidth = rect.width - labelWidth - buttonWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, valueWidth, rect.height);
			Rect btnRect = new Rect(valueRect.x + valueWidth + valueSpace, rect.y, buttonWidth, rect.height);

			GUI.Label(labelRect, label);
			font = (Font)EditorGUI.ObjectField(valueRect, font, typeof(Font), true);
			if (GUI.Button(btnRect, "Select"))
			{
				List<Object> list = new List<Object>(fontList.Count);
				foreach(Font f in fontList) list.Add(f);
				UnityObjectSelectWindow.ShowWindow<Font>(font, list, selectFont);
			}

			return font;
		}

		public static ImageAtlas ImageAtlasField (string label, ImageAtlas imageAtlas, System.Action<Object> selectAtlas)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueSpace = 10;
			float buttonWidth = 44;
			float valueWidth = rect.width - labelWidth - buttonWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, valueWidth, rect.height);
			Rect btnRect = new Rect(valueRect.x + valueWidth + valueSpace, rect.y, buttonWidth, rect.height);

			GUI.Label(labelRect, label);
			imageAtlas = (ImageAtlas)EditorGUI.ObjectField(valueRect, imageAtlas, typeof(ImageAtlas), true);
			if (GUI.Button(btnRect, "Select"))
			{
				UnityObjectSelectWindow.ShowWindow<ImageAtlas>(imageAtlas, selectAtlas);
			}

			return imageAtlas;
		}

		public static ImageAtlas ImageAtlasField (string label, ImageAtlas imageAtlas)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueSpace = 10;
			float valueWidth = rect.width - labelWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, valueWidth, rect.height);

			GUI.Label(labelRect, label);
			return (ImageAtlas)EditorGUI.ObjectField(valueRect, imageAtlas, typeof(ImageAtlas), true);
		}

		private static string[] ImageInfoListToNameArray (System.Collections.Generic.List<ImageInfo> list)
		{
			if (list == null)
			{
				return new string[0];
			}

			int count = list.Count;

			string[] names = new string[count];
			for(int i = 0; i < count; ++i)
			{
				names[i] = list[i].name;
			}

			return names;
		}

		public static ImageInfo ImageInfoField (string label, ImageInfo current, System.Collections.Generic.List<ImageInfo> list, System.Action<ImageInfo> onSelect)
		{
			const float textureShowSize = 100;
			Rect rect = GetLineWithHeight(textureShowSize);

			float valueSpace = 10;
			float textureWidth = textureShowSize;
			float enumWidth = rect.width - labelWidth - valueSpace - textureWidth;
			float enumHeight = 30;
			float textureRectX = rect.x + rect.width - textureWidth;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect enumRect = new Rect(labelRect.x + labelWidth, rect.y, enumWidth, enumHeight);
			//Rect buttonRect = new Rect(enumRect.x, enumRect.y + textureShowSize / 2, enumRect.width, enumRect.height);
			Rect textureRect = new Rect(textureRectX, rect.y, textureWidth, rect.height);

			GUI.Label(labelRect, label);
			current = PopupField<ImageInfo>(enumRect, current, list == null? null:list.ToArray(), ImageInfoListToNameArray(list));

			if (list == null || list.Count == 0)
			{
				//GUI.Button(buttonRect, "Empty to select");
			}
			else
			{
				if (GUI.Button(textureRect, ""))
				{
					if (Event.current.button == 0)
					{
						ImageInfoSelectWindow.Show(current, list, onSelect);
					}
					else if (Event.current.button == 1)
					{
						GenericMenu menu = new GenericMenu();
						menu.AddItem(new GUIContent("Edit"), false, ImageInfoSelectWindow.ToEditImageInfo, current);
						menu.AddItem(new GUIContent("CopyName"), false, ImageInfoSelectWindow.CopyImageInfoName, current);

						menu.ShowAsContext();
					}
				}
			}


			ImageInfoPreview.DrawAreaPreview(current, textureRect);

			return current;
		}

		public static Shader ShaderField (string label, Shader shader, System.Action<Object> selectShader)
		{
			Rect rect = EditorUILayout.GetLine();

			float valueSpace = 10;
			float resetButtonWidth = 44;
			float valueWidth = rect.width - labelWidth - resetButtonWidth - valueSpace;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(labelRect.x + labelWidth, rect.y, valueWidth, rect.height);
			Rect btnRect = new Rect(valueRect.x + valueWidth + valueSpace, rect.y, resetButtonWidth, rect.height);

			GUI.Label(labelRect, label);
			shader = (Shader)EditorGUI.ObjectField(valueRect, shader, typeof(Shader), true);
			if (GUI.Button(btnRect, "Select"))
			{
				UnityObjectSelectWindow.ShowWindow<Shader>(shader, selectShader);
			}

			return shader;
		}

		private static T PopupField<T>(Rect pos, T select, T[] array, string[] titleArray)
		{
			int arrayLen  = array == null? 0 : array.Length;
			int titleArrayLen = titleArray == null? 0 : titleArray.Length;

			if (arrayLen != titleArrayLen)
			{
				throw new System.Exception("T[] array lenght: " + arrayLen + ", string[] titleArray length: " + titleArrayLen + ", not the same");
			}
				
			if (arrayLen == 0)
			{
				if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
				{
					UnityEngine.Object selectObject = select as UnityEngine.Object;
					if (selectObject == null)
					{
						EditorGUI.Popup(pos, 0, new string[]{"Array is Empty", "Current: null" + typeof(T).Name});
					}
					else
					{
						EditorGUI.Popup(pos, 0, new string[]{"Array is Empty", "Current: " + selectObject.name});
					}
				}
				else
				{
					if (select == null)
					{
						EditorGUI.Popup(pos, 0, new string[]{"Array is Empty", "Current: null" + typeof(T).Name});
					}
					else
					{
						EditorGUI.Popup(pos, 0, new string[]{"Array is Empty", "Current: not null" + typeof(T).Name});
					}
				}

				return select;
			}
			else
			{
				int selectIndex = -1;
				if (select != null)
				{
					for (int i = 0; i < arrayLen; ++i)
					{
						if (select.Equals(array[i]))
						{
							selectIndex = i;
						}
					}
				}

				++selectIndex;

				string[] showTitleArray = new string[titleArray.Length + 1];
				showTitleArray[0] = "default or null";
				System.Array.Copy(titleArray, 0 , showTitleArray, 1, titleArray.Length);

				selectIndex = EditorGUI.Popup(pos, selectIndex, showTitleArray);

				return selectIndex == 0 ? default(T) : array[selectIndex - 1];
			}
		}

		public static System.Enum  EnumPopupField(string label, System.Enum value)
		{
			//T[] array = (T[])System.Enum.GetValues(typeof(T));
			//string[] titleArray = System.Enum.GetNames(typeof(T));

			Rect rect = EditorUILayout.GetLine();

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

			GUI.Label(labelRect, label);//EditorGUI.EnumPopup();
			return EditorGUI.EnumPopup(valueRect, (System.Enum)value);//PopupField<T>(valueRect, value, array, titleArray);
		}
	}
		
}