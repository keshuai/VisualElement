﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public abstract class CXTransformFloatArrayCurve : CXTransformValueCurve 
{
	public CXTransformFloatArrayCurve ( int arrLen) { this.ArrLen = arrLen; }

	public readonly int ArrLen;
	protected float[] DestValue;

	protected List<Action<float[]>> SetDelegateList = new List<Action<float[]>> ();

	public void AddSetDelegate ( Action<float[]> setDelegate )
	{
		if ( null != setDelegate && !this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Add ( setDelegate );
		}
	}

	public void RemoveSetDelegate ( Action<float[]> setDelegate )
	{
		if ( null != setDelegate && this.SetDelegateList.Contains ( setDelegate ) )
		{
			this.SetDelegateList.Remove ( setDelegate );
		}
	}

	protected void SetValue ( float[] value )
	{
		foreach ( Action<float[]> setDelegate in this.SetDelegateList )
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

	/// +

	protected float[] Add ( float[] arr1, float[] arr2 )
	{
		int len = this.ArrLen;
		float[] result = new float[ len ];
		for ( int i = 0; i < len; ++i )
		{
			result[i] = arr1[i] + arr2[i];
		}

		return result;
	}

	/// -
	protected float[] Sub ( float[] arr1, float[] arr2 )
	{
		int len = this.ArrLen;
		float[] result = new float[ len ];
		for ( int i = 0; i < len; ++i )
		{
			result[i] = arr1[i] - arr2[i];
		}

		return result;
	}

	/// *
	protected float[] Mul ( float[] arr, float factor )
	{
		int len = this.ArrLen;
		float[] result = new float[ len ];
		for ( int i = 0; i < len; ++i )
		{
			result[i] = arr[i] * factor;
		}

		return result;
	}

	/// /
	protected float[] Div ( float[] arr, float factor )
	{
		int len = this.ArrLen;
		float[] result = new float[ len ];
		for ( int i = 0; i < len; ++i )
		{
			result[i] = arr[i] / factor;
		}

		return result;
	}

}