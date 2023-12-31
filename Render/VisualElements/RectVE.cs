﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

namespace CX
{
	public abstract class RectVE : VEle 
	{
		[SerializeField][HideInInspector] protected float m_Width = 100;
		[SerializeField][HideInInspector] protected float m_Height = 100;
		/// 用于特殊的情况: 在不允许外面设定长宽时有用
		[SerializeField][HideInInspector] protected bool m_CanSetSize = true;

		[SerializeField][HideInInspector] protected bool m_SizeChanged = true;

		public float Width
		{
			get 
			{
				return m_Width;
			}
			set
			{
				if (value < 0) value = 0;

				if (m_CanSetSize && m_Width != value)
				{
					m_Width = value;
					m_SizeChanged = true;
					this.MarkNeedUpdate();
				}
			}
		}

		public float Height
		{
			get 
			{
				return m_Height;
			}
			set
			{
				if (value < 0) value = 0;

				if (m_CanSetSize && m_Height != value)
				{
					m_Height = value;
					m_SizeChanged = true;
					this.MarkNeedUpdate();
				}
			}
		}

		public Vector2 Size
		{
			get
			{
				return new Vector2(m_Width, m_Height);
			}
			set
			{
				this.Width = value.x;
				this.Height = value.y;
			}
		}


		void OnDrawGizmos ()
		{
			CXGizmos.Draw2DRect(this.GetGizmosMatrix(), m_Width, m_Height);
		}
	}
}