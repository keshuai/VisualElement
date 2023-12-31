﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	public class ImageColorVE : ImageNormalVE 
	{
		[SerializeField][HideInInspector] private Color m_Color = Color.white;

		public Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				if (m_Color != value)
				{
					m_Color = value;
					m_ColorChanged = true;
				}
			}
		}

		protected override void UpdateColor ()
		{
			m_ColorChanged = false;

			int vertexIndex = this.internalVertexIndex;
			List<Color> colList = m_DrawCall.m_ColList;

			Color c = m_Color;
			c.a *= m_Alpha;

			colList[vertexIndex    ] = c;
			colList[vertexIndex + 1] = c;
			colList[vertexIndex + 2] = c;
			colList[vertexIndex + 3] = c;
		}
	}
}