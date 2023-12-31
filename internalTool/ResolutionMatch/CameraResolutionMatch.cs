﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraResolutionMatch : MonoBehaviour 
{
	public enum MatchType
	{
		Pixel,
		Width,
		Height,
		FillScreen,
	}

	[HideInInspector]public MatchType Type;

	[System.NonSerialized]public int ScreenWidth;
	[System.NonSerialized]public int ScreenHeight;

	[HideInInspector]public int Width = 1080;
	[HideInInspector]public int Height = 1920;
	[HideInInspector]public float Distance = 960;

	private bool m_PerfectPixelMatch = true;
	private float m_MatchScaleX = 1f;
	private float m_MatchScaleY = 1f;

	/// 当前屏幕分辨率是否与设定分辨率1:1完美匹配
	public bool PerfectPixelMatch { get { return m_PerfectPixelMatch; } }

	/// 当前设定分辨率与屏幕分辨率的水平比率, 当完美匹配时, 此比例为 1 : 1
	public float MatchScaleX { get { return m_MatchScaleX;} }
	/// 当前设定分辨率与屏幕分辨率的水平比率, 当完美匹配时, 此比例为 1 : 1
	public float MatchScaleY { get { return m_MatchScaleY; } }

	void Awake ()
	{
		CXUITool.Match = this;
		this.Update();
	}

	void Update ()
	{
		if (ScreenWidth != Screen.width || ScreenHeight != Screen.height)
		{
			ScreenWidth = Screen.width;
			ScreenHeight = Screen.height;

			this.Apply();
		}
	}


	public void Apply ()
	{
		float distance = this.Distance;

		Vector3 pos = this.transform.localPosition;
		pos.z = -distance;
		this.transform.localPosition = pos;

		Camera camera = this.GetComponent<Camera>();
		distance += distance;
		if (camera.farClipPlane < distance)
		{
			camera.farClipPlane = distance;
		}

		if (GetComponent<Camera>().orthographic)
		{
			if (Type == MatchType.Pixel)
			{
				Set2DPixel(GetComponent<Camera>(), ScreenHeight);
			}
			else if (Type == MatchType.FillScreen)
			{
				Set2DFillScreen(GetComponent<Camera>(), Width, Height);
			}
			else if (Type == MatchType.Width)
			{
				if (ScreenWidth == Width && ScreenHeight == Height)
				{
					Set2DPixel(GetComponent<Camera>(), ScreenHeight);
				}
				else
				{
					Set2DScaleBaseOnWidth(GetComponent<Camera>(), Width);
				}
			}
			else if (Type == MatchType.Height)
			{
				if (ScreenWidth == Width && ScreenHeight == Height)
				{
					Set2DPixel(GetComponent<Camera>(), ScreenHeight);
				}
				else
				{
					if (Mathf.Abs(ScreenHeight / (float)ScreenWidth - Height / (float)Width) < 0.02f)
					{
						Set2DScaleBaseOnWidth(GetComponent<Camera>(), Width);
					}
					else
					{
						Set2DScaleBaseOnHeight(GetComponent<Camera>(), Height);
					}
				}
			}
		}
		else
		{
			if (Type == MatchType.Pixel)
			{
				Set3DPixel(GetComponent<Camera>(), ScreenHeight, Distance);
			}
			else if (Type == MatchType.FillScreen)
			{
				Set3DFillScreen(GetComponent<Camera>(), Width, Height, Distance);
			}
			else if (Type == MatchType.Width)
			{
				if (ScreenWidth == Width && ScreenHeight == Height)
				{
					Set3DPixel(GetComponent<Camera>(), ScreenHeight, Distance);
				}
				else
				{
					Set3DScaleBaseOnWidth(GetComponent<Camera>(), Width, Distance);
				}

			}
			else if (Type == MatchType.Height)
			{
				if (ScreenWidth == Width && ScreenHeight == Height)
				{
					Set3DPixel(GetComponent<Camera>(), ScreenHeight, Distance);
				}
				else
				{
					if (Mathf.Abs(ScreenHeight / (float)ScreenWidth - Height / (float)Width) < 0.02f)
					{
						Set3DScaleBaseOnWidth(GetComponent<Camera>(), Width, Distance);
					}
					else
					{
						Set3DScaleBaseOnHeight(GetComponent<Camera>(), Height, Distance);
					}
				}
			}
		}
	}
	
	public void Set3DScaleBaseOnWidth (Camera camera, float width, float distance)
	{
		Set3DScaleBaseOnHeight(camera, width * ScreenHeight / (float)ScreenWidth, distance);
	}

	public void Set3DScaleBaseOnHeight (Camera camera, float height, float distance)
	{
		camera.ResetAspect();
		camera.fieldOfView = ( Mathf.Atan ( height * 0.5f / distance ) / Mathf.PI ) * 180f * 2f;

		m_PerfectPixelMatch = Mathf.Abs(height - ScreenHeight) < 0.0001;
		m_MatchScaleX = m_MatchScaleY = height / ScreenHeight;
	}

	/// 设置3D相机 全屏非比例拉伸
	public void Set3DFillScreen (Camera camera, float width, float height, float distance)
	{
		Set3DScaleBaseOnHeight(camera, height, distance);
		camera.aspect = width / height;//  aspect 是以高度系数为标准计算宽度系数

		m_MatchScaleX = width / ScreenWidth;
		m_MatchScaleY = height / ScreenHeight;
		m_PerfectPixelMatch = (Mathf.Abs(width - ScreenWidth) < 0.0001) && (Mathf.Abs(height - ScreenHeight) < 0.0001);
	}

	/// 设置3D相机 屏幕像素匹配
	public void Set3DPixel (UnityEngine.Camera camera, int screenHeight, float distance)
	{
		Set3DScaleBaseOnHeight(camera, screenHeight, distance);

		m_PerfectPixelMatch = true;
		m_MatchScaleX = m_MatchScaleY = 1f;
	}

	/// 设置2D相机基于高度的缩放, 
	public void Set2DScaleBaseOnHeight (Camera camera, float height)
	{
		camera.ResetAspect();
		camera.orthographicSize = height * 0.5f;

		m_PerfectPixelMatch = Mathf.Abs(height - ScreenHeight) < 0.0001;
		m_MatchScaleX = m_MatchScaleY = height / ScreenHeight;
	}
	/// 设置2D相机基于宽度的缩放, 
	public void Set2DScaleBaseOnWidth (Camera camera, float width)
	{
		Set2DScaleBaseOnHeight (camera, width * ( ScreenHeight / (float)ScreenWidth ));
	}
	/// 设置2D相机 全屏非比例拉伸
	public void Set2DFillScreen (Camera camera, float width, float height)
	{
		camera.aspect = width / height;
		camera.orthographicSize = height * 0.5f;

		m_MatchScaleX = width / ScreenWidth;
		m_MatchScaleY = height / ScreenHeight;
		m_PerfectPixelMatch = (Mathf.Abs(width - ScreenWidth) < 0.0001) && (Mathf.Abs(height - ScreenHeight) < 0.0001);
	}

	/// 设置2D相机 屏幕像素匹配
	public void Set2DPixel (Camera camera, float screenHeight)
	{
		camera.ResetAspect();
		camera.orthographicSize = screenHeight * 0.5f;

		m_PerfectPixelMatch = true;
		m_MatchScaleX = m_MatchScaleY = 1f;
	}
}