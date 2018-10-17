using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CX
{
	public class ImageFrameAnimation : MonoBehaviour 
	{
		[SerializeField] ImageColorVE m_Image;
		[SerializeField] ImageInfo[] m_ImageInfos;
		[SerializeField] [HideInInspector] float m_OnceTime = 2f;
		[SerializeField] [HideInInspector] float m_OneFrameTime = 0.1f;
		[SerializeField] bool m_Play;
	
		public ImageColorVE image
		{
			get { return m_Image; }
			set
			{
				m_Image = value;
			}
		}
		
		public ImageInfo[] imageInfos
		{
			get { return m_ImageInfos; }
			set 
			{ 
				m_ImageInfos = value; 
				
				int frameCount = m_ImageInfos == null? 0 : m_ImageInfos.Length;
				if (frameCount > 0) this.m_OneFrameTime = m_OnceTime / frameCount;
			}
		}
	
		// 循环一次多长时间
		public float onceTime
		{ 
			get { return m_OnceTime; } 
			set 
			{ 
				if (value <= 0) value = 0.1f;
				
				if (m_OnceTime != value)
				{
					m_OnceTime = value;
					
					int frameCount = m_ImageInfos == null? 0 : m_ImageInfos.Length;
					if (frameCount > 0) this.m_OneFrameTime = value / frameCount;
				}
			} 
		}
		public bool play { get { return m_Play; } set { m_Play = value; } }


		private int m_CurrentIndex = 0;
		private float m_CurrentTime = 0;

		private void Update()
		{
			if (!play) return;
			if (image == null || imageInfos == null || imageInfos.Length == 0)
			{
				return;
			}
		
			m_CurrentTime += Time.deltaTime;
			while(m_CurrentTime >= m_OneFrameTime)
			{
				m_CurrentTime -= m_OneFrameTime;
				++m_CurrentIndex;
			}
			
			if (m_CurrentIndex < 0) m_CurrentIndex = 0;
			if (m_CurrentIndex >= imageInfos.Length) m_CurrentIndex = m_CurrentIndex % imageInfos.Length;
			
			m_Image.ImageInfo = m_ImageInfos[m_CurrentIndex];
		}
	}
}
