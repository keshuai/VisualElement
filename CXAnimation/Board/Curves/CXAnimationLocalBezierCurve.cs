﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXAnimationLocalBezierCurve : CXAnimationCurve 
{
	public Transform Target;

	public float StartDelay;
	public Vector3[] PowerPoints;
	public float Duration = 0.5f;
	[SerializeField][HideInInspector]
	public AnimationCurve U3DCurve;

	private CXTransformVector3BezierCurve m_Curve = null;

	public override object Ani_Target { get { if (!this.Target) { this.Target = null; } return this.Target; } }
	public override bool Ani_Loop { get { return false; } }
	public override float Ani_Duration { get { return this.Duration; } }

	private void SetValue(Vector3 v)
	{
		this.Target.localPosition = v;
	}

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
			m_Curve = new CXTransformVector3BezierCurve();
			m_Curve.AttachObject = this;
			m_Curve.AddSetDelegate(this.SetValue);
			m_Curve.AddCallBack(this._PlayEnd);
		}

		if (this.U3DCurve != null && this.U3DCurve.keys.Length != 0)
		{
			this.U3DCurve = null;
		}

		m_Curve.Init(this.PowerPoints, this.Duration, this.U3DCurve);
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