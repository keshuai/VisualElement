/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

namespace CX
{
	public enum ViewAssetType
	{
		Empty = 0,
		ImageAtlas = 1,
		TTF = 2,
	}

	// TODO 使用Mono重写ViewAsset对象
	[System.Serializable]
	public class ViewAsset 
	{
		[SerializeField][HideInInspector] public int index;
		[SerializeField][HideInInspector] public ViewAssetType assetType;
		// [SerializeField][HideInInspector] private ImageAtlas m_Atlas;
		// [SerializeField][HideInInspector] private Font m_Font;

		[SerializeField][HideInInspector] private UnityEngine.Object m_Asset;
		
		// public Font ttfFont
		// {
		// 	get { return m_Font; }
		// 	set { m_Font = value; }
		// }
		// public ImageAtlas imageAtlas
		// {
		// 	get { return m_Atlas; }
		// 	set { m_Atlas = value; }
		// }
		public Font GetTTFFont ()
		{
			return m_Asset as Font;
		}
		public bool SetTTF (Font ttf)
		{
			this.assetType = ViewAssetType.TTF;
			if (m_Asset != ttf)
			{
				m_Asset = ttf;
				return true;
			}
			return false;
		}
		public ImageAtlas GetImageAtlas()
		{
			return m_Asset as ImageAtlas;
		}
		public bool SetImageAtlas (ImageAtlas imageAtlas)
		{
			this.assetType = ViewAssetType.ImageAtlas;
			if (m_Asset != imageAtlas)
			{
				m_Asset = imageAtlas;
				return true;
			}
			return false;
		}

		public Texture texture
		{
			get
			{
				if (this.assetType == ViewAssetType.TTF)
				{
					Font font = m_Asset as Font;
					return font == null? null: font.material.mainTexture;
				}
				else if (this.assetType == ViewAssetType.ImageAtlas)
				{
					ImageAtlas imageAtlas = m_Asset as ImageAtlas;
					return imageAtlas == null? null: imageAtlas.Texture;
				}
				return null;
			}
		}
	}

	/// TODO 加入alpha属性
	/// 克隆之后的层级问题 需要解决 
	[ExecuteInEditMode]
	public class View : Drawcall
	{
		// 最大应该可以支持8个 但先只支持4个吧
		public const int assetMaxCount = 4;
		[SerializeField][HideInInspector] private ViewAsset[] m_Assets = new ViewAsset[assetMaxCount];

		public ViewAsset GetAsset (int index)
		{
			if(index >= 0 && index < assetMaxCount)
			{
				if (m_Assets[index] == null)
				{
					Debug.LogError("null");
				}
				return m_Assets[index];
			}
				
			return null;
		}
		
		public void SetAsset (int index, ViewAsset asset)
		{
			if(index >= 0 && index < assetMaxCount)
				m_Assets[index] = asset;
		}

		public List<Font> fontSelectList 
		{
			get
			{	
				List<Font> list = new List<Font>();

				for (int i = 0; i < View.assetMaxCount; ++i)
				{
					if (m_Assets[i].assetType == ViewAssetType.TTF)
					{
						Font f = m_Assets[i].GetTTFFont();
						if (f != null)
						{
							list.Add(f);
						}
					}
				}

				return list;
			}
		}

		public List<ImageInfo> imageInfoList
		{
			get
			{	
				List<ImageInfo> list = new List<ImageInfo>();

				for (int i = 0; i < View.assetMaxCount; ++i)
				{
					if (m_Assets[i].assetType == ViewAssetType.ImageAtlas)
					{
						ImageAtlas atlas = m_Assets[i].GetImageAtlas();
						if (atlas != null)
						{
							list.AddRange(atlas.ImageInfoArray);
						}
					}
				}

				return list;
			}
		}

		internal int GetAssetIndexWithTTF (Font ttf)
		{
			if (ttf == null) return -1;

			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				if (m_Assets[i].assetType == ViewAssetType.TTF)
				{
					if (m_Assets[i].GetTTFFont() == ttf)
					{
						return i;
					}
				}
			}

			return -1;
		}
		internal int GetAssetIndexWithImageAtlas (ImageAtlas imageAtlas)
		{
			if (imageAtlas == null) return -1;

			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				if (m_Assets[i].assetType == ViewAssetType.ImageAtlas)
				{
					if (m_Assets[i].GetImageAtlas() == imageAtlas)
					{
						return i;
					}
				}
			}
			
			return -1;
		}
		internal int GetAssetIndexWithImageInfo (ImageInfo imageInfo)
		{
			if (imageInfo == null) return -1;
			ImageAtlas imageAtlas = imageInfo.atlas;

			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				if (m_Assets[i].assetType == ViewAssetType.ImageAtlas)
				{
					if (m_Assets[i].GetImageAtlas() == imageAtlas)
					{
						return i;
					}
				}
			}

			return -1;
		}

		public Font GetTTF (int assetIndex)
		{
			if (assetIndex >= 0 && assetIndex < View.assetMaxCount)
				return m_Assets[assetIndex].GetTTFFont();

			return null;
		}

		public void SetTTF (int assetIndex, Font ttf)
		{
			if (assetIndex >= 0 && assetIndex < View.assetMaxCount)
			{
				if (m_Assets[assetIndex].SetTTF(ttf))
				{
					this.MarkNeedUpdate();
					this.SetFontTextureToMat();
				}
			}
		}

		public ImageAtlas GetImageAtlas (int assetIndex)
		{
			if (assetIndex >= 0 && assetIndex < View.assetMaxCount)
				return m_Assets[assetIndex].GetImageAtlas();

			return null;
		}
		
		public void SetImageAtlas(int assetIndex, ImageAtlas imageAtlas)
		{
			if (assetIndex >= 0 && assetIndex < View.assetMaxCount)
			{
				if (m_Assets[assetIndex].SetImageAtlas(imageAtlas))
				{
					this.MarkNeedUpdate();
					this.SetFontTextureToMat();
				}
			}
		}
		
		void OnTTFRebuild (Font font)
		{
			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				if (m_Assets[i].assetType == ViewAssetType.TTF)
				{
					// Rebuild Text
					// Debug.Log("OnTTFRebuild");
					foreach(VEle e in m_DepthIndexArray)
					{
						if (e is Label)
						{
							((Label) e).textChanged = true;
						}
					}
				}
			}
		}

		public void SetFontTextureToMat ()
		{
			Material material = this.mat;
			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				ViewAsset asset = m_Assets[i];
				material.SetTexture("_MainTex" + i, asset.texture);
			}
		}
		protected override void VirtualAwake ()
		{
			// 通过代码动态创建的会是null状态
			for (int i = 0; i < View.assetMaxCount; ++i)
			{
				if (m_Assets[i] == null)
				{
					m_Assets[i] = new ViewAsset();
					m_Assets[i].index = i;
				}
			}
			
			base.VirtualAwake ();
			this.SetFontTextureToMat();
			Font.textureRebuilt += this.OnTTFRebuild;
		}
		protected override void VirtualOnDestroy ()
		{
			base.VirtualOnDestroy ();
			Font.textureRebuilt -= this.OnTTFRebuild;
		}
		protected override void VirtualShaderChanged ()
		{
			base.VirtualShaderChanged ();
			this.SetFontTextureToMat();
		}
	}
}