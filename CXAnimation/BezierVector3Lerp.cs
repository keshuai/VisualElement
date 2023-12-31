﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class BezierVector3Lerp 
{
	private Vector3[] PowerPoints;

	public BezierVector3Lerp ( Vector3[] powerPoints )
	{
		#if CXDebug
		if ( powerPoints.Length < 2 ) 
		{
			Debug.LogError ( "数组元素个数必须大于或等于2" );
		}
		#endif

		this.PowerPoints = powerPoints;
	}

	public Vector3 Evaluate ( float k )
	{
		return GetPointsBetweenNPoint ( PowerPoints, k ) [0];
	}

	private Vector3[] GetPointsBetweenNPoint ( Vector3[] points, float k )
	{
		int len = points.Length - 1;


		#if CXDebug
		if ( len < 1 ) 
		{
			Debug.LogError ( "数组元素个数必须大于或等于2" );
		}
		#endif

		Vector3[] newPoints = new Vector3[ len ];

		for ( int i = 0; i < len ; ++i )
		{
			newPoints[i] = GetPointBetweenTwoPoint ( points[i], points[i + 1], k );
		}

		if ( len == 1 )
		{
			return newPoints;
		}

		return GetPointsBetweenNPoint ( newPoints, k );
	}

	private Vector3 GetPointBetweenTwoPoint ( Vector3 first, Vector3 second, float k )
	{
		return first + ( second - first ) * k;
	}
}