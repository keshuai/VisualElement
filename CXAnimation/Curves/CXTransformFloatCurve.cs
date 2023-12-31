﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/**************************************************************************************
 * 
 * EndPlayer.com
 * 
 * float值 变化曲线 基类
 *
 *	AddSetDelegate ( Action<float> setDelegate ) :	添加 赋值函数指针
 *	RemoveSetDelegate ( Action<float> setDelegate ) :	移除 赋值函数指针
 *	
 *	AddCallBack ( Action callBack ) :		添加 结束回调函数指针
 *	RemoveCallback ( Action callBack ) :	移除 结束回调函数指针
 *
 **************************************************************************************/

public abstract class CXTransformFloatCurve : CXTransformValueCurve
{
	protected float DestValue;
	protected List<Action<float>> SetDelegateList = new List<Action<float>> ();

	public void AddSetDelegate ( Action<float> setDelegate )
	{
		if ( null != setDelegate && !this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Add ( setDelegate );
		}
	}

	public void RemoveSetDelegate ( Action<float> setDelegate )
	{
		if ( null != setDelegate && this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Remove ( setDelegate );
		}
	}

	protected void SetValue ( float value )
	{
		foreach ( Action<float> setDelegate in this.SetDelegateList )
		{
			setDelegate ( value );
		}
	}

	public override void MakeItLifeEnd ()
	{
		this.Alive = false;
		this.SetValue ( this.DestValue );
		this.CallBack();
	}
}