﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformVector3PhysxUpCurve : CXTransformVector3Curve 
{
	public Vector3 StartValue;
	public Vector3 Accelerate;
	public float Duration;
	public float CurrentTime;

	public void Init ( Vector3 startValue, Vector3 destValue, float duration )
	{
		// set
		this.DestValue = destValue;
		this.StartValue = startValue;
		this.Duration = duration;
		this.CurrentTime = 0f;

		// cal
		this.Accelerate = 2f * (destValue - startValue) / (duration * duration);
	}

	public override void TimeStep ( float timeStep )
	{
		this.CurrentTime += timeStep;
		if ( this.CurrentTime < this.Duration )
		{
			this.SetValue ( this.StartValue + 0.5f * this.Accelerate * this.CurrentTime * this.CurrentTime );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}