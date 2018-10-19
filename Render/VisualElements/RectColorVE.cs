/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	/// 矩形 
	public class RectColorVE : RectVE 
	{
		[SerializeField][HideInInspector] private Vector2 m_Pivot;
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


		private void UpdatePosition ()
		{
			if (this.MatrixChanged || m_ScaleChanged || m_SizeChanged)
			{
				int index = this.internalVertexIndex;
				List<Vector3> verList = m_DrawCall.m_VerList;

				m_ScaleChanged = false;
				m_SizeChanged = false;

				Matrix4x4 matrix = this.GetMatrix();

				float halfW = m_Scale * m_Width / 2;
				float halfH = m_Scale * m_Height / 2;

				Vector3 p0 = new Vector3(-halfW, -halfH, 0);
				Vector3 p1 = new Vector3( halfW, -halfH, 0);
				Vector3 p2 = new Vector3( halfW,  halfH, 0);
				Vector3 p3 = new Vector3(-halfW,  halfH, 0);

				p0 = matrix.MultiplyPoint3x4(p0);
				p1 = matrix.MultiplyPoint3x4(p1);
				p2 = matrix.MultiplyPoint3x4(p2);
				p3 = matrix.MultiplyPoint3x4(p3);

				verList[index    ] = p0;
				verList[index + 1] = p1;
				verList[index + 2] = p2;
				verList[index + 3] = p3;
			}
		}

		private void UpdateColor ()
		{	
			if (m_ColorChanged)
			{
				m_ColorChanged = false;
				this.MarkNeedUpdate();

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

		public override void virtualUpdateVertexIndex (List<int> vertexList)
		{
			int vertexIndex = this.internalVertexIndex;

			vertexList.Add(vertexIndex + 0);
			vertexList.Add(vertexIndex + 1);
			vertexList.Add(vertexIndex + 2);

			vertexList.Add(vertexIndex + 0);
			vertexList.Add(vertexIndex + 2);
			vertexList.Add(vertexIndex + 3);
		}

		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			this.internalVertexCount = 4;

			verList.AddEmptyRange(4);
			colList.AddEmptyRange(4);

			Vector2 uv = VEle.NoUV;
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
		}

		protected override void virtualLateUpdate ()
		{
			this.UpdatePosition();
			this.UpdateColor();
		}

		public override void virtualAwake ()
		{
			base.virtualAwake ();
		
		}
	}
}

