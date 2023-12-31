﻿/*******************************************
 ** CX  UTF-16 Little-Endian 模版**
 *******************************************/

using UnityEngine;
using System.Collections;

namespace CX
{
	public class ViewController : MonoBehaviour
	{
		public Transform ViewTrans;
		public View view;
	
		/// 当前控制器的 导航器 
		public ViewControllerNavigation Navigation 
		{ 
			get 
			{
				Transform parent = this.transform.parent;
				if (parent == null)
				{
					return null;
				}
	
				return parent.GetComponentInParent<ViewControllerNavigation>();
			}
		}
	
		public virtual void Init ()
		{
	
		}
		
		public virtual void OnScreenSizeChanged ()
		{
			
		}
	
		public virtual void WillActive()
		{
			// 
			if (this.view == null)
			{
				GameObject o = new GameObject();
				Transform trans = o.transform;
				trans.parent = this.transform;
				trans.localPosition = Vector3.zero;
				trans.name = "ViewTrans";
				this.ViewTrans = trans;
				this.view = trans.gameObject.AddComponent<View>();
			}
		}
		public virtual void DidActive(){}
	
		public virtual void WillDisable(){}
		public virtual void DidDisable()
		{
			//CXUIPool.Collect( this );
//			Destroy( this.ViewTrans.gameObject );
//			this.ViewTrans = null;
//			this.view = null;
		}
	
		public virtual void Release()
		{
//			this.WillDisable();
//			this.DidDisable();
//			Destroy( this.gameObject );
		}
	}
}