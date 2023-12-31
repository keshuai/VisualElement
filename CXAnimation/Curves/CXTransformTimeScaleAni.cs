﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformTimeScaleAni : CXTransformValueCurve 
{
	public float CurrentTime;
	public float Duration;
	public float DestValue;
	public AnimationCurve U3DCurve;

	public void Init (float duration, AnimationCurve u3dCurve)
	{
		this.CurrentTime = 0f;
		this.Duration = duration;
		this.U3DCurve = u3dCurve;
		this.DestValue = this.U3DCurve.Evaluate(1f);
	}

	public override void MakeItLifeEnd ()
	{
		this.Alive = false;
		this.CallBack();
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;
		if (this.CurrentTime < this.Duration)
		{
			Time.timeScale = this.U3DCurve.Evaluate(this.CurrentTime / this.Duration);
		}
		else
		{
			Time.timeScale = this.DestValue;
			this.MakeItLifeEnd();
		}
	}


}