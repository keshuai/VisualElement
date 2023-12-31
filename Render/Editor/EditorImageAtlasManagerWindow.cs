﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Collections.Generic;
using System.IO;

namespace CXEditor
{
	[InitializeOnLoad]
	public class EditorImageAtlasManagerWindow : ScriptableWizard 
	{
		static EditorImageAtlasManagerWindow()
		{
			s_AssetsFolder = "Assets/ImageAtlas";
		}

		private static EditorImageAtlasManagerWindow GetInstance()
		{
			return Tools.GetWizardWindowInstance<EditorImageAtlasManagerWindow>();
		}

		private static string s_AssetsFolder;
		public static string AssetsFolder { get {return s_AssetsFolder;}}
		private static bool AssetsFolderExist
		{
			get
			{
				return System.IO.Directory.Exists(CXEditor.Tools.AssetsParentPath + "/" + AssetsFolder);
			}
		}

		public static List<ImageAtlas> FindAllAtlas ()
		{
			const string prefabSuffix = ".prefab";
			List<ImageAtlas> atlasList = new List<ImageAtlas>();

			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

			foreach(string path in allAssetPaths)
			{
				if (path.EndsWith(prefabSuffix, System.StringComparison.OrdinalIgnoreCase))
				{
					ImageAtlas atlas = AssetDatabase.LoadAssetAtPath<ImageAtlas>(path);
					if (atlas != null && !atlasList.Contains(atlas))
					{
						atlasList.Add(atlas);
					}
				}
			}

			atlasList.Sort(delegate(ImageAtlas o1, ImageAtlas o2) { return o1.name.CompareTo(o2.name); });
			return atlasList;
		}
	}
}