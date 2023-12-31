﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class CXTouch 
{
	public CXTouch initWithTouchAndIndex ( Touch touch, int fingerId )
	{
		this.FingerId = fingerId;

		#if UNITY_ANDROID // Android 重写 静止事件

		TouchPhase currentphase = touch.phase;

		this.PrevScreenPos = this.CurrentScreenPos;
		this.CurrentScreenPos = touch.position;

		// 由 开始/静止 切换到 移动
		if (currentphase == TouchPhase.Moved || currentphase == TouchPhase.Stationary)
		{
			if (this.TouchPhase == TouchPhase.Moved)
			{
				this.ScreenDeltaMove = (this.CurrentScreenPos - this.PrevScreenPos);
			}
			else
			{
				this.ScreenDeltaMove += (this.CurrentScreenPos - this.PrevScreenPos);
			}

		if (this.ScreenDeltaMove.sqrMagnitude < 4 * 4 * CXUITool.DpiScale * CXUITool.DpiScale)
			{
				this.TouchPhase = TouchPhase.Stationary;
			}
			else// from static to move
			{
				this.TouchPhase = TouchPhase.Moved;
			}
		}
		// 开始状态
		else if (currentphase == TouchPhase.Began)
		{
			this.ScreenDeltaMove = Vector2.zero;
			this.TouchPhase = TouchPhase.Began;
		}
		// 其他默认
		else
		{
			this.ScreenDeltaMove = this.CurrentScreenPos - this.PrevScreenPos;
			this.TouchPhase = currentphase;
		}

		#else

		if ( this.TouchPhase == TouchPhase.Canceled )// cancel状态之后只能再次从Began开始
		{
			if ( touch.phase == TouchPhase.Began )
			{
				this.TouchPhase = TouchPhase.Began;
			}
		}
		else
		{
			this.TouchPhase = touch.phase;//非cancel状态，与原touch保持一致。
		}

		if ( this.TouchPhase == TouchPhase.Began )
		{
			this.CurrentScreenPos = touch.position;
			this.PrevScreenPos = this.CurrentScreenPos;
		}
		else
		{
			this.PrevScreenPos = this.CurrentScreenPos;
			this.CurrentScreenPos = touch.position;
		}
		this.ScreenDeltaMove = touch.deltaPosition;
		#endif

		return this;
	}

	/// <summary>
	/// 手指Id
	/// </summary>
	public int FingerId;

	/// <summary>
	/// 前一帧的屏幕Touch位置
	/// </summary>
	public Vector2 PrevScreenPos;

	/// <summary>
	/// 当前帧的屏幕Touch位置
	/// </summary>
	public Vector2 CurrentScreenPos;

	/// <summary>
	/// 屏幕坐标的偏移
	/// </summary>
	public Vector2 ScreenDeltaMove
	{
		get { return m_ScreenDeltaMove; }
		set { m_ScreenDeltaMove = value; m_UIDeltaMove = CXUITool.GetUIDeltaWithScreenDelta (value); }
	}
	private Vector2 m_ScreenDeltaMove;

	/// <summary>
	/// UI坐标系的偏移
	/// </summary>
	public Vector2 UIDeltaMove
	{
		get{ return m_UIDeltaMove; }
	}
	private Vector2 m_UIDeltaMove;

	/// <summary>
	/// 当前UI坐标系的Touch位置
	/// </summary>
	public Vector2 CurrentUIPos
	{
		get 
		{
			return CXUITool.GetUIPosWithScreenPos(this.CurrentScreenPos);
		}
	}

	/// <summary>
	/// 当前UI坐标系的Touch位置
	/// </summary>
	public Vector3 CurrentUIPosVector3
	{
		get
		{
			Vector2 uiPos = CurrentUIPos;
			return new Vector3 ( uiPos.x, uiPos.y, 0f );
		}
	}

	/// <summary>
	/// Touch状态
	/// </summary>
	public TouchPhase TouchPhase
	{
		get { return m_touchPhase; }
		set 
		{ 
			if ( m_touchPhase != value )
			{
				m_touchPhase = value;
				if ( value == TouchPhase.Canceled )
				{
					this.CancelAllEvent();
				}
				else if (value == TouchPhase.Began)
				{
					/// 在移动设备iOS上,在切换到输入时，上一个Touch事件不会清理 
					if (this.EventList.Count > 0)
					{
						#if CXDebug
						//UnityEngine.Debug.Log("TouchPhase.Began:" + "遗留未清理事件个数=>" + this.EventList.Count);
						#endif
						this.EventList.Clear();
					}
				}
			}
		}
	}


	private TouchPhase m_touchPhase;

	/// <summary>
	/// 焦点事件
	/// **当焦点事件设置为非空时，将不再解析焦点事件之外的任何事件
	/// </summary>
	public CXTouchEvent FocusEvent
	{
		get { return m_FocusEvent; }
		set 
		{ 
			if ( value != m_FocusEvent )
			{
				if ( m_FocusEvent != null )
				{
					m_FocusEvent.OnTouchCancel( this );// 取消前一个焦点事件
				}

				m_FocusEvent = value;

				if ( value != null )
				{
					this.CancelAllNotFocusEvent();
				}
			}
		}
	}
	private CXTouchEvent m_FocusEvent;

	/// <summary>
	/// 正在解析中的事件列表
	/// </summary>
	List<CXTouchEvent> EventList = new List<CXTouchEvent>();

	/// <summary>
	/// 正在解析中的事件个数
	/// </summary>
	public int EventCount { get { return this.EventList.Count; }}


	/// <summary>
	/// 添加一个正在解析中的事件
	/// </summary>
	public void AddEvent ( CXTouchEvent e )
	{
		if ( !EventList.Contains( e ) )
		{
			EventList.Add( e );
		}
	}

	/// <summary>
	/// 取消焦点事件
	/// </summary>
	private void CancelFocusEvent ()
	{
		if ( m_FocusEvent != null )
		{
			m_FocusEvent.OnTouchCancel( this );
			m_FocusEvent = null;
		}
	}

	/// <summary>
	/// 取消所有非焦点事件
	/// </summary>
	private void CancelAllNotFocusEvent ()
	{
		foreach ( CXTouchEvent e in EventList ) 
		{
			if ( e != m_FocusEvent )
			{
				if (e != null)
				{
					e.OnTouchCancel( this );
				}
			}
		}

		EventList.Clear();
	}

	/// <summary>
	/// 取消所有事件(包括焦点事件)
	/// </summary>
	private void CancelAllEvent ()
	{
		CancelAllNotFocusEvent();
		CancelFocusEvent();
	}


	/// <summary>
	/// 解析完成。同时会取消所有事件,!!!包括本事件!!!
	/// ***逻辑建议先 ParseDone，后触发事件，以避免循环逻辑的Bug
	/// </summary>
	public void ParseDone ()
	{
		this.TouchPhase = TouchPhase.Canceled;
	}

	/// <summary>
	/// 解析完成。同时会取消其他所有事件，本事件不会触发取消
	/// ***逻辑建议先 ParseDone，后触发事件，以避免循环逻辑的Bug
	/// </summary>
	public void ParseDone (CXTouchEvent touch)
	{
		if (touch == this.FocusEvent)
		{
			m_FocusEvent = null;
		}

		this.EventList.Remove(touch);

		this.TouchPhase = TouchPhase.Canceled;
	}

	/// <summary>
	/// 是否与否个碰撞器相撞
	/// </summary>
	public bool HitCollider ( Collider collider )
	{
		return collider == null? false : collider.bounds.Contains(this.CurrentScreenPos);
	}

	/// <summary>
	/// 是否与否个碰撞器相撞
	/// </summary>
	public bool HitBoxCollider ( BoxCollider collider )
	{
		if (collider == null)
		{
			return false;
		}

		Vector3 center = collider.center + collider.transform.position;
		Vector3 halfSize = collider.size * 0.5f;
		float left = center.x - halfSize.x;
		float right = center.x + halfSize.x;
		float top = center.y + halfSize.y;
		float bottom = center.y - halfSize.y;

		Vector2 pos = this.CurrentUIPos;

		return pos.x >= left && pos.x <+ right && pos.y >= bottom && pos.y <= top;
	}


	/// <summary>
	/// 已连接的事件中，是否包含某个种类的事件
	/// 可用于同类事件的互斥，比如两个按钮一前一后顺序解析，在后一个解析事件中则可以判断是否已解析同类事件 来确认是否需要解析自己
	/// </summary>
	public bool ContainsParseType (System.Type eventType)
	{
		foreach (CXTouchEvent e in this.EventList)
		{
			if (e.GetType() == eventType)
			{
				return true;
			}
		}

		return false;
	}
	/// <summary>
	/// 已连接的事件中，是否包含某个种类的事件
	/// 可用于同类事件的互斥，比如两个按钮一前一后顺序解析，在后一个解析事件中则可以判断是否已解析同类事件 来确认是否需要解析自己
	/// </summary>
	public bool ContainsParseType <T>() where T : CXTouchEvent
	{
		return this.ContainsParseType(typeof(T));
	}
}