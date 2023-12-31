﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CXEditor
{
	public class ClassParser
	{
		/// 此单行是否是注释 
		public static bool SingleLineIsNote (string singlelLine)
		{
			if (singlelLine == null)
			{
				return false;
			}
	
			char c;

			for(int index = 0, end = singlelLine.Length; index < end; ++index)
			{
				c = singlelLine[index];
				bool separator = c == ' ' || c == '\t' || c == ';';
				if (!separator)
				{
					if (c == '/')
					{
						return ++index < end && singlelLine[index] == '/';
					}
					else
					{
						return false;
					}
				}
			}

			return false;
		}

		/// 单行字段关键字分割
		/// 例如 "public int test; " 使用{' ', '\t', ';'} 分割成 {"public", "int", "test"}
		public static string[] SingleLineToKeyWords (string fieldlLine)
		{
			if(fieldlLine == null)
			{
				return null;
			}

			List<string> ret = new List<string>();

			char c;

			int subStart = -1;

			int length = fieldlLine.Length;
			for(int index = 0; index < length; ++index)
			{
				c = fieldlLine[index];

				if (c == ' ' || c == '\t' || c == ';')
				{
					if (subStart != -1)
					{
						ret.Add(fieldlLine.Substring(subStart, index - subStart));
						subStart = -1;
					}
				}
				else
				{
					if (subStart == -1)
					{
						subStart = index;
					}
				}
			}

			if (subStart != -1)
			{
				ret.Add(fieldlLine.Substring(subStart, length - subStart));
			}

			return ret.ToArray();
		}
	}
}