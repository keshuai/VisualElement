﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;


public class TestSwipeGesture : MonoBehaviour 
{
	// 开始
	void Start () 
	{
		CXSwipeGesture gesture = CXSwipeGesture.EnableGesture( this.gameObject );

		gesture.LeftToRightEvent = this.LeftToRight;
		gesture.RightToLeftEvent = this.RightToLeft;
		gesture.TopToBottomEvent = this.TopToBottom;
		gesture.BottomToTopEvent = this.BottomToTop;
	}


	void LeftToRight ()
	{
		Debug.Log( "触发手势 轻扫 从左到右" );
	}

	void RightToLeft ()
	{
		Debug.Log( "触发手势 轻扫 从右到左" );
	}

	void TopToBottom ()
	{
		Debug.Log( "触发手势 轻扫 从上到下" );
	}


	void BottomToTop ()
	{
		Debug.Log( "触发手势 轻扫 从下到上" );
	}


}

#endif