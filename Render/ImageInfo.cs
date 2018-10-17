using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CX
{
	public class ImageInfo : ScriptableObject
	{
		public ImageAtlas atlas;
		public int id;
	
		//------------------------------------------------------------------------------------------------
		// Base Info
		//------------------------------------------------------------------------------------------------

		/// 主体宽度, 主体x,y均为0
		public int width;
		/// 主体宽度, 主体x,y均为0
		public int height;

		public float uvXMin;
		public float uvXMax;
		public float uvYMin;
		public float uvYMax;

		/// bottom left uv
		public Vector2 uv0 { get { return new Vector2(uvXMin, uvYMin);} }
		/// bottom right uv
		public Vector2 uv1 { get { return new Vector2(uvXMax, uvYMin);} }
		/// top right uv
		public Vector2 uv2 { get { return new Vector2(uvXMax, uvYMax);} }
		/// top left uv
		public Vector2 uv3 { get { return new Vector2(uvXMin, uvYMax);} }

		//------------------------------------------------------------------------------------------------
		// Visual Info (trim)
		//------------------------------------------------------------------------------------------------

		// 用于转换trim图片的顶点坐标, 这里面的值得区间为[0, 1]
		public bool trimmed;

		/// trim像素 left
		public int trimLft;
		/// trim像素 right
		public int trimRgt;
		/// trim像素 top
		public int trimTop;
		/// trim像素 bottom
		public int trimBtm;

		public int trimWidth { get { return this.width - this.trimLft - this.trimRgt; }}
		public int trimHeight { get { return this.height - this.trimTop - this.trimBtm; }}

		public float trimXmin_normalized { get { return this.trimLft / (float)this.width;} }
		public float trimXmax_normalized { get { return this.trimRgt / (float)this.width;} }
		public float trimYmin_normalized { get { return this.trimBtm / (float)this.height;} }
		public float trimYmax_normalized { get { return this.trimTop / (float)this.height;} }

		public float trimWidth_normalized { get { return (this.width - this.trimLft - this.trimRgt) / (float)this.width;} }
		public float trimHeight_normalized { get { return (this.height - this.trimTop - this.trimBtm) / (float)this.height;} }

		//------------------------------------------------------------------------------------------------
		// Nine Info
		//------------------------------------------------------------------------------------------------

		public bool hasNineInfo { get { return (this.nineLft | this.nineRgt | this.nineTop | this.nineBtm) != 0; }}

		/// 九宫格 左边 
		public int nineLft;
		/// 九宫格 右边 
		public int nineRgt;
		/// 九宫格 上边 
		public int nineTop;
		/// 九宫格 下边 
		public int nineBtm;

		public int nineWidth { get { return this.width - this.trimLft - this.trimRgt - this.nineLft - this.nineRgt; }}
		public int nineHeight { get { return this.height - this.trimTop - this.trimBtm - this.nineTop - this.nineBtm; }}

		public float nineUVXMin { get { return this.uvXMin + this.nineLft / (float) this.atlas.Texture.width; } }
		public float nineUVXMax { get { return this.uvXMax - this.nineRgt / (float) this.atlas.Texture.width; } }
		public float nineUVYMin { get { return this.uvYMin + this.nineBtm / (float) this.atlas.Texture.height;  } }
		public float nineUVYMax { get { return this.uvYMax - this.nineTop / (float) this.atlas.Texture.height;  } }

		public static ImageInfo NewInstance ()
		{
			return ScriptableObject.CreateInstance<ImageInfo>();
		}
	}

}
