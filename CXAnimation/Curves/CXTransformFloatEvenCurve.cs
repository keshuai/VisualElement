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
 * float值 线性均匀 变化曲线, 对外仅提供初始化接口
 *
 *	Init ( float startValue, float destValue, float duration )
 *	初始值，目标值，所需时间
 *
 **************************************************************************************/

public class CXTransformFloatEvenCurve : CXTransformFloatCurve
{

	public float Speed;
	public float RemainingTime;

	public void Init ( float startValue, float destValue, float duration )
	{
		// set
		this.DestValue = destValue;
		this.RemainingTime = duration;

		// cal
		this.Speed = (destValue - startValue) / duration;
	}

	public override void TimeStep ( float timeStep )
	{
		this.RemainingTime -= timeStep;
		if ( this.RemainingTime > 0f )
		{
			this.SetValue ( this.DestValue - this.Speed * this.RemainingTime );
		}
		else
		{
			this.MakeItLifeEnd();
		}
	}
}