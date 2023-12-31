﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/


/****************************************************************
 ** 来源: http://blog.sina.com.cn/s/blog_647422b90101de8x.html **
 ****************************************************************/

namespace CXEditor
{
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	public class EditorStyleViewer : ScriptableWizard 
	{
		[MenuItem("CX/EditorGUIStyleViewer")]
		public static void GetInstance()
		{
			Tools.GetWizardWindowInstance<EditorStyleViewer>();
		}

		Vector2 scrollPosition = new Vector2(0,0);
		string search = "";


		void OnGUI()
		{
			GUILayout.BeginHorizontal("HelpBox");
			GUILayout.Label("Click a Sample to copy its Name to your Clipboard","MiniBoldLabel");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Search:");
			search = EditorGUILayout.TextField(search);

			GUILayout.EndHorizontal();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			foreach(GUIStyle style  in GUI.skin.customStyles)
			{

				if(style.name.ToLower().Contains(search.ToLower()))
				{
					GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
					GUILayout.Space(7);
					if(GUILayout.Button(style.name,style))
					{
						EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
					GUILayout.EndHorizontal();
					GUILayout.Space(11);
				}
			}

			GUILayout.EndScrollView();
		}
	}
}