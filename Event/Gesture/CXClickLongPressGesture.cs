/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


/// 点击和长按的混合手势
public interface ICXGestureEventClickLongPress
{
	/// 开始点击
	void GestureOnTouchBegin ();
	/// 长按开始 
	void GestureOnLongTouchStart ();
	/// 长按介绍 
	void GestureOnLongTouchEnd ();
	/// 点击，未长按   
	void GestureOnClicked ();
	/// 手势取消 
	void GestureOnCancel ();
}

public class CXClickLongPressGesture : CXTouchEvent 
{

	public static CXClickLongPressGesture CreateGeture (CX.VEle w, ICXGestureEventClickLongPress gestureDelegate)
	{
		CXClickLongPressGesture gesture = w.gameObject.GetComponent<CXClickLongPressGesture>();
		if (gesture == null)
		{
			gesture = w.gameObject.AddComponent<CXClickLongPressGesture>();
		}

		Collider collider = w.gameObject.GetComponent<Collider>();
		if (collider == null)
		{
			w.gameObject.AddComponent<BoxCollider>();
			w.autoResizeBoxCollider = true;
		}

		gesture.GestureDelegate = gestureDelegate;
		gesture.element = w;
		return gesture;
	}

	// fields
	public ICXGestureEventClickLongPress GestureDelegate;

	private CXTouch Touch;
	private bool IsPressDown;
	private bool Moved;
	private bool LongTouched;
	private float TouchTime;

	private const float LongPressTime = 0.3f;

	void Start () 
	{
	}

	void Reset ()
	{
		this.Touch = null;
		this.Moved = false;
		this.IsPressDown = false;
		this.LongTouched = false;
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if ( touch.TouchPhase == TouchPhase.Began )
		{
			if (this.Touch == null)
			{
				this.Touch = touch;
				this.IsPressDown = true;
				this.TouchTime = Time.realtimeSinceStartup;

				/// 事件
				if (this.GestureDelegate != null)
				{
					this.GestureDelegate.GestureOnTouchBegin();
				}
			}
		}
		else if (touch.TouchPhase == TouchPhase.Stationary)
		{
			if (touch == this.Touch && !this.Moved && !this.LongTouched && Time.realtimeSinceStartup - this.TouchTime > LongPressTime)
			{
				this.LongTouched = true;
				/// 事件
				if (this.GestureDelegate != null)
				{
					this.GestureDelegate.GestureOnLongTouchStart();
				}
			}
		}
		else if (touch.TouchPhase == TouchPhase.Moved)
		{
			if (this.Touch == touch)
			{
				this.Moved = true;
			}
		}
		else if ( touch.TouchPhase == TouchPhase.Ended )
		{
			if ( this.IsPressDown && this.Touch == touch )
			{
				touch.ParseDone(this);

				bool longTouched = this.LongTouched;
				this.Reset();

				/// 事件  
				if (this.GestureDelegate != null)
				{
					if (longTouched)
					{
						this.GestureDelegate.GestureOnLongTouchEnd();
					}
					else
					{
						this.GestureDelegate.GestureOnClicked();
					}
				}
			}
		}
	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if ( this.Touch == touch )
		{
			this.Reset();
			/// 事件  
			if (this.GestureDelegate != null)
			{
				this.GestureDelegate.GestureOnCancel();
			}
		}
	}
}