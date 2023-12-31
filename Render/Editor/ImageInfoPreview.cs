﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using CX;

namespace CXEditor
{
	/// ImageInfo绘制预览图 
	public class ImageInfoPreview
	{
		public static Color s_LftLineColor = new Color(1f, 0.5f, 0f, 1f);
		public static Color s_RgtLineColor = Color.green;
		public static Color s_TopLineColor = Color.cyan;
		public static Color s_BtmLineColor = Color.yellow;

		/// 通过Image宽高比来获取绘制区域
		private static Rect GetPriviewRect (Rect windowRect, float imageWidth, float imageHeight)
		{
			// 长宽比
			float windowAspect = windowRect.width / windowRect.height;
			float imageAspect = imageWidth / imageHeight;
			if (windowAspect == imageAspect)
			{
				return windowRect;
			}

			// 以宽求高
			if (imageAspect > windowAspect)
			{
				float height = windowRect.width * imageHeight / imageWidth;
				return new Rect(windowRect.x, windowRect.y + (windowRect.height - height) / 2, windowRect.width, height);
			}

			// 以高求宽
			float width = windowRect.height * imageAspect;
			return new Rect(windowRect.x + (windowRect.width - width) / 2, windowRect.y, width, windowRect.height);
		}

		/// 透明背景纹理 
		private static Texture s_BackgroundTexture;
		/// 透明背景纹理 
		private static Texture BackgroundTexture
		{
			get
			{
				if (s_BackgroundTexture == null)
				{
					Color c0 = new Color(0.8f, 0.8f, 0.8f, 1.0f);
					Color c1 = new Color(0.6f, 0.6f, 0.6f, 1.0f);

					Texture2D background = new Texture2D(16, 16);
					background.name = "[Generated] ImageInfo Preview Background";
					background.hideFlags = HideFlags.DontSave;

					for (int y = 0; y < 8; ++y)
					{
						for (int x = 0; x < 8; ++x)
						{
							background.SetPixel(x, y, c0);
						}
						for (int x = 8; x < 16; ++x)
						{
							background.SetPixel(x, y, c1);
						}
					}
					for (int y = 8; y < 16; ++y)
					{
						for (int x = 0; x < 8; ++x)
						{
							background.SetPixel(x, y, c1);
						}
						for (int x = 8; x < 16; ++x)
						{
							background.SetPixel(x, y, c0);
						}
					}

					background.filterMode = FilterMode.Point;
					background.Apply();

					s_BackgroundTexture = background;
				}

				return s_BackgroundTexture;
			}
		}

		/// 绘制透明背景 
		private static void DrawBackground (Rect position)
		{
			// 裁剪
			GUI.BeginGroup(position);
			{
				Texture background = BackgroundTexture;

				int width  = Mathf.RoundToInt(position.width);
				int height = Mathf.RoundToInt(position.height);

				for (int y = 0; y < height; y += background.height)
				{
					for (int x = 0; x < width; x += background.width)
					{
						GUI.DrawTexture(new Rect(x, y, background.width, background.height), background);
					}
				}
			}
			GUI.EndGroup();
		}

		/// 区域背景纹理 
		private static Texture s_AreaTexture;
		/// 区域背景纹理 
		public static Texture AreaTexture
		{
			get
			{
				if (s_AreaTexture == null)
				{
					Texture2D tex = new Texture2D(1, 1);
					tex.name = "[Generated] Black Texture";
					tex.hideFlags = HideFlags.DontSave;

					tex.SetPixels(new Color[]{new Color(0.3f, 0.3f, 0.3f, 1f)});

					tex.filterMode = FilterMode.Point;
					tex.Apply();

					s_AreaTexture = tex;
				}

				return s_AreaTexture;
			}
		}

		private static float ImagePixelToWindowRectX (int pixel, float imageWidth, float windowRectWidth)
		{
			return pixel * windowRectWidth / imageWidth;
		}
		private static float ImagePixelToWindowRectY (int pixel, float imageHeight, float windowRectHeight)
		{
			return pixel * windowRectHeight / imageHeight;
		}

		public static void DrawAreaPreview (ImageInfo _this, Rect r)
		{
			GUI.DrawTexture(r, AreaTexture);
			DrawPreview(_this, r);
		}

		public static void DrawPreview (ImageInfo _this, Rect r)
		{
			if (_this == null || r.width == 0 || r.height == 0 || _this.width == 0 || _this.height == 0)
			{
				return;
			}

			Rect postion = GetPriviewRect(r, _this.width, _this.height);

			// 黑白相间为透明背景
			// 黄色框表示主体部分
			// 绿色框表示可视部分


			// 绘制背景
			DrawBackground(postion);

			// 进行主体区域到可视区域的位置变换
			if (_this.trimmed)
			{
				// 上下颠倒
				postion = new Rect(postion.x + _this.trimXmin_normalized * postion.width, postion.y +  _this.trimYmax_normalized * postion.height, _this.trimWidth_normalized * postion.width, _this.trimHeight_normalized * postion.height);
			}

			// 绘制可视部分
			Rect uv = new Rect(_this.uvXMin, _this.uvYMin, _this.uvXMax - _this.uvXMin, _this.uvYMax - _this.uvYMin);
			GUI.DrawTextureWithTexCoords(postion, _this.atlas.Texture, uv, true);

			const float lineWidth = 1;

			if (_this.hasNineInfo)
			{
				// 绘制九宫格拉伸信息
				//if (_this.nineLft != 0)
				{
					float offset = ImagePixelToWindowRectX(_this.nineLft, _this.trimWidth, postion.width);
					EditorUI.DrawVerLine(postion.y, postion.y + postion.height, postion.x + offset, lineWidth, s_LftLineColor);
				}

				//if (_this.nineRgt != 0)
				{
					float offset = ImagePixelToWindowRectX(_this.nineRgt, _this.trimWidth, postion.width);
					EditorUI.DrawVerLine(postion.y, postion.y + postion.height, postion.x + postion.width - offset, lineWidth, s_RgtLineColor);
				}

				//if (_this.nineTop != 0)
				{
					float offset = ImagePixelToWindowRectY(_this.nineTop, _this.trimHeight, postion.height);
					EditorUI.DrawHorLine(postion.x, postion.x + postion.width, postion.y + offset, lineWidth, s_TopLineColor);
				}

				//if (_this.nineBtm != 0)
				{
					float offset = ImagePixelToWindowRectY(_this.nineBtm, _this.trimHeight, postion.height);
					EditorUI.DrawHorLine(postion.x, postion.x + postion.width, postion.y + postion.height - offset, lineWidth, s_BtmLineColor);
				}
			}
		}
	}
}