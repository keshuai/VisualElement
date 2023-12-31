﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// 动画控制版 
public class CXAnimationBoard : MonoBehaviour 
{
	/// 唤醒时是否播放
	[SerializeField][HideInInspector]
	public bool PlayOnAwake = false;

	[SerializeField][HideInInspector]
	public float StartDelay = 0f;

	/// 动画曲线集合 
	[SerializeField][HideInInspector]
	private List<CXAnimationCurve> m_CurveList = new List<CXAnimationCurve>();

	public List<CXAnimationCurve> CurveList { get { return m_CurveList; } }
	public int CurveCount { get { return m_CurveList.Count; } }


	void Awake ()
	{
		if (this.PlayOnAwake)
		{
			this.Play();
		}
	}

	/// 通过动画曲线名称，查找曲线 
	public CXAnimationCurve FindCurve (string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}

		foreach (CXAnimationCurve curve in CurveList)
		{
			if (curve.name == name)
			{
				return curve;
			}
		}

		return null;
	}

	///  
	public void AddCurve (CXAnimationCurve curve)
	{
		if (curve != null)
		{
			CurveList.Add(curve);
		}
	}

	/// 添加一个曲线
	public T AddCurve<T>() where T : CXAnimationCurve
	{
		T curve = this.gameObject.AddComponent<T>();
		m_CurveList.Add(curve);

		return curve;
	}

	/// 删除一个曲线
	/// ***Application 运行时调用，编辑器不可调用
	public void RemoveCurve (CXAnimationCurve curve)
	{
		if (curve != null)
		{
			CurveList.Remove(curve);
		}

		Destroy (curve);
	}

	/// 进行播放 
	/// 仅在Application（程序）运行时有效
	public void Play ()
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			return;
		}
		#endif

		if (this.StartDelay > 0f) 
		{
			CXTransformValueCenter.Instance.DelayCall(this._Play, this.StartDelay);
		}
		else
		{
			this._Play();
		}
	}

	private void _Play ()
	{
		foreach (CXAnimationCurve ani in CurveList)
		{
			ani.Play();
		}
	}

	/// 停止播放 
	public void Stop ()
	{
		foreach (CXAnimationCurve ani in CurveList)
		{
			ani.Stop();
		}
	}
}