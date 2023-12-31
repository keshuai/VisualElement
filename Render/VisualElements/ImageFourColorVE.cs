﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	public class ImageFourColorVE : ImageNormalVE 
	{
		[SerializeField][HideInInspector] private Color m_Color0 = new Color(255f/255f, 182f/255f, 156f/255f, 1f);
		[SerializeField][HideInInspector] private Color m_Color1 = new Color(216f/255f, 114f/255f, 255f/255f, 1f);
		[SerializeField][HideInInspector] private Color m_Color2 = new Color( 71f/255f, 206f/255f, 255f/255f, 1f);
		[SerializeField][HideInInspector] private Color m_Color3 = new Color(114f/255f, 255f/255f, 150f/255f, 1f);


		public Color TopLeftColor
		{
			get
			{
				return m_Color3;
			}
			set
			{
				if (m_Color3 != value)
				{
					m_Color3 = value;
					m_ColorChanged = true;
				}
			}
		}

		public Color TopRightColor
		{
			get
			{
				return m_Color2;
			}
			set
			{
				if (m_Color2 != value)
				{
					m_Color2 = value;
					m_ColorChanged = true;
				}
			}
		}

		public Color BottomLeftColor
		{
			get
			{
				return m_Color0;
			}
			set
			{
				if (m_Color0 != value)
				{
					m_Color0 = value;
					m_ColorChanged = true;
				}
			}
		}

		public Color BottomRightColor
		{
			get
			{
				return m_Color1;
			}
			set
			{
				if (m_Color1 != value)
				{
					m_Color1 = value;
					m_ColorChanged = true;
				}
			}
		}

		protected override void UpdateColor ()
		{
			m_ColorChanged = false;

			int vertexIndex = this.internalVertexIndex;
			List<Color> colList = m_DrawCall.m_ColList;

			Color c = m_Color0;
			c.a *= m_Alpha;
			colList[vertexIndex    ] = c;

			c = m_Color1;
			c.a *= m_Alpha;
			colList[vertexIndex + 1] = c;

			c = m_Color2;
			c.a *= m_Alpha;
			colList[vertexIndex + 2] = c;

			c = m_Color3;
			c.a *= m_Alpha;
			colList[vertexIndex + 3] = c;
		}

		/*
		[SerializeField][HideInInspector] protected Vector3 p0;
		[SerializeField][HideInInspector] protected Vector3 p1;
		[SerializeField][HideInInspector] protected Vector3 p2;
		[SerializeField][HideInInspector] protected Vector3 p3;

		public override void virtualUpdateUnLightMesh (List<Vector3> verList, List<Vector2> uvList, List<Color> colList, List<int> triList)
		{
			int triIndex= verList.Count;

			if (this.MatrixChanged || m_ScaleChanged || m_SizeChanged)
			{
				m_ScaleChanged = false;
				m_SizeChanged = false;

				Matrix4x4 matrix = this.GetMatrix();

				float halfW = m_Scale * m_Width / 2;
				float halfH = m_Scale * m_Height / 2;

				p0 = new Vector3(-halfW, -halfH, 0);
				p1 = new Vector3( halfW, -halfH, 0);
				p2 = new Vector3( halfW,  halfH, 0);
				p3 = new Vector3(-halfW,  halfH, 0);

				p0 = matrix.MultiplyPoint3x4(p0);
				p1 = matrix.MultiplyPoint3x4(p1);
				p2 = matrix.MultiplyPoint3x4(p2);
				p3 = matrix.MultiplyPoint3x4(p3);
			}

			verList.Add(p0);
			verList.Add(p1);
			verList.Add(p2);
			verList.Add(p3);

			if (m_ImageInfo == null)
			{
				Vector2 uv = VEle.NoUV;
				uvList.Add(uv);
				uvList.Add(uv);
				uvList.Add(uv);
				uvList.Add(uv);
			}
			else
			{
				uvList.Add(m_ImageInfo.uv0);
				uvList.Add(m_ImageInfo.uv1);
				uvList.Add(m_ImageInfo.uv2);
				uvList.Add(m_ImageInfo.uv3);
			}

			Color c = m_Color0;
			c.a *= m_Alpha;
			colList.Add(c);

			c = m_Color1;
			c.a *= m_Alpha;
			colList.Add(c);

			c = m_Color2;
			c.a *= m_Alpha;
			colList.Add(c);

			c = m_Color3;
			c.a *= m_Alpha;
			colList.Add(c);

			triList.Add(triIndex + 0);
			triList.Add(triIndex + 1);
			triList.Add(triIndex + 2);
			triList.Add(triIndex + 0);
			triList.Add(triIndex + 2);
			triList.Add(triIndex + 3);
		}
		*/
	}
}