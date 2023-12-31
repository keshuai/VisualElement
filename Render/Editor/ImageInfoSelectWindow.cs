﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using CX;

namespace CXEditor
{
	public class ImageInfoSelectWindow : ScriptableWizard 
	{
		private static ImageInfoSelectWindow GetInstance ()
		{
			return CXEditor.Tools.GetWizardWindowInstance<ImageInfoSelectWindow>();
		}

		static Rect s_SavedWindowPosition = new Rect (0, 0, 0, 0);
		static Rect SavedWindowPosition
		{
			get
			{
				if (s_SavedWindowPosition.x <= 0 || 
					s_SavedWindowPosition.y <= 0 ||
					s_SavedWindowPosition.width <= 100||
					s_SavedWindowPosition.height <= 100
				)
				{
					// internal class InspectorWindow : EditorWindow, IHasCustomMenu
					System.Type inspectorWindowType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
					EditorWindow[] inspectorWindows = Resources.FindObjectsOfTypeAll(inspectorWindowType) as EditorWindow[];
					if (inspectorWindows == null || inspectorWindows.Length == 0)
					{
						s_SavedWindowPosition = new Rect(300, 200 , 500, 450);
					}
					else
					{
						Rect inspectorRect = inspectorWindows[0].position;
						s_SavedWindowPosition = inspectorRect; 
						s_SavedWindowPosition.x -= s_SavedWindowPosition.width;
					}
				}

				return s_SavedWindowPosition;
			}
			set
			{
				s_SavedWindowPosition = value;
			}
		}

		public static void Show (ImageInfo current, List<ImageInfo> selections, System.Action<ImageInfo> callback)
		{
			ImageInfoSelectWindow win = GetInstance();
			win.position = SavedWindowPosition;
			win.Current = current;
			win.Selections = selections;
			win.SelectCallback = callback;
			win.ShowPopup();
		}

		private void OnLostFocus ()
		{
			base.Close();
		}

		private ImageInfo m_Current;
		private System.Action<ImageInfo> m_SelectCallback = null;
		private List<ImageInfo> m_Selections = null;
		private List<ImageInfo> m_SelectionShowList = new List<ImageInfo> ();
		private string m_SearchFiter = "";

		private Vector2 m_ScrollPos;

		public System.Action<ImageInfo> SelectCallback 
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

		public ImageInfo Current 
		{
			get 
			{
				return m_Current;
			}
			set 
			{
				m_Current = value;
			}
		}

		public List<ImageInfo> Selections 
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

		public void SetSelectionStringArray (List<ImageInfo> selectionStringArray)
		{
			m_Selections = selectionStringArray;

			m_SearchFiter = "";
			this.SearchFiterChanged();
		}


		void SearchFiterChanged ()
		{
			m_SelectionShowList.Clear();
			m_SelectionShowList.Add(null);

			if (string.IsNullOrEmpty(m_SearchFiter))
			{
				m_SelectionShowList.AddRange(m_Selections);
			}
			else
			{
				foreach(ImageInfo t in m_Selections)
				{
					if (t.name.Contains(m_SearchFiter))
					{
						m_SelectionShowList.Add(t);
					}
				}
			}
		}

		void OnGUI ()
		{
			SavedWindowPosition = this.position;

			if (EditorUILayout.SearchFieldChanged(ref m_SearchFiter))
			{
				this.SearchFiterChanged();
			}
			this.DrawSelections();
		}

		public static void ToEditImageInfo(object o)
		{
			ImageInfo imageInfo = o as ImageInfo;
			if (imageInfo != null)
			{
				Selection.activeObject = imageInfo;
			}
		}

		public static void CopyImageInfoName (object o)
		{
			ImageInfo imageInfo = o as ImageInfo;
			if (imageInfo != null)
			{
				GUIUtility.systemCopyBuffer = imageInfo.name;
			}
		}

		const float space = 20;
		const float size = 100;
		const float labelHeight = 20;

		static Rect GetLabelPos (Rect imagePos)
		{
			imagePos.y += ( size + 4) ;
			imagePos.height = labelHeight;
			return imagePos;
		}

		void DrawSelections ()
		{
			GUIStyle labelStyle = new GUIStyle("ScriptText");
			labelStyle.alignment = TextAnchor.MiddleCenter;
			GUIStyle selectedLabelStyle = new GUIStyle("LODSliderRangeSelected");
			selectedLabelStyle.alignment = TextAnchor.MiddleCenter;
			GUIStyle emptyButtonStyle = new GUIStyle("TL SelectionButtonName");

			float viewWidth = EditorGUIUtility.currentViewWidth;// CXEditor.EditorUILayout.GetLine().width;
			m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);
			{
				Rect pos = new Rect(space, space, size, size);
				int lineCount = 1;

				foreach(ImageInfo imageInfo in m_SelectionShowList)
				{
					if (pos.x + size > viewWidth)
					{
						pos.x = space;
						pos.y += (space + size + labelHeight);
						{
							++lineCount;
						}
					}

					if (GUI.Button(pos, "", emptyButtonStyle))
					{
						if (Event.current.button == 0)
						{
							if (m_SelectCallback != null)
							{
								m_Current = imageInfo;
								m_SelectCallback(imageInfo);
							}
						}
						else if (Event.current.button == 1)
						{
							GenericMenu menu = new GenericMenu();
							menu.AddItem(new GUIContent("Edit"), false, ToEditImageInfo, imageInfo);
							menu.AddItem(new GUIContent("CopyName"), false, CopyImageInfoName, imageInfo);

							menu.ShowAsContext();
						}
					}
					if (Event.current.type == EventType.Repaint)
					{
						string imageInfoName = imageInfo == null? "None (null)" : imageInfo.name;
						if (m_Current == imageInfo)
						{
							ImageInfoPreview.DrawAreaPreview(imageInfo, pos);
							GUI.Label(GetLabelPos(pos), imageInfoName, selectedLabelStyle);
						}
						else
						{
							ImageInfoPreview.DrawAreaPreview(imageInfo, pos);
							GUI.Label(GetLabelPos(pos), imageInfoName, labelStyle);
						}
					}

					pos.x += (space + size);
				}

				GUILayout.Space(pos.y + (space + size + labelHeight));
			}
			GUILayout.EndScrollView();
		}
	}
}