/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;

[RequireComponent(typeof(Camera))]
public class CXTouchParser : MonoBehaviour 
{


	int m_MaxSupportTouchCount = 5;

	bool m_UseMouse;

	[SerializeField]private Camera m_CatchedCamera;
	public Camera CatchedCamera { get { return m_CatchedCamera; } }

	private int m_touchCount = 0;
	public int TouchCount { get { return m_touchCount; } }


	/// 用于Touch的传递拦截，只能是前面拦截后面的
	private bool m_StopParse;

	private CXTouchEvent m_FocusEvent;
	/// 全局焦点事件，会拦截所有事件，不论是前面的还是后面的。(比如使用键盘输入，键盘只能有一个，同一时间只能向一个输入框输入) 
	public CXTouchEvent FocusEvent
	{
		get { return m_FocusEvent; }
		set
		{
			if (value != m_FocusEvent)
			{
				CXTouchEvent oldFocusEvent = m_FocusEvent;
				m_FocusEvent = value;

				if (oldFocusEvent != null)
				{
					// 全局焦点事件的取消 没有touch 可能导致一些问题
					oldFocusEvent.OnTouchCancel(null);
				}
			}
		}
	}


	/// 直接使用缓存对象，而不是创建，提高每帧的效率 
	/// 将 touch Index 与 fingerId 变为一致
	public CXTouch CreateTouchWithUnityTouch (Touch touch)
	{
		int fingerId = touch.fingerId;

		if ( fingerId < 0  )
		{
			fingerId = 0;

			#if CXDebug
			throw new System.Exception("fingerId < 0");
			#endif
		}


		if ( fingerId < m_MaxSupportTouchCount )
		{
			return m_CachedTouches[fingerId].initWithTouchAndIndex(touch, fingerId);
		}

		#if CXDebug
		Debug.LogError("fingerId out of set:" + fingerId + "/" + m_MaxSupportTouchCount);
		#endif

		return null;
	}


	bool prevMouseDown = false;// 鼠标前一次状态


	private CXTouch[] m_CachedTouches;// 缓存起来达到每帧调用的最大效率，而不用每帧都重构对象，也不用调换顺序
	private CXTouch[] m_CurrentTouchs;// 当前使用的Touch数组

	public CXTouch GetCurrentTouch (int index)
	{
		return m_CurrentTouchs[index];
	}

	/// <summary>
	/// 检查鼠标 在pc上有效
	/// </summary>
	void CheckMouse ()
	{
		// 只解析左键，按下与非按下
		bool down = Input.GetMouseButton( 0 );
		#if UNITY_EDITOR
		Vector2 mousePos = Input.mousePosition;
		if ( mousePos.x < 0 || mousePos.x > Screen.width ||  mousePos.y < 0 || mousePos.y > Screen.height)
		{
			down = false;
		}
		#endif
		CXTouch touch = m_CachedTouches[0];

		if (prevMouseDown)
		{
			if ( touch.TouchPhase == TouchPhase.Canceled )
			{
				m_touchCount = 0;
			}
			else
			{
				if (down)//move
				{
					touch.PrevScreenPos = touch.CurrentScreenPos;
					touch.CurrentScreenPos = Input.mousePosition;
					touch.ScreenDeltaMove = touch.CurrentScreenPos - touch.PrevScreenPos;

					m_touchCount = 1;
					if(touch.ScreenDeltaMove.sqrMagnitude < 4 * 4 * CXUITool.DpiScale * CXUITool.DpiScale)// ( touch.CurrentScreenPos.Equals( touch.PrevScreenPos )  )
					{
						touch.TouchPhase = TouchPhase.Stationary;
					}
					else
					{
						touch.TouchPhase = TouchPhase.Moved;
					}
				}
				else// end
				{
					touch.PrevScreenPos = touch.CurrentScreenPos;
					touch.CurrentScreenPos = Input.mousePosition;
					touch.ScreenDeltaMove = touch.CurrentScreenPos - touch.PrevScreenPos;

					m_touchCount = 1;
					touch.TouchPhase = TouchPhase.Ended;
				}
			}

		}
		else
		{
			if (down)//begin
			{
				touch.CurrentScreenPos = Input.mousePosition;
				touch.PrevScreenPos = touch.CurrentScreenPos;
				touch.ScreenDeltaMove = Vector2.zero;

				m_touchCount = 1;
				touch.TouchPhase = TouchPhase.Began;
			}
			else// empty
			{
				touch.TouchPhase = TouchPhase.Canceled;
				m_touchCount = 0;
			}
		}

		prevMouseDown = down;
	}

