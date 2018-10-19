/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Collections.Generic;

namespace CXEditor
{
	[CustomEditor(typeof(Drawcall))]
	public class DrawcallInspector : Editor 
	{
		static Dictionary<System.Type, List<System.Type>> s_SupportedType_Dic = new Dictionary<System.Type, List<System.Type>>();

		/// 在Inspector中使用[InitializeOnLoad]调用此函数来添加drawcall的VE支持类型

		public static void AddSupportedType<T1, T2>() where T1 : Drawcall where T2 : VEle
		{
			System.Type drawcallType = typeof(T1);
			System.Type veType = typeof(T2);

			List<System.Type> list = GetSupportedTypeList(drawcallType);

			if (list.Contains(veType))
			{
				Debug.LogError(veType.Name + " add twice or more, please check it !");
				return;
			}
			list.Add(veType);
		}

		private static List<System.Type> GetSupportedTypeList(System.Type drawcallType)
		{
			List<System.Type> list;

			if (s_SupportedType_Dic.ContainsKey(drawcallType))
			{
				list = s_SupportedType_Dic[drawcallType];
			}
			else
			{
				list = new List<System.Type>();
				s_SupportedType_Dic[drawcallType] = list;
			}
			return list;
		}

		Drawcall _thisDrawcall;

		// 添加按钮文本
		protected GUIContent m_IconToolbarPlus;
		private System.Type m_AddType = null;

		CXInspectorLayoutColorFrame m_DrawShaderFrame = new CXInspectorLayoutColorFrame("DrawcallInfo");

		CXInspectorLayoutColorFrame m_DrawcallInfoFrame = new CXInspectorLayoutColorFrame("DrawcallInfo(Look Only)");
			
		CXInspectorLayoutColorFrame m_Frame = new CXInspectorLayoutColorFrame("VE list");

		List<System.Type> m_SupportedTypeList;

		protected virtual void Awake ()
		{
			_thisDrawcall = (Drawcall)this.target;
			m_SupportedTypeList = GetSupportedTypeList(_thisDrawcall.GetType());

			m_Frame.NoSwitch = true;
			m_IconToolbarPlus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"));// "+"
		}

		public override void OnInspectorGUI ()
		{
			this.DrawClip();
			this.DrawShaderInspector();
			this.DrawVEList();
			this.DrawMeshMatInspector();

			//_thisDrawcall.MarkNeedUpdate();
		}

		void DrawClip()
		{
			_thisDrawcall.Clip = EditorUILayout.BoolField("Clip", _thisDrawcall.Clip);
			if (_thisDrawcall.Clip)
			{
				Vector4 range = _thisDrawcall.ClipRange;
				Vector2 rangeX =  new Vector2(range.x, range.y);
				Vector2 rangeY = new Vector2(range.z, range.w);
				rangeX = EditorUILayout.RangeField("X Clip", rangeX);
				rangeY = EditorUILayout.RangeField("Y Clip", rangeY);
				_thisDrawcall.ClipRange = new Vector4(rangeX.x, rangeX.y, rangeY.x, rangeY.y);
			}
		}

		void OnSelectShader (Object o)
		{
			_thisDrawcall.Shader = o as Shader;
		}

		protected void DrawShaderInspector ()
		{
			Drawcall _this = (Drawcall)this.target;

			m_DrawShaderFrame.NoSwitch = true;
			m_DrawShaderFrame.Begin();
			_this.Shader = EditorUILayout.ShaderField("Shader", _this.Shader, OnSelectShader);
			_this.RenderQueue = EditorUILayout.IntField("RenderQueue", _this.RenderQueue);

			m_DrawShaderFrame.End();
		}

		void DoAddVEWithType ()
		{
			Drawcall _this = (Drawcall)this.target;
			if (m_AddType != null)
			{
				VEle e = _this.NewElement(m_AddType);

				// 在编辑器模式下, 对uv进行初始化
				ImageVE imageElement = e as ImageVE;
				if (imageElement != null)
				{
					//EasyReflect.CallMethod(imageElement, "OnSetImageInfo");
				}


				m_AddType = null;
			}
		}

		protected void DrawVEList ()
		{
			this.DoAddVEWithType();

			Drawcall _this = (Drawcall)this.target;
			m_Frame.NoSwitch = true;
			m_Frame.Begin();

			VEle[] els = _this.ElementList;

			if (els.Length == 0)
			{
				EditorGUILayout.HelpBox("this array no item", MessageType.Info);
			}
			else
			{
				for(int index = 0; index < els.Length; ++index)
				{
					VEle e = els[index];
					if (e == null)
					{
						if ((e as System.Object) == null)
						{
							EditorUILayout.TextField("     ◆ " + index + ": ", "null C# Object");//这种null不等于内存泄漏了吗
						}
						else
						{
							EditorUILayout.TextField("     ◆ " +index + ": ", "null Unity Object");
						}
					}
					else
					{
						EditorUILayout.ObjectField("     ◆ " +index + ": ", e, e.GetType());
					}
				}
			}
			this.DrawAddButton();
			m_Frame.End();
		}

		private void DrawAddButton()
		{
			// 计算添加按钮（x, y, width, hight）
			Rect btPosition = GUILayoutUtility.GetRect(m_IconToolbarPlus, GUI.skin.button);
			const float addButonWidth = 150f;
			btPosition.x = btPosition.x + (btPosition.width - addButonWidth) / 2;
			btPosition.width = addButonWidth;

			if (m_SupportedTypeList == null)
			{
			_thisDrawcall = this.target as Drawcall;
				m_SupportedTypeList = GetSupportedTypeList(_thisDrawcall.GetType());
			}

			if (m_SupportedTypeList.Count == 0)
			{
				EditorGUILayout.HelpBox("No supported VE type in current drawcall !", MessageType.Warning);
			}
			else
			{
				// 添加项按钮
				if (GUI.Button(btPosition, m_IconToolbarPlus))
				{
					TypeSelectorWindow.Show(GetSelectWindowRect(btPosition.y + 50, 300), m_SupportedTypeList, SelectAtIndex);
				}
			}
		
		}
		static Rect GetSelectWindowRect (float guiY, float height)
		{
			const float indent = 20;

			float viewWidth = EditorGUIUtility.currentViewWidth - indent * 2;
			Vector2 screenPos = EditorGUIUtility.GUIToScreenPoint(new Vector2(indent, guiY));

			return new Rect(screenPos.x, screenPos.y, viewWidth, height);
		}

		void SelectAtIndex (System.Type t)
		{
			m_AddType = t;
		}

		protected void DrawMeshMatInspector ()
		{
			m_DrawcallInfoFrame.NoSwitch = true;

			Drawcall _this = (Drawcall)this.target;
			m_DrawcallInfoFrame.Begin();
			{
				EditorGUILayout.ObjectField("Mesh", _this.Mesh, typeof(Mesh), true);
				EditorGUILayout.ObjectField("Material", _this.Mat, typeof(Material), true);
			}
			m_DrawcallInfoFrame.End();
		}
	}
}

