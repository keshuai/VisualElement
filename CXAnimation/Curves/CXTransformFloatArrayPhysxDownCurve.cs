﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatArrayPhysxDownCurve : CXTransformFloatArrayCurve 
{
	public CXTransformFloatArrayPhysxDownCurve ( int arrLen ) : base (arrLen) {}

	public float[] Accelerate;
	public float RemainingTime;

	public void Init ( float[] startValue, float[] destValue, float duration )
	{
		#if UNITY_EDITOR
		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif

		// set
		this.DestValue = destValue;
		this.RemainingTime = duration;

		// cal
		// 0.5 * a * t * t  = s ==> a = 2s/(t * t)
		this.Accelerate = Div ( Mul ( Sub ( destValue, startValue ), 2f ) , duration * duration );
	}

	public override void TimeStep ( float timeStep )
	{
		this.RemainingTime -= timeStep;
		if ( this.RemainingTime > 0f )
		{
			this.SetValue ( Sub ( this.DestValue, Mul ( this.Accelerate, 0.5f *  this.RemainingTime * this.RemainingTime )) );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}