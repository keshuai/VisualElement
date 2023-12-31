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
	public class UnityObjectSelectWindow : ScriptableWizard
	{
		private static UnityObjectSelectWindow GetInstance()
		{
			return Tools.GetWizardWindowInstance<UnityObjectSelectWindow>();
		}

		// 从整个Unity中查找对象并选择
		public static void ShowWindow<T>(Object selectValue, System.Action<Object> callback) where T : UnityEngine.Object
		{
			UnityObjectSelectWindow w = GetInstance();
			w.ObjectType = typeof(T);
			w.Callback = callback;
			w.SelectValue = selectValue;
			w.NeedFind = true;
		}

		// 从指定的列表中选择
		public static void ShowWindow<T>(Object selectValue, List<Object> selectList,System.Action<Object> callback) where T : UnityEngine.Object
		{
			UnityObjectSelectWindow w = GetInstance();
			w.ObjectType = typeof(T);
			w.Callback = callback;
			w.SelectValue = selectValue;
			w.ObjectList = selectList;
			w.NeedFind = false;
		}
			
		private void OnLostFocus ()
		{
			if (!m_Finding)
			{
				base.Close();
			}
		}


		bool m_NeedFind = true;

		System.Type m_Type;
		System.Action<Object> m_Callback;
		List<Object> m_ObjectList = new List<Object>();
		Object m_SelectValue;
		private Vector2 m_ScrollPos;

		private bool m_HasFindAll = false;
		private bool m_Finding = false;
		private string[] m_Extensions;

		public System.Type ObjectType 
		{
			get 
			{
				return m_Type;
			}
			set 
			{
				if (value == null)
				{
					Debug.LogError("type can not be null");
				}
				else
				{
					if (m_Type != value)
					{
						m_Type = value;
						this.GetObjectList();
					}
				}
			}
		}

		public System.Action<Object> Callback
		{
			get
			{
				return m_Callback;
			}
			set
			{
				m_Callback = value;
			}
		}

		public List<Object> ObjectList 
		{
			get 
			{
				return m_ObjectList;
			}
			set 
			{
				m_ObjectList = value;
			}
		}

		public Object SelectValue 
		{
			get 
			{
				return m_SelectValue;
			}
			set 
			{
				m_SelectValue = value;
			}
		}

		public bool NeedFind
		{
			get { return m_NeedFind; }
			set { m_NeedFind = value;}
		}

		void DrawLine (Object value)
		{
			const float selecBtnWidth = 80;
			const float space = 10;

			Rect rect = CXEditor.EditorUILayout.GetLine();
			float valueWidth = rect.width - selecBtnWidth - space;

			Rect valRect = new Rect(rect.x, rect.y, valueWidth, rect.height);
			Rect btnRect = new Rect(rect.x + valueWidth + space, rect.y, selecBtnWidth, rect.height);

			Color defaultColor = GUI.backgroundColor;
			if (m_SelectValue == value)
			{
				GUI.backgroundColor = new Color(0f, 0.2f, 0.8f, 0.2f);
			}
			EditorGUI.ObjectField(valRect, value, m_Type, true);

			GUI.backgroundColor = defaultColor;

			if (GUI.Button(btnRect, "Select"))
			{
				if (m_Callback != null)
				{
					m_Callback(value);
					base.Close();
				}
			}
		}

		void GetObjectList ()
		{
			Object[] all = Resources.FindObjectsOfTypeAll(m_Type);
			m_ObjectList.Clear();
			m_ObjectList.AddRange(all);
			this.SortObjectList();
		}

		void FindAll ()
		{
			m_Finding = true;
			m_HasFindAll = true;

			bool isComponentType = m_Type.IsSubclassOf(typeof(Component));

			if (m_Extensions == null)
			{
				if (isComponentType)
				{
					m_Extensions = new string[]{".prefab"};
				}
				else if (m_Type == typeof(Shader))
				{
					m_Extensions = new string[]{".shader"};
				}
			}

			if (m_Extensions != null)
			{
				string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

				for (int pathIndex = 0; pathIndex < allAssetPaths.Length; ++pathIndex)
				{
					string path = allAssetPaths[pathIndex];

					bool extensionMatch = false;
					for (int extensionIndex = 0; extensionIndex < m_Extensions.Length; ++extensionIndex)
					{
						if (path.EndsWith(m_Extensions[extensionIndex], System.StringComparison.OrdinalIgnoreCase))
						{
							extensionMatch = true;
							break;
						}
					}

					if (extensionMatch)
					{
						EditorUtility.DisplayProgressBar("Loading", "Find all assets, please wait...", pathIndex / (float)allAssetPaths.Length);

						Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
						if (mainAsset != null && !m_ObjectList.Contains(mainAsset))
						{
							if (isComponentType)
							{
								if (Tools.IsPrefab(mainAsset))
								{
									Component[] comps = (mainAsset as GameObject).GetComponents(m_Type);
									foreach(Component comp in comps)
									{
										if (!m_ObjectList.Contains(comp))
										{
											m_ObjectList.Add(comp);
										}
									}
								}
							}
							else
							{
								System.Type t = mainAsset.GetType();
								if (t == m_Type || t.IsSubclassOf(m_Type))
								{
									if (!m_ObjectList.Contains(mainAsset))
									{
										m_ObjectList.Add(mainAsset);
									}
								}
							}
						}

					}
				}

				this.SortObjectList();
			}

			EditorUtility.ClearProgressBar();
			m_Finding = false;
		}

		void SortObjectList ()
		{
			m_ObjectList.Sort(delegate(Object o1, Object o2) { return o1.name.CompareTo(o2.name); });
		}

		void DrawSelections ()
		{
			m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);

			this.DrawLine(null);
			foreach(Object value in m_ObjectList)
			{
				this.DrawLine(value);
			}

			GUILayout.EndScrollView();
		}


		void DrawFindAllButton ()
		{
			if (!m_HasFindAll)
			{
				const float verSpace = 8f;
				const float btnMaxWidth = 180f;

				GUILayout.Space(verSpace);

				Rect btnRect = EditorUILayout.GetLineWithStyle("LargeButton");
				if (btnRect.width > btnMaxWidth)
				{
					btnRect = new Rect(btnRect.x + (btnRect.width - btnMaxWidth) / 2, btnRect.y, btnMaxWidth, btnRect.height);
				}

				bool findAll = GUI.Button(btnRect, "Find All", "LargeButton");
				if (findAll) FindAll();
			}
		}


		void CheckWidth ()
		{
			const float minWidth = 200;

			Rect rect = this.position;
			if (rect.width < minWidth)
			{
				rect.width = minWidth;
				this.position = rect;
			}
		}

		void OnGUI ()
		{
			this.CheckWidth();
			this.DrawSelections();
			if (this.NeedFind)
				this.DrawFindAllButton();
		}
	}
}