﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{

	public class ImageNineEx2 : VEle 
	{
		struct Item
		{
			public int x;
			public int y;
		}

		[SerializeField] protected ImageInfo m_ImageInfo1;
		[SerializeField][HideInInspector] protected ImageInfo m_ImageInfo2;
		[SerializeField]protected bool m_OneCenter = false;

		[SerializeField][HideInInspector] List<Item> m_Items = new List<Item>();
		[SerializeField][HideInInspector] bool m_ItemsChanged = false;

		public ImageInfo ImageInfo
		{
			get                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
			{
				return m_ImageInfo1;
			}
			set
			{
				if (m_ImageInfo1 != value)
				{
					m_ImageInfo1 = value;
					m_UVChanged = true;
				}
			}
		}

		public ImageInfo ImageInfo2
		{
			get                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
			{
				return m_ImageInfo2;
			}
			set
			{
				if (m_ImageInfo2 != value)
				{
					m_ImageInfo2 = value;
					m_UVChanged = true;
				}
			}
		}

		public override void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList)
		{
			this.internalVertexCount = 0;
		}

		public override void virtualUpdateVertexIndex (List<int> vertexList)
		{

		}

		protected override void virtualLateUpdate ()
		{
			if (m_ItemsChanged)
			{

			}
		}
	}
}