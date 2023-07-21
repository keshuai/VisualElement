/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	/// 九宫格延展
	/// 居中显示, 通过水平方向和垂直方向来延展

	public class ImageNineExtVE : ImageVE 
	{
		[SerializeField][HideInInspector] private Color m_Color = Color.white;
		[SerializeField][HideInInspector] private int m_Hor;
		[SerializeField][HideInInspector] private int m_Ver;

		[SerializeField][HideInInspector] private bool m_HorVerChanged = true;


		public Color Color
		{
			get { return m_Color; }
			set 
			{
				if (m_Color != value)
				{
					m_Color = value;
					m_ColorChanged = true;
				}
			}
		}

		public int Hor
		{
			get
			{
				return m_Hor;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				if (m_Hor != value)
				{
					m_Hor = value;
					m_HorVerChanged = true;
				}
			}
		}

		public int Ver
		{
			get
			{
				return m_Ver;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				if (m_Ver != value)
				{
					m_Ver = value;
					m_HorVerChanged = true;
				}
			}
		}

		// 普通四个顶点的白色矩形 
		private void AddSubMesh_NoImage (List<Vector3> verList, List<Vector2> uvList, List<Color> colList, List<int> triList)
		{

		}

		// 普通四个顶点的白色矩形 
		// m_Hor = 1, m_Ver = 1时, 等同于普通图片
		private void AddSubMesh_ImageNormal (List<Vector3> verList, List<Vector2> uvList, List<Color> colList, List<int> triList)
		{

		}




		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			// 顶点个数动态变化的, 初始化为0 
			this.internalVertexCount = 0;
		}
		public override void virtualUpdateVertexIndex (List<int> vertexList)
		{
			if (this.internalVertexCount > 0 && m_ImageInfo != null)
			{
				int index = this.internalVertexIndex;
				int vertexCount = this.internalVertexCount;

				for(int i = 0; i < vertexCount; i += 4)
				{
					vertexList.Add(index + i    );
					vertexList.Add(index + i + 1);
					vertexList.Add(index + i + 2);

					vertexList.Add(index + i    );
					vertexList.Add(index + i + 2);
					vertexList.Add(index + i + 3);
				}
			}
		}

		// 完整九宫格延展信息
		//
		// 
		//       0---------|---------|---------|--------max(out)
		//       |         |         |         |         |
		//       |    1    |   top   |   top   |    1    |
		//       |         |         |         |         |
		//       |---------0---------|--------max--------|
		//       |         |         |         |         |
		//       |   lft   |    N    |    N    |   rgt   |
		//       |         |         |         |         |
		//       |---------|---------|---------|---------|
		//       |         |         |         |         |
		//       |   lft   |    N    |    N    |   rgt   |
		//       |         |         |         |         |
		//       |--------min--------|---------0---------|
		//       |         |         |         |         |
		//       |    1    |   btm   |   btm   |    1    |
		//       |         |         |         |         |
		// (out)min--------|---------|---------|---------0
		//  
		// 

		private void FullUpdatePositionUV ()
		{
			int vertexIndex = this.internalVertexIndex;
			List<Vector3> verList = m_DrawCall.m_VerList;
			List<Vector2> uvList  = m_DrawCall.m_UVList;
			//List<Color>   colList = m_DrawCall.m_ColList;

			// use matrix to scale
			Matrix4x4 matrix = this.GetMatrix() * this.GetScaleMatrix();

			int w = m_ImageInfo.nineWidth;
			int h = m_ImageInfo.nineHeight;

			// total width height (contains trimmed pixels)
			m_Width = w * m_Hor + m_ImageInfo.trimLft + m_ImageInfo.nineLft + m_ImageInfo.trimRgt + m_ImageInfo.nineRgt;
			m_Height = h * m_Ver + m_ImageInfo.trimBtm + m_ImageInfo.nineBtm+ m_ImageInfo.trimTop + m_ImageInfo.nineTop;
			// lock the size
			m_CanSetSize = false;

			float xmax = m_Hor * w * 0.5f;
			float ymax = m_Ver * h * 0.5f;
			float xmin = -xmax;
			float ymin = -ymax;

			// offset from (trimmed pixels + nine pixels)
			float offsetX = (m_ImageInfo.trimLft - m_ImageInfo.trimRgt + m_ImageInfo.nineLft - m_ImageInfo.nineRgt) / 2f;
			float offsetY = (m_ImageInfo.trimBtm - m_ImageInfo.trimTop + m_ImageInfo.nineBtm - m_ImageInfo.nineTop) / 2f;
			xmin += offsetX;
			xmax += offsetX;
			ymin += offsetY;
			ymax += offsetY;

			float uvXmin = this.TransUVImageInfoX(m_ImageInfo.nineUVXMin);
			float uvXmax = this.TransUVImageInfoX(m_ImageInfo.nineUVXMax);
			float uvYmin = m_ImageInfo.nineUVYMin;
			float uvYmax = m_ImageInfo.nineUVYMax;

			// real out, no trimmed pixel
			float outXmin = xmin - m_ImageInfo.nineLft;
			float outXmax = xmax + m_ImageInfo.nineRgt;
			float outYmin = ymin - m_ImageInfo.nineBtm;
			float outYmax = ymax + m_ImageInfo.nineTop;

			float uvOutXmin = this.TransUVImageInfoX(m_ImageInfo.uvXMin);
			float uvOutXmax = this.TransUVImageInfoX(m_ImageInfo.uvXMax);
			float uvOutYmin = m_ImageInfo.uvYMin;
			float uvOutYmax = m_ImageInfo.uvYMax;

			// 四个角固定不变

			// top left
			{
				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmin, ymax, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmin, uvYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmin, ymax, 0));
				uvList[vertexIndex++] = new Vector2(uvXmin, uvYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmin, outYmax, 0));
				uvList[vertexIndex++] = new Vector2(uvXmin, uvOutYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmin, outYmax, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmin, uvOutYmax);
			}

			// top right
			{

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmax, ymax, 0));
				uvList[vertexIndex++] = new Vector2(uvXmax, uvYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmax, ymax, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmax, uvYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmax, outYmax, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmax, uvOutYmax);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmax, outYmax, 0));
				uvList[vertexIndex++] = new Vector2(uvXmax, uvOutYmax);
			}

			// bottom left
			{
				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmin, outYmin, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmin, uvOutYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmin, outYmin, 0));
				uvList[vertexIndex++] = new Vector2(uvXmin, uvOutYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmin, ymin, 0));
				uvList[vertexIndex++] = new Vector2(uvXmin, uvYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmin, ymin, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmin, uvYmin);
			}

			// bottom right
			{
				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmax, outYmin, 0));
				uvList[vertexIndex++] = new Vector2(uvXmax, uvOutYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmax, outYmin, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmax, uvOutYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(outXmax, ymin, 0));
				uvList[vertexIndex++] = new Vector2(uvOutXmax, uvYmin);

				verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(xmax, ymin, 0));
				uvList[vertexIndex++] = new Vector2(uvXmax, uvYmin);
			}

			float eachXmin, eachXmax, eachYmin, eachYmax;

			// 上下按水平方向延展
			for(int i = 0; i < m_Hor; ++i)
			{
				// top
				{
					eachXmin = xmin + i * w;
					eachXmax = eachXmin + w;
					eachYmin = ymax;
					eachYmax = outYmax;

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvOutYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvOutYmax);
				}

				// bottom
				{
					//eachXmin = xmin + i * w;
					//eachXmax = eachXmin + w;
					eachYmin = outYmin;
					eachYmax = ymin;

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvOutYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvOutYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmin);
				}
			}

			// 左右按垂直方向延展
			for(int i = 0; i < m_Ver; ++i)
			{
				// left

				{
					eachXmin = outXmin;
					eachXmax = xmin;
					eachYmin = ymin + i * h;
					eachYmax = eachYmin + h;

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvOutXmin, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvOutXmin, uvYmax);

				}

				// right
				{
					eachXmin = xmax;
					eachXmax = outXmax;
					//eachYmin = ymin + i * h;
					//eachYmax = eachYmin + h;

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvOutXmax, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvOutXmax, uvYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmax);
				}

			}

			// 中间双向延展
			for(int iHor = 0; iHor < m_Hor; ++iHor)
			{
				for(int iVer = 0; iVer < m_Ver; ++iVer)
				{
					eachXmin = xmin + iHor * w;
					eachXmax = eachXmin + w;
					eachYmin = ymin + iVer * h;
					eachYmax = eachYmin + h;

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmin, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmin);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmax, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmax, uvYmax);

					verList[vertexIndex] = matrix.MultiplyPoint3x4(new Vector3(eachXmin, eachYmax, 0));
					uvList[vertexIndex++] = new Vector2(uvXmin, uvYmax);
				}
			}
		}

		private void UpdatePositionUV ()
		{
			if (m_HorVerChanged || m_UVChanged || m_ScaleChanged || this.MatrixChanged)
			{
				m_HorVerChanged = false;
				m_ScaleChanged = false;
				m_UVChanged = false;

				this.MarkNeedUpdateVertexIndex();

				if (m_ImageInfo != null)
				{
					this.FullUpdatePositionUV();
				}  
			}
		}

		protected virtual void UpdateColor ()
		{
			m_ColorChanged = false;

			Color c = m_Color;
			c.a *= m_Alpha;
			//Color c = new Color(1f, 1f, 1f, m_Alpha);
			List<Color> colList = m_DrawCall.m_ColList;

			int vertexIndex = this.internalVertexIndex;
			int vertexEnd = vertexIndex + this.internalVertexCount;

			while(vertexIndex < vertexEnd)
			{
				colList[vertexIndex++] = c;
			}
		}
	
		protected override void virtualLateUpdate ()
		{
			// 改变缓冲区大小
			if (m_HorVerChanged)
			{
				// 上下左右 4 * 4 = 16
				// 水平方向 m_Hor * 8
				// 垂直方向 m_Ver * 8;           
				// 中间双 m_Hor * m_Ver * 4;
				int newVertexCount = 16 + (m_Hor + m_Ver)* 8 + m_Hor * m_Ver * 4;
				// 缓冲区容量变大时扩充
				// 缓冲区容量变小时暂不处理
				if (newVertexCount > this.internalVertexCount)
				{
					// 新增定点 需要新增颜色
					m_ColorChanged = true;
				}

				// 进行扩容
				m_DrawCall.ElementVertexCountChangedOnUpdate(this, this.internalVertexCount, newVertexCount);
				this.MarkNeedUpdate();
			}

			// 更新位置与UV
			this.UpdatePositionUV();

			// 更新颜色
			if (m_ColorChanged)
			{
				this.UpdateColor();
				this.MarkNeedUpdate();
			}
		}
	}
}