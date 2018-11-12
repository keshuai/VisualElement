/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
namespace CXEditor
{
	[CustomEditor(typeof(VEle))][CanEditMultipleObjects]
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

			EditorUILayout.IntField("Depth(Read Only)", _this.depth);
			_this.Alpha = EditorUILayout.RangeField("Alpha", _this.Alpha, 0 ,1);
			_this.Scale = EditorUILayout.FloadField("Scale", _this.Scale);
			//_this.NotInChildRoot = EditorUILayout.BoolField("NotInChildRoot", _this.NotInChildRoot);

			// 触发 ExecuteInEditMode 更新
			EditorUtility.SetDirty(target);
			if (_this.Drawcall != null)
			{
				EditorUtility.SetDirty(_this.Drawcall);
			}
		} 
	}
}