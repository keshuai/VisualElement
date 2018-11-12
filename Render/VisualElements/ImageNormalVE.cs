/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CX
{
	class PivotValue
	{
		public static Vector2 Center = Vector2.zero;

		public static Vector2        Left = new Vector2(-0.5f,     0);
		public static Vector2       Right = new Vector2( 0.5f,     0);
		public static Vector2         Top = new Vector2(    0,  0.5f);
		public static Vector2      Bottom = new Vector2(    0, -0.5f);

		public static Vector2     TopLeft = new Vector2(-0.5f,  0.5f);
		public static Vector2    TopRight = new Vector2( 0.5f,  0.5f);
		public static Vector2  BottomLeft = new Vector2(-0.5f, -0.5f);
		public static Vector2 BottomRight = new Vector2( 0.5f, -0.5f);

	}

	///
	/// 可使用ImageAtlasDrawcall进行渲染
	/// 
	/// Altas 多个贴图 (当一个大图装不下所有的时)
	/// 多个贴图时,前后顺序不再有用
	public class ImageNormalVE : ImageVE
	{
		[SerializeField][HideInInspector] protected Vector3 p0;
		[SerializeField][HideInInspector] protected Vector3 p1;
		[SerializeField][HideInInspector] protected Vector3 p2;
		[SerializeField][HideInInspector] protected Vector3 p3;

		[SerializeField][HideInInspector] protected Vector2 m_Pivot = Vector2.zero;
		[SerializeField][HideInInspector] protected bool m_PivotChanged = false;

		public Vector2 Pivot
		{
			get{ return m_Pivot; }
			set 
			{ 
				if (m_Pivot != value)
				{
					m_Pivot = value;
					m_PivotChanged = true;
				}
			}
		}

		public void FitImageSize ()
		{
			if (m_ImageInfo != null)
			{
				this.Width = m_ImageInfo.width;
				this.Height = m_ImageInfo.height;
			}
		}
			

		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			this.internalVertexCount = 4;

			verList.AddEmptyRange(4);
			uvList .AddEmptyRange(4);
			colList.AddEmptyRange(4);
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
			
		protected void UpdatePosition ()
		{
			if (this.MatrixChanged || m_ScaleChanged || m_SizeChanged || m_PivotChanged)
			{
				m_ScaleChanged = false;
				m_SizeChanged = false;
				m_PivotChanged = false;

				float xMin, xMax, yMin, yMax;

				xMin = -0.5f - m_Pivot.x;
				xMax = xMin + 1;
				yMin = -0.5f - m_Pivot.y;
				yMax = yMin + 1;

				// trim 与 不trim 之间的变化
				if (m_ImageInfo != null && m_ImageInfo.trimmed)
				{
					xMin += m_ImageInfo.trimXmin_normalized;
					xMax -= m_ImageInfo.trimXmax_normalized;
					yMin += m_ImageInfo.trimYmin_normalized;
					yMax -= m_ImageInfo.trimYmax_normalized;
				}

				float width = m_Scale * m_Width;
				float height = m_Scale * m_Height;
				xMin *= width;
				xMax *= width;
				yMin *= height;
				yMax *= height;

				Vector3 p0 = new Vector3(xMin, yMin, 0);
				Vector3 p1 = new Vector3(xMax, yMin, 0);
				Vector3 p2 = new Vector3(xMax, yMax, 0);
				Vector3 p3 = new Vector3(xMin, yMax, 0);

				Matrix4x4 matrix = this.GetMatrix();
				p0 = matrix.MultiplyPoint3x4(p0);
				p1 = matrix.MultiplyPoint3x4(p1);
				p2 = matrix.MultiplyPoint3x4(p2);
				p3 = matrix.MultiplyPoint3x4(p3);

				int vertexIndex = this.internalVertexIndex;
				m_DrawCall.m_VerList[vertexIndex    ] = p0;
				m_DrawCall.m_VerList[vertexIndex + 1] = p1;
				m_DrawCall.m_VerList[vertexIndex + 2] = p2;
				m_DrawCall.m_VerList[vertexIndex + 3] = p3;
			}
		}

		private void UpdateUV ()
		{
			m_UVChanged = false;
			this.MarkNeedUpdate();

			int vertexIndex = this.internalVertexIndex;
			List<Vector2> uvList = m_DrawCall.m_UVList;

			if (m_ImageInfo == null)
			{
				Vector2 uv = VEle.NoUV;
				uvList[vertexIndex    ] = uv;
				uvList[vertexIndex + 1] = uv;
				uvList[vertexIndex + 2] = uv;
				uvList[vertexIndex + 3] = uv;
			}
			else
			{
				uvList[vertexIndex    ] = this.TransUVImageInfo(m_ImageInfo.uv0);
				uvList[vertexIndex + 1] = this.TransUVImageInfo(m_ImageInfo.uv1);
				uvList[vertexIndex + 2] = this.TransUVImageInfo(m_ImageInfo.uv2);
				uvList[vertexIndex + 3] = this.TransUVImageInfo(m_ImageInfo.uv3);
			}
		}

		protected virtual void UpdateColor ()
		{
			m_ColorChanged = false;

			int vertexIndex = this.internalVertexIndex;
			List<Color> colList = m_DrawCall.m_ColList;

			Color c = new Color(1f, 1f, 1f, m_Alpha);
			colList[vertexIndex    ] = c;
			colList[vertexIndex + 1] = c;
			colList[vertexIndex + 2] = c;
			colList[vertexIndex + 3] = c;
		}

		protected override void virtualLateUpdate ()
		{
			this.UpdatePosition();

			if (m_UVChanged)
			{
				this.UpdateUV();
				this.MarkNeedUpdate();
			}

			if (m_ColorChanged)
			{
				this.UpdateColor();
				this.MarkNeedUpdate();
			}
		}
			
		void OnDrawGizmos ()
		{
			CXGizmos.Draw2DRect(this.GetGizmosMatrix(), m_Width, m_Height, m_Scale, m_Pivot);
		}
	}
}