﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTouchEvent : MonoBehaviour
{
	[SerializeField] private CX.Drawcall _panel;
	[SerializeField] public CX.VEle widget;
	public CX.Drawcall panel 
	{
		get { return this.widget == null? _panel : this.widget.Drawcall; }
		set { _panel = value; }
	}

	public virtual void OnTouchEvent ( CXTouchParser touchParser, CXTouch touch ){}
	public virtual void OnTouchCancel ( CXTouch touch ){}
	
	// OnDisable 时应该进行事件取消处理 尤其是焦点事件
}