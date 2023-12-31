﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;

/**************************************************************************************
 * 
 * EndPlayer.com
 * 
 * float值 Unity 3D 自带 趋势可视编辑 变化曲线, 对外仅提供初始化接口
 *
 *	Init ( float startValue, float destValue, float duration, AnimationCurve u3dCurve )
 *	初始值，目标值，所需时间, u3d自带的时间从0到1的趋势曲线
 *
 **************************************************************************************/

public class CXTransformFloatU3DCurve : CXTransformFloatCurve 
{
	public float StartValue;
	public float DeltaValue;
	public float CurrentTime;
	public float Duration;
	/// ***此项属性，慎用，在曲线所依附的对象销毁时，切记清理曲线，以免内存泄露
	public bool Loop;
	public AnimationCurve U3DCurve;

//	public void Init ( float startValue, float destValue, float duration, AnimationCurve u3dCurve )
//	{
//		this.StartValue = startValue;
//		this.DestValue = destValue;
//		this.Duration = duration;
//		this.CurrentTime = 0f;
//		this.DeltaValue = destValue - startValue;
//		this.U3DCurve = u3dCurve;
//	}

	public void Init ( float startValue, float destValue, float duration, AnimationCurve u3dCurve )
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