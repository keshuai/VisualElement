﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public abstract class CXAnimationCurve : MonoBehaviour 
{
	/// 动画曲线名称，用于动态修改时查找动画用 
	public string Name;

	/// 播放此动画曲线 
	public abstract void Play ();

	/// 停止此动画曲线 
	public abstract void Stop ();

	/// 动画对象 
	public abstract  object Ani_Target { get; }
	/// 动画是否循环 
	public abstract bool Ani_Loop { get; }
	/// 动画的时间长度, 当动画无限长时，返回-1f 
	public abstract float Ani_Duration { get; }
}