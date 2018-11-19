using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CX
{

	public enum ElementAssetType
	{
		TTF = 0,
		ImageAtlas = 1,
	}

	/// 元素使用的资产
	/// 目前内含图集或TTF
	/// 将来可加入图片式文字或其他 
	public class ElementAsset : ScriptableObject 
	{
		/// 1.View 更换Asset时，需要重置部分assetIndex
		/// 2.Asset 内部资源变更时， 需要重置部分assetIndex
		/// 3.

		[SerializeField][HideInInspector]
		ElementAssetType m_AssetType;

		[SerializeField][HideInInspector]
		Font m_TTF;

		[SerializeField][HideInInspector]
		ImageAtlas m_ImageAtlas;

		public Font ttf
		{
			get
			{
				if (m_AssetType == ElementAssetType.TTF)
				{
					return m_TTF;
				}

				return null;
			}
		}

		public ImageAtlas imageAtlas
		{
			get
			{
				if (m_AssetType == ElementAssetType.ImageAtlas)
				{
					return m_ImageAtlas;
				}

				return null;
			}
		}
	}
}
