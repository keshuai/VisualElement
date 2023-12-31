﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// CXNoneGesture. 拦截手势, 位于其后的 (非焦点)手势 都无法解析。
/// </summary>



public class CXNoneGesture : CXTouchEvent 
{
	public static CXNoneGesture EnableGesture (GameObject o)
	{
		return EnableGesture (o ,false);
	}

	public static CXNoneGesture EnableGesture (GameObject o, bool autoResizeBoxCollider)
	{
		if (o == null)
		{
			return null;
		}

		CXNoneGesture noneGesture = o.GetComponent<CXNoneGesture>();
		if (noneGesture == null)
		{
			noneGesture = o.gameObject.AddComponent<CXNoneGesture>();
		}
		noneGesture.AutoResizeBoxCollider = autoResizeBoxCollider;

		return noneGesture;
	}


	public BoxCollider Collider 
	{ 
		get 
		{ 
			Collider c = this.GetComponent<Collider>(); 
			return c == null? this.gameObject.AddComponent<BoxCollider>() : c as BoxCollider; 
		} 
	}
	public int depth 
	{ 
		get  
		{ 
			CX.VEle w = this.GetComponent<CX.VEle>();
			return w == null? 0 : w.depth;
		}  
		set  
		{ 
			CX.VEle w = this.GetComponent<CX.VEle>();
			if (w != null)
			{
				//w.depth = value;
			}
		} 
	}

	public bool AutoResizeBoxCollider
	{
		get 
		{ 
			CX.VEle w = this.GetComponent<CX.VEle>();
			return w == null? false : w.autoResizeBoxCollider;
		}
		set 
		{
			CX.VEle w = this.GetComponent<CX.VEle>();
			if (w != null)
			{
				w.autoResizeBoxCollider = value; 
				if (value)
				{
					//this.Collider.size = w.localSize;
				}
			}
		}
	}



	void Awake ()
	{
	}

	public void SetBoxColliderSize (float w, float h)
	{
		this.SetBoxColliderSize(new Vector3(w, h, 0f));
	}
	public void SetBoxColliderSize (Vector3 size)
	{
		CX.VEle w = this.GetComponent<CX.VEle>();
		if (w != null)
		{
			w.autoResizeBoxCollider = false;
		}


		Collider c = this.GetComponent<Collider>();
		BoxCollider boxCollider = null;
		if (c == null)
		{
			boxCollider = this.gameObject.AddComponent<BoxCollider>();
		}
		else
		{
			boxCollider = c as BoxCollider;
		}

		if (boxCollider != null)
		{
			boxCollider.size = size;
		}
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		touchParser.StopParse();
	}
	public override void OnTouchCancel (CXTouch touch)
	{
	}
}