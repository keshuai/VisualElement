﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CX
{

	/// 用于接收 来自 任意线程 发来的消息并执行
	/// 主要用于非主线程不能调用UnityAPI的情况
	/// 执行为异步执行，无返回值
	/// 在使用之前需要在主线程初始化
	public sealed class UnityThread : MonoBehaviour 
	{
		/// 初始化静态单例
		[RuntimeInitializeOnLoadMethod]
		public static void Init ()
		{
			if ( unityThread == null )
			{
				GameObject unityThreadGameObject = new GameObject();
				unityThreadGameObject.hideFlags = HideFlags.HideInHierarchy;
				DontDestroyOnLoad (unityThreadGameObject);

				unityThread = unityThreadGameObject.AddComponent <UnityThread>();
				unityThread.name = "";
				unityThread.Loop();
			}
		}

		/////////////////////////////////成员区(包括属性)///////////////////////////////////////////////////////////////////////////////////////////////// 
		private static UnityThread unityThread = null;
		private List<Action> m_actions = new List<Action>();
		private List<Action> m_actionsAdded = new List<Action>();

		/////////////////////////////////行为区///////////////////////////////////////////////////////////////////////////////////////////////// 

		private void Loop ()
		{
			this.InvokeRepeating("Call", 0f, 1f/60f);
		}

		private void Call ()
		{
			lock (this)
			{
				if (m_actionsAdded.Count > 0)
				{
					m_actions.AddRange(m_actionsAdded);
					m_actionsAdded.Clear();
				}
			}
			
			foreach(Action action in m_actions)
			{
				action();
			}
			
			m_actions.Clear();
		}
		
		private void AddAction (Action action)
		{
			if (action != null)
			{
				lock(this)
				{
					m_actionsAdded.Add(action);
				}
			}
		}

		public static void AsyncCall (Action action)
		{
			if (unityThread != null)
			{
				unityThread.AddAction(action);
			}
			else
			{
				Debug.Log("unityThread null");
			}
		}
	}
}
