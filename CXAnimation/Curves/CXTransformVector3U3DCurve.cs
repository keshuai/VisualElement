﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

public class CXTransformVector3U3DCurve : CXTransformVector3Curve 
{
	public Vector3 StartValue;
	public Vector3 DeltaValue;
	public float CurrentTime;
	public float Duration;
	public bool Loop;
	public AnimationCurve U3DCurve;

	/// 目标值根据曲线自动计算,请确保曲线终点的准确性
	public void Init ( Vector3 startValue, Vector3 destValue, float duration, AnimationCurve u3dCurve )
	{
		this.StartValue = startValue;

		this.Duration = duration;
		this.CurrentTime = 0f;
		this.DeltaValue = destValue - startValue;
		this.U3DCurve = u3dCurve;

		this.DestValue = startValue + this.DeltaValue * u3dCurve.Evaluate (1f);
	}
		

	public override void TimeStep ( float timeStep )
	{
		this.CurrentTime += timeStep;

		if ( !this.Loop && this.CurrentTime >= this.Duration)
		{
			this.MakeItLifeEnd();
		}
		else
		{
			this.SetValue ( this.StartValue + this.DeltaValue * this.U3DCurve.Evaluate (this.CurrentTime / this.Duration) );
		}

	}
}