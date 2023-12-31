﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// Easy extend For Reflect
public static class EasyReflect 
{

	//---------------------------------------------------------------------------------------------
	// call class method (static method)
	//---------------------------------------------------------------------------------------------
	public static void CallStaticMethod<T>(string methodName)
	{
		CallStaticMethod(typeof(T), methodName);
	}
	public static TResult CallStaticMethod<TResult, T>(string methodName)
	{
		return (TResult)CallStaticMethod(typeof(T), methodName);
	}
	public static object CallStaticMethod(Type type, string methodName)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			throw new Exception(type.Name + " no matching method named " + methodName);
		}

		return method.Invoke(null, null);
	}
	public static void CallStaticMethod<T>(string methodName, Type[] paramTypes, object[] paramValues)
	{
		CallStaticMethod(typeof(T), methodName, paramTypes, paramValues);
	}
	public static TResult CallStaticMethod<TResult, T>(string methodName, Type[] paramTypes, object[] paramValues)
	{
		return (TResult)CallStaticMethod(typeof(T), methodName, paramTypes, paramValues);
	}
	public static object CallStaticMethod(Type type, string methodName, Type[] paramTypes, object[] paramValues)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
		if (method == null)
		{
			throw new Exception(type.Name + " no matching method named " + methodName);
		}

		return method.Invoke(null, paramValues);
	}

	//---------------------------------------------------------------------------------------------
	// call instance method
	//---------------------------------------------------------------------------------------------
	public static T CallMethod <T>(object o, string methodName)
	{
		return (T)CallMethod(o, methodName);
	}
	public static object CallMethod (object o, string methodName)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect can not get the instance type");
		}

		MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			throw new Exception(type.Name + " no matching method named " + methodName);
		}

		return method.Invoke(o, null);
	}

	public static T CallMethod<T>(object o, string methodName, Type[] paramTypes, object[] paramValues)
	{
		return (T)CallMethod(o, methodName, paramTypes, paramValues);
	}
	public static object CallMethod(object o, string methodName, Type[] paramTypes, object[] paramValues)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
		if (method == null)
		{
			throw new Exception(type.Name + " no matching method named " + methodName);
		}

		return method.Invoke(o, paramValues);
	}


	//---------------------------------------------------------------------------------------------
	// static filed
	//---------------------------------------------------------------------------------------------

	public static void SetStaticField <T>(string filedName, object v)
	{
		SetStaticField(typeof(T), filedName, v);
	}
	public static void SetStaticField (Type type, string filedName, object v)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		FieldInfo fieldInfo = type.GetField(filedName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo == null)
		{
			throw new Exception(type.Name + " no filed named " + filedName);
		}

		fieldInfo.SetValue(null, v);
	}

	public static TResult GetStaticField <TResult, T>( string filedName)
	{
		return (TResult)GetStaticField(typeof(T), filedName);
	}

	public static object GetStaticField (Type type, string name)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		FieldInfo fieldInfo = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo == null)
		{
			throw new Exception(type.Name + " no filed named " + name);
		}

		return fieldInfo.GetValue(null);
	}


	//---------------------------------------------------------------------------------------------
	// instance filed
	//---------------------------------------------------------------------------------------------

	public static void SetField (object o, string filedName, object v)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		FieldInfo fieldInfo = type.GetField(filedName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo == null)
		{
			throw new Exception(type.Name + " no filed named " + filedName);
		}

		fieldInfo.SetValue(o, v);
	}

	public static T GetField <T>(object o, string filedName)
	{
		return (T)GetField(o, filedName);
	}

	public static object GetField (object o, string name)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		FieldInfo fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo == null)
		{
			throw new Exception(type.Name + " no filed named " + name);
		}

		return fieldInfo.GetValue(o);
	}

	public static T GetAnyField <T>(object o)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		Type targetType = typeof(T);
		FieldInfo[] fieldInfoArray = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach (FieldInfo fieldInfo in fieldInfoArray)
		{
			if (fieldInfo.FieldType == targetType || fieldInfo.FieldType.IsSubclassOf(targetType))
			{
				return (T)fieldInfo.GetValue(o);
			}
		}

		return default(T);
	}


	//---------------------------------------------------------------------------------------------
	// static property
	//---------------------------------------------------------------------------------------------

	public static void SetStaticProperty <T>(string name, object v)
	{
		SetStaticProperty(typeof(T), name, v);
	}

	public static void SetStaticProperty (Type type, string name, object v)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (propertyInfo == null)
		{
			throw new Exception(type.Name + " no property named " + name);
		}

		MethodInfo setMethod = propertyInfo.GetSetMethod();
		if (setMethod == null)
		{
			setMethod = propertyInfo.GetSetMethod(true);
		}

		if (setMethod == null)
		{
			throw new Exception(type.Name + " has no set " + name);
		}

		setMethod.Invoke(null, new object[]{v});
	}

	public static TResult GetStaticProperty <TResult, T>(string name)
	{
		return (TResult)GetStaticProperty(typeof(T), name);
	}

	public static object GetStaticProperty (Type type, string name)
	{
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (propertyInfo == null)
		{
			throw new Exception(type.Name + " no property named " + name);
		}

		MethodInfo getMethod = propertyInfo.GetGetMethod();// iOS can not use get reflect, but can use method
		if (getMethod == null)
		{
			getMethod = propertyInfo.GetGetMethod(true);
		}

		if (getMethod == null)
		{
			throw new Exception(type.Name + " has no get " + name);
		}

		return getMethod.Invoke(null, null);
	}

	//---------------------------------------------------------------------------------------------
	// instance property
	//---------------------------------------------------------------------------------------------

	public static void SetProperty (object o, string name, object v)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (propertyInfo == null)
		{
			throw new Exception(type.Name + " no property named " + name);
		}

		MethodInfo setMethod = propertyInfo.GetSetMethod();
		if (setMethod == null)
		{
			setMethod = propertyInfo.GetSetMethod(true);
		}

		if (setMethod == null)
		{
			throw new Exception(type.Name + " has no set " + name);
		}

		setMethod.Invoke(o, new object[]{v});
	}

	public static T GetProperty <T>(object o, string name)
	{
		return (T)GetProperty(o, name);
	}

	public static object GetProperty (object o, string name)
	{
		if (o == null)
		{
			throw new Exception("Reflect instance can not be bull");
		}

		Type type = o.GetType();
		if (type == null)
		{
			throw new Exception("Reflect type can not be bull");
		}

		PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (propertyInfo == null)
		{
			throw new Exception(type.Name + " no property named " + name);
		}

		MethodInfo getMethod = propertyInfo.GetGetMethod();// iOS can not use get reflect, but can use method
		if (getMethod == null)
		{
			getMethod = propertyInfo.GetGetMethod(true);
		}

		if (getMethod == null)
		{
			throw new Exception(type.Name + " has no get " + name);
		}

		return getMethod.Invoke(o, null);
	}
}