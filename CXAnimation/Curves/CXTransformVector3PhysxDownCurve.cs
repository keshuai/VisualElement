/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformVector3PhysxDownCurve : CXTransformVector3Curve 
{
	public Vector3 Accelerate;
	public float RemainingTime;

	public void Init ( Vector3 startValue, Vector3 destValue, float duration )
	{
		// set
		this.DestValue = destValue;
		this.RemainingTime = duration;

		// cal
		// 0.5 * a * t * t  = s ==> a = 2s/(t * t)
		this.Accelerate = 2f * (destValue - startValue) / (duration * duration);
	}

	public override void TimeStep ( float timeStep )
	{
		this.RemainingTime -= timeStep;
		if ( this.RemainingTime > 0f )
		{
			this.SetValue ( this.DestValue - 0.5f * this.Accelerate *  this.RemainingTime * this.RemainingTime );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}