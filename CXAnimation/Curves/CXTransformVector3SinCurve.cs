﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformVector3SinCurve : CXTransformVector3Curve 
{
	private Vector3 CenterValue;
	private Vector3 Amplitude;
	private float PIOffset;
	private float Duration;
	private float PISpeed;
	private float CurrentTime;
	public bool Loop;

	public void InitForSpringbackOnce ( Vector3 startValue, Vector3 destValue, float duration )
	{
		this.DestValue = startValue;
		this.CenterValue = startValue;
		this.Amplitude = destValue - startValue;
		this.PIOffset = 0f;
		this.PISpeed = 1f / duration; //(0~1)
		this.CurrentTime = 0f;
		this.Duration = duration;
	}

	public void InitForSmoothMove ( Vector3 startValue, Vector3 destValue, float duration )
	{
		this.DestValue = destValue;
		this.CenterValue = (destValue + startValue) * 0.5f;
		this.Amplitude = (destValue - startValue) * 0.5f;
		this.PIOffset = -0.5f;
		this.PISpeed = 1f / duration; //( -0.5~0.5)
		this.CurrentTime = 0f;
		this.Duration = duration;
	}

	public void InitForLoop ( Vector3 centerValue, Vector3 amplitude, float piOffset, float frequency )
	{
		this.Loop = true;
		this.Amplitude =  amplitude;
		this.DestValue = centerValue;
		this.CenterValue = centerValue;
		this.PIOffset = piOffset;
		this.CurrentTime = 0f;
		this.PISpeed = frequency * 0.5f;
	}

	// 	通用的正弦,  可用于定制各种特定的曲线运动
	//  	中心值, 振幅, 持续的周期PI数, 当Loop开始时，频率为 piCycle / duration
	public void Init ( Vector3 centerValue, Vector3 amplitude, float piOffset, float piCycle, float duration )
	{
		this.Amplitude =  amplitude;
		this.DestValue = centerValue;
		this.CenterValue = centerValue;
		this.Duration = duration;
		this.PIOffset = piOffset;
		this.CurrentTime = 0f;
		this.PISpeed = piCycle / duration;
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;

		if ( this.Loop || this.CurrentTime < this.Duration )
		{
			float currentPIValue = this.PIOffset + this.PISpeed * this.CurrentTime;

			this.SetValue ( this.CenterValue + Mathf.Sin ( currentPIValue * Mathf.PI ) * this.Amplitude );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}