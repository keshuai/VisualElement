﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatArrayPhysxUPCurve : CXTransformFloatArrayCurve 
{
	public CXTransformFloatArrayPhysxUPCurve ( int arrLen ) : base (arrLen) {}

	public float[] StartValue;
	public float[] Accelerate;
	public float Duration;
	public float CurrentTime;

	public void Init ( float[] startValue, float[] destValue, float duration )
	{

		#if UNITY_EDITOR
		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif

		// set
		this.StartValue = startValue;
		this.DestValue = destValue;
		this.Duration = duration;
		this.CurrentTime = 0f;

		// cal
		// 0.5 * a * t * t  = s ==> a = 2s/(t * t)
		this.Accelerate = Div ( Mul ( Sub ( destValue, startValue ), 2f ) , duration * duration );
	}

	public override void TimeStep ( float timeStep )
	{
		this.CurrentTime += timeStep;
		if ( this.CurrentTime < this.Duration )
		{
			this.SetValue ( Add ( this.StartValue, Mul ( this.Accelerate, 0.5f *  this.CurrentTime * this.CurrentTime )) );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}