	/// <summary>
	/// 在移动设备上有效
	/// </summary>
	void CheckTouches ()
	{
		int touchCount = Input.touchCount;
		if (touchCount > m_MaxSupportTouchCount)
		{
			touchCount = m_MaxSupportTouchCount;
		}

		m_touchCount = touchCount;

		if ( touchCount == 0 )
		{
			return;
		}

		Touch touch;
		for ( int i = 0; i < touchCount; ++i )
		{
			touch = Input.GetTouch(i);// 此处 i (index) 并不等于 fingerId
			m_CurrentTouchs[i] = this.CreateTouchWithUnityTouch(touch);
		}
	}

	void ParseTouches ()
	{
		Ray ray;
		CXTouch touch;
		int touchCount = this.TouchCount;

		for ( int i = 0; i < touchCount; ++i )
		{
			touch = m_CurrentTouchs[i];

			if (touch != null)
			{
				if (m_FocusEvent == null)
				{
					ray = m_CatchedCamera.ScreenPointToRay( touch.CurrentScreenPos );
					RaycastHit[] hits = Physics.RaycastAll( ray );
					this.PostTouches( hits, touch );
				}
				else
				{
					this.PostTouches( null, touch );
				}
			}
		}

		for ( int i = 0; i < touchCount; ++i )
		{
			touch = m_CurrentTouchs[i];

			if (touch != null)
			{
				if (touch.TouchPhase == TouchPhase.Ended || touch.TouchPhase == TouchPhase.Canceled)
				{
					touch.ParseDone();
				}
			}
		}
	}

	void PostTouches ( RaycastHit[] touchTargets, CXTouch touch )
	{
		/// 取消状态不进行解析 
		if ( touch.TouchPhase == TouchPhase.Canceled )
		{
			return;
		}

		/// 全局焦点事件 
		if (m_FocusEvent != null)
		{
			// 只有在处于激活状态才发送事件 非激活状态取消该事件
			if (m_FocusEvent.enabled && m_FocusEvent.gameObject.activeInHierarchy)
			{
				m_FocusEvent.OnTouchEvent(this, touch);
			}
			else
			{
				m_FocusEvent.OnTouchCancel(touch);
				this.FocusEvent = null;
			}
				
			return;
		}

		/// 非全局焦点事件 
		if ( touch.FocusEvent != null )// 如果已有焦点事件，直接解析焦点事件
		{
			touch.FocusEvent.OnTouchEvent(this, touch);
			return;
		}

		/// 目标为空时无需解析 
		if ( touchTargets == null )
		{
			return;
		}

		/// 目标为多个时进行排序 
		if ( touchTargets.Length > 1 )
		{
			// hit 没有做排序, 
			SortTouchTargets( touchTargets );
		}

		
		if ( touchTargets != null )
		{
			for ( int i = 0, len = touchTargets.Length; i < len; ++i )
			{
				CXTouchEvent[] components = touchTargets[i].collider.GetComponents<CXTouchEvent>();
				CXTouchEvent touchEvent = null;
				for ( int componentIndex = 0, componentsLen = components.Length; componentIndex < componentsLen; ++componentIndex )
				{
					touchEvent = components[componentIndex];
					if (touchEvent.enabled)
					{
						if ( touch.FocusEvent == null )// 无焦点事件，正常解析
						{
							touchEvent.OnTouchEvent( this, touch );
							touch.AddEvent( touchEvent ); // 先解析后添加

							if (m_FocusEvent != null)
							{
								return;
							}

							if (touch.FocusEvent != null)// 添加焦点事件后,不再解析其他事件
							{
								return;
							}

							if ( touch.TouchPhase == TouchPhase.Canceled )// 触发完成检查状态
							{
								return;
							}
						}
						else if ( touch.FocusEvent == touchEvent ) // 有焦点事件事件，仅解析焦点事件
						{
							touchEvent.OnTouchEvent( this, touch );

							if (m_FocusEvent != null)
							{
								return;
							}

							if ( touch.TouchPhase == TouchPhase.Canceled )// 触发完成检查状态
							{
								return;
							}
						}

						if ( m_StopParse )
						{
							m_StopParse = false;
							return;
						}
					}
				}
				//Debug.Log( "点击目标：" + touchTargets[i].collider.name );
			}
		}
	}

	/// <summary>
	/// 排序函数，Unity API 并未对 RaycastHit[] 进行排序
	/// 同时此处对 NGUI depth进行特别处理
	/// </summary>
	static void SortTouchTargets (RaycastHit[] touchTargets)
	{
		int len = touchTargets.Length;
		for ( int i = 0; i < len; ++i )
		{
			for ( int j = i + 1; j < len; ++j )
			{
				if ( CustomCompareTouchTargets ( touchTargets[i], touchTargets[j] ) )
				{
					RaycastHit t = touchTargets[i];
					touchTargets[i] = touchTargets[j];
					touchTargets[j] = t;
				}
			}
		}
	}

