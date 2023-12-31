﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public abstract class CXAnimationTransShake : CXAnimationCurve 
{
	public Transform Target;

	public float StartDelay;
	public CXTransformShakeType ShakeType = CXTransformShakeType.PingPong;
	public Vector3 DestValue;
	public Vector3 Amplitude;
	public float Frequency;
	public float Duration = 0.5f;
	public bool Loop = false;
	[SerializeField][HideInInspector]
	public AnimationCurve U3DCurve;

	private CXTransformVector3ShakeCurve m_Curve = null;

	public override object Ani_Target { get { if (!this.Target) { this.Target = null; } return this.Target; } }
	public override bool Ani_Loop { get { return this.Loop; } }
	public override float Ani_Duration { get { return this.Duration; } }

	public abstract void SetValue(Vector3 v);

	public override void Play ()
	{
		if (this.Target != null)
		{
			if (this.StartDelay > 0f)
			{
				CXTransformValueCenter.Instance.DelayCall(this._Play, this.StartDelay);
			}
			else
			{
				this._Play();
			}
		}
	}

	private void _Play ()
	{
		if (m_Curve == null)
		{
			m_Curve = new CXTransformVector3ShakeCurve();
			m_Curve.AttachObject = this;
			m_Curve.AddSetDelegate(this.SetValue);
			m_Curve.AddCallBack(this._PlayEnd);
		}

		m_Curve.Init(this.DestValue, this.Amplitude, this.Frequency, this.Duration, this.U3DCurve);
		m_Curve.ShakeType = this.ShakeType;
		m_Curve.Loop = this.Loop;
		m_Curve.MakeItAlive();
	}

	private void _PlayEnd ()
	{
		m_Curve = null;
	}

	public override void Stop ()
	{
		if (m_Curve != null)
		{
			m_Curve.MakeItDie();
			m_Curve = null;
		}
	}
}