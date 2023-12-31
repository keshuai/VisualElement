﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// 监视面板里里模块化的 框框
public class CXInspectorLayoutColorFrame
{
	// config

	const float ContentToFrameFactor = 0.04f;
	static int _defaultIndent = 12;
	static Color[] _defaultColorArray = new Color[]
	{
		new Color(128f/255f, 128f/255f, 128f/255f, 1f),
		new Color(66f/255f, 142f/255f, 101f/255f, 1f),
		new Color(156f/255f, 116f/255f, 94f/255f, 1f),
	};
	static Color GetColorWithFrameIndex (int frameIndex)
	{
		return _defaultColorArray[frameIndex % _defaultColorArray.Length];
	}

	static GUIStyle _TitleStyle;
	static GUIStyle TitleStyle 
	{
		get
		{
			if (_TitleStyle == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.FindStyle("button"));
				style.alignment = TextAnchor.MiddleLeft;
				_TitleStyle = style;
			}

			return _TitleStyle;
		}
	}

	// fields

	private string _title;
	private bool _enable = true;
	private Color _color;
	private Color _contentColor;

	private bool _noSwitch = false;

	float left;
	float top;
	float right;
	float bottom;

	int _indent = 4;

	private string _identifier;
	private bool _identifierNoInit = true;
	//private bool m_Disposed = false;


	private bool useDefaultColor = true;


	static int _frameIndex = 0;
	/// 用于锁定第一个frameIndex
	private bool _isFirstFrame = false;/// 当在inspector中点击颜色时，editor gui渲染会被中断，frameIndex便得不到正确的值, 因此用_isFirstFrame来锁定第一个frameIndex

	private int _index = 0;

	private int _lineWidth = 4;

	// propertys 

	public int index 
	{ 
		get { return _index; } 
		private set
		{
			if (_index != value)
			{
				_index = value;
				if (useDefaultColor)
				{
					this.SetColor(GetColorWithFrameIndex(value));
				}
			}
		}
	}


	public string title
	{
		get { return _title; }
		set { _title = value; }
	}

	public bool enable
	{
		get { return _enable; }
		set 
		{ 
			if (_enable != value)
			{
				_enable = value;

				if (!string.IsNullOrEmpty(_identifier))
				{
					PlayerPrefs.SetInt(_identifier, value? 1 : 0);
				}
			}
		}
	}

	public bool NoSwitch
	{
		get
		{
			return _noSwitch;
		}
		set
		{
			_noSwitch = value;
		}
	}

	public Color color
	{
		get { return _color; }
		set { if (_color != value) { this.useDefaultColor = false; this.SetColor(value); } }
	}

	public int indent
	{
		get {return _indent;}
		set 
		{
			value = Mathf.Clamp(value, 0, Screen.width / 5);
			_indent = value;
		}
	}

	public int LineWidth
	{
		get
		{
			return _lineWidth;
		}
		set
		{
			_lineWidth = value;
		}
	}

	public CXInspectorLayoutColorFrame () : this("", null){}
	public CXInspectorLayoutColorFrame (string title) : this(title, null){}
	public CXInspectorLayoutColorFrame (string title, Color color) : this(title, color, null){}

	public CXInspectorLayoutColorFrame (string title, string identifier)
	{
		this.useDefaultColor = true;
		this.title = title;
		this._identifier = identifier;

		this.SetColor(new Color(0.5f, 0.5f, 0.5f, 1f));
	}

	public CXInspectorLayoutColorFrame (string title, Color color, string identifier)
	{
		this.useDefaultColor = false;
		this.title = title;
		this._identifier = identifier;

		this.SetColor(color);
	}

	private void InitIdentifier ()
	{
		if (_identifierNoInit)
		{
			if (!string.IsNullOrEmpty(_identifier))
			{
				_enable = PlayerPrefs.GetInt(_identifier, 1) == 1;
			}
			_identifierNoInit = false;
		}
	}

	private void SetColor (Color c)
	{
		_color = c;

		c.a *= ContentToFrameFactor;
		_contentColor = c;
	}


	/// noSwitch时, 返回是否被点击
	/// 没有noSwitch时, 返回是否开启
	public bool Begin()
	{
		this.InitIdentifier();

		++_frameIndex;

		// 锁定第一个frameIndex
		{
			if (_frameIndex == 1)
			{
				_isFirstFrame = true;
			}

			if (_isFirstFrame)
			{
				_frameIndex = 1;
			}
		}
		this.index = _frameIndex - 1;

		_indent = _frameIndex == 1 ? 0 : _defaultIndent;


		//GUILayout.BeginHorizontal("AS TextArea");
		GUILayout.BeginHorizontal();
		GUILayout.Space(_indent);
		GUILayout.BeginVertical();

		// ◄, ▬, ►, ▲, ▌, ▼
		GUILayout.Label("");
		Rect startRect = GUILayoutUtility.GetLastRect();
		this.top = startRect.yMax - 4;
		this.left = startRect.xMin - 8;


		Color t = GUI.backgroundColor;
		Color c = this.color;c.a *= _enable ? 0.2f : 0.4f;
		GUI.backgroundColor = c;

		Rect rect = CXEditor.EditorUILayout.GetLine();

		bool clicked = false;

		if (_noSwitch)
		{
			clicked = GUI.Button(rect, "◆ " + _title, TitleStyle);
		}
		else
		{
			if(GUI.Button(rect, ((_enable? "▼ ":"► ") + _title), TitleStyle))
			{
				this.enable = !_enable;
			}
		}

		GUI.backgroundColor = t;

		if (_noSwitch)
		{
			return clicked;
		}
		else
		{
			return _enable;
		}

		//return _noSwitch || _enable;
	}

	public void End ()
	{
		--_frameIndex;

		GUILayout.Label("");
		Rect endRect = GUILayoutUtility.GetLastRect();

		GUILayout.EndVertical();
		GUILayout.Space(_indent + 8 - (_frameIndex > 0 ? 12 : -5));
		GUILayout.EndHorizontal();


		this.bottom = endRect.yMin + 4;
		this.right = endRect.xMax + 8;

		this.DrawFrame();
	}

	public void DrawWithFunction (System.Action function)
	{
		if (this.Begin())
		{
			if (function!= null)
				function();
		}
		this.End();
	}

	private void DrawFrame()
	{
		float lineWidth = _lineWidth;

		Rect contentRect = new Rect(this.left, this.top, this.right - this.left, this.bottom - this.top);
		Rect leftFrameRect = new Rect(this.left, this.top + lineWidth, lineWidth, this.bottom - this.top - lineWidth * 2);
		Rect rightFrameRect = new Rect(this.right - lineWidth, this.top + lineWidth, lineWidth, this.bottom - this.top - lineWidth * 2);
		Rect topFramRect = new Rect(this.left, this.top, this.right - this.left, lineWidth);
		Rect bottomFramRect = new Rect(this.left, this.bottom - lineWidth, this.right - this.left, lineWidth);

		Texture2D t2dFrame = new Texture2D(1, 1);
		Color c = _color;

		//t2dFrame.SetPixels(new Color[]{c, c, c, c});
		t2dFrame.SetPixels(new Color[]{c});
		t2dFrame.Apply();

		GUI.DrawTexture(leftFrameRect, t2dFrame);
		GUI.DrawTexture(rightFrameRect, t2dFrame);
		GUI.DrawTexture(topFramRect, t2dFrame);
		GUI.DrawTexture(bottomFramRect, t2dFrame);

		Texture2D t2dContent = new Texture2D(1, 1);
		c = _contentColor;
		//t2dContent.SetPixels(new Color[]{c, c, c, c});
		t2dContent.SetPixels(new Color[]{c});
		t2dContent.Apply();
		GUI.DrawTexture(contentRect, t2dContent);

		UnityEngine.Object.DestroyImmediate(t2dFrame);
		UnityEngine.Object.DestroyImmediate(t2dContent);
	}
}

#endif