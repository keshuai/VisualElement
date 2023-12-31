﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


/**************************************************************************************
 * 
 * EndPlayer.com
 * 
 * 值变化曲线基类
 *
 *	Name : 	1.曲线的名称同时也是唯一标识符, 在同一时间存活的同名曲线只能存在一个
 *			2.在构造对象时，名称为空, 即认为曲线为匿名曲线，匿名曲线无法通过名称查找, 可以存在任意多份, 用于不需要使用名称查找的曲线
 *			3.可以通过名称查找对应的曲线 Find ( string name )
 *
 *	Alive : 	存活的
 *
 *	Pause : 	暂停的
 *
 *	MakeItAlive ():	使对象存活，并解除暂停，开始起作用
 *
 *	MakeItDie ():	使对象死亡, 在没用引用该对象时，下一帧会被清理回收内存
 *				*** 当 赋值部分的对象被强制销毁前，需要使该对象死亡, 必须在改变 gameObject的localPosition属性时，在Destroy ()需要进行该操作，或者将 该操作写入 OnDestroy() 里
 *	
 *
 **************************************************************************************/
using System.Collections.Generic;
using System;

public abstract class CXTransformValueCurve
{
	// 此对象为曲线的生命附着对象，当此对象销毁时，曲线自动销毁。默认为ValueCenter，用不销毁。
	// 主要是为了解决， 当动画未完成而其他调用强制销毁了动画对象时产生Exception。同时可用于自动销毁永久性动画曲线。
	public UnityEngine.Object AttachObject = CXTransformValueCenter.Instance;
	public bool Alive;
	public bool Pause;
	protected List<Action> CallBackList = new List<Action>();

	/// 活跃，并解除暂停, 以及加入动画队列
	public void MakeItAlive ()
	{
		this.Alive = true;
		this.Pause = false;
		CXTransformValueCenter.Instance.AddCurve ( this );
	}

	/// 直接杀掉曲线
	public void MakeItDie ()
	{
		this.Alive = false;
	}

	/// 步调
	public abstract void TimeStep ( float timeStep );

	/// 立即结束曲线的生命周期, (会立即进行最终目标值设定以及进行回调)
	public abstract void MakeItLifeEnd ();

	public void AddCallBack ( Action callBack )
	{
		if ( null != callBack && !this.CallBackList.Contains ( callBack ) )
		{
			this.CallBackList.Add ( callBack );
		}
	}

	public void RemoveCallback ( Action callBack )
	{
		if ( null != callBack && this.CallBackList.Contains ( callBack ) )
		{
			this.CallBackList.Remove ( callBack );
		}
	}

	protected void CallBack ()
	{
		foreach ( Action callBack in this.CallBackList )
		{
			callBack();
		}
	}
}