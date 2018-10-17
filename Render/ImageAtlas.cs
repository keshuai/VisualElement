using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CX
{
	/// 图集
	/// 既可以在编辑器下编辑静态Atlas
	/// 也可以在程序运行时代码动态生成Atlas

	public class ImageAtlas : MonoBehaviour
	{
		[SerializeField][HideInInspector] private string m_AtlasIDClassGUID;
		[SerializeField][HideInInspector] private ImageInfo[] m_ImageInfos;
		[SerializeField][HideInInspector] private Texture m_Texture;

		/// 请不要修改此数据 
		public ImageInfo[] ImageInfoArray
		{
			get
			{
				return m_ImageInfos;
			}
		}

		/// 当前图集使用纹理 
		/// (包括字体的纹理)
		public Texture Texture
		{
			get
			{
				return m_Texture;
			}
			set
			{
				m_Texture = value;
			}
		}

		/// 每调用一次 便生成一次数组  
		public string[] iamgeNameArray
		{
			get
			{
				int len = m_ImageInfos.Length;
				string[] nameArray = new string[len];
				for(int i = 0; i < len; ++i)
				{
					nameArray[i] = m_ImageInfos[i].name;
				}

				return nameArray;
			}
		}

		/// 使用 图片名 进行获取
		/// 较慢
		public ImageInfo ImageInfoWithName(string name)
		{
			for(int i = 0, len = m_ImageInfos.Length; i < len; ++i)
			{
				if (m_ImageInfos[i].name.Equals(name))
				{
					return m_ImageInfos[i];
				}
			}

			return null;
		}

		/// 图片名转换为数组索引
		public int ImageNameToIndex(string name)
		{
			for(int i = 0, len = m_ImageInfos.Length; i < len; ++i)
			{
				if (m_ImageInfos[i].name.Equals(name))
				{
					return i;
				}
			}

			return -1;
		}

		/// 使用 数组索引 进行获取
		/// 非常快
		public ImageInfo ImageInfoWithIndex(int index)
		{
			if (index < 0 || index >= m_ImageInfos.Length)
			{
				return null;
			}

			return m_ImageInfos[index];
		}

		/// 使用 数组索引 + 图片名 进行获取
		/// 折中方式, 不快不慢
		public ImageInfo ImageInfoWithIndexName (int index, string name)
		{
			if (index < 0)
			{
				return null;
			}

			if (index < m_ImageInfos.Length)
			{
				ImageInfo data = m_ImageInfos[index];
				if (name == data.name)
				{
					return data;
				}
			}

			foreach(ImageInfo data in m_ImageInfos)
			{
				if (name == data.name)
				{
					return data;
				}
			}

			return null;
		}
	}

}
