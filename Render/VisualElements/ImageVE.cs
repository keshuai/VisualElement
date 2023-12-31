﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CX
{
	///
	/// 可使用ImageAtlasDrawcall进行渲染
	/// 
	/// Altas 多个贴图 (当一个大图装不下所有的时)
	/// 多个贴图时,前后顺序不再有用
	public abstract class ImageVE : RectVE
	{
		public View ImageDrawCall { get { return (View)m_DrawCall;}}
		[SerializeField][HideInInspector] protected ImageInfo m_ImageInfo;
		[SerializeField][HideInInspector] protected bool m_Trimmed;

		public ImageInfo ImageInfo
		{
			get                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
			{
				return m_ImageInfo;
			}
			set
			{
				if (m_ImageInfo != value)
				{
					m_ImageInfo = value;
					m_UVChanged = true;

					if (this.ImageDrawCall != null)
						this.internalAssetIndex = this.ImageDrawCall.GetAssetIndexWithImageInfo(value);

					// trim 会影响position
					bool trimmed = value == null? false : value.trimmed;
					if (m_Trimmed != trimmed)
					{
						m_Trimmed = trimmed;
						m_SizeChanged = true;
					}
				}
			}
		}

		/// 编辑器下, 引用的imageInfo数据被改变时调用
		private void OnEditorUVChanged ()
		{
			m_UVChanged = true;
		}
	}
}