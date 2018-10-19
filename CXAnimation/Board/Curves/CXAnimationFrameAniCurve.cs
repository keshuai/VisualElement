/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXAnimationFrameAniCurve : CXAnimationCurve 
{
	public CX.ImageVE Target;

	public float StartDelay;
	public string Prefix;
	public int StartIndex;
	public int EndIndex;
	public float Scale;
	public float Duration = 0.5f;
	public bool Loop = false;

	private CXTransformFrameAni m_Curve = null;

	public override object Ani_Target { get { if (!this.Target) { this.Target = null; } return this.Target; } }
	public override bool Ani_Loop { get { return this.Loop; } }
	public override float Ani_Duration { get { return this.Duration; } }

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
			m_Curve = new CXTransformFrameAni();
			m_Curve.AttachObject = this;
			m_Curve.AddCallBack(this._PlayEnd);
		}

		m_Curve.Init(this.Target, this.Prefix, this.StartIndex, this.EndIndex, this.Duration);
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