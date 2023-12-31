﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformVector3SinDampCurve : CXTransformVector3Curve 
{
	private Vector3 MaxAmplitude;
	private float PICycle;
	private float PIOffset;
	private float AmplitudeDampPow;
	private float Duration;
	private float CurrentTime;

	// 内置的 终点震荡 运动
	public void InitForCrashDefault ( Vector3 startValue, Vector3 destValue, float duration )
	{
		// 振幅衰减幂次系数 = 7f;
		// 持续的周期PI数 = 4.5f;
		this.InitForCrashCustom ( startValue, destValue, duration, 7f, 4.5f );
	}
	// 自定义 终点震荡 运动
	// amplitudeDampPow 振幅衰减幂次系数, 持续的周期PI数
	public void InitForCrashCustom ( Vector3 startValue, Vector3 destValue, float duration, float amplitudeDampPow, float piCycle )
	{
		this.Init ( destValue, destValue - startValue, amplitudeDampPow, -0.5f, piCycle, duration);
	}

	// 	通用的正弦震荡衰减,  可用于定制各种特定的曲线运动
	//  	震荡中心值, 最大振幅, 振幅衰减幂次系数, 持续的周期PI数, 持续的时间
	public void Init ( Vector3 destValue, Vector3 amplitude, float amplitudeDampPow, float piOffset, float piCycle, float duration )
	{
		this.MaxAmplitude =  amplitude;
		this.DestValue = destValue;
		this.Duration = duration;
		this.AmplitudeDampPow = amplitudeDampPow;
		this.PIOffset = piOffset;
		this.PICycle = piCycle;
		this.CurrentTime = 0f;
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;

		if ( this.CurrentTime < this.Duration )
		{
			float timeScale = this.CurrentTime / this.Duration;

			Vector3 currentAmplitude = Mathf.Pow ( 1f - timeScale, this.AmplitudeDampPow ) * this.MaxAmplitude;
			float currentPIValue = this.PIOffset + this.PICycle * timeScale;

			this.SetValue ( this.DestValue + Mathf.Sin ( currentPIValue * Mathf.PI ) * currentAmplitude );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}