/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	public enum LabelEffect
	{
		None = 0,
		Outline,
		Shadow,
		Shell,
	}

	public enum LabelAlignmentX
	{
		Left = 0,
		Center,
		Right,
	}

	public enum LabelAlignmentY
	{
		Top = 0,
		Center,
		Bottom,
	}

	// 只获取和计算一次,存储计算得到的数据.
	// 可加快渲染速度,也可用于单个字的精确位置事件
	public struct LabelCharInfo
	{
		public char ch;
		public bool visible;

		public int lineIndex;

		public int minX;
		public int maxX;
		public int minY;
		public int maxY;

		public Vector2 uv0;
		public Vector2 uv1;
		public Vector2 uv2;
		public Vector2 uv3;
	}

	public class Label : RectVE 
	{
		public View view { get { return (View)m_DrawCall;}}
		[SerializeField][HideInInspector] protected Vector2 m_Pivot = Vector2.zero;
		[SerializeField][HideInInspector] protected string m_Text;
		// the label vertext count changed with text
		[SerializeField][HideInInspector] protected int m_VertexCount = 0;
		[SerializeField][HideInInspector] protected bool m_TextChanged = false;



		public bool lockWidth;
		public bool lockHeight;

		public int spacingX = 0;
		public int spacingY = 0;

		public LabelAlignmentX alignmentX = LabelAlignmentX.Left;
		public LabelAlignmentY alignmentY = LabelAlignmentY.Top;


		public Color color = Color.white;
		public LabelEffect effect;
		public Color effectColor = Color.white;
		public Vector2 effectOffset = new Vector2(2, -1);

		public FontStyle fontStyle;
		public int fontSize = 46;

		// 显示不全 
		// 在同时锁定高宽时可能出现显示不全
		// 在高度锁定同时限制行数的时候可能出现显示不全
		public bool noShowAll;

		[HideInInspector][System.NonSerialized]
		public LabelCharInfo[] charInfos;

		[HideInInspector][System.NonSerialized]
		public List<int> lineWidthList;

		[HideInInspector][System.NonSerialized]
		public int charsHeight;

		public string Text 
		{
			get { return m_Text; }
			set 
			{ 
				if (m_Text != value)  
				{ 
					m_Text = value; 
					m_TextChanged = true;
					this.MarkNeedUpdate();
				}
			}
		}

		public ViewAsset asset
		{
			get { return m_Asset; }
			set
			{
				if (m_Asset != value)
				{
					m_Asset = value;
				}
			}
		}

		public Font TTFFont
		{
			get
			{
				return this.view.GetTTF(this.internalAssetIndex);
			}
			set
			{
				int assetIndex = this.view.GetAssetIndexWithTTF(value);
				if (assetIndex != this.internalAssetIndex)
				{
					this.internalAssetIndex = assetIndex;
					this.MarkNeedUpdate();
				}
			}
		}


		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			// 顶点个数动态变化的, 初始化为0 
			this.internalVertexCount = 0;
		}
		public override void virtualUpdateVertexIndex (List<int> vertexList)
		{
			if (this.internalVertexCount > 0 && m_Text != null && m_Text.Length != 0)
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
		protected override void virtualLateUpdate ()
		{
			base.virtualLateUpdate ();

			Font font = this.TTFFont;
			if (font != null)
			{
				if (m_TextChanged)
				{
					m_DrawCall.ElementVertexCountChangedOnUpdate(this, this.internalVertexCount, this.currentVertexCount);

					if (!string.IsNullOrEmpty(m_Text))
						font.RequestCharactersInTexture(m_Text, this.fontSize, this.fontStyle);
				}

				this.FullUpdate();
			}
		}

		private int currentVertexCount
		{
			get
			{
				if (string.IsNullOrEmpty(m_Text)) 
				{
					return 0;
				}

				int count = m_Text.Length * 4;

				if (this.effect == LabelEffect.None)
				{
					return count;
				}
				else if (this.effect == LabelEffect.Outline)
				{
					return count * 5;
				}
				else if (this.effect == LabelEffect.Shadow) 
				{
					return count + count;
				}

				return count;
			}
		}

		private void FullUpdate ()
		{
			int vertexIndex = this.internalVertexIndex;
			List<Vector3> verList = m_DrawCall.m_VerList;
			List<Vector2> uvList  = m_DrawCall.m_UVList;
			List<Color>   colList = m_DrawCall.m_ColList;

			// use matrix to scale

			if (m_Text != null && m_Text.Length != 0)
			{
				if (this.effect == LabelEffect.None)
				{
					CollectMeshValues_TTF_Normal(vertexIndex , verList, uvList, colList);
				}
				else if (this.effect == LabelEffect.Shadow)
				{
					CollectMeshValues_TTF_Shadow(vertexIndex , verList, uvList, colList);
				}
				else if (this.effect == LabelEffect.Outline)
				{
					CollectMeshValues_TTF_OutLine(vertexIndex , verList, uvList, colList);
				}
			}
		}

		static Color CombineColor (Color c1, Color c2)
		{
			return new Color (
				((int)(c1.r * 255)) * 256 + (int)(c2.r * 255),
				((int)(c1.g * 255)) * 256 + (int)(c2.g * 255),
				((int)(c1.b * 255)) * 256 + (int)(c2.b * 255),
				((int)(c1.a * 255)) * 256 + (int)(c2.a * 255)
			);
		}
	
		void CollectMeshValues_TTF_Normal (int vertexIndex, List<Vector3> vers, List<Vector2> uvs, List<Color> cols)//, int[] tris)
		{
			this.CalculateLabelFrame();

			Matrix4x4 matrix = this.GetMatrix() * this.GetScaleMatrix();

			int chCount = this.charInfos.Length;
			int width = Mathf.RoundToInt(this.Width);
			int height = Mathf.RoundToInt(this.Height);

			int posX = 0;
			int posY = 0;

			Color color = this.color;
			if(this.effect == LabelEffect.Outline)
			{
				color = CombineColor(color, this.effectColor);
			}

			LabelCharInfo chInfo;

			int currentLineIndex = 0;
			int currentLineWidth = this.lineWidthList[0];
			int posXAlignment = 0;
			int posYAlignment = 0;

			if (this.alignmentX == LabelAlignmentX.Left)
			{
				posXAlignment = -width / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Center)
			{
				posXAlignment = -currentLineWidth / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Right)
			{
				posXAlignment = width / 2 - currentLineWidth;
			}

			posYAlignment = height / 2;

			if (height != this.charsHeight)
			{
				if (this.alignmentY == LabelAlignmentY.Center)
				{
					posYAlignment -= (height - this.charsHeight) / 2;
				}
				else if (this.alignmentY == LabelAlignmentY.Bottom)
				{
					posYAlignment -= (height - this.charsHeight);
				}
			}


			LabelCharInfo[] charInfos = this.charInfos;
			for(int i = 0; i < chCount; ++i)
			{
				chInfo = charInfos[i];

				if (this.alignmentX == LabelAlignmentX.Center)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = -currentLineWidth / 2;
					}
				}
				else if (this.alignmentX == LabelAlignmentX.Right)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = width / 2 - currentLineWidth;
					}
				}
					

				if (chInfo.visible)
				{
					vers[vertexIndex + 0] = matrix.MultiplyPoint3x4(new Vector3(chInfo.minX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0));
					vers[vertexIndex + 1] = matrix.MultiplyPoint3x4(new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0));
					vers[vertexIndex + 2] = matrix.MultiplyPoint3x4(new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0));
					vers[vertexIndex + 3] = matrix.MultiplyPoint3x4(new Vector3(chInfo.minX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0));

					uvs[vertexIndex + 0] = this.TransUVLabel(chInfo.uv0);
					uvs[vertexIndex + 1] = this.TransUVLabel(chInfo.uv1);
					uvs[vertexIndex + 2] = this.TransUVLabel(chInfo.uv2);
					uvs[vertexIndex + 3] = this.TransUVLabel(chInfo.uv3);

				}
				else
				{
					vers[vertexIndex + 0] = Vector3.zero;
					vers[vertexIndex + 1] = Vector3.zero;
					vers[vertexIndex + 2] = Vector3.zero;
					vers[vertexIndex + 3] = Vector3.zero;

					uvs[vertexIndex + 0] = Vector2.zero;
					uvs[vertexIndex + 1] = Vector2.zero;
					uvs[vertexIndex + 2] = Vector2.zero;
					uvs[vertexIndex + 3] = Vector2.zero;
				}


				cols[vertexIndex + 0] = color;
				cols[vertexIndex + 1] = color;
				cols[vertexIndex + 2] = color;
				cols[vertexIndex + 3] = color;

				vertexIndex += 4;
			}
		}


		void CollectMeshValues_TTF_Shadow (int vertexIndex, List<Vector3> vers, List<Vector2> uvs, List<Color> cols)//, int[] tris)
		{
			this.CalculateLabelFrame();

			Matrix4x4 matrix = this.GetMatrix() * this.GetScaleMatrix();

			int chCount = this.charInfos.Length;
			int width = Mathf.RoundToInt(this.Width);
			int height = Mathf.RoundToInt(this.Height);

			int posX = 0;
			int posY = 0;

			float offsetX = this.effectOffset.x;
			float offsetY = this.effectOffset.y;

			Vector3 v0, v1, v2, v3;

			Color color = this.color;
			Color effectColor = this.effectColor;

			LabelCharInfo chInfo;

			int currentLineIndex = 0;
			int currentLineWidth = this.lineWidthList[0];
			int posXAlignment = 0;
			int posYAlignment = 0;

			if (this.alignmentX == LabelAlignmentX.Left)
			{
				posXAlignment = -width / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Center)
			{
				posXAlignment = -currentLineWidth / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Right)
			{
				posXAlignment = width / 2 - currentLineWidth;
			}

			posYAlignment = height / 2;

			if (height != this.charsHeight)
			{
				if (this.alignmentY == LabelAlignmentY.Center)
				{
					posYAlignment -= (height - this.charsHeight) / 2;
				}
				else if (this.alignmentY == LabelAlignmentY.Bottom)
				{
					posYAlignment -= (height - this.charsHeight);
				}
			}


			LabelCharInfo[] charInfos = this.charInfos;
			for(int i = 0; i < chCount; ++i)
			{
				chInfo = charInfos[i];

				if (this.alignmentX == LabelAlignmentX.Center)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = -currentLineWidth / 2;
					}
				}
				else if (this.alignmentX == LabelAlignmentX.Right)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = width / 2 - currentLineWidth;
					}
				}
					

				if (chInfo.visible)
				{
					v0 = new Vector3(chInfo.minX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0);
					v1 = new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0);
					v2 = new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0);
					v3 = new Vector3(chInfo.minX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0);
					
					vers[vertexIndex + 4] = matrix.MultiplyPoint3x4(v0);
					vers[vertexIndex + 5] = matrix.MultiplyPoint3x4(v1);
					vers[vertexIndex + 6] = matrix.MultiplyPoint3x4(v2);
					vers[vertexIndex + 7] = matrix.MultiplyPoint3x4(v3);

					v0.x += offsetX; v1.x += offsetX; v2.x += offsetX; v3.x += offsetX;
					v0.y += offsetY; v1.y += offsetY; v2.y += offsetY; v3.y += offsetY;

					vers[vertexIndex + 0] = matrix.MultiplyPoint3x4(v0);
					vers[vertexIndex + 1] = matrix.MultiplyPoint3x4(v1);
					vers[vertexIndex + 2] = matrix.MultiplyPoint3x4(v2);
					vers[vertexIndex + 3] = matrix.MultiplyPoint3x4(v3);

					uvs[vertexIndex + 0] = this.TransUVLabel(chInfo.uv0);
					uvs[vertexIndex + 1] = this.TransUVLabel(chInfo.uv1);
					uvs[vertexIndex + 2] = this.TransUVLabel(chInfo.uv2);
					uvs[vertexIndex + 3] = this.TransUVLabel(chInfo.uv3);

					uvs[vertexIndex + 4] = uvs[vertexIndex + 0];
					uvs[vertexIndex + 5] = uvs[vertexIndex + 1];
					uvs[vertexIndex + 6] = uvs[vertexIndex + 2];
					uvs[vertexIndex + 7] = uvs[vertexIndex + 3];
				}
				else
				{
					vers[vertexIndex + 0] = Vector3.zero;
					vers[vertexIndex + 1] = Vector3.zero;
					vers[vertexIndex + 2] = Vector3.zero;
					vers[vertexIndex + 3] = Vector3.zero;
					vers[vertexIndex + 4] = Vector3.zero;
					vers[vertexIndex + 5] = Vector3.zero;
					vers[vertexIndex + 6] = Vector3.zero;
					vers[vertexIndex + 7] = Vector3.zero;

					uvs[vertexIndex + 0] = Vector2.zero;
					uvs[vertexIndex + 1] = Vector2.zero;
					uvs[vertexIndex + 2] = Vector2.zero;
					uvs[vertexIndex + 3] = Vector2.zero;
					uvs[vertexIndex + 4] = Vector2.zero;
					uvs[vertexIndex + 5] = Vector2.zero;
					uvs[vertexIndex + 6] = Vector2.zero;
					uvs[vertexIndex + 7] = Vector2.zero;
				}


				cols[vertexIndex + 0] = effectColor;
				cols[vertexIndex + 1] = effectColor;
				cols[vertexIndex + 2] = effectColor;
				cols[vertexIndex + 3] = effectColor;

				cols[vertexIndex + 4] = color;
				cols[vertexIndex + 5] = color;
				cols[vertexIndex + 6] = color;
				cols[vertexIndex + 7] = color;

				vertexIndex += 8;
			}
		}
		void CollectMeshValues_TTF_OutLine (int vertexIndex, List<Vector3> vers, List<Vector2> uvs, List<Color> cols)//, int[] tris)
		{
			this.CalculateLabelFrame();

			Matrix4x4 matrix = this.GetMatrix() * this.GetScaleMatrix();

			int chCount = this.charInfos.Length;
			int width = Mathf.RoundToInt(this.Width);
			int height = Mathf.RoundToInt(this.Height);

			int posX = 0;
			int posY = 0;

			float offsetX = this.effectOffset.x;
			float offsetY = this.effectOffset.y;

			Vector3 v0, v1, v2, v3;

			Color color = this.color;
			Color effectColor = this.effectColor;

			LabelCharInfo chInfo;

			int currentLineIndex = 0;
			int currentLineWidth = this.lineWidthList[0];
			int posXAlignment = 0;
			int posYAlignment = 0;

			if (this.alignmentX == LabelAlignmentX.Left)
			{
				posXAlignment = -width / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Center)
			{
				posXAlignment = -currentLineWidth / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Right)
			{
				posXAlignment = width / 2 - currentLineWidth;
			}

			posYAlignment = height / 2;

			if (height != this.charsHeight)
			{
				if (this.alignmentY == LabelAlignmentY.Center)
				{
					posYAlignment -= (height - this.charsHeight) / 2;
				}
				else if (this.alignmentY == LabelAlignmentY.Bottom)
				{
					posYAlignment -= (height - this.charsHeight);
				}
			}


			LabelCharInfo[] charInfos = this.charInfos;
			for(int i = 0; i < chCount; ++i)
			{
				chInfo = charInfos[i];

				if (this.alignmentX == LabelAlignmentX.Center)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = -currentLineWidth / 2;
					}
				}
				else if (this.alignmentX == LabelAlignmentX.Right)
				{
					if (chInfo.lineIndex != currentLineIndex)
					{
						currentLineIndex = chInfo.lineIndex;
						currentLineWidth = this.lineWidthList[currentLineIndex];
						posXAlignment = width / 2 - currentLineWidth;
					}
				}
					

				if (chInfo.visible)
				{
					v0 = new Vector3(chInfo.minX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0);
					v1 = new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.minY + posYAlignment + posY, 0);
					v2 = new Vector3(chInfo.maxX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0);
					v3 = new Vector3(chInfo.minX + posXAlignment + posX, chInfo.maxY + posYAlignment + posY, 0);
					
					vers[vertexIndex +  0] = matrix.MultiplyPoint3x4(new Vector3(v0.x - offsetX, v0.y - offsetY, v0.z));
					vers[vertexIndex +  1] = matrix.MultiplyPoint3x4(new Vector3(v1.x - offsetX, v1.y - offsetY, v1.z));
					vers[vertexIndex +  2] = matrix.MultiplyPoint3x4(new Vector3(v2.x - offsetX, v2.y - offsetY, v2.z));
					vers[vertexIndex +  3] = matrix.MultiplyPoint3x4(new Vector3(v3.x - offsetX, v3.y - offsetY, v3.z));
					
					vers[vertexIndex +  4] = matrix.MultiplyPoint3x4(new Vector3(v0.x + offsetX, v0.y - offsetY, v0.z));
					vers[vertexIndex +  5] = matrix.MultiplyPoint3x4(new Vector3(v1.x + offsetX, v1.y - offsetY, v1.z));
					vers[vertexIndex +  6] = matrix.MultiplyPoint3x4(new Vector3(v2.x + offsetX, v2.y - offsetY, v2.z));
					vers[vertexIndex +  7] = matrix.MultiplyPoint3x4(new Vector3(v3.x + offsetX, v3.y - offsetY, v3.z));

					vers[vertexIndex +  8] = matrix.MultiplyPoint3x4(new Vector3(v0.x + offsetX, v0.y + offsetY, v0.z));
					vers[vertexIndex +  9] = matrix.MultiplyPoint3x4(new Vector3(v1.x + offsetX, v1.y + offsetY, v1.z));
					vers[vertexIndex + 10] = matrix.MultiplyPoint3x4(new Vector3(v2.x + offsetX, v2.y + offsetY, v2.z));
					vers[vertexIndex + 11] = matrix.MultiplyPoint3x4(new Vector3(v3.x + offsetX, v3.y + offsetY, v3.z));

					vers[vertexIndex + 12] = matrix.MultiplyPoint3x4(new Vector3(v0.x - offsetX, v0.y + offsetY, v0.z));
					vers[vertexIndex + 13] = matrix.MultiplyPoint3x4(new Vector3(v1.x - offsetX, v1.y + offsetY, v1.z));
					vers[vertexIndex + 14] = matrix.MultiplyPoint3x4(new Vector3(v2.x - offsetX, v2.y + offsetY, v2.z));
					vers[vertexIndex + 15] = matrix.MultiplyPoint3x4(new Vector3(v3.x - offsetX, v3.y + offsetY, v3.z));

					vers[vertexIndex + 16] = matrix.MultiplyPoint3x4(v0);
					vers[vertexIndex + 17] = matrix.MultiplyPoint3x4(v1);
					vers[vertexIndex + 18] = matrix.MultiplyPoint3x4(v2);
					vers[vertexIndex + 19] = matrix.MultiplyPoint3x4(v3);


					uvs[vertexIndex +  0] = this.TransUVLabel(chInfo.uv0);
					uvs[vertexIndex +  1] = this.TransUVLabel(chInfo.uv1);
					uvs[vertexIndex +  2] = this.TransUVLabel(chInfo.uv2);
					uvs[vertexIndex +  3] = this.TransUVLabel(chInfo.uv3);

					uvs[vertexIndex +  4] = uvs[vertexIndex + 0];
					uvs[vertexIndex +  5] = uvs[vertexIndex + 1];
					uvs[vertexIndex +  6] = uvs[vertexIndex + 2];
					uvs[vertexIndex +  7] = uvs[vertexIndex + 3];
					
					uvs[vertexIndex +  8] = uvs[vertexIndex + 0];
					uvs[vertexIndex +  9] = uvs[vertexIndex + 1];
					uvs[vertexIndex + 10] = uvs[vertexIndex + 2];
					uvs[vertexIndex + 11] = uvs[vertexIndex + 3];

					uvs[vertexIndex + 12] = uvs[vertexIndex + 0];
					uvs[vertexIndex + 13] = uvs[vertexIndex + 1];
					uvs[vertexIndex + 14] = uvs[vertexIndex + 2];
					uvs[vertexIndex + 15] = uvs[vertexIndex + 3];

					uvs[vertexIndex + 16] = uvs[vertexIndex + 0];
					uvs[vertexIndex + 17] = uvs[vertexIndex + 1];
					uvs[vertexIndex + 18] = uvs[vertexIndex + 2];
					uvs[vertexIndex + 19] = uvs[vertexIndex + 3];

					
				}
				else
				{
					vers[vertexIndex +  0] = Vector3.zero;
					vers[vertexIndex +  1] = Vector3.zero;
					vers[vertexIndex +  2] = Vector3.zero;
					vers[vertexIndex +  3] = Vector3.zero;
					vers[vertexIndex +  4] = Vector3.zero;
					vers[vertexIndex +  5] = Vector3.zero;
					vers[vertexIndex +  6] = Vector3.zero;
					vers[vertexIndex +  7] = Vector3.zero;
					vers[vertexIndex +  8] = Vector3.zero;
					vers[vertexIndex +  9] = Vector3.zero;
					vers[vertexIndex + 10] = Vector3.zero;
					vers[vertexIndex + 11] = Vector3.zero;
					vers[vertexIndex + 12] = Vector3.zero;
					vers[vertexIndex + 13] = Vector3.zero;
					vers[vertexIndex + 14] = Vector3.zero;
					vers[vertexIndex + 15] = Vector3.zero;
					vers[vertexIndex + 16] = Vector3.zero;
					vers[vertexIndex + 17] = Vector3.zero;
					vers[vertexIndex + 18] = Vector3.zero;
					vers[vertexIndex + 19] = Vector3.zero;

					uvs[vertexIndex +  0] = Vector2.zero;
					uvs[vertexIndex +  1] = Vector2.zero;
					uvs[vertexIndex +  2] = Vector2.zero;
					uvs[vertexIndex +  3] = Vector2.zero;
					uvs[vertexIndex +  4] = Vector2.zero;
					uvs[vertexIndex +  5] = Vector2.zero;
					uvs[vertexIndex +  6] = Vector2.zero;
					uvs[vertexIndex +  7] = Vector2.zero;
					uvs[vertexIndex +  8] = Vector2.zero;
					uvs[vertexIndex +  9] = Vector2.zero;
					uvs[vertexIndex + 10] = Vector2.zero;
					uvs[vertexIndex + 11] = Vector2.zero;
					uvs[vertexIndex + 12] = Vector2.zero;
					uvs[vertexIndex + 13] = Vector2.zero;
					uvs[vertexIndex + 14] = Vector2.zero;
					uvs[vertexIndex + 15] = Vector2.zero;
					uvs[vertexIndex + 16] = Vector2.zero;
					uvs[vertexIndex + 17] = Vector2.zero;
					uvs[vertexIndex + 18] = Vector2.zero;
					uvs[vertexIndex + 19] = Vector2.zero;
				}


				cols[vertexIndex +  0] = effectColor;
				cols[vertexIndex +  1] = effectColor;
				cols[vertexIndex +  2] = effectColor;
				cols[vertexIndex +  3] = effectColor;

				cols[vertexIndex +  4] = effectColor;
				cols[vertexIndex +  5] = effectColor;
				cols[vertexIndex +  6] = effectColor;
				cols[vertexIndex +  7] = effectColor;

				cols[vertexIndex +  8] = effectColor;
				cols[vertexIndex +  9] = effectColor;
				cols[vertexIndex + 10] = effectColor;
				cols[vertexIndex + 11] = effectColor;

				cols[vertexIndex + 12] = effectColor;
				cols[vertexIndex + 13] = effectColor;
				cols[vertexIndex + 14] = effectColor;
				cols[vertexIndex + 15] = effectColor;

				cols[vertexIndex + 16] = color;
				cols[vertexIndex + 17] = color;
				cols[vertexIndex + 18] = color;
				cols[vertexIndex + 19] = color;

				vertexIndex += 20;
			}
		}

		// 获取指定位置char的显示位置
		public Vector2 GetCursorPosition (int charIndex)
		{
			int lineHeight = this.fontSize + this.spacingY;
		
			LabelCharInfo[] charInfos = this.charInfos;
			if (charInfos == null || charInfos.Length == 0)
			{
				//if (this.alignmentX == LabelAlignmentX.Left)
				//{
				//	return new Vector2(-this.Width / 2, -lineHeight / 2);
				//}
				//else if (this.alignmentX == LabelAlignmentX.Center)
				//{
				//	return new Vector2(0, -lineHeight / 2);
				//}
				//else
				//{
				//	return new Vector2(this.Width / 2, -lineHeight / 2);
				//}
				
				return Vector2.zero;
			}
			
			LabelCharInfo chInfo;
			int index = charIndex;
			int charInfosLength = charInfos.Length;
			
			bool end = false;
			// 末尾
			if (charIndex >= charInfosLength)
			{
				index = charInfosLength - 1;
				end = true;
			}
			else if (charIndex < 0) charIndex = 0;
			
			chInfo = charInfos[index];
			while(!chInfo.visible)
			{
				if (--index < 0)
				{
					break;
				}
				chInfo = charInfos[index];
			}
			
			int posXAlignment = 0;
			int posYAlignment = 0;
			int width = Mathf.RoundToInt(this.Width);
			int height = Mathf.RoundToInt(this.Height);
			
			if (this.alignmentX == LabelAlignmentX.Left)
			{
				posXAlignment = -width / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Center)
			{
				posXAlignment = -this.lineWidthList[chInfo.lineIndex] / 2;
			}
			else if (this.alignmentX == LabelAlignmentX.Right)
			{
				posXAlignment = width / 2 - this.lineWidthList[chInfo.lineIndex];
			}

			posYAlignment = height / 2;
			if (height != this.charsHeight)
			{
				if (this.alignmentY == LabelAlignmentY.Center)
				{
					posYAlignment -= (height - this.charsHeight) / 2;
				}
				else if (this.alignmentY == LabelAlignmentY.Bottom)
				{
					posYAlignment -= (height - this.charsHeight);
				}
			}
			
			return new Vector2((end?chInfo.maxX : chInfo.minX) + posXAlignment, - lineHeight / 2 -lineHeight * chInfo.lineIndex + posYAlignment);

		}

		// 计算自动的frame大小
		void CalculateLabelFrame ()
		{
			int width = Mathf.RoundToInt(this.Width);
			int height = Mathf.RoundToInt(this.Height);

			// 计算三个数据 最大宽度.行数.高度
			// 回车键换行.
			string text = m_Text;
			int chCount = text.Length;
			int fontSize = this.fontSize;
			FontStyle fontStyle = this.fontStyle;
			int spacingX = this.spacingX;

			char ch;
			CharacterInfo chInfo;

			int maxFrameX = 0;
			int lineHeight = this.fontSize + this.spacingY;
			int xAdvance = 0;
			int yAdvance = -Mathf.RoundToInt(this.fontSize * 0.86f);


			this.ResetLabelCharInfoArray();
			LabelCharInfo[] charInfos = this.charInfos;

			int lineIndex = 0;
			List<int> lineWidthList;

			Font font = this.TTFFont;

			// 四种情况
			// 都没锁定时
			// 锁定宽度时计算行数,回车与字数折行
			// 锁定高度时计算最大宽度
			// 都锁定时无需计算

			if (this.lockWidth)
			{
				if (this.lockHeight)
				{
					// 高宽已固定, 可能出现装不下的情况

					maxFrameX = width;
					int maxLines = height / lineHeight;
					if (height - maxLines * lineHeight >= this.fontSize)
					{
						++maxLines;
					}
					lineWidthList = new List<int>(maxLines);

					for(int i = 0; i < chCount; ++i)
					{
						ch = text[i];
						charInfos[i].ch = ch;

						if (ch == '\n')
						{
							if (lineIndex + 1 >= maxLines)
							{
								this.noShowAll = true;
								break;
							}

							lineIndex += 1;
							lineWidthList.Add(xAdvance);
							xAdvance = 0;
							yAdvance -= lineHeight;
						}
						else
						{
							if (font.GetCharacterInfo(ch, out chInfo, fontSize, fontStyle))
							{
								if (xAdvance + chInfo.advance > maxFrameX)
								{
									// 高度判断
									if (lineIndex + 1 >= maxLines)
									{
										this.noShowAll = true;
										break;
									}

									// 换行判断
									lineIndex += 1;
									lineWidthList.Add(xAdvance);
									xAdvance = 0;
									yAdvance -= lineHeight;
								}

								charInfos[i].visible = true;// default is false
								charInfos[i].lineIndex = lineIndex;

								charInfos[i].minX = chInfo.minX + xAdvance;
								charInfos[i].maxX = chInfo.maxX + xAdvance;
								charInfos[i].minY = chInfo.minY + yAdvance;
								charInfos[i].maxY = chInfo.maxY + yAdvance;

								charInfos[i].uv0 = chInfo.uvBottomLeft;
								charInfos[i].uv1 = chInfo.uvBottomRight;
								charInfos[i].uv2 = chInfo.uvTopRight;
								charInfos[i].uv3 = chInfo.uvTopLeft;

								xAdvance += (chInfo.advance + spacingX);
							}
						}
					}

					lineWidthList.Add(xAdvance);
					this.charsHeight = lineIndex * lineHeight + this.fontSize;
				}
				else
				{
					// 宽已固定
					this.noShowAll = false;
					lineWidthList = new List<int>();
					maxFrameX = width;

					for(int i = 0; i < chCount; ++i)
					{
						ch = text[i];
						charInfos[i].ch = ch;

						if (ch == '\n')
						{
							lineIndex += 1;
							lineWidthList.Add(xAdvance);
							xAdvance = 0;
							yAdvance -= lineHeight;
						}
						else
						{
							if (font.GetCharacterInfo(ch, out chInfo, fontSize, fontStyle))
							{
								if (xAdvance + chInfo.advance > maxFrameX)
								{
									// 换行判断
									lineIndex += 1;
									lineWidthList.Add(xAdvance);
									xAdvance = 0;
									yAdvance -= lineHeight;
								}

								charInfos[i].visible = true;
								charInfos[i].lineIndex = lineIndex;

								charInfos[i].minX = chInfo.minX + xAdvance;
								charInfos[i].maxX = chInfo.maxX + xAdvance;
								charInfos[i].minY = chInfo.minY + yAdvance;
								charInfos[i].maxY = chInfo.maxY + yAdvance;

								charInfos[i].uv0 = chInfo.uvBottomLeft;
								charInfos[i].uv1 = chInfo.uvBottomRight;
								charInfos[i].uv2 = chInfo.uvTopRight;
								charInfos[i].uv3 = chInfo.uvTopLeft;

								xAdvance += (chInfo.advance + spacingX);
							}
						}
					}

					// 最后一行的宽度
					lineWidthList.Add(xAdvance);
					this.Height = this.charsHeight = lineIndex * lineHeight + this.fontSize;
				}
			}
			else
			{
				if (this.lockHeight)
				{
					// 固定高度
					this.noShowAll = false;

					int maxLines = height / lineHeight;
					if (height - maxLines * lineHeight >= this.fontSize) 
					{
						++maxLines;
					}
					lineWidthList = new List<int>(maxLines);

					for(int i = 0; i < chCount; ++i)
					{
						ch = text[i];
						charInfos[i].ch = ch;

						if (ch == '\n')
						{
							if (lineIndex + 1 >= maxLines)
							{
								this.noShowAll = true;
								break;
							}

							lineIndex += 1;
							yAdvance -= lineHeight;
							lineWidthList.Add(xAdvance);
							maxFrameX = Mathf.Max(xAdvance, maxFrameX);
							xAdvance = 0;
						}
						else
						{
							if (font.GetCharacterInfo(ch, out chInfo, fontSize, fontStyle))
							{
								charInfos[i].visible = true;
								charInfos[i].lineIndex = lineIndex;

								charInfos[i].minX = chInfo.minX + xAdvance;
								charInfos[i].maxX = chInfo.maxX + xAdvance;
								charInfos[i].minY = chInfo.minY + yAdvance;
								charInfos[i].maxY = chInfo.maxY + yAdvance;

								charInfos[i].uv0 = chInfo.uvBottomLeft;
								charInfos[i].uv1 = chInfo.uvBottomRight;
								charInfos[i].uv2 = chInfo.uvTopRight;
								charInfos[i].uv3 = chInfo.uvTopLeft;

								xAdvance += (chInfo.advance + spacingX);
							}
						}
					}

					lineWidthList.Add(xAdvance);
					maxFrameX = Mathf.Max(xAdvance, maxFrameX);
					this.Width = maxFrameX;
					this.charsHeight = lineIndex * lineHeight + this.fontSize;
				}
				else
				{
					// 都不固定
					this.noShowAll = false;
					lineWidthList = new List<int>();


					for(int i = 0; i < chCount; ++i)
					{
						ch = text[i];
						charInfos[i].ch = ch;

						if (ch == '\n')
						{
							lineIndex += 1;
							yAdvance -= lineHeight;
							lineWidthList.Add(xAdvance);
							maxFrameX = Mathf.Max(xAdvance, maxFrameX);
							xAdvance = 0;
						}
						else
						{
							if (font.GetCharacterInfo(ch, out chInfo, fontSize, fontStyle))
							{
								charInfos[i].visible = true;
								charInfos[i].lineIndex = lineIndex;

								charInfos[i].minX = chInfo.minX + xAdvance;
								charInfos[i].maxX = chInfo.maxX + xAdvance;
								charInfos[i].minY = chInfo.minY + yAdvance;
								charInfos[i].maxY = chInfo.maxY + yAdvance;

								charInfos[i].uv0 = chInfo.uvBottomLeft;
								charInfos[i].uv1 = chInfo.uvBottomRight;
								charInfos[i].uv2 = chInfo.uvTopRight;
								charInfos[i].uv3 = chInfo.uvTopLeft;

								xAdvance += (chInfo.advance + spacingX);
							}
						}
					}

					// 最后一行的宽度
					lineWidthList.Add(xAdvance);

					maxFrameX = Mathf.Max(xAdvance, maxFrameX);

					this.Width = maxFrameX;
					this.Height = this.charsHeight = lineIndex * lineHeight + this.fontSize;
				}
			}

			this.lineWidthList = lineWidthList;
		}

		void UpdateColor ()
		{

		}

		void ResetLabelCharInfoArray ()
		{
			if (m_Text == null || m_Text.Length == 0)
			{
				this.charInfos = null;
			}
			else
			{
				if (this.charInfos == null)
				{
					this.charInfos = new LabelCharInfo[m_Text.Length];
				}
				else if (this.charInfos.Length == m_Text.Length)
				{
					System.Array.Clear(this.charInfos, 0, this.charInfos.Length);
				}
				else
				{
					this.charInfos = new LabelCharInfo[m_Text.Length];
				}
			}
		}

	}
}