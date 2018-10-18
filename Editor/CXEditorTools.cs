/*******************************************
 * CX  UTF-16 Little-Endian 模版
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal.VersionControl;



public class CXEditorTools
{
	[MenuItem ("CX/GameObject/查看对象拥有的脚本全名")]
	private static void PrintAllScriptName () 
	{
		Transform[] ts = Selection.GetTransforms ( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable );

		if ( ts == null || ts.Length == 0 )
		{
			Debug.Log ( "当前无选中对象" );
		}

		foreach ( Transform t in ts )
		{
			Component[] cs = t.GetComponents<Component>();

			if ( cs == null || cs.Length == 0 )
			{
				Debug.Log (  "[ " + t.name  +  " ]不包含脚本，可能选择的对象不在 Hierarchy 中" );
			}else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append ( "[ " );
				sb.Append ( t.name );
				sb.Append ( " ]拥有脚本: " );



				foreach ( Component c in cs )
				{
					sb.Append ( c.GetType().FullName );
					sb.Append ( "  ; " );
				}

				Debug.Log ( sb.ToString() );
			}
		}
	}

	[MenuItem ("CX/GameObject/查看对象Shader名称")]
	private static void PrintShader ()
	{
		Transform[] ts = Selection.GetTransforms ( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable );

		if ( ts == null || ts.Length == 0 )
		{
			Debug.Log ( "当前无选中对象" );
		}

		foreach ( Transform t in ts )
		{
			Renderer[] cs = t.GetComponents<Renderer>();

			if ( cs == null || cs.Length == 0 )
			{
				Debug.Log (  "[ " + t.name  +  " ] 中没有发现Renderer, 注: 不在Hierarchy中的不能识别" );
			}else
			{
				foreach ( Renderer c in cs )
				{
					if ( c.material != null && c.material.shader != null )
					{
						Debug.Log ( "[ " + t.name  +  " ] 中发现Renderer Shader: " + c.material.shader.name  );
					}
					else
					{
						Debug.Log ( "Renderer Shader 为空" );
					}
				}
			}
		}
	}



	public static T GetComponentFromSelection <T> () where T : Component
	{
		Transform[] ts = Selection.GetTransforms ( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable );

		if ( ts == null || ts.Length == 0 )
		{
			Debug.Log ( "当前无选中对象" );
		}

		foreach ( Transform t in ts )
		{
			T[] cs = t.GetComponents<T>();

			if ( cs == null || cs.Length == 0 )
			{
				Debug.Log (  "[ " + t.name  +  " ] 中没有发现 " + typeof (T).FullName + ", 注: 不在Hierarchy中的不能识别" );
			}else
			{
				return cs[0];
			}
		}

		return null;
	}

	[ MenuItem ( "CX/Tool/OpenCsLine" ) ]
	public static void OpenCsLine ()
	{
		string input = Input( "输入文件路径:lineNumber" );
		Debug.Log ( input );
		if ( !string.IsNullOrEmpty ( input ) )
		{
			int i = input.Length - 1;
			for (; i > 0; --i  )
			{
				if ( input[i] == ':' || input[i] == '-' )
				{
					break;
				}
			}
			string path = input.Substring ( 0, i );
			path = path.Replace ( ':', '/' );
			string number = input.Substring ( i + 1, input.Length - i - 1);
			path = System.IO.Path.Combine (Path.GetDirectoryName(Application.dataPath), path );
			Debug.Log ( path );
			Debug.Log ( number );
			try
			{
				int line = int.Parse ( number );
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal ( path, line );
			}
			catch
			{}

		}
	}

	public static string Input ()
	{
		return CXEditorTools.Input ( "输入字符串" );
	}

	public static string Input ( string title )
	{
		return CXEditorTools.Input ( title, "输入字符串" );
	}

	public static string Input ( string title, string replace )
	{
		string str = EditorUtility.SaveFilePanel ( title, "/", replace, "" );
		if ( str != null && str.Length > 1 )
		{
			str = str.Substring ( 1 );
		}
		return str;
	}


	public static void PlayOrStop ()
	{
		EditorApplication.ExecuteMenuItem("Edit/Play");
	}

	public static string GetTransformFullName ( Transform trans )
	{
		List<Transform> transList = new List<Transform>();
		while ( trans!= null )
		{
			transList.Add ( trans );
			trans = trans.parent;
		}

		StringBuilder sb = new StringBuilder();
		for ( int i = transList.Count - 1; i >=0; --i )
		{
			sb.Append ( transList[i].name );
			sb.Append ( '/' );
		}

		return sb.ToString ();
	}

	public static string GetSelectionFolderPathInAssetDatabase ()
	{
		if (Selection.activeObject != null)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

			if (!string.IsNullOrEmpty(path))
			{
				int dot = path.LastIndexOf('.');
				int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
				if (slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
			}
		}


		return "Assets/";
	}

	public static string GetSelectionFolderFullPathInAssetDatabase ()
	{
		string assetsPath = Application.dataPath;
		return assetsPath.Substring ( 0, assetsPath.Length - 6 ) + GetSelectionFolderPathInAssetDatabase();
	}

	/// Load 绝对目录 fullPath 的 T 类型资源文件
	public static T LoadAssetsAtFullPath<T>(string fullPath) where T : UnityEngine.Object
	{
		return (T)CXEditorTools.LoadAssetsAtFullPath(fullPath,typeof(T));
	}
	/// Load 绝对目录 fullPath 的 type 类型资源文件
	public static UnityEngine.Object LoadAssetsAtFullPath (string fullPath, System.Type type)
	{
		return AssetDatabase.LoadAssetAtPath( "Assets" +  fullPath.Replace( Application.dataPath, "" ), type);
	}

	/// Load 绝对目录 fullPath 的 所有 T 类型资源文件
	public static T[] GetAllAssetsAtFullPath<T>(string fullPath) where T : UnityEngine.Object
	{
		string[] files = Directory.GetFiles( fullPath, "*", SearchOption.AllDirectories );
		List<T> objectList = new List<T>();
		T o;
		foreach (string file in files)
		{
			o = CXEditorTools.LoadAssetsAtFullPath<T>(file);
			if ( o != null )
			{
				objectList.Add(o); 
			}
		} 

		return objectList.ToArray();
	}
	/// Load 绝对目录 fullPath 的 所有 type 类型资源文件
	public static UnityEngine.Object[] GetAllAssetsAtFullPath (string fullPath, System.Type type)
	{
		string[] files = Directory.GetFiles( fullPath, "*", SearchOption.AllDirectories );
		List<UnityEngine.Object> objectList = new List<Object>();
		UnityEngine.Object o;
		foreach ( string file in files )
		{
			o = LoadAssetsAtFullPath( file, type );
			if ( o != null )
			{
				objectList.Add( o ); 
			}
		} 

		return objectList.ToArray();
	}

	/// 获取所选目录下所有T类型资源 
	public static T[] GetAllAssetsAtSelectionPath<T>() where T : UnityEngine.Object
	{
		return GetAllAssetsAtFullPath<T>(GetSelectionFolderFullPathInAssetDatabase());
	}
	/// 获取所选目录下所有type类型资源 
	public static UnityEngine.Object[] GetAllAssetsAtSelectionPath (System.Type type)
	{
		return GetAllAssetsAtFullPath(GetSelectionFolderFullPathInAssetDatabase(), type);
	}

	/// 获取工程内所有T类型的资源
	public static T[] GetAllAssetsInProject<T>() where T : UnityEngine.Object
	{
		List<T> oList = new List<T>();

		string[] pathArray = AssetDatabase.GetAllAssetPaths();

		foreach (string path in pathArray)
		{
			T o = (T)AssetDatabase.LoadAssetAtPath (path, typeof(T));
			if (o != null)
			{
				oList.Add(o);
			}
		}

		return oList.ToArray();
	}

	public static T[] GetAllComponentInProject<T>() where T : UnityEngine.Component
	{
		List<T> rendererList = new List<T>();

		string[] pathArray = AssetDatabase.GetAllAssetPaths();
		foreach ( string path in pathArray )
		{
			GameObject o = (GameObject)AssetDatabase.LoadAssetAtPath ( path, typeof (GameObject) );
			if ( o != null )
			{
				T[] renderArray = o.GetComponentsInChildren<T>(true);
				rendererList.AddRange(renderArray);
			}
		}

		return rendererList.ToArray();
	}

	public static Component[] GetAllComponentInProject(System.Type type)
	{
		List<Component> rendererList = new List<Component>();

		string[] pathArray = AssetDatabase.GetAllAssetPaths();
		foreach ( string path in pathArray )
		{
			GameObject o = (GameObject)AssetDatabase.LoadAssetAtPath ( path, typeof (GameObject) );
			if ( o != null )
			{
				Component[] renderArray = o.GetComponentsInChildren(type,true);
				rendererList.AddRange(renderArray);
			}
		}

		return rendererList.ToArray();
	}

	public static bool DeleteAssets ( UnityEngine.Object asset )
	{
		return AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( asset ) );
	}
		
	/// * asset
	public static bool SaveAssets (Object asset)
	{
		return SaveAssets (asset, "Assets", "asset");
	}
	/// * asset
	/// * 目录
	public static bool SaveAssets (Object asset, string path)
	{
		return SaveAssets (asset, path, "asset");
	}
	/// * asset
	/// * 目录
	/// * 扩展名 
	public static bool SaveAssets (Object asset, string path, string extension)
	{
		if (asset == null)
		{
			Debug.LogError("null can not save asset");
			return false;
		}

		if (string.IsNullOrEmpty(path))
		{
			path = "Assets";
		}

		path = EditorUtility.SaveFilePanelInProject("save assets", asset.name, extension, "", path);

		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		try
		{
			AssetDatabase.CreateAsset(asset, path);
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// * T : ScriptableObject
	/// * DefaultPath "Assets";
	public static ScriptableObject CreateScriptableObjectAssets<T>() where T : ScriptableObject
	{
		return CreateScriptableObjectAssets (typeof(T), null);
	}

	/// * ScriptableObject type
	/// * DefaultPath "Assets";
	public static ScriptableObject CreateScriptableObjectAssets (System.Type type)
	{
		return CreateScriptableObjectAssets (type, null);
	}

	/// * T : ScriptableObject
	/// * 目录
	public static ScriptableObject CreateScriptableObjectAssets<T>(string path) where T : ScriptableObject
	{
		return CreateScriptableObjectAssets (typeof(T), path);
	}
	/// * ScriptableObject type
	/// * 目录
	public static ScriptableObject CreateScriptableObjectAssets (System.Type type, string path)
	{
		if (type == null)
		{
			throw new System.NullReferenceException();
			return null;
		}

		System.Type targetType = typeof(ScriptableObject);

		if (type != targetType)
		{
			bool isBaseToScriptableObject = false;
			System.Type baseType = type.BaseType;
			while (baseType != null)
			{
				if (baseType == targetType)
				{
					isBaseToScriptableObject = true;
					break;
				}

				baseType = baseType.BaseType;
			}

			if (!isBaseToScriptableObject)
			{
				throw new System.Exception(type.Name + " is not sub class of ScriptableObject");
				return null;
			}

			// type.IsSubclassOf will error on Unity.Object interface Type
//			if (type.IsSubclassOf(typeof(ScriptableObject)))
//			{
//				throw new System.Exception(type.Name + " is not sub class of ScriptableObject");
//				return null;
//			}
		}




		const string DefaultPath = "Assets";

		if (string.IsNullOrEmpty(path))
		{
			path = DefaultPath;
		}

		const string extension = "asset";

		path = EditorUtility.SaveFilePanelInProject("save assets", "name", extension, "", path);

		if (string.IsNullOrEmpty(path))
		{
			return null;
		}

		ScriptableObject asset = ScriptableObject.CreateInstance(type);
		AssetDatabase.CreateAsset(asset, path);

		return asset;
	}
}


