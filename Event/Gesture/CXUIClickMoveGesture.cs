﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


/// UI组合手势(点击、移动) 的代理
public interface ICXUIClickMoveGestureDelegate
{
	/// 当手势开始时，触发此函数
	void OnTouchBegin ();
	/// 当移动时触发此函数 ==> 移动事件
	void OnTouchMove (Vector2 moveDelta);
	/// 当手势结束时，之前未移动，触发此函数 ==> 单击事件 
	void OnTouchUpInSide ();
	/// 当此手势结束时触发此函数，无论是否有触发事件。
	void OnTouchCancel();
}

/// UI组合手势(点击、移动) 
public class CXUIClickMoveGesture : CXTouchEvent
{
	public ICXUIClickMoveGestureDelegate GestureDelegate;

	CXTouch m_Touch;
	bool m_Moved;

	// 开始
	void Start () 
	{
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if (touch.TouchPhase == TouchPhase.Began)
		{
			if (m_Touch == null)
			{
				m_Touch = touch;

				if (this.GestureDelegate != null)
				{
					this.GestureDelegate.OnTouchBegin();
				}
			}
		}
		else if (touch.TouchPhase == TouchPhase.Moved)
		{
			if (touch == m_Touch)
			{
				m_Moved = true;
				touch.FocusEvent = this;

				if (this.GestureDelegate != null)
				{
					this.GestureDelegate.OnTouchMove(touch.UIDeltaMove);
				}
			}
		}
		else if (touch.TouchPhase == TouchPhase.Ended)
		{
			if (touch == m_Touch)
			{
				bool moved = m_Moved;
				touch.ParseDone();

				if (!moved && this.GestureDelegate != null)
				{
					this.GestureDelegate.OnTouchUpInSide();
				}
			}
		}
	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if (touch == m_Touch)
		{
			m_Touch = null;
			m_Moved = false;

			if (this.GestureDelegate != null)
			{
				this.GestureDelegate.OnTouchCancel();
			}
		}
	}

}