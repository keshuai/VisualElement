using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentEx 
{
	public static T CheckComponent<T>(this Component _this) where T : Component
	{
		if (_this == null) 
		{
			throw new System.Exception("this component null");
		}

		T t = _this.GetComponent<T>();
		if (t == null)
		{
			t = _this.GetComponent<T>();
		}
		if (t == null)
		{
			t = _this.gameObject.AddComponent<T>();
		}

		return t;
	}

	public static T AddChild<T>(this Component _this) where T : Component
	{
		if (_this == null) 
		{
			throw new System.Exception("this component null");
		}

		GameObject o = new GameObject(typeof(T).Name);
		o.transform.SetParent(_this.transform,  false);
		return o.AddComponent<T>();
	}
}
