﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Reflection;

namespace CXEditor
{
	public class InspectorPlusWindow : EditorWindow //ScriptableWizard//
	{
		[MenuItem("CX/Inspector PlusWindow")]
		private static void MenuItemFunction ()
		{
			InspectorPlusWindow win = GetInstance();
			win.titleContent = new GUIContent("✡ Inspector Plus");
			win.autoRepaintOnSceneChange = true;
			win.ShowUtility();
		}

		public static InspectorPlusWindow GetInstance ()
		{
			return EditorWindow.GetWindow<InspectorPlusWindow>();
			//return Tools.GetWizardWindowInstance<HierarchyExtendWindow>();
		}

		private void OnSelectionChange ()
		{
			base.Repaint ();
		}

		private	static Object Buffer_Object { get {return ValueBuffer.Buffer_Object;} set { ValueBuffer.Buffer_Object = value; }}
		private GameObject m_Object;
	
		private Vector2 m_ScrollPos;

		float copyButtonWidth = 50;
		float width;

		float frameIndent = 16;
		float filedSpace = 8;

		CXInspectorLayoutColorFrame m_Frame = new CXInspectorLayoutColorFrame();

		void OnGUI ()
		{
			m_Frame.NoSwitch = true;
			width = this.position.width;

			m_Object = Selection.activeGameObject;
			if (m_Object == null)
			{
				GUILayout.Label("No selected Object");
				return;
			}



			m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
			GUILayout.BeginHorizontal();
			GUILayout.Space(frameIndent);
			GUILayout.BeginVertical();

			m_Frame.title = "Current copy value ◆";
			m_Frame.Begin();
			Buffer_Object = this.UnityObjectField(Buffer_Object);
			m_Frame.End();

			MonoBehaviour[] components = m_Object.GetComponents<MonoBehaviour>();

			for (int i = 0, len = components.Length; i < len; ++i)
			{
				MonoBehaviour comp = components[i];
				m_Frame.title = comp.GetType().Name + "◆ 【 click to copy 】";
				bool clicked = m_Frame.Begin();
				if (clicked)
				{
					Buffer_Object = comp;
				}

				this.DrawComponentInspector(comp);
				m_Frame.End();

			}

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();


		}

		private static bool IsPublicInInspector (FieldInfo field)
		{
			FieldAttributes fieldAtt = field.Attributes;
			object[] atts = field.GetCustomAttributes(true);

			bool isPublic = fieldAtt == FieldAttributes.Public;
			bool noneSerialized = false;
			bool serializeField = false;
			bool hideInInspector = false;
			foreach(object a in atts)
			{
				System.Type t = a.GetType();
				if (t == typeof(SerializeField))
				{
					serializeField = true;
				}
				else if (t == typeof(HideInInspector))
				{
					hideInInspector = true;
				}
				else if (t == typeof(System.NonSerializedAttribute))
				{
					noneSerialized = true;
				}
			}

			return (!noneSerialized) && (!hideInInspector) && (serializeField || isPublic);
		}

		bool CanPaste (System.Type type)
		{
			if (Buffer_Object == null)
			{
				Debug.Log("paste value is null ,can not paste");
				return false;
			}

			bool canPaste = Buffer_Object.GetType() == type || Buffer_Object.GetType().IsSubclassOf(type);
			if (!canPaste)
			{
				Debug.Log("paste value type diff ,can not paste");
			}

			return canPaste;
		}
	

		Object UnityObjectField (string label, Object value, System.Type valueType)
		{
			float totalWidth  = this.width - this.frameIndent - 30;
			Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.button);

			float buttonWidth = this.copyButtonWidth;
			float labelWidth = 80;
			float fieldWidth = totalWidth - buttonWidth - labelWidth;

			Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
			Rect fieldRect = new Rect(rect.x + labelWidth, rect.y, fieldWidth, rect.height);
			Rect btnRect = new Rect(rect.x + labelWidth + fieldWidth, rect.y, buttonWidth, rect.height);

			GUI.Label(labelRect, label);
			Object newValue = EditorGUI.ObjectField(fieldRect, value, valueType, true);
			if (GUI.Button(btnRect, "Paste"))
			{
				if (this.CanPaste(valueType))
				{
					newValue = Buffer_Object;
				}
			}
			return newValue;
		}

		Object UnityObjectField (Object value)
		{
			return EditorGUILayout.ObjectField(value, value == null? typeof(Object): value.GetType(), true);
		}

		void DrawComponentInspector (MonoBehaviour comp)
		{
			System.Type compTyep = comp.GetType();


			FieldInfo[] fields = compTyep.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			for(int i= 0, len = fields.Length; i < len; ++i)
			{
				FieldInfo field = fields[i];
				bool isPublicField = IsPublicInInspector(field);
				if (isPublicField)
				{
					if (field.FieldType.IsSubclassOf(typeof(MonoBehaviour)))
					{
						MonoBehaviour oldValue = (MonoBehaviour)field.GetValue(comp);
						MonoBehaviour newValue = (MonoBehaviour)this.UnityObjectField(field.Name, oldValue, field.FieldType);
						if (newValue != oldValue)
						{
							field.SetValue(comp, newValue);
						}

						GUILayout.Space(filedSpace);
					}
					else
					{
						//other field
					}
				}
			}

			PropertyInfo[] propertys = compTyep.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for(int i= 0, len = propertys.Length; i < len; ++i)
			{
				PropertyInfo property = propertys[i];

				if (property.CanWrite)
				{
					if (property.PropertyType.IsSubclassOf(typeof(MonoBehaviour)))
					{
						MonoBehaviour oldValue = (MonoBehaviour)property.GetValue(comp, null);
						MonoBehaviour newValue = (MonoBehaviour)this.UnityObjectField(property.Name, oldValue, property.PropertyType);
						if (newValue != oldValue)
						{
							property.SetValue(comp, newValue, null);
						}

						GUILayout.Space(filedSpace);
					}
					else
					{
						//other field
					}
				}
			}
		}

	}
}