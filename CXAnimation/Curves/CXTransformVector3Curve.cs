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
 * Vector3 变化曲线 基类
 *
 *	AddSetDelegate ( Action<Vector3> setDelegate ) :		添加 赋值函数指针
 *	RemoveSetDelegate ( Action<Vector3> setDelegate ) :	移除 赋值函数指针
 *	
 *	AddCallBack ( Action callBack ) :		添加 结束回调函数指针
 *	RemoveCallback ( Action callBack ) :	移除 结束回调函数指针
 *
 **************************************************************************************/
public abstract class CXTransformVector3Curve : CXTransformValueCurve
{
	public Vector3 DestValue;
	protected List<Action<Vector3>> SetDelegateList = new List<Action<Vector3>> ();

	public void AddSetDelegate ( Action<Vector3> setDelegate )
	{
		if ( null != setDelegate && !this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Add ( setDelegate );
		}
	}

	public void RemoveSetDelegate ( Action<Vector3> setDelegate )
	{
		if ( null != setDelegate && this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Remove ( setDelegate );
		}
	}

	protected void SetValue ( Vector3 value )
	{
		foreach ( Action<Vector3> setDelegate in this.SetDelegateList )
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