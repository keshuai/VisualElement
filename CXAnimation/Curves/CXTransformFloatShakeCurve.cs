﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatShakeCurve : CXTransformFloatCurve 
{
	private float Amplitude;
	private float PISpeed;
	private float Duration;
	private float CurrentTime;
	public AnimationCurve AmpCurve;
	public bool Loop;
	public CXTransformShakeType ShakeType = CXTransformShakeType.PingPong;

	public void Init ( float shakePos, float amplitude, float frequency, float duration, AnimationCurve ampCurve )
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
			float currentAmplitude = this.Amplitude * this.AmpCurve.Evaluate(this.CurrentTime / this.Duration);

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