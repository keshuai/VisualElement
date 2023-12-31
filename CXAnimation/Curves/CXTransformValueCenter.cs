﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**********************************************
 * 
 * EndPlayer.com
 * 
 * 此类为 值变化 管理类，对外仅提供一个接口
 *
 *	CXTransformValueCenter.Instance.Pause
 *	true 暂停动画
 *	false 继续动画
 *
 **********************************************/
 

public class CXTransformValueCenter : MonoBehaviour 
{
	// --------------------------------------------------
	// static instance / 静态单例对象
	// --------------------------------------------------
	public static CXTransformValueCenter Instance
	{
		get
		{
			if ( static_Instance == null )
			{
				static_Instance = (new GameObject("CXTransformValueCenter")).AddComponent<CXTransformValueCenter>();
			}

			return static_Instance;
		}
	}
	private static CXTransformValueCenter static_Instance;

//	[RuntimeInitializeOnLoadMethod]
//	static void Init ()
//	{
//		//Debug.Log("RuntimeInitializeOnLoadMethod");
//	}

	// --------------------------------------------------
	// instance field / 字段
	// --------------------------------------------------

	private List<CXTransformValueCurve> AnimationCurveList = new List<CXTransformValueCurve>();

	#if UNITY_EDITOR
	public int CurveCount { get { return this.AnimationCurveList.Count; } }
	#endif


	public void DelayCall (Action action, float delay)
	{
		if (action != null)
		{
			if (delay > 0f)
			{
				this.StartCoroutine(this._DelayCall(action, delay));
			}
			else
			{
				action();
			}
		}
	}

	private IEnumerator _DelayCall (Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action();
		yield break;
	}

	// --------------------------------------------------
	// add / 添加一个曲线，请不要调用此方法
	// --------------------------------------------------
	public void AddCurve ( CXTransformValueCurve curve )
	{
		if ( !this.AnimationCurveList.Contains ( curve ) )
		{
			this.AnimationCurveList.Add ( curve );
		}
	}

	// 暂停
	public bool Pause;

	// --------------------------------------------------
	// update 
	// --------------------------------------------------

	void FixedUpdate ()
	{
		if ( this.Pause )
		{
			return;
		}

		// ** foreach不可使用, 在回调过程或单步中可能再次添加曲线, 也可能删除其他曲线
		// ** 循环中新添加的曲线应该放在下一帧执行

		CXTransformValueCurve curve;

		// ** len  之后的曲线都是在单步执行过程中添加的，必须留在下一帧执行

		for ( int i = 0, len = this.AnimationCurveList.Count; i < len;  )
		{
			curve = this.AnimationCurveList[i];

			if ( !curve.Alive || !curve.AttachObject )
			{
				// remove
				this.AnimationCurveList.RemoveAt ( i );
				--len;
			}
			else
			{
				if ( !curve.Pause )
				{
					curve.TimeStep ( Time.fixedDeltaTime );
				}
				++i;
			}
		}

	}

}