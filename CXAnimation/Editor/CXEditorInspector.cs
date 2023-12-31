﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

public class CXEditorInspector : Editor
{
	public const float MaxWidth = 230f;
	public const float LargerSpaceValue = 40f;
	public const float SmallSpaceValue = 10f;

	public static void ShowLargerSpace ()
	{
		GUILayout.Space ( LargerSpaceValue );
	}

	public static void ShowSmallSpace ()
	{
		GUILayout.Space ( SmallSpaceValue );
	}

	public static void LabelField ( string title, ref string value )
	{
		value = EditorGUILayout.TextField ( title, value );
	}

	public static bool Button ( string text )
	{
		return GUILayout.Button ( text, GUILayout.Width (MaxWidth), GUILayout.Height ( 30f ) );
	}

	public static int IntField ( string title, int value )
	{
		GUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel(title);

			if (GUILayout.Button("-", GUILayout.Width(30))) 
			{
				--value;
			}
			value = EditorGUILayout.IntField(value, GUILayout.Width ( 50f ));
			if (GUILayout.Button("+", GUILayout.Width(30f))) 
			{
				++value;
			}

		}
		GUILayout.EndHorizontal();

		return value;
	}



	public static bool IntField ( string title, ref int value )
	{
		int v = IntField ( title, value );
		if ( v != value )
		{
			value = v;
			return true;
		}
		return false;
	}

	public static bool FloatField ( string title, ref float value )
	{
		float v = EditorGUILayout.FloatField ( title, value );
		if ( v != value )
		{
			value = v;
			return true;
		}
		return false;
	}

	public static void FloatField ( string title, ref float value, System.Action action )
	{
		if ( FloatField ( title, ref value ) )
		{
			if (action != null)
			{
				action();
			}
		}
	}

	public static bool Vector3Field ( string title, ref Vector3 value )
	{
		Vector3 v = EditorGUILayout.Vector3Field ( title, value );
		if ( !v.Equals ( value ) )
		{
			value = v;
			return true;
		}

		return false;
	}
	public static bool Vector2Field ( string title, ref Vector2 value )
	{
		Vector2 v = EditorGUILayout.Vector2Field ( title, value );
		if ( !v.Equals ( value ) )
		{
			value = v;
			return true;
		}

		return false;
	}

	public static void Vector3Field ( string title, ref Vector3 value, System.Action action )
	{
		if ( Vector3Field ( title, ref value ) )
		{
			if (action != null)
			{
				action();
			}
		}
	}

	public static bool ColorField ( string title, ref Color value )
	{
		Color v = EditorGUILayout.ColorField ( title, value );
		if ( !v.Equals ( value ) )
		{
			value = v;
			return true;
		}

		return false;
	}

	public static Texture TextureField ( string title, Texture tx )
	{
		return (Texture)EditorGUILayout.ObjectField ( title, tx, typeof (Texture), true );
	}

	public static Texture TextureField ( string title, ref Texture tx )
	{
		Texture t = TextureField ( title, tx );
		if ( t != tx )
		{
			tx = t;
		}
		return t;
	}

	public static float[] FloatArrayField ( string title, float[] value )
	{
		GUILayout.Label ( title );
		int len = value == null ? 0 : value.Length;
		int oldLen = len;
		len = IntField ( "数组长度", len );
		if ( len != oldLen )
		{
			Debug.Log ( len );
			float[] vs = new float[ len ];
			if ( value != null )
			{
				System.Array.Copy ( value, vs, Mathf.Min ( len, oldLen ));
			}
			return vs;
		}


		if ( value != null && value.Length != 0 )
		{
			for ( int i = 0, max = value.Length; i < max; ++i )
			{
				FloatField ( "元素" + i, ref value[i] );
			}
		}

		ShowLargerSpace();
		return value;
	}

	public static Vector3[] Vector3ArrayField ( string title, Vector3[] value )
	{
		GUILayout.Label ( title );
		int len = value == null ? 0 : value.Length;
		int oldLen = len;
		len = IntField ( "Array Length", len );
		if ( len != oldLen )
		{
			Vector3[] vs = new Vector3[ len ];
			if ( value != null )
			{
				System.Array.Copy ( value, vs, Mathf.Min ( len, oldLen ));
			}
			return vs;
		}


		if ( value != null && value.Length != 0 )
		{
			for ( int i = 0, max = value.Length; i < max; ++i )
			{
				Vector3Field ( "Element " + i, ref value[i] );
			}
		}

		ShowLargerSpace();
		return value;
	}

	public static Vector2[] Vector2ArrayField ( string title, Vector2[] value )
	{
		GUILayout.Label ( title );
		int len = value == null ? 0 : value.Length;
		int oldLen = len;
		len = IntField ( "Array Length", len );
		if ( len != oldLen )
		{
			Vector2[] vs = new Vector2[ len ];
			if ( value != null )
			{
				System.Array.Copy ( value, vs, Mathf.Min ( len, oldLen ));
			}
			return vs;
		}


		if ( value != null && value.Length != 0 )
		{
			for ( int i = 0, max = value.Length; i < max; ++i )
			{
				Vector2Field ( "Element " + i, ref value[i] );
			}
		}

		ShowLargerSpace();
		return value;
	}

	public static Color[] ColorArrayField ( string title, Color[] value )
	{
		GUILayout.Label ( title );
		int len = value == null ? 0 : value.Length;
		int oldLen = len;
		len = IntField ( "Array Length", len );
		if ( len != oldLen )
		{
			Color[] vs = new Color[ len ];
			if ( value != null )
			{
				System.Array.Copy ( value, vs, Mathf.Min ( len, oldLen ));
			}
			return vs;
		}


		if ( value != null && value.Length != 0 )
		{
			for ( int i = 0, max = value.Length; i < max; ++i )
			{
				ColorField ( "Element " + i, ref value[i] );
			}
		}

		ShowLargerSpace();
		return value;
	}

	private static bool CurveFieldMore = false;
	private static AnimationCurve ClipCurve = new AnimationCurve();
	public static AnimationCurve CurveField (AnimationCurve curve)
	{
		return CurveField ("Curve", curve);
	}
	public static AnimationCurve CurveField (string title, AnimationCurve curve)
	{
		if (curve == null)
		{
			curve = new AnimationCurve();
		}

		GUILayout.BeginHorizontal();
		GUI.color = CurveFieldMore? new Color(0.7f, 0.7f, 0.7f, 1f) : Color.white;
		GUILayout.Space(3);
		if (GUILayout.Button(title, GUILayout.Height(14)))
		{
			CurveFieldMore = !CurveFieldMore;
		}
		GUI.color = Color.white;

		curve = EditorGUILayout.CurveField(curve);
		GUILayout.EndHorizontal();

		if (CurveFieldMore)
		{
			GUILayout.Space(3f);
			GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1f);
			CurveKeysField(curve);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("LinearUp"))
			{
				curve.SetValue(AnimationCurveExtend.LinearUp);
			}
			if (GUILayout.Button("SmoothUp"))
			{
				curve.SetValue (AnimationCurveExtend.SmoothUp);
			}
			if (GUILayout.Button("AccelerateUp"))
			{
				curve.SetValue(AnimationCurveExtend.AccelerateUp);
			}
			if (GUILayout.Button("DecelerateUp"))
			{
				curve.SetValue(AnimationCurveExtend.DecelerateUp);
			}
			if (GUILayout.Button("SpringBack"))
			{
				curve.SetValue(AnimationCurveExtend.SpringBack);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("LinearDown"))
			{
				curve.SetValue(AnimationCurveExtend.LinearDown);
			}
			if (GUILayout.Button("SmoothDown"))
			{
				curve.SetValue (AnimationCurveExtend.SmoothDown);
			}
			if (GUILayout.Button("AccelerateDown"))
			{
				curve.SetValue(AnimationCurveExtend.AccelerateDown);
			}
			if (GUILayout.Button("DecelerateDown"))
			{
				curve.SetValue(AnimationCurveExtend.DecelerateDown);
			}
			if (GUILayout.Button("AutoFlat"))
			{
				curve.AutoFlat();
			}
			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
		}

		return curve;
	}
	private static void CurveKeysField (AnimationCurve curve)
	{
		if (curve == null)
		{
			curve = new AnimationCurve();
		}
		Keyframe[] keys = curve.keys;
		int len = keys.Length;
		int oldLen = len;
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Print Code"))
			{
				PrintCurve(curve);
			}
			if (GUILayout.Button("Copy"))
			{
				ClipCurve.SetValue(curve);
			}
			if (GUILayout.Button("Paste"))
			{
				keys = ClipCurve.keys;
				curve.preWrapMode = ClipCurve.preWrapMode;
				curve.postWrapMode = ClipCurve.postWrapMode;
			}

			EditorGUILayout.LabelField("keys", GUILayout.Width(50f));
			if (GUILayout.Button("-", GUILayout.Width(30))) 
			{
				--len;
			}
			len = EditorGUILayout.IntField(len, GUILayout.Width ( 50f ));
			if (GUILayout.Button("+", GUILayout.Width(30f))) 
			{
				++len;
			}

		}
		GUILayout.EndHorizontal();
		if ( len != oldLen )
		{
			Keyframe[] newKeys = new Keyframe[ len ];
			if ( keys != null )
			{
				System.Array.Copy ( keys, newKeys, Mathf.Min ( len, oldLen ));
			}
			keys = newKeys;
		}


		if ( keys != null && keys.Length != 0 )
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.TextField("time");
			EditorGUILayout.TextField("value");
			EditorGUILayout.TextField("inTangent");
			EditorGUILayout.TextField("outTangent");
			EditorGUILayout.TextField("tangentMode");
			GUILayout.EndHorizontal();
			for ( int i = 0, max = keys.Length; i < max; ++i )
			{
				GUILayout.BeginHorizontal();
				keys[i].time = EditorGUILayout.FloatField(keys[i].time);
				keys[i].value = EditorGUILayout.FloatField(keys[i].value);
				keys[i].inTangent = EditorGUILayout.FloatField(keys[i].inTangent);
				keys[i].outTangent = EditorGUILayout.FloatField(keys[i].outTangent);
				keys[i].tangentMode = EditorGUILayout.IntField(keys[i].tangentMode);
				GUILayout.EndHorizontal();
			}
		}

		curve.keys = keys;
		GUILayout.Space(10f);
	}
	// 将曲线的keys以代码形式打印
	public static void PrintCurve (AnimationCurve curve)
	{
		if (curve == null)
		{
			return;
		}

		Keyframe[] keys = curve.keys;
		if (keys == null)
		{
			return;
		}

		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("Keyframe[] keys = new Keyframe[]\n");
		sb.Append("{\n");
		foreach ( Keyframe key in keys )
		{
			sb.Append("\tnew Keyframe(){");
			sb.AppendFormat("time = {0}f, value = {1}f, inTangent = {2}f, outTangent = {3}f, tangentMode = {4}", key.time, key.value, key.inTangent, key.outTangent, key.tangentMode );
			sb.Append("},\n");
		}
		sb.Append("};\n");
		if (curve.preWrapMode == WrapMode.ClampForever && curve.postWrapMode == WrapMode.ClampForever)
		{
			sb.Append("AnimationCurve u3dCurve = new AnimationCurve(keys);\n");
		}
		else
		{
			sb.Append("AnimationCurve u3dCurve = new AnimationCurve(keys){preWrapMode = WrapMode.");
			sb.Append(curve.preWrapMode.ToString());
			sb.Append(", postWrapMode = WrapMode.");
			sb.Append(curve.postWrapMode.ToString());
			sb.Append("};\n");
		}

		Debug.Log( sb.ToString() );
	}

	public static T PopupField<T>(T select, T[] array)
	{
		return PopupField (null, select, array);
	}
	public static T PopupField<T>(string label, T select, T[] array)
	{
		int len  = array.Length;
		string[] titleArray = new string[len];

		for (int i = 0; i < len; ++i)
		{
			titleArray[i] = array[i].ToString();
		}

		return PopupField<T>(label, select, array, titleArray);
	}

	public static T PopupField<T>(T select, T[] array, string[] titleArray)
	{
		return PopupField (null, select, array, titleArray);
	}

	public static T PopupField<T>(string label, T select, T[] array, string[] titleArray)
	{
		int arrayLen  = array.Length;
		int titleArrayLen = titleArray.Length;

		if (arrayLen != titleArrayLen)
		{
			throw new System.Exception("T[] array lenght: " + arrayLen + ", string[] titleArray length: " + titleArrayLen + ", not the same");
		}

		int selectIndex = -1;
		if (select != null)
		{
			for (int i = 0; i < arrayLen; ++i)
			{
				if (select.Equals(array[i]))
				{
					selectIndex = i;
				}
			}
		}

		++selectIndex;

		string[] showTitleArray = new string[titleArray.Length + 1];
		showTitleArray[0] = "default or null";
		System.Array.Copy(titleArray, 0 , showTitleArray, 1, titleArray.Length);

		if (string.IsNullOrEmpty(label))
		{
			selectIndex= EditorGUILayout.Popup(selectIndex, showTitleArray);
		}
		else
		{
			selectIndex = EditorGUILayout.Popup(label, selectIndex, showTitleArray);
		}

		return selectIndex == 0 ? default(T) : array[selectIndex - 1];
	}
	

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		ShowSmallSpace();
		GUILayout.Label ( "以上为系统默认", GUILayout.Width ( MaxWidth ) );
		ShowLargerSpace();
	}

	public string AssetsPath
	{
		get
		{
			return AssetDatabase.GetAssetOrScenePath ( this.target );
		}
	}

	public string AssetsFolderPath
	{
		get
		{
			string assetsPath = this.AssetsPath;
			if ( assetsPath == null || assetsPath == "")
			{
				return assetsPath;
			}

			return GetFolder ( assetsPath );

		}
	}

	/// 路径所在的文件夹
	private static string GetFolder ( string path )
	{
		if ( path == null) return path;

		int len = path.Length;

		if ( len <= 1 ) return path;

		if ( path [ len -1 ] == '/' )
		{
			path = path.Substring ( 0, --len );
		}

		int i = len - 1;

		for ( ; i > 0; --i )
		{
			if ( path [ i ] == '/' )
			{
				break;
			}
		}

		return path.Substring ( 0, i );

	}
}