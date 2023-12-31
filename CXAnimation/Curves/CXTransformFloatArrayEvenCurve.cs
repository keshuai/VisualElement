﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatArrayEvenCurve : CXTransformFloatArrayCurve 
{
	public CXTransformFloatArrayEvenCurve ( int arrLen ) : base (arrLen) {}

	public float[] Speed;
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
		//this.Speed = (destValue - startValue) / duration; 
		this.Speed = Div ( Sub ( destValue, startValue ) , duration);
	}

	public override void TimeStep ( float timeStep )
	{
		this.RemainingTime -= timeStep;
		if ( this.RemainingTime > 0f )
		{
			//this.SetValue ( this.DestValue - this.Speed * this.RemainingTime );
			this.SetValue ( Sub ( this.DestValue, Mul ( this.Speed, this.RemainingTime ) ));
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}