/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;

/// 触摸屏点击事件(手势)
/// 触摸抬起时仍在检测范围内时触发触摸屏点击事件
/// 手势不做音效处理
public class CXTouchUpInside : CXTouchEvent
{
	// static methods
	public static CXTouchUpInside SetEvent ( CX.VEle w, Action clickedAction )
	{
		return CXTouchUpInside.SetEvent(w, null, clickedAction, null, true);
	}
	public static CXTouchUpInside SetEvent ( CX.VEle w, Action clickedAction, bool autoResizeBoxCollider )
	{
		return CXTouchUpInside.SetEvent(w, null, clickedAction, null, autoResizeBoxCollider);
	}
	public static CXTouchUpInside SetEvent ( CX.VEle w, Action pressedAction, Action clickedAction, Action canceledAction )
	{
		return CXTouchUpInside.SetEvent (w, pressedAction, clickedAction, canceledAction, true);
	}
	public static CXTouchUpInside SetEvent ( CX.VEle w, Action pressedAction, Action clickedAction, Action canceledAction, bool autoResizeBoxCollider )
	{
		CXTouchUpInside gesture = w.GetComponent<CXTouchUpInside>();
		if (gesture == null)
		{
			gesture = w.gameObject.AddComponent<CXTouchUpInside>();
		}


		if (w.GetComponent<Collider>() == null)
		{
			w.gameObject.AddComponent<BoxCollider>();
		}

		if (autoResizeBoxCollider)
		{
			//w.ResizeCollider();
		}

		w.autoResizeBoxCollider = autoResizeBoxCollider;

		gesture.SetEvent(pressedAction, clickedAction, canceledAction );
		gesture.element = w;
		return gesture;
	}

	// fields
	CXTouch m_Touch;
	public Action PressedAction;
	public Action ClickedAction;
	public Action CanceledAction;


	// methods

	public void SetEvent ( Action pressedAction, Action clickedAction, Action canceledAction )
	{
		this.PressedAction = pressedAction;
		this.ClickedAction = clickedAction;
		this.CanceledAction = canceledAction;
	}

	void Reset ()
	{
		m_Touch = null;
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if ( touch.TouchPhase == TouchPhase.Began )
		{
			this.m_Touch = touch;

			if ( this.PressedAction != null ) 
			{
				this.PressedAction(); 
			}
		}
		else if ( touch.TouchPhase == TouchPhase.Ended )
		{
			if ( this.m_Touch == touch )
			{
				touch.ParseDone();

				if ( this.ClickedAction != null ) 
				{
					this.ClickedAction();
				}
			}
		}
	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if ( this.m_Touch == touch )
		{
			this.Reset();

			if ( this.CanceledAction != null )
			{
				this.CanceledAction(); 
			}
		}
	}
}