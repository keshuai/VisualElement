﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatArraySinDampCurve : CXTransformFloatArrayCurve 
{
	public CXTransformFloatArraySinDampCurve ( int arrLen ) : base (arrLen) {}

	private float[] MaxAmplitude;
	private float PICycle;
	private float PIOffset;
	private float AmplitudeDampPow;
	private float Duration;
	//private float PISpeed;
	private float CurrentTime;

	// 内置的 终点震荡 运动
	public void InitForCrashDefault ( float[] startValue, float[] destValue, float duration )
	{
		#if UNITY_EDITOR
		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif
		// 振幅衰减幂次系数 = 7f;
		// 持续的周期PI数 = 4.5f;
		this.InitForCrashCustom ( startValue, destValue, duration, 7f, 4.5f );
	}
	// 自定义 终点震荡 运动
	// amplitudeDampPow 振幅衰减幂次系数, 持续的周期PI数
	public void InitForCrashCustom ( float[] startValue, float[] destValue, float duration, float amplitudeDampPow, float piCycle )
	{
		#if UNITY_EDITOR
		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif
		this.Init ( destValue, Sub ( destValue, startValue ), amplitudeDampPow, -0.5f, piCycle, duration);
	}

	// 	通用的正弦震荡衰减,  可用于定制各种特定的曲线运动
	//  	震荡中心值, 最大振幅, 振幅衰减幂次系数, 持续的周期PI数, 持续的时间
	public void Init ( float[] destValue, float[] amplitude, float amplitudeDampPow, float piOffset, float piCycle, float duration )
	{
		#if UNITY_EDITOR
		if ( this.ArrLen != destValue.Length || this.ArrLen != amplitude.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif

		this.MaxAmplitude = amplitude;
		this.DestValue = destValue;
		this.Duration = duration;
		this.AmplitudeDampPow = amplitudeDampPow;
		this.PIOffset = piOffset;
		this.PICycle = piCycle;
		this.CurrentTime = 0f;
		//this.PISpeed = piCycle / duration;
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;

		if ( this.CurrentTime < this.Duration )
		{
			float timeScale = this.CurrentTime / this.Duration;

			float[] currentAmplitude = Mul ( this.MaxAmplitude, Mathf.Pow ( 1f - timeScale, this.AmplitudeDampPow ) );
			float currentPIValue = this.PIOffset + this.PICycle * timeScale;

			//this.SetValue ( this.DestValue + Mathf.Sin ( currentPIValue * Mathf.PI ) * currentAmplitude ); 
			this.SetValue ( Add( this.DestValue, Mul( currentAmplitude, Mathf.Sin ( currentPIValue * Mathf.PI ) )) );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}