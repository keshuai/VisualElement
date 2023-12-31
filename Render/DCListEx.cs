﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	internal static class DCListEx 
	{
		/// 添加空数据 
		public static void AddEmptyRange<T>(this List<T> list, int count)
		{
			list.AddRange(new T[count]);
		}

		/// 添加空数据
		public static void InsertEmptyRange<T>(this List<T> list, int index, int count)
		{
			list.InsertRange(index, new T[count]);
		}

		/// 检查list两个部分是否有重叠 
		public static bool CheckRangeOverlap(int index1, int count1, int index2, int count2)
		{
			int value = index1;
			if (value >= index2 && value < index2 + count2)
			{
				return true;
			}

			value = index1 + count1 - 1;
			if (value >= index2 && value < index2 + count2)
			{
				return true;
			}

			value = index2;
			if (value >= index1 && value < index1 + count1)
			{
				return true;
			}

			value = index2 + count2 - 1;
			if (value >= index1 && value < index1 + count1)
			{
				return true;
			}

			return false;
		}

		/// 将list中的两部分交换位置 
		/// 两个部分不能有重叠部分
		public static void SwapRange<T>(this List<T> list, int index1, int count1, int index2, int count2)
		{
			// fast for same count
			// 两块数量相等时的快速处理
			if (count1 == count2)
			{
				T tmp;
				for (int end = index1 + count1; index1 < end; ++index1, ++index2)
				{
					tmp = list[index1];
					list[index1] = list[index2];
					list[index2] = tmp;
				}
			}
			else 
			{
				// swap if index1 > index2
				if (index1 > index2)
				{
					index1 = index1 ^ index2;
					index2 = index1 ^ index2;
					index1 = index1 ^ index2;

					count1 = count1 ^ count2;
					count2 = count1 ^ count2;
					count1 = count1 ^ count2;
				}

				T[] tmp1 = new T[count1];
				list.CopyTo(index1, tmp1, 0, count1);

				// fast for nearby
				// 相邻时, 快速处理
				//if (index1 + count1 == index2)
				//{
				//}
				//else

				// 通用型处理 
				{
					T[] tmp2 = new T[count2];
					list.CopyTo(index2, tmp2, 0, count2);

					list.RemoveRange(index2, count2);
					list.RemoveRange(index1, count1);

					list.InsertRange(index1, tmp2);
					list.InsertRange(index2 + (count2 - count1), tmp1);
				}
			}
		}



		/// 将list中的一部分移动到指定位置 
		public static void MoveRangeToIndex<T>(this List<T> list, int rangeIndex, int rangeCount, int toIndex)
		{
			// not need move
			if (toIndex >= rangeIndex && toIndex <= rangeIndex + rangeCount)
			{
				return;
			}

			T[] tmp = new T[rangeCount];
			list.CopyTo(rangeIndex, tmp, 0, rangeCount);

			if (rangeIndex > toIndex)
			{
				list.RemoveRange(rangeIndex, rangeCount);
				list.InsertRange(toIndex, tmp);
			}
			else
			{
				list.RemoveRange(rangeIndex, rangeCount);
				list.InsertRange(toIndex - rangeCount, tmp);
			}
		}

		/// 将list中的一部分移动到指定位置 
		public static void MoveRangeToEnd<T>(this List<T> list, int rangeIndex, int rangeCount)
		{
			// not need move
			if (rangeIndex + rangeCount == list.Count)
			{
				return;
			}

			T[] tmp = new T[rangeCount];
			list.CopyTo(rangeIndex, tmp, 0, rangeCount);

			list.RemoveRange(rangeIndex, rangeCount);
			list.AddRange(tmp);
		}
	}
}