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
	public class ImageAtlasUpdateWarningDetailsWindow : ScriptableWizard 
	{
		public static ImageAtlasUpdateWarningDetailsWindow GetInstance()
		{
			return Tools.GetWizardWindowInstance<ImageAtlasUpdateWarningDetailsWindow>();
		}

		public static void Show(List<MonoBehaviour> refComponentList, List<ImageInfo> refInPrefabInfoList)
		{
			ImageAtlasUpdateWarningDetailsWindow win  = GetInstance();
			win.Set(refComponentList, refInPrefabInfoList);
		}

		private List<MonoBehaviour> m_ComponentList;
		//private List<ImageInfo> m_ImageInfoList;


		void Set(List<MonoBehaviour> refComponentList, List<ImageInfo> refInPrefabInfoList)
		{
			m_ComponentList = refComponentList;
			//m_ImageInfoList = refInPrefabInfoList;
		}

		GameObject GetPrefabRootGameObject (MonoBehaviour comp)
		{
			Transform trans = comp.transform;
			return trans.root.gameObject;
		}

		void ShowOne (MonoBehaviour comp)
		{
			const float prefabStringWidth = 50;
			const float ComponStringWidth = 80;
			Rect rect = EditorUILayout.GetLine();
			float objWidth = (rect.width - prefabStringWidth - ComponStringWidth) / 2;

			Rect prefabStringRect = new Rect(rect.x, rect.y, prefabStringWidth, rect.height);
			Rect GameObjectRect = new Rect(prefabStringRect.x + prefabStringWidth, rect.y, objWidth, rect.height);
			Rect ComponStringRect = new Rect(GameObjectRect.x + objWidth, rect.y, ComponStringWidth, rect.height);
			Rect ComponentRect = new Rect(ComponStringRect.x + ComponStringWidth, rect.y, objWidth, rect.height);

			GameObject root = GetPrefabRootGameObject(comp);

			GUI.Label(prefabStringRect, "Prefab");
			EditorGUI.ObjectField(GameObjectRect, root, typeof(GameObject), true);
			GUI.Label(ComponStringRect, "Component");
			EditorGUI.ObjectField(ComponentRect, comp, comp.GetType(), true);
		}

		void OnGUI ()
		{
			foreach(MonoBehaviour comp in m_ComponentList)
			{
				ShowOne(comp);
			}
		}
	}
}