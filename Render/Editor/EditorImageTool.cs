﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CXEditor
{
	public class EditorImageTool  
	{
		[MenuItem("CX/图片工具/Trim PNG", false, 4)]
		private static void TrimPNG ()
		{
			string path = EditorUtility.OpenFilePanel("选着原图", "", "");

			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			if (!System.IO.File.Exists(path))
			{
				return;
			}

			Texture2D source = new Texture2D(1, 1);
			byte[] sourceData = System.IO.File.ReadAllBytes(path);
			source.LoadImage(sourceData);

			int sourceWidth = source.width;
			int sourceHeight = source.height;

			int trimX0 = 0;
			int trimY0 = 0;
			int trimX1 = sourceWidth - 1; 
			int trimY1 = sourceHeight - 1;

			Color empty = new Color(0f, 0f, 0f, 0f);

			// x : 0->
			for (int x = trimX0; x < sourceWidth; ++x)
			{
				for (int y = 0; y < sourceHeight; ++y)
				{
					if (source.GetPixel(x, y) != empty)
					{
						trimX0 = x;
						goto ForEnd1;
					}
				}
			}
			ForEnd1:

			// x : (width - 1)->
			for (int x = trimX1; x >= 0; --x)
			{
				for (int y = 0; y < sourceHeight; ++y)
				{
					if (source.GetPixel(x, y) != empty)
					{
						trimX1 = x;
						goto ForEnd2;
					}
				}
			}
			ForEnd2:

			// y : 0->
			for (int y = trimY0; y < sourceHeight; ++y)
			{
				for (int x = 0; x < sourceWidth; ++x)
				{
					if (source.GetPixel(x, y) != empty)
					{
						trimY0 = y;
						goto ForEnd3;
					}
				}
			}
			ForEnd3:

			// y : (height - 1)->
			for (int y = trimY1; y > 0; --y)
			{
				for (int x = 0; x < sourceWidth; ++x)
				{
					if (source.GetPixel(x, y) != empty)
					{
						trimY1 = y;
						goto ForEnd4;
					}
				}
			}
			ForEnd4:

			if (trimX0 == 0 && trimX1 == sourceWidth - 1 && trimY0 == 0 && trimY1 == sourceHeight -1)
			{
				// no trim
				EditorUtility.DisplayDialog("No Trim", path + "\ndo not need trim.", "ok");
				return;
			}

			if (trimX0 >= trimX1 || trimY0 >= trimY1)
			{
				// empty png
				EditorUtility.DisplayDialog("Empty png", path + "\nis an empty png", "ok");
				return;
			}
				
			Debug.Log(trimX0 + "," + trimX1 + "," + trimY0 + "," + trimY1);

			int destWidth = trimX1 - trimX0 + 1;
			int destHeight = trimY1 - trimY0 + 1;

			Texture2D dest = new Texture2D(destWidth, destHeight);
		
			Color c;

			for (int x = 0; x < destWidth; ++x)
			{
				for (int y = 0; y < destHeight; ++y)
				{
					c = source.GetPixel(x + trimX0, y + trimY0);
					dest.SetPixel(x , y, c);
				}
			}

			byte[] destData = dest.EncodeToPNG();
			System.IO.File.WriteAllBytes(path + "trimmed.png", destData);

			UnityEngine.Object.DestroyImmediate(source);
			UnityEngine.Object.DestroyImmediate(dest);

			// show trimmed png
			if (EditorUtility.DisplayDialog("trimmed", "saved to :" + path + "trimmed.png\n", "To folder", "Not to folder"))
			{
				System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(path));
			}
		}
	}
}