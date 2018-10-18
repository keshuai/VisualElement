﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
namespace CXEditor
{
	[CustomEditor(typeof(VEle))]
	public class VEleInspector : Editor 
	{
		/// 编辑器下的自动添加 
		private void CheckDracall (VEle _this)
		{
			if (_this.Drawcall == null)
			{
				Drawcall drawcall = _this.transform.GetComponentInParent<Drawcall>();
				if (drawcall == null)
				{
					EditorGUILayout.HelpBox("this V Ele no Drawcall", MessageType.Error);
				}
				else
				{
					drawcall.AddElement(_this);
				}
			}
			else if (!_this.Drawcall.HasVEle(_this))
			{
				_this.Drawcall.AddElement(_this);
			}
		}


		void DepthMoveBack()
		{
			VEle _this = (VEle)this.target;
			_this.Drawcall.ElementIndexSub1(_this);
		}

		void DepthMoveFront()
		{
			VEle _this = (VEle)this.target;
			_this.Drawcall.ElementIndexAdd1(_this);
		}

		public override void OnInspectorGUI ()
		{
			GUILayout.Space(10);

			VEle _this = (VEle)this.target;
			this.CheckDracall(_this);
			
			EditorUILayout.ObjectField("View(Read Only)", _this.Drawcall, typeof(Drawcall));
			EditorUILayout.IntField("Asset Index(Read Only)", EasyReflect.GetField<int>(_this, "internalAssetIndex"));

			if (_this.Drawcall == null)
			{
				return;
			}

			EditorUILayout.IntFieldPlus2("Depth", "【 " + _this.depth + " 】", DepthMoveBack, DepthMoveFront);
			_this.Alpha = EditorUILayout.RangeField("Alpha", _this.Alpha, 0 ,1);
			_this.Scale = EditorUILayout.FloadField("Scale", _this.Scale);
			//_this.NotInChildRoot = EditorUILayout.BoolField("NotInChildRoot", _this.NotInChildRoot);
		} 
	}
}