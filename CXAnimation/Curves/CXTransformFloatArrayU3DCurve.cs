﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFloatArrayU3DCurve : CXTransformFloatArrayCurve 
{
	public CXTransformFloatArrayU3DCurve ( int arrLen ) : base (arrLen) {}

	public float[] StartValue;
	public float[] DeltaValue;
	public float CurrentTime;
	public float Duration;
	public bool Loop;
	public AnimationCurve U3DCurve;

//	public void Init ( float[] startValue, float[] destValue, float duration, AnimationCurve u3dCurve )
//	{
//		#if UNITY_EDITOR
//		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
//		{
//			Debug.LogError ( "数组长度不匹配" );
//		}
//		#endif
//
//		this.StartValue = startValue;
//		this.DestValue = destValue;
//		this.Duration = duration;
//		this.CurrentTime = 0f;
//		//this.DeltaValue = destValue - startValue;
//		this.DeltaValue = Sub ( destValue, startValue );
//		this.U3DCurve = u3dCurve;
//	}

	public void Init ( float[] startValue, float[] destValue, float duration, AnimationCurve u3dCurve )
	{
		#if UNITY_EDITOR
		if ( this.ArrLen != startValue.Length || this.ArrLen != destValue.Length )
		{
			Debug.LogError ( "数组长度不匹配" );
		}
		#endif

		this.StartValue = startValue;
		//this.DestValue = destValue;
		this.Duration = duration;
		this.CurrentTime = 0f;
		this.DeltaValue = Sub ( destValue, startValue );//this.DeltaValue = destValue - startValue;
		this.U3DCurve = u3dCurve;

		this.DestValue = Add( startValue, Mul( this.DeltaValue, u3dCurve.Evaluate (1f) ) );
		//this.DestValue = startValue + this.DeltaValue * u3dCurve.Evaluate (1f);
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
			//this.SetValue ( this.StartValue + this.DeltaValue * this.U3DCurve.Evaluate (this.CurrentTime / this.Duration) );
			this.SetValue ( Add ( this.StartValue, Mul ( this.DeltaValue, this.U3DCurve.Evaluate ( this.CurrentTime / this.Duration ) ) ) );
		}
	}
}