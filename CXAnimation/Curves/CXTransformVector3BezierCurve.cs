﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformVector3BezierCurve : CXTransformVector3Curve 
{
	private BezierVector3Lerp Bezier;
	private float CurrentTime;
	private float Duration;
	public AnimationCurve U3DCurve;

	public void Init ( Vector3[] powerPoints, float duration )
	{
		this.Init(powerPoints, duration, null);
	}

	public void Init ( Vector3[] powerPoints, float duration, AnimationCurve u3dCurve )
	{
		this.Bezier = new BezierVector3Lerp( powerPoints );
		this.DestValue = powerPoints[powerPoints.Length - 1];
		this.Duration = duration;
		this.U3DCurve = u3dCurve;
		this.CurrentTime = 0f;
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;
		if ( this.CurrentTime < this.Duration )
		{
			float k = this.CurrentTime / this.Duration;
			if (U3DCurve != null)
			{
				k = U3DCurve.Evaluate(k);
			}
			this.SetValue( this.Bezier.Evaluate( k ) );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}