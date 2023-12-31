﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace CXEditor
{
	public class TypeSelectorWindow : ScriptableWizard 
	{
		private static TypeSelectorWindow GetInstance ()
		{
			return CXEditor.Tools.GetWizardWindowInstance<TypeSelectorWindow>();
		}

		public static void Show (float yPos, List<System.Type> selections, System.Action<System.Type> callback)
		{
			const float indent = 20;
			const float height = 300;

			float viewWidth = EditorGUIUtility.currentViewWidth - indent * 2;
			Vector2 screenPos = EditorGUIUtility.GUIToScreenPoint(new Vector2(indent, yPos + 50));

			Rect position = new Rect(screenPos.x, screenPos.y, viewWidth, height);
			
			Show(position, selections, callback);
		}

		public static void Show (Rect position, List<System.Type> selections, System.Action<System.Type> callback)
		{
			TypeSelectorWindow win = GetInstance();
			win.position = position;
			win.Selections = selections;
			win.SelectCallback = callback;
			win.ShowPopup();
		}

		private System.Action<System.Type> m_SelectCallback = null;
		private List<System.Type> m_Selections = null;
		private List<System.Type> m_SelectionShowList = new List<System.Type> ();
		private string m_SearchFiter = "";

		public void SetSelectionStringArray (List<System.Type> selectionStringArray)
		{
			m_Selections = selectionStringArray;

			m_SearchFiter = "";
			this.SearchFiterChanged();
		}

		public System.Action<System.Type> SelectCallback 
		{
			get 
			{
				return m_SelectCallback;
			}
			set 
			{
				m_SelectCallback = value;
			}
		}

		public List<System.Type> Selections 
		{
			get 
			{
				return m_Selections;
			}
			set 
			{
				this.SetSelectionStringArray(value);
			}
		}

		private void OnLostFocus ()
		{
			base.Close();
		}


		void SearchFiterChanged ()
		{
			m_SelectionShowList.Clear();
			if (string.IsNullOrEmpty(m_SearchFiter))
			{
				m_SelectionShowList.AddRange(m_Selections);
			}
			else
			{
				foreach(System.Type t in m_Selections)
				{
					if (t.Name.Contains(m_SearchFiter))
					{
						m_SelectionShowList.Add(t);
					}
				}
			}
		}

		void OnGUI ()
		{
			if (EditorUILayout.SearchFieldChanged(ref m_SearchFiter))
			{
				this.SearchFiterChanged();
			}
			this.DrawSelections();
		}


		void DrawSelections ()
		{
			foreach(System.Type t in m_SelectionShowList)
			{
				if (GUILayout.Button(t.Name))
				{
					if (m_SelectCallback != null)
					{
						m_SelectCallback(t);
					}

					base.Close();
				}
			}
		}
	}
}