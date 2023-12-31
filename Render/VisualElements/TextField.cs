﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	public class TextField : RectVE 
	{
		public View view { get { return (View)m_DrawCall;}}
		[SerializeField][HideInInspector] protected Vector2 m_Pivot = Vector2.zero;
		[SerializeField][HideInInspector] protected string m_Text;
		// the label vertext count changed with text
		[SerializeField][HideInInspector] protected int m_VertexCount = 0;
		[SerializeField][HideInInspector] protected bool m_TextChanged = false;

		[SerializeField][HideInInspector] protected bool m_CursorShow = true;
		[SerializeField][HideInInspector] protected Color m_CursorColor = Color.white;
		[SerializeField][HideInInspector] protected bool m_FrameShow = false;
		[SerializeField][HideInInspector] protected int m_FrameBorderSize = 2;
		[SerializeField][HideInInspector] protected Color m_FrameColor = Color.white;

		public bool lockWidth;
		public bool lockHeight;

		public int spacingX = 0;
		public int spacingY = 0;

		public LabelAlignmentX alignmentX = LabelAlignmentX.Left;
		public LabelAlignmentY alignmentY = LabelAlignmentY.Top;

		public Color color = Color.white;

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
					m_DrawCall.MarkNeedUpdate();
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

			if (this.Show)
			{
				//if (m_TextChanged)
				{
					m_DrawCall.ElementVertexCountChangedOnUpdate(this, this.internalVertexCount, this.currentVertexCount);

				}
				//if (m_TextChanged && !string.IsNullOrEmpty(m_Text))
				{
					Font font = this.TTFFont;
					if (font != null) font.RequestCharactersInTexture(m_Text, this.fontSize, this.fontStyle);
				}
				if (this.TTFFont != null)
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

				// cursor/selection 4 
				// frame 4 * 4

				return count;
			}
		}

		void Update ()
		{
			
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
				CollectMeshValues_TTF_Normal(vertexIndex , verList, uvList, colList);
			}
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
