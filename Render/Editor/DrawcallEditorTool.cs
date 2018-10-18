﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using CX;

namespace CXEditor
{
	public class DrawcallEditorTool 
	{
		internal static bool RuntimeIsEditor
		{
			get
			{
				return EasyReflect.GetStaticField<bool, DrawcallTool>("RuntimeIsEditor");
			}
			set
			{
				EasyReflect.SetStaticField<DrawcallTool>("RuntimeIsEditor", value);
			}
		}

		[MenuItem("CX/Drawcall/Detach Childs")]

		static void DetachChilds ()
		{
			RuntimeIsEditor = false;

			Drawcall[] drawcalls = GameObject.FindObjectsOfType<Drawcall>();
			foreach(Drawcall dc in drawcalls)
			{
				if (dc.ChildRoot.parent == dc.transform)
				{
					dc.ChildRoot.SetParent(null, false);
				}
			}
		}

		[MenuItem("CX/Drawcall/Together Childs")]
		static void TogetherChilds ()
		{
			RuntimeIsEditor = true;

			Drawcall[] drawcalls = GameObject.FindObjectsOfType<Drawcall>();
			foreach(Drawcall dc in drawcalls)
			{
				if (dc.ChildRoot != null && dc.ChildRoot.parent == null)
				{
					dc.ChildRoot.SetParent(dc.transform, false);
				}
			}
		}
	}
}