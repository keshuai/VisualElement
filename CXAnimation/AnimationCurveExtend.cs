﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


// Unity 自带曲线代码扩展
// 编辑器编辑曲线时可自动切线平坦处理，但代码未提供API，因此扩展
public static class AnimationCurveExtend 
{

	// 曲线切线平坦化
	public static void AutoFlat ( this AnimationCurve curve )
	{
		for ( int i = curve.length - 1; i >= 0; --i )
		{
			curve.SmoothTangents (i, 0);
		}
	}

	// 构造切线平坦的曲线
	public static AnimationCurve FlatCurve ( params Keyframe[] keys )
	{
		AnimationCurve curve = new AnimationCurve();
		curve.keys = keys;
		curve.AutoFlat ();
		return curve;
	}
	
	// 添加一个Frame，同时平滑处理切线
	public static void AddFlatKey ( this AnimationCurve curve, float time, float value )
	{
		curve.AddKey ( time, value );
		curve.AutoFlat ();
	}

	public static void SetValue ( this AnimationCurve curve, AnimationCurve target )
	{
		curve.keys = target.keys;
		curve.preWrapMode = target.preWrapMode;
		curve.postWrapMode = target.postWrapMode;
	}

	// 线性
	public static AnimationCurve LinearUp
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 1f, outTangent = 1f, tangentMode = 10},
				new Keyframe(){time = 1f, value = 1f, inTangent = 1f, outTangent = 1f, tangentMode = 10},
			};
			return new AnimationCurve(keys);
		}
	}
	public static AnimationCurve LinearDown
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 1f, inTangent = -1f, outTangent = -1f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 0f, inTangent = -1f, outTangent = -1f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}




	// 加速曲线
	public static AnimationCurve AccelerateUp
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 2f, outTangent = 2f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}
	public static AnimationCurve DecelerateDown
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 1f, inTangent = -2f, outTangent = -2f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}

	// 减速
	public static AnimationCurve DecelerateUp
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 2f, outTangent = 2f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}
	public static AnimationCurve AccelerateDown
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 0f, inTangent = -2f, outTangent = -2f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}

	// 平滑 渐进
	public static AnimationCurve SmoothIn
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 0.75f, value = 0.9f, inTangent = 0.9f, outTangent = 0.9f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}

	// 平滑
	public static AnimationCurve SmoothUp
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}
	// 平滑
	public static AnimationCurve SmoothDown
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}

	// 结尾回弹
	public static AnimationCurve SpringBack
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 0f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 0.8175627f, value = 1.084469f, inTangent = 1.605451f, outTangent = 1.605451f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 2f, outTangent = 2f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}

	// 结尾回弹
	public static AnimationCurve One
	{
		get
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(){time = 0f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
				new Keyframe(){time = 1f, value = 1f, inTangent = 0f, outTangent = 0f, tangentMode = 0},
			};
			return new AnimationCurve(keys);
		}
	}


}