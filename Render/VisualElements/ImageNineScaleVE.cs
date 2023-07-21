/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	/// 九宫格拉伸 
	public class ImageNineScaleVE : ImageVE 
	{
		[SerializeField][HideInInspector] protected Vector2 m_Pivot = Vector2.zero;

		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			const int vertexCount = 16;

			this.internalVertexCount = vertexCount;
			verList.AddEmptyRange(vertexCount);
			uvList.AddEmptyRange(vertexCount);
			colList.AddEmptyRange(vertexCount);
		}

		// 完整九宫格顶点信息
		//
		// [y]
		//  |
		//  |
		//  |
		// [3] 12--------13--------14--------15
		//  |  |         |         |         |
		//  |  |         |         |         |
		//  |  |         |         |         |
		// [2] 8---------9---------10--------11
		//  |  |         |         |         |
		//  |  |         |         |         |
		//  |  |         |         |         |
		// [1] 4---------5---------6---------7
		//  |  |         |         |         |
		//  |  |         |         |         |
		//  |  |         |         |         |
		// [0] 0---------1---------2---------3
		//  |
		// -|-[0]------ [1]-------[2]-------[3]-------[x]

		public override void virtualUpdateVertexIndex (List<int> triList)
		{
			int triIndex = this.internalVertexIndex;
			// tri 1
			triList.Add(triIndex + 0);
			triList.Add(triIndex + 1);
			triList.Add(triIndex + 5);
			triList.Add(triIndex + 0);
			triList.Add(triIndex + 5);
			triList.Add(triIndex + 4);
			// tri 2
			triList.Add(triIndex + 1);
			triList.Add(triIndex + 2);
			triList.Add(triIndex + 6);
			triList.Add(triIndex + 1);
			triList.Add(triIndex + 6);
			triList.Add(triIndex + 5);
			// tri 3
			triList.Add(triIndex + 2);
			triList.Add(triIndex + 3);
			triList.Add(triIndex + 7);
			triList.Add(triIndex + 2);
			triList.Add(triIndex + 7);
			triList.Add(triIndex + 6);
			// tri 4
			triList.Add(triIndex + 4);
			triList.Add(triIndex + 5);
			triList.Add(triIndex + 9);
			triList.Add(triIndex + 4);
			triList.Add(triIndex + 9);
			triList.Add(triIndex + 8);
			// tri 5
			triList.Add(triIndex + 5);
			triList.Add(triIndex + 6);
			triList.Add(triIndex + 10);
			triList.Add(triIndex + 5);
			triList.Add(triIndex + 10);
			triList.Add(triIndex + 9);
			// tri 6
			triList.Add(triIndex + 6);
			triList.Add(triIndex + 7);
			triList.Add(triIndex + 11);
			triList.Add(triIndex + 6);
			triList.Add(triIndex + 11);
			triList.Add(triIndex + 10);
			// tri 7
			triList.Add(triIndex + 8);
			triList.Add(triIndex + 9);
			triList.Add(triIndex + 13);
			triList.Add(triIndex + 8);
			triList.Add(triIndex + 13);
			triList.Add(triIndex + 12);
			// tri 8
			triList.Add(triIndex + 9);
			triList.Add(triIndex + 10);
			triList.Add(triIndex + 14);
			triList.Add(triIndex + 9);
			triList.Add(triIndex + 14);
			triList.Add(triIndex + 13);
			// tri 9
			triList.Add(triIndex + 10);
			triList.Add(triIndex + 11);
			triList.Add(triIndex + 15);
			triList.Add(triIndex + 10);
			triList.Add(triIndex + 15);
			triList.Add(triIndex + 14);

		}

		private void UpdatePosition ()
		{
			if (m_UVChanged || this.MatrixChanged || m_ScaleChanged || m_SizeChanged)
			{
				m_ScaleChanged = false;
				m_SizeChanged = false;
				this.MarkNeedUpdate();

				int trimLft = 0;
				int trimRgt = 0;
				int trimBtm = 0;
				int trimTop = 0;

				int nineLft = 0;
				int nineRgt = 0;
				int nineBtm = 0;
				int nineTop = 0;

				if (m_ImageInfo != null)
				{
					trimLft = m_ImageInfo.trimLft;
					trimRgt = m_ImageInfo.trimRgt;
					trimBtm = m_ImageInfo.trimBtm;
					trimTop = m_ImageInfo.trimTop;

					nineLft = m_ImageInfo.nineLft;
					nineRgt = m_ImageInfo.nineRgt;
					nineBtm = m_ImageInfo.nineBtm;
					nineTop = m_ImageInfo.nineTop;
				}

				Matrix4x4 matrix = this.GetMatrix();

				float width = m_Scale * m_Width;
				float height = m_Scale * m_Height;

				float x0, x3, y0, y3;
				float x1, x2, y1, y2;

				x0 = -0.5f - m_Pivot.x;
				x3 = x0 + 1;
				x0 *= width;
				x3 *= width;
				x0 += trimLft;
				x3 -= trimRgt;

				y0 = -0.5f - m_Pivot.y;
				y3 = y0 + 1;
				y0 *= height;
				y3 *= height;
				y0 += trimBtm;
				y3 -= trimTop;

				x1 = x0 + nineLft;
				x2 = x3 - nineRgt;
				y1 = y0 + nineBtm;
				y2 = y3 - nineTop;

				int vertexIndex = this.internalVertexIndex;
				List<Vector3> verList = m_DrawCall.m_VerList;

				verList[vertexIndex +  0] = matrix.MultiplyPoint3x4(new Vector3(x0, y0, 0));//  0 = (0, 0)
				verList[vertexIndex +  1] = matrix.MultiplyPoint3x4(new Vector3(x1, y0, 0));//  1 = (1, 0)
				verList[vertexIndex +  2] = matrix.MultiplyPoint3x4(new Vector3(x2, y0, 0));//  2 = (2, 0)
				verList[vertexIndex +  3] = matrix.MultiplyPoint3x4(new Vector3(x3, y0, 0));//  3 = (3, 0)
				verList[vertexIndex +  4] = matrix.MultiplyPoint3x4(new Vector3(x0, y1, 0));//  4 = (0, 1)
				verList[vertexIndex +  5] = matrix.MultiplyPoint3x4(new Vector3(x1, y1, 0));//  5 = (1, 1)
				verList[vertexIndex +  6] = matrix.MultiplyPoint3x4(new Vector3(x2, y1, 0));//  6 = (2, 1)
				verList[vertexIndex +  7] = matrix.MultiplyPoint3x4(new Vector3(x3, y1, 0));//  7 = (3, 1)
				verList[vertexIndex +  8] = matrix.MultiplyPoint3x4(new Vector3(x0, y2, 0));//  8 = (0, 2)
				verList[vertexIndex +  9] = matrix.MultiplyPoint3x4(new Vector3(x1, y2, 0));//  9 = (1, 2)
				verList[vertexIndex + 10] = matrix.MultiplyPoint3x4(new Vector3(x2, y2, 0));// 10 = (2, 2)
				verList[vertexIndex + 11] = matrix.MultiplyPoint3x4(new Vector3(x3, y2, 0));// 11 = (3, 2)
				verList[vertexIndex + 12] = matrix.MultiplyPoint3x4(new Vector3(x0, y3, 0));// 12 = (0, 3)
				verList[vertexIndex + 13] = matrix.MultiplyPoint3x4(new Vector3(x1, y3, 0));// 13 = (1, 3)
				verList[vertexIndex + 14] = matrix.MultiplyPoint3x4(new Vector3(x2, y3, 0));// 14 = (2, 3)
				verList[vertexIndex + 15] = matrix.MultiplyPoint3x4(new Vector3(x3, y3, 0));// 15 = (3, 3)
			}
		}

		private void UpdateUV ()
		{
			m_UVChanged = false;

			int vertexIndex = this.internalVertexIndex;
			List<Vector2> uvs = m_DrawCall.m_UVList;

			if (m_ImageInfo == null)
			{
				Vector2 uv = VEle.NoUV;
				uvs[vertexIndex +  0] = uv;
				uvs[vertexIndex +  1] = uv;
				uvs[vertexIndex +  2] = uv;
				uvs[vertexIndex +  3] = uv;
				uvs[vertexIndex +  4] = uv;
				uvs[vertexIndex +  5] = uv;
				uvs[vertexIndex +  6] = uv;
				uvs[vertexIndex +  7] = uv;
				uvs[vertexIndex +  8] = uv;
				uvs[vertexIndex +  9] = uv;
				uvs[vertexIndex + 10] = uv;
				uvs[vertexIndex + 11] = uv;
				uvs[vertexIndex + 12] = uv;
				uvs[vertexIndex + 13] = uv;
				uvs[vertexIndex + 14] = uv;
				uvs[vertexIndex + 15] = uv;
			}
			else
			{
				float x0 = this.TransUVImageInfoX(m_ImageInfo.uvXMin);
				float x1 = this.TransUVImageInfoX(m_ImageInfo.nineUVXMin);
				float x2 = this.TransUVImageInfoX(m_ImageInfo.nineUVXMax);
				float x3 = this.TransUVImageInfoX(m_ImageInfo.uvXMax);
				float y0 = m_ImageInfo.uvYMin;
				float y1 = m_ImageInfo.nineUVYMin;
				float y2 = m_ImageInfo.nineUVYMax;
				float y3 = m_ImageInfo.uvYMax;

				uvs[vertexIndex +  0] = new Vector2(x0, y0);//  0 = (0, 0)
				uvs[vertexIndex +  1] = new Vector2(x1, y0);//  1 = (1, 0)
				uvs[vertexIndex +  2] = new Vector2(x2, y0);//  2 = (2, 0)
				uvs[vertexIndex +  3] = new Vector2(x3, y0);//  3 = (3, 0)
				uvs[vertexIndex +  4] = new Vector2(x0, y1);//  4 = (0, 1)
				uvs[vertexIndex +  5] = new Vector2(x1, y1);//  5 = (1, 1)
				uvs[vertexIndex +  6] = new Vector2(x2, y1);//  6 = (2, 1)
				uvs[vertexIndex +  7] = new Vector2(x3, y1);//  7 = (3, 1)
				uvs[vertexIndex +  8] = new Vector2(x0, y2);//  8 = (0, 2)
				uvs[vertexIndex +  9] = new Vector2(x1, y2);//  9 = (1, 2)
				uvs[vertexIndex + 10] = new Vector2(x2, y2);// 10 = (2, 2)
				uvs[vertexIndex + 11] = new Vector2(x3, y2);// 11 = (3, 2)
				uvs[vertexIndex + 12] = new Vector2(x0, y3);// 12 = (0, 3)
				uvs[vertexIndex + 13] = new Vector2(x1, y3);// 13 = (1, 3)
				uvs[vertexIndex + 14] = new Vector2(x2, y3);// 14 = (2, 3)
				uvs[vertexIndex + 15] = new Vector2(x3, y3);// 15 = (3, 3)
			}
		}
		protected virtual void UpdateColor ()
		{
			Color c = new Color(1, 1, 1, m_Alpha);

			int vertexIndex = this.internalVertexIndex;
			List<Color> colList = m_DrawCall.m_ColList;
			// 
			colList[vertexIndex +  0] = c;
			colList[vertexIndex +  1] = c;
			colList[vertexIndex +  2] = c;
			colList[vertexIndex +  3] = c;
			//
			colList[vertexIndex +  4] = c;
			colList[vertexIndex +  5] = c;
			colList[vertexIndex +  6] = c;
			colList[vertexIndex +  7] = c;
			//
			colList[vertexIndex +  8] = c;
			colList[vertexIndex +  9] = c;
			colList[vertexIndex + 10] = c;
			colList[vertexIndex + 11] = c;
			//
			colList[vertexIndex + 12] = c;
			colList[vertexIndex + 13] = c;
			colList[vertexIndex + 14] = c;
			colList[vertexIndex + 15] = c;
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
	}
}