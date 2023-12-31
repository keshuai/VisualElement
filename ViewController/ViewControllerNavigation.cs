﻿/*******************************************
 ** CX  UTF-16 Little-Endian 模版**
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CX
{
	// 兼容预设编辑与代码动态创建
	public class ViewControllerNavigation : MonoBehaviour
	{
		[SerializeField]List<ViewController> ViewControllerList;
		Stack<ViewController> vcStack = new Stack<ViewController>();

		ViewController Current;
		[SerializeField]ViewController StartController;
		public Action<ViewController, Action> DisableAnimation;
		public Action<ViewController, Action> ActiveAnimation;
	
		public ViewControllerNavigation ParentNav
		{
			get 
			{
				Transform parentTrans = this.transform.parent;
				if (parentTrans == null)
				{
					return null;
				}
	
				return parentTrans.GetComponentInParent<ViewControllerNavigation>();
			}
		}
	
		void Awake ()
		{
			if (ViewControllerList == null)
				ViewControllerList = new List<ViewController>();
		}

		void Start()
		{
			if (this.Current == null && this.StartController != null)
			{
				this.Current = this.StartController;
				this.Current.WillActive();
				this.Current.DidActive();
			}
		}

		// 销毁
		public void Release ()
		{
			foreach ( ViewController vc in ViewControllerList )
			{
				vc.Release();
			}
		}
	
	
		public void LoadFirstViewController<T>() where T :ViewController
		{
			this.Current = this.Get(typeof(T));
			this.Current.WillActive();
			this.Current.DidActive();
		}
	
		public T Find<T> () where T : ViewController
		{
			return Find( typeof(T) ) as T;
		}
	
		public ViewController Find ( Type vcType )
		{
			foreach ( ViewController vc in ViewControllerList )
			{
				if ( vc.GetType().Equals( vcType ) )
				{
					return vc;
				}
			}
	
			return null;
		}
	
		public T Get<T> () where T : ViewController
		{
			return Get( typeof (T) ) as T; 
		}
	
		public ViewController Get ( Type vcType )
		{
			ViewController vc = Find( vcType );
			if ( vc == null )
			{
				vc = (ViewController)(new GameObject().AddComponent(vcType));
				vc.transform.parent = this.transform;
				vc.transform.localPosition = Vector3.zero;
				#if UNITY_EDITOR
				vc.name = vcType.Name;
				#endif
				vc.Init();
	
				ViewControllerList.Add( vc );
			}
	
			return vc;
		}
	
	
		public void ActiveViewController <T>() where T : ViewController
		{
			this.ActiveViewController( typeof(T) );
		}
	
		public void ActiveViewController (Type vcType)
		{
			ViewController activeView = this.Get (vcType);
			if (activeView != this.Current)
			{
				this.vcStack.Push(activeView);
				Debug.Log("Push vc");
				this.DoChangeViewController( activeView, this.Current );
				this.Current = activeView;
			}
		}
		void GoBack (ViewController vc)
		{
			this.DoChangeViewController(vc, this.Current);
			this.Current = vc;
		}
	
		public void GoBack ()
		{
			Debug.Log("GoBack");
			if (this.vcStack.Count > 1)
			{
				this.vcStack.Pop();
				ViewController activeView = this.vcStack.Peek();
				Debug.Log("Pop vc");
				this.GoBack(activeView);
			}
		}
	
		public void GoForward()
		{
	
		}
	
	
		void DoChangeViewController ( ViewController willActiveVC, ViewController willDisableVC )
		{
	//		Action didActiveAction = delegate 
	//		{
	//			if (willActiveVC != null)
	//			{
	//				willActiveVC.DidActive();
	//			}
	//		};
	
	
			Action activeAction = delegate 
			{
				// 在函数内部等待一帧, UIPool. Destroy需要等待一帧才会执行
				this.StartCoroutine( this.DoChangeViewController_ActiveView( willActiveVC, willDisableVC ) );
	//			if (willDisableVC != null)
	//			{
	//				willDisableVC.DidDisable();
	//			}
	//
	//			if (willActiveVC != null)
	//			{
	//				willActiveVC.WillActive();
	//			}
	//
	//			if (willActiveVC != null && ActiveAnimation != null)
	//			{
	//				ActiveAnimation(willActiveVC, didActiveAction);
	//			}
	//			else
	//			{
	//				didActiveAction();
	//			}
			};
	
	
			if (willDisableVC != null)
			{
				willDisableVC.WillDisable();
			}
	
			if (willDisableVC != null && DisableAnimation != null)
			{
				DisableAnimation ( willDisableVC, activeAction );
			}
			else
			{
				activeAction();
			}
		}
	
		IEnumerator DoChangeViewController_ActiveView (ViewController willActiveVC, ViewController willDisableVC)
		{
			Action didActiveAction = delegate 
			{
				if (willActiveVC != null)
				{
					willActiveVC.DidActive();
				}
			};
	
			if (willDisableVC != null)
			{
				willDisableVC.DidDisable();
			}
	
			yield return null;
			yield return null;
	
			if (willActiveVC != null)
			{
				willActiveVC.WillActive();
			}
	
			if (willActiveVC != null && ActiveAnimation != null)
			{
				ActiveAnimation(willActiveVC, didActiveAction);
			}
			else
			{
				didActiveAction();
			}
	
			yield break;
		}
	
	}
}