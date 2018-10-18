/*******************************************
 ** CX  UTF-16 Little-Endian 模版**
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CXEditorCSEncodingConvertion 
{
	
	[MenuItem ("Assets/cs编码/查看/选中脚本")]
	public static void CheckSelection ()
	{
		Object select = Selection.activeObject;
		if ( select == null )
		{
			Debug.Log ("selection empty");
		}
		else
		{
			string assetPath =  AssetDatabase.GetAssetPath (Selection.activeObject);
			Encoding encoding = GetFileEncodeType(assetPath);
			UnityEditor.EditorUtility.DisplayDialog ("编码查看", string.Format("{0}:\n{1}", assetPath, encoding), "确定");
		}
	}

	private static bool IsCSharpCodeFile ( string fileName )
	{
		return fileName != null && fileName.Length > 2 && fileName.EndsWith (".cs");
	}

	[MenuItem ( "Assets/cs编码/查看/工程 所有cs 非UTF8 非Ascii" )]
	public static void CheckAllCSharpCodeFileInProject ()
	{
		int count;
		string log;
		CheckCSharpCodeFileAtFolder (Application.dataPath, out count, out log);

		string title = "工程目录";
		string content = "一共 " + count  + " 个cs文件编码不匹配\n\n\n" + (count > 10? "": log);
		Debug.Log ( log );
		UnityEditor.EditorUtility.DisplayDialog ( title, content, "确定" );
	}

	[MenuItem ( "Assets/cs编码/查看/所选目录 所有cs 非UTF8 非Ascii" )]
	public static void CheckAllCSharpCodeFileInSelectFolder ()
	{
		string path = CXEditorTools.GetSelectionFolderFullPathInAssetDatabase();
		int count;
		string log;
		CheckCSharpCodeFileAtFolder (path, out count, out log);

		string title = "目录:" + path;
		string content = "一共 " + count  + " 个cs文件编码不匹配\n\n\n" + (count > 10? "": log);
		Debug.Log ( log );
		UnityEditor.EditorUtility.DisplayDialog (title, content, "确定");
	}

	public static void CheckCSharpCodeFileAtFolder ( string folderPath, out int count, out string s )
	{
		StringBuilder sb = new StringBuilder();

		string[] fileArray = Directory.GetFiles (folderPath, "*.cs", SearchOption.AllDirectories);

		count = 0;
		foreach (string file in fileArray)
		{
			Encoding encoding = GetFileEncodeType(file);
			if (!(Encoding.UTF8.Equals(encoding) || Encoding.ASCII.Equals(encoding)))
			{
				sb.Append (encoding);
				sb.Append (':');
				sb.Append (file);
				sb.Append ("\n");
				++count;
			}
		}

		s = sb.ToString();
	}


	[MenuItem ( "Assets/cs编码/转换/当前至UTF8" )]
	public static void ConvertSelectionCSharpCodeToLittleEndian ()
	{
		Object selection = Selection.activeObject;
		if ( selection == null)
		{
			EditorUtility.DisplayDialog ( "", "未选中cs脚本", "确定" );
			return;
		}
		else
		{
			string path =  AssetDatabase.GetAssetPath ( Selection.activeObject );
			Encoding encoding = GetFileEncodeType ( path );
			string content;
			if (encoding.Equals (Encoding.UTF8))
			{
				content = "UTF8 ,匹配, 无需转换";
			}
			else
			{
				ConvertTextEncodingToUTF8 (path);
				content = encoding + "==> UTF8 成功";
			}

			UnityEditor.EditorUtility.DisplayDialog ( path, content, "确定" );
		}


	}

	[MenuItem ( "Assets/cs编码/转换/所选目录 所有cs至UTF8" )]
	public static void ConvertSelectFolderAllCSharpCodeToLittleEndian ()
	{
		string path = CXEditorTools.GetSelectionFolderFullPathInAssetDatabase();
		ConvertFolderAllCSharpCodeToLittleEndian ( path );
		EditorUtility.DisplayDialog ( path, "转换完成", "确定" );
	}

	[MenuItem ( "Assets/cs编码/转换/工程 所有cs至UTF8" )]
	public static void ConvertProjectAllCSharpCodeToLittleEndian ()
	{
		string path = Application.dataPath;
		ConvertFolderAllCSharpCodeToLittleEndian ( path );
		EditorUtility.DisplayDialog ( path, "转换完成", "确定" );
	}



	public static void ConvertFolderAllCSharpCodeToLittleEndian ( string folderPath )
	{
		string[] fileArray = Directory.GetFiles ( folderPath, "*.cs", SearchOption.AllDirectories );

		foreach ( string file in fileArray )
		{
			Encoding encoding = GetFileEncodeType ( file );
			if ( !Encoding.UTF8.Equals (encoding) )
			{
				ConvertTextEncodingToUTF8 ( file );
			}
		}
	}


	public static void ConvertTextEncodingToUTF8 (string file)
	{
		string content = File.ReadAllText(file);
		File.WriteAllText (file, content, Encoding.UTF8);
	}


	public static Encoding GetFileEncodeType(string filename)
	{
		FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
		BinaryReader br = new BinaryReader(fs);
		byte[] buffer = br.ReadBytes(2);

		Encoding encoding;

		if(buffer[0]>=0xEF)
		{
			if(buffer[0]==0xEF && buffer[1]==0xBB)
			{
				encoding = Encoding.UTF8;
			}
			else if(buffer[0]==0xFE && buffer[1]==0xFF)
			{
				encoding = Encoding.BigEndianUnicode;
			}
			else if(buffer[0]==0xFF && buffer[1]==0xFE)
			{
				encoding = Encoding.Unicode; // UTF-16 Little-Endian == Encoding.GetEncoding ("UTF-16") == Encoding.Unicode
			}
			else
			{
				encoding = Encoding.Default;
			}
		}
		else
		{
			encoding = Encoding.Default;
		}

		br.Close();
		fs.Close();

		return encoding;
	}
}