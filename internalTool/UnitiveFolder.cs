﻿using UnityEngine;
using System.IO;

/// <summary>
/// 统一文件路径类
/// </summary>
public static class UnitiveFolder
{
	#if UNITY_EDITOR
	[UnityEditor.MenuItem ( "CX/UnitiveFolder/Open" )]
	private static void EditorOpenLogFolder () {System.Diagnostics.Process.Start(Folder);}
	#endif

	public static string Folder
	{
		get
		{
			string path = Application.persistentDataPath + "/offline";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
				
				// iphone
				if (!Application.isEditor && Application.platform == RuntimePlatform.IPhonePlayer)
				{
					#if UNITY_IOS
					// 设置 SetPathNotBackUpOnICloud
					UnityEngine.iOS.Device.SetNoBackupFlag(path);
					#endif
				}
			}
			
			return path;
		}
	}

	/// FilePath
	public static string GetPath(string fileName) { return Folder + fileName; }

	/// FileURL
	public static string GetURL(string fileName) { return "file://" + Folder + fileName; }

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// 程序所在的目录，非文件目录
	public static string ApplicationPath
	{
		get
		{
			int segmentationCount = 1;
			int segmentationIndex = 0;

			string appPath = Application.dataPath;
			if (appPath[appPath.Length - 1] == '/')
			{
				appPath = appPath.Substring(0, appPath.Length - 1);
			}

			int len = appPath.Length;

			int i;
			char c;
			for (i = len - 1; i < len; --i)
			{
				c = appPath[i];
				if (c == '/')
				{
					if (++segmentationIndex == segmentationCount)
						break;
				}
			}

			return appPath.Substring(0, i + 1);
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// 流媒体文件目录, 不建议使用
	public static string StreamingAssetsPath
	{
		get
		{
			#if UNITY_STANDALONE || UNITY_EDITOR
			return Application.dataPath + "/StreamingAssets";

			#elif UNITY_IPHONE
			return Application.dataPath +"/Raw";

			#elif UNITY_ANDROID
			return "jar:file://" + Application.dataPath + "!/assets/";

			#else
			return Application.dataPath +"/StreamingAssets";

			#endif
		}
	}
}


