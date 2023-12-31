﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	/// 用于测试DCListEx
	internal class DCListExTest : MonoBehaviour 
	{
		public List<int> m_List = new List<int>();
		public int m_ListCount = 10;

		void InitList ()
		{
			m_List.Clear();
			for(int i = 0; i < m_ListCount; ++i)
			{
				m_List.Add(i);
			}
		}

		public int SwapRangeTest_index1 = 0;
		public int SwapRangeTest_count1 = 2;
		public int SwapRangeTest_index2 = 5;
		public int SwapRangeTest_count2 = 2;

		void SwapRangeTest ()
		{
			m_List.SwapRange(SwapRangeTest_index1, SwapRangeTest_count1, SwapRangeTest_index2, SwapRangeTest_count2);
		}

		public int MoveRangeToIndexTest_rangeIndex = 1;
		public int MoveRangeToIndexTest_rangeCount = 2;
		public int MoveRangeToIndexTest_toIndex = 5;

		void MoveRangeToIndexTest ()
		{
			m_List.MoveRangeToIndex(MoveRangeToIndexTest_rangeIndex, MoveRangeToIndexTest_rangeCount, MoveRangeToIndexTest_toIndex);
		}

		// 开始
		void Start () 
		{
			this.InitList();
		}

		public bool m_InitList = false;
		public bool m_TestSwapRange = false;
		public bool m_TestMoveRange = false;

		// 每帧更新
		void Update () 
		{
			if (m_InitList)
			{
				m_InitList = false;
				this.InitList();
			}

			if (m_TestSwapRange)
			{
				m_TestSwapRange = false;
				this.SwapRangeTest();
			}

			if (m_TestMoveRange)
			{
				m_TestMoveRange = false;
				this.MoveRangeToIndexTest();
			}
		}
	
	}
}