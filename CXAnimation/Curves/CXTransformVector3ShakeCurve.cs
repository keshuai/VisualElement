﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

public enum CXTransformShakeType
{
	PingPong = 0,	// 来回振动
	Plus = 1,		// 正振动, X轴上方
	Minus = 2,	// 负振动, X轴下方
}

public class CXTransformVector3ShakeCurve : CXTransformVector3Curve 
{
	private Vector3 Amplitude;
	private float PISpeed;
	private float Duration;
	private float CurrentTime;
	public AnimationCurve AmpCurve;
	public bool Loop;
	public CXTransformShakeType ShakeType = CXTransformShakeType.PingPong;

	public void Init ( Vector3 shakePos, Vector3 amplitude, float frequency, float duration, AnimationCurve ampCurve )
	{
		this.DestValue = shakePos;
		this.Amplitude =  amplitude;
		this.Duration = duration;
		this.CurrentTime = 0f;
		this.PISpeed = frequency * 2 * Mathf.PI;// 2PI 为一个振动周期
		this.AmpCurve = ampCurve;
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;

		if ( this.Loop || this.CurrentTime < this.Duration )
		{
			Vector3 currentAmplitude = this.Amplitude * this.AmpCurve.Evaluate(this.CurrentTime / this.Duration);

			float sin = Mathf.Sin ( this.PISpeed * this.CurrentTime );
			if ((this.ShakeType == CXTransformShakeType.Plus && sin < 0) ||  (this.ShakeType == CXTransformShakeType.Minus && sin > 0))
			{
				sin = -sin;
			}

			this.SetValue ( this.DestValue + sin * currentAmplitude );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}