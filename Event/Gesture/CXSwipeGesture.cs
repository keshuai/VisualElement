﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;


public class CXSwipeGesture : CXTouchEvent
{
	public static CXSwipeGesture EnableGesture ( GameObject o )
	{
		if ( o == null )
		{
			return null;
		}

		CXSwipeGesture gesture = o.GetComponent<CXSwipeGesture>();
		if (gesture == null)
		{
			gesture = o.AddComponent<CXSwipeGesture>();
		}

		return gesture;
	}

	CXTouch Touch;
	bool Began;
	float StartTime;
	Vector2 StartPos;

	public Action LeftToRightEvent;
	public Action RightToLeftEvent;
	public Action TopToBottomEvent;
	public Action BottomToTopEvent;

	const float SwipeTime = 0.15f;
	const float SwipeMag = 88f;

	void Reset ()
	{
		this.Touch = null;
		this.Began = false;
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if ( touch.TouchPhase == TouchPhase.Began )
		{
			if ( this.Touch == null )
			{
				this.Touch = touch;
				this.Began = true;
				this.StartTime = Time.realtimeSinceStartup;
				this.StartPos = touch.CurrentScreenPos;
			}
		}
		else if ( touch.TouchPhase != TouchPhase.Began &&  touch.TouchPhase != TouchPhase.Canceled )
		{
			if ( this.Touch == touch && this.Began)
			{
				if ( Time.realtimeSinceStartup > this.StartTime + SwipeTime )
				{
					Vector2 delta = touch.CurrentScreenPos - this.StartPos;
					this.ParseGesture( delta, touch );
				}
			}
		}

	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if ( touch == this.Touch )
		{
			this.Reset();
		}
	}

	void ParseGesture ( Vector2 delta, CXTouch touch )
	{
		if ( delta.magnitude < SwipeMag )
		{
			return;
		}

		if ( Mathf.Abs( delta.x ) > 3f * Mathf.Abs( delta.y ) )
		{
			if ( delta.x > 0 )// left to right
			{
				if ( this.LeftToRightEvent != null )
				{
					touch.ParseDone();
					this.LeftToRightEvent();
				}
			}
			else // right to left
			{
				if ( this.RightToLeftEvent != null )
				{
					touch.ParseDone();
					this.RightToLeftEvent();
				}
			}
		}
		else if ( Mathf.Abs( delta.y ) > 3f * Mathf.Abs( delta.x ) )
		{
			if ( delta.y > 0 )// bottom to top
			{
				if ( this.BottomToTopEvent != null )
				{
					touch.ParseDone();
					this.BottomToTopEvent();
				}
			}
			else// top to bottom
			{
				if ( this.TopToBottomEvent != null )
				{
					touch.ParseDone();
					this.TopToBottomEvent();
				}
			}
		}
	}

}