/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

public interface ICXGestureEventTouchBegin
{
	void GestureOnTouchBegin ();
	void GestureOnTouchEnd ();
}

/// 非常简单手势，开始触摸-->结束，开始一个事件，结束一个事件 
public class CXGestureTouchBegin : CXTouchEvent
{
	public static CXGestureTouchBegin CreateGeture (CX.VEle w, ICXGestureEventTouchBegin gestureDelegate)
	{
		CXGestureTouchBegin gesture = w.gameObject.GetComponent<CXGestureTouchBegin>();
		if (gesture == null)
		{
			gesture = w.gameObject.AddComponent<CXGestureTouchBegin>();
		}

		Collider collider = w.gameObject.GetComponent<Collider>();
		if (collider == null)
		{
			w.gameObject.AddComponent<BoxCollider>();
			w.autoResizeBoxCollider = true;
		}

		gesture.GestureDelegate = gestureDelegate;

		return gesture;
	}

	public static CXGestureTouchBegin CreateGeture (CX.VEle w, ICXGestureEventTouchBegin gestureDelegate, Vector3 colliderSize)
	{
		CXGestureTouchBegin gesture = w.gameObject.GetComponent<CXGestureTouchBegin>();
		if (gesture == null)
		{
			gesture = w.gameObject.AddComponent<CXGestureTouchBegin>();
		}

		w.autoResizeBoxCollider = false;

		BoxCollider collider = w.gameObject.GetComponent<BoxCollider>();
		if (collider == null)
		{
			collider = w.gameObject.AddComponent<BoxCollider>();
		}

		collider.size = colliderSize;

		gesture.GestureDelegate = gestureDelegate;
		gesture.element = w;
		return gesture;
	}


	public ICXGestureEventTouchBegin GestureDelegate;

	CXTouch Touch;

	void Reset ()
	{
		this.Touch = null;
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if (touch.TouchPhase == TouchPhase.Began)
		{
			if (this.Touch == null)
			{
				this.Touch = touch;
				touch.FocusEvent = this;
			
				if (this.GestureDelegate != null)
				{
					this.GestureDelegate.GestureOnTouchBegin();
				}
			}
		}
		else if (touch.TouchPhase == TouchPhase.Ended)
		{
			if (touch == this.Touch)
			{
				touch.ParseDone();
			}
		}

	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if (this.Touch == touch)
		{
			this.Reset();

			if (this.GestureDelegate != null)
			{
				this.GestureDelegate.GestureOnTouchEnd();
			}
		}
	}


}