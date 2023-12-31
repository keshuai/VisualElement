﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;

namespace CX
{
	public class ShaderNames 
	{
		// 命名规则

		// Array前面的都是数组形式的参数
		// Array之后底杠+名称代表uniform属性


		/// 仅适用颜色属性的着色器
		public const string SolidColor = "CX/SolidColor";

		/// 仅适用颜色数组的着色器
		public const string ColorArray = "CX/ColorArray";
		/// 颜色数组 + 整体alpha
		public const string ColorArray_Alpha = "CX/ColorArray_Alpha";
		/// 颜色数组 + 裁剪
		public const string ColorArray_Clip = "CX/ColorArray_Clip";
		/// 颜色数组 + 整体alpha + 裁剪
		public const string ColorArray_Alpha_Clip = "CX/ColorArray_Alpha_Clip";

		/// uv数组与颜色数组 
		public const string UVColorArray = "CX/UVColorArray";
		public const string UVColorArray_Alpha = "CX/UVColorArray_Alpha";
		public const string UVColorArray_Clip = "CX/UVColorArray_Clip";
		public const string UVColorArray_Alpha_Clip = "CX/UVColorArray_Alpha_Clip";

		public const string UIView = "UIView";
	}
}