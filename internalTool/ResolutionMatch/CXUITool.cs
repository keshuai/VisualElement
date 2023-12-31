﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXUITool 
{
	public static CameraResolutionMatch Match;
	public static CXTouchParser EventParser;
	public static float DpiScale = 1f;
	public static float UIScale = 1f;


	public static void InitSpiScale ()
	{
		// 设置屏幕dpi倍数, 像素密度比率, 130为ipad 2的dpi
		DpiScale = Screen.dpi / 130f;
		if (DpiScale == 0f) DpiScale = 1f;
	}


	public static Vector2 GetUIDeltaWithScreenDelta (Vector2 delta)
	{
		if (Match == null)
		{
			return delta;
		}

		delta.x *= Match.MatchScaleX;
		delta.y *= Match.MatchScaleY;
	
		return delta;
	}

	public static Vector2 GetUIPosWithScreenPos (Vector2 screenPos)
	{
		if (Match == null)
		{
			return screenPos;
		}

		screenPos.x = (screenPos.x - Match.ScreenWidth * 0.5f) * Match.MatchScaleX;
		screenPos.y = (screenPos.y - Match.ScreenHeight * 0.5f) * Match.MatchScaleY;

		return screenPos;
	}

	public static int TouchScreenKeyboardHeight
	{
		get
		{
			#if UNITY_STANDALONE || UNITY_WEBGL

			return 0;

			#else

			float height = TouchScreenKeyboard.area.height;

			float sh = Screen.height;
			float rh = Match.Height;

			return Mathf.RoundToInt(rh * (height / sh));

			#endif
		}
	}

	public static int TouchScreenKeyboardTop 
	{
		get
		{
			return -Match.Height / 2 + TouchScreenKeyboardHeight;
		}
	}
}