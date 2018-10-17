using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX
{
	// 漏空方形
	public class RectBorderVE : RectVE 
	{
		[SerializeField][HideInInspector] private Vector2 m_Pivot;
		[SerializeField][HideInInspector] private Color m_Color = Color.white;
		
		[SerializeField][HideInInspector] private float m_BorderWidth = 2;
		[SerializeField][HideInInspector] private float m_BorderHeight = 2;
		
		
		public float BorderWidth
		{
			get 
			{
				return m_BorderWidth;
			}
			set
			{
				if (value < 0) value = 0;

				if (m_BorderWidth != value)
				{
					m_BorderWidth = value;
					m_SizeChanged = true;
					this.MarkNeedUpdate();
				}
			}
		}

		public float BorderHeight
		{
			get 
			{
				return m_BorderHeight;
			}
			set
			{
				if (value < 0) value = 0;

				if (m_BorderHeight != value)
				{
					m_BorderHeight = value;
					m_SizeChanged = true;
					this.MarkNeedUpdate();
				}
			}
		}

		public Vector2 BorderSize
		{
			get
			{
				return new Vector2(m_BorderWidth, m_BorderHeight);
			}
			set
			{
				this.BorderWidth = value.x;
				this.BorderHeight = value.y;
			}
		}
		

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

				//11-8-------------------------7
				// | 9-----------------------6-5
				// | |                       | |
				// | |                       | |
				// | |                       | |
				// | |                       | |
				// | |                       | |
				// | |                       | |
				// 3-10----------------------2 |
				// 0-------------------------1 4

				float halfW = m_Scale * m_Width / 2;
				float halfH = m_Scale * m_Height / 2;
				float borderX = m_BorderWidth;
				float borderY = m_BorderHeight;
				

				Vector3  p0 = new Vector3(-halfW, -halfH, 0);
				Vector3  p4 = new Vector3( halfW, -halfH, 0);
				Vector3  p7 = new Vector3( halfW,  halfH, 0);
				Vector3 p11 = new Vector3(-halfW,  halfH, 0);
				
				Vector3  p1 =  p4;  p1.x -= borderX;
				Vector3  p2 =  p1;  p2.y += borderY;
				Vector3  p3 =  p0;  p3.y += borderY;
				
				Vector3  p5 =  p7;  p5.y -= borderY;
				Vector3  p6 =  p5;  p6.x -= borderX;
				
				Vector3  p8 = p11;  p8.x += borderX;
				Vector3  p9 =  p8;  p9.y -= borderY;
				
				Vector3 p10 =  p3; p10.x += borderX;
				

				verList[index     ] = matrix.MultiplyPoint3x4(p0);
				verList[index +  1] = matrix.MultiplyPoint3x4(p1);
				verList[index +  2] = matrix.MultiplyPoint3x4(p2);
				verList[index +  3] = matrix.MultiplyPoint3x4(p3);
				
				verList[index +  4] = matrix.MultiplyPoint3x4(p4);
				verList[index +  5] = matrix.MultiplyPoint3x4(p5);
				verList[index +  6] = matrix.MultiplyPoint3x4(p6);
				verList[index +  7] = matrix.MultiplyPoint3x4(p7);
				
				verList[index +  8] = matrix.MultiplyPoint3x4(p8);
				verList[index +  9] = matrix.MultiplyPoint3x4(p9);
				verList[index + 10] = matrix.MultiplyPoint3x4(p10);
				verList[index + 11] = matrix.MultiplyPoint3x4(p11);
				
			}
		}

		private void UpdateColor ()
		{	
			if (m_ColorChanged)
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
				
				colList[vertexIndex + 4] = c;
				colList[vertexIndex + 5] = c;
				colList[vertexIndex + 6] = c;
				colList[vertexIndex + 7] = c;
				
				colList[vertexIndex + 8] = c;
				colList[vertexIndex + 9] = c;
				colList[vertexIndex + 10] = c;
				colList[vertexIndex + 11] = c;
			}
		}

		public override void virtualUpdateVertexIndex (List<int> vertexList)
		{
			int vertexIndex = this.internalVertexIndex;
			
			//11-8-------------------------7
			// | 9-----------------------6-5
			// | |                       | |
			// | |                       | |
			// | |                       | |
			// | |                       | |
			// | |                       | |
			// | |                       | |
			// 3-10----------------------2 |
			// 0-------------------------1 4
			vertexList.Add(vertexIndex +  0);
			vertexList.Add(vertexIndex +  1);
			vertexList.Add(vertexIndex +  2);
			vertexList.Add(vertexIndex +  0);
			vertexList.Add(vertexIndex +  2);
			vertexList.Add(vertexIndex +  3);
			
			vertexList.Add(vertexIndex +  1);
			vertexList.Add(vertexIndex +  4);
			vertexList.Add(vertexIndex +  5);
			vertexList.Add(vertexIndex +  1);
			vertexList.Add(vertexIndex +  5);
			vertexList.Add(vertexIndex +  6);
			
			vertexList.Add(vertexIndex +  7);
			vertexList.Add(vertexIndex +  8);
			vertexList.Add(vertexIndex +  9);
			vertexList.Add(vertexIndex +  7);
			vertexList.Add(vertexIndex +  9);
			vertexList.Add(vertexIndex +  5);
			
			vertexList.Add(vertexIndex + 11);
			vertexList.Add(vertexIndex +  3);
			vertexList.Add(vertexIndex + 10);
			vertexList.Add(vertexIndex + 11);
			vertexList.Add(vertexIndex + 10);
			vertexList.Add(vertexIndex +  8);
		}

		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			this.internalVertexCount = 12;

			verList.AddEmptyRange(12);
			colList.AddEmptyRange(12);

			Vector2 uv = VEle.NoUV;
			
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
			
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
			uvList.Add(uv);
			
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
