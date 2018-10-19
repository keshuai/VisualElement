using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX
{
	//将索引记录在元素中
	[System.Serializable]
    public class VEList
    {
		private List<VEle> m_ElemeList = new List<VEle>();
		private List<VEle> m_DepthList = new List<VEle>();

		public bool Contains (VEle e)
		{
			int index0 = e.internalElementIndex;
			int index1 = e.internalDepthIndex;

			return 
				index0 >= 0 && index0 < m_ElemeList.Count &&
				index1 >= 0 && index1 < m_DepthList.Count &&
				m_ElemeList[index0] == e &&
				m_DepthList[index1] == e;
		}

		public void AddEle (VEle e)
		{
			if (e.internalElementIndex == -1 && e.internalDepthIndex == -1)
			{
				e.internalElementIndex = m_ElemeList.Count;
				m_ElemeList.Add(e);

				e.internalDepthIndex = m_DepthList.Count;
				m_DepthList.Add(e);
			}
			if (this.Contains(e))
			{
				throw new System.Exception(string.Format("element has already contains {0}", e.name));
			}
		}

		public void Remove (VEle e)
		{
			if (this.Contains(e))
			{
				m_ElemeList.RemoveAt(e.internalElementIndex);
				m_DepthList.RemoveAt(e.internalDepthIndex);

				e.internalElementIndex = -1;
				e.internalDepthIndex = -1;
			}
		}
    }

}
