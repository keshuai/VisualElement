﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CXTouchParser))]
public class EventManger : MonoBehaviour 
{
	private static EventManger static_EventManager;

	/// 强制设置开启关闭
	/// 会使计时开启失效，切记一定主动设置重新开启
	/// ****若非必要，使用 DisableSomeTime ( float disableTime ) 代替
	public static bool ForceEnableEvent
	{
		get
		{
			if ( static_EventManager == null )
			{
				#if CXDebug
				UnityEngine.Debug.LogWarning ( "当前 EventManger 为空" );
				#endif
				return true;
			}
			else
			{
				return static_EventManager.TouchParser.enabled;
			}
		}
		set
		{
			if ( static_EventManager == null )
			{
				#if CXDebug
				UnityEngine.Debug.LogWarning ( "当前 EventManger 为空" );
				#endif
			}
			else
			{
				static_EventManager.ForceDisable = !value;
                			static_EventManager.TouchParser.enabled = value;
				ResetDisableTime ( 0f );
            		}
		}
	}

	/// 关闭一段时间后重新自动开启 
	/// 连续调用 刷新会刷新delayTime, 以当前最长的为准
	public static void DisableSomeTime ( float disableTime )
	{
		if ( static_EventManager == null )
		{
			#if CXDebug
			UnityEngine.Debug.LogWarning ( "当前 EventManger 为空" );
			#endif
		}
		else
		{
			static_EventManager.InternalDisableTime ( disableTime );
        		}
	}

	///   重置关闭时间到指定值
	public static void ResetDisableTime ( float disableTime )
	{
		if ( static_EventManager == null )
		{
			#if CXDebug
			UnityEngine.Debug.LogWarning ( "当前 EventManger 为空" );
			#endif
		}
		else
		{
			static_EventManager.DisableTime = disableTime;
		}
	}



	[SerializeField]private CXTouchParser TouchParser;
	[SerializeField]private bool ForceDisable;
	[SerializeField]private float DisableTime;

	void Awake ()
	{
		if ( static_EventManager == null )
		{
			CXTouchParser cam = this.GetComponent<CXTouchParser>();
			if ( cam == null )
			{
				Destroy ( this );
				#if CXDebug
				UnityEngine.Debug.LogWarning ( "EventManger 需要挂在到有CXTouchParser的对象上" );
				#endif
				return;
			}

			this.TouchParser = cam;
			static_EventManager = this;
		}
		else
		{
			Destroy ( this );
			#if CXDebug
			UnityEngine.Debug.LogWarning ( "EventManger 只需要一个" );
			#endif
		}
	}

	 
	private void InternalDisableTime ( float delayTime )
	{
		this.DisableTime = Mathf.Max ( delayTime, this.DisableTime );
		if ( this.DisableTime > 0f )
		{
			this.TouchParser.enabled = false;
		}
	}

	void Update ()
	{
		this.DisableTime -= Time.deltaTime;
		if ( this.DisableTime < 0 )
		{
			this.DisableTime = 0f;
		}

		if ( !this.ForceDisable && this.DisableTime == 0f && this.TouchParser.enabled == false )
		{
			this.TouchParser.enabled = true;
		}
	}

}