	/// <summary>
	/// 自定义排序函数取代系统系统排序
	/// </summary>
	/// <returns>当t2比t1近时返回ture</returns>
	/// <param name="t1">T1.</param>
	/// <param name="t2">T2.</param>
	static bool CustomCompareTouchTargets(RaycastHit t1, RaycastHit t2)
	{
		
		float deltaDistance = t1.distance - t2.distance;
		//Debug.Log( t1.distance + "," + t2.distance );
		if ( deltaDistance != 0f )// 有前后相距时
		{
			return deltaDistance > 0; //t1 远时调换
		}


		CXTouchEvent touchEvent1 = t1.collider.GetComponent<CXTouchEvent>();
		CXTouchEvent touchEvent2 = t2.collider.GetComponent<CXTouchEvent>();

		if (touchEvent1 == null || touchEvent2 == null)
		{
			return false;
		}

		if (touchEvent1.view == null)
		{
			return touchEvent2.view != null;
		}

		if (touchEvent2.view == null)
		{
			return false;
		}

		if (touchEvent1.view.RenderQueue > touchEvent2.view.RenderQueue)
		{
			return false;
		}

		if (touchEvent1.view.RenderQueue < touchEvent2.view.RenderQueue)
		{
			return true;
		}

		if (touchEvent1.element == null)
		{
			return touchEvent2.element != null;
		}

		if (touchEvent2.element == null)
		{
			return false;
		}

		return touchEvent2.element.depth > touchEvent1.element.depth;

		/*
		UIWidget w2 = t2.collider.GetComponent<UIWidget>();
		UIWidget w1 = t1.collider.GetComponent<UIWidget>();

		if ( w1 == null )
		{
			if ( w2 == null )// 相等
			{
				//Debug.Log( "====> w12 null" );
				return false;
			}

			//Debug.Log( "====> w1 null" );
			return true;// 使ngui t2在前
		}

		if  ( w2 == null )
		{
			//Debug.Log( "====> w2 null" );
			return false;// 使ngui t1在前
		}


		//Debug.Log( "====> sort depth" );
		if (w1.panel == w2.panel)
		{
			return w2.depth > w1.depth;
		}

		if (w1.panel == null)
		{
			// 此时w2.panel一定不为空,需要调换
			return true;
		}

		if (w2.panel == null)
		{
			// 此时w1.panel一定不为空,无要调换
			return false;
		}

		return w2.panel.depth > w1.panel.depth;

		*/
	}
	
	public bool CheckTouchHitCollider (CXTouch touch, Collider collider)
	{
		Ray ray = m_CatchedCamera.ScreenPointToRay(touch.CurrentScreenPos);
		RaycastHit[] hits = Physics.RaycastAll(ray);
		for (int i = 0, max = hits.Length; i < max; ++i)
		{
			if (hits[i].collider == collider)
			{
				return true;
			}
		}
		
		return false;
	}
		
	/// <summary>
	/// 每帧更新，对事件进行解析
	/// </summary>
	void Update ()
	{
		this.CheckInit();
		
		if ( m_UseMouse )
		{
			this.CheckMouse();
		}
		else
		{
			this.CheckTouches();
		}

		this.ParseTouches();
	}
	
	

	private void CheckInit ()
	{
		if (m_CachedTouches == null)
		{
			m_CachedTouches = new CXTouch[m_MaxSupportTouchCount];
			for ( int i = 0; i < m_MaxSupportTouchCount; ++i)
			{
				m_CachedTouches[i] = new CXTouch();
			}
	
			m_CurrentTouchs = new CXTouch[m_MaxSupportTouchCount];
	
			Input.multiTouchEnabled = true;// 开启多点触摸
			
			
			CXUITool.EventParser = this;
			
			RuntimePlatform platform = Application.platform;
			m_UseMouse = 	Application.isEditor || 
							platform == RuntimePlatform.WindowsPlayer ||
							platform == RuntimePlatform.OSXPlayer ||
							platform == RuntimePlatform.WebGLPlayer
							;
					
			if (m_UseMouse)
			{
				m_CurrentTouchs[0] = m_CachedTouches[0];
			}
	
			m_CatchedCamera = this.GetComponent<Camera>();
		}
	}

	/// <summary>
	/// 初始化
	/// </summary>
	void Awake ()
	{
		this.CheckInit();

	}

	/// <summary>
	/// 停止向下层解析事件，相当于拦截后面所有的事件解析
	/// </summary>
	public void StopParse ()
	{
		m_StopParse = true;
	}

}