/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CX;
using System.Collections.Generic;


namespace CXEditor
{
	///  ImageNineEx2 编辑器
	public class ImageNineEx2Editor :  ScriptableWizard
	{
		private static ImageNineEx2Editor GetInstance ()
		{
			return CXEditor.Tools.GetWizardWindowInstance<ImageNineEx2Editor>();
		}

		public static void Show (ImageNineEx2 target)
		{
			ImageNineEx2Editor win = GetInstance();
			win.Target = target;
			win.ShowPopup();
			win.OnStart();
		}

		private void OnLostFocus ()
		{
			//base.Close();
		}

		private void OnDestroy ()
		{
			Debug.Log("OnDestroy window");
		}

		private ImageNineEx2 m_Target;

		public ImageNineEx2 Target 
		{
			get 
			{
				return m_Target;
			}
			set 
			{
				m_Target = value;
			}
		}

		const int CoordinateLineWidth = 2;
		static Color CoordinateXColor = new Color(0f, 1f, 0f, 0.8f);
		void DrawCoordinate ()
		{
			Rect rect = this.position;
			EditorUI.DrawHorLine(0, rect.width, rect.height / 2, CoordinateLineWidth, CoordinateXColor);
			EditorUI.DrawVerLine(0, rect.height, rect.width / 2, CoordinateLineWidth, CoordinateXColor);

		}

		const int GuideLineWidth = 1;
		static Color GuideLineColor = new Color (0f, 1f, 0.8f, 1f);
		void DrawGuideLine ()
		{
			Rect rect = this.position;
			int w = (int)rect.width;
			int h = (int)rect.height;

			int imageWidth = m_Target.ImageInfo.width;
			int imageHeight = m_Target.ImageInfo.height;

			for(int iY = 0; iY <= h / 2; iY += imageHeight)
			{
				EditorUI.DrawHorLine(0, rect.width, rect.height / 2 + iY, GuideLineWidth, GuideLineColor);
				EditorUI.DrawHorLine(0, rect.width, rect.height / 2 - iY, GuideLineWidth, GuideLineColor);
			}

			for(int iX = 0; iX <= w / 2; iX += imageWidth)
			{
				EditorUI.DrawVerLine(0, rect.height, rect.width / 2 + iX, GuideLineWidth, GuideLineColor);
				EditorUI.DrawVerLine(0, rect.height, rect.width / 2 - iX, GuideLineWidth, GuideLineColor);
			}

		}

		float m_ViewScale = 1f;
		ESUI.View m_View = new ESUI.View();

		class ImagePosition
		{
			public ESUI.Image image;
			public int x;
			public int y;

			public ImagePosition(ESUI.Image image, int x, int y)
			{
				this.image = image;
				this.x = x;
				this.y = y;
			}
		}

		List<ImagePosition> m_ImageList = new List<ImagePosition>();

		void OnStart ()
		{
			m_View.Window = this;


		}

		void OnClick (ESUI.Button btn)
		{
		}

		void ParseMouseEvent ()
		{

		}

		void AddPosition (int x, int y)
		{
			bool found = false;
			foreach(ImagePosition eachImage in m_ImageList)
			{
				if (eachImage.x == x && eachImage.y == y)
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				//int centerX = (int)this.position.width / 2;
				//int centerY = (int)this.position.height / 2;

				int xPosition = x * m_Target.ImageInfo.width;
				int yPosition = y * m_Target.ImageInfo.height;

				if (x > 0)
				{
					xPosition -= m_Target.ImageInfo.width / 2;
				}
				else
				{
					xPosition += m_Target.ImageInfo.width / 2;
				}

				if (y > 0)
				{
					yPosition -= m_Target.ImageInfo.height / 2;
				}
				else
				{
					yPosition += m_Target.ImageInfo.height / 2;
				}

				ESUI.Image image = new ESUI.Image(m_Target.ImageInfo);
				image.SetPosition(xPosition, yPosition, m_Target.ImageInfo.width, m_Target.ImageInfo.height);

				ImagePosition imagePostion = new ImagePosition(image, x, y);

				m_View.AddElement(image);
				m_ImageList.Add(imagePostion);

				m_View.Repaint();
			}
		}

		void DeletePosition (int x, int y)
		{
			ImagePosition found = null;
			foreach(ImagePosition eachImage in m_ImageList)
			{
				if (eachImage.x == x && eachImage.y == y)
				{
					found = eachImage;
					break;
				}
			}

			if (found != null)
			{
				m_View.RemoveElement(found.image);
				m_ImageList.Remove(found);

				m_View.Repaint();
			}
		}

		void ParseEvent ()
		{
			Event event_ = Event.current;

			if ((event_.type == EventType.MouseDown || event_.type == EventType.MouseDrag) && event_.button < 2)
			{
				int mouseX = (int)event_.mousePosition.x;
				int mouseY = (int)event_.mousePosition.y;
				int centerX = (int)this.position.width / 2;
				int centerY = (int)this.position.height / 2;

				int x = (mouseX - centerX) / m_Target.ImageInfo.width;
				int y = -(mouseY - centerY) / m_Target.ImageInfo.height;

				if (mouseX >= centerX)
				{
					x = x + 1;
				}
				else
				{
					x = x - 1;
				}

				if (mouseY <= centerY)
				{
					y = y + 1;
				}
				else
				{
					y = y - 1;
				}

				Debug.Log("" + x + "," + y);
				// lft btn
				if (event_.button == 0)
				{
					this.AddPosition(x, y);
				}
				// rgt btn
				else if (event_.button == 1)
				{
					this.DeletePosition(x, y);
				}
			}
		}

		void OnGUI () 
		{
			if (m_Target == null)
			{
				return;
			}

			GUI.Label(new Rect(0, 0, 40, 20), "scale");
			m_ViewScale = EditorGUI.FloatField(new Rect(42, 0, 40, 20), m_ViewScale);


			this.DrawGuideLine();
			this.DrawCoordinate();
		

			Rect pos = this.position;
			m_View.DrawGUI(50, 50, pos.width - 100, pos.height - 100);
			this.ParseMouseEvent();
			this.ParseEvent();
		}
		
		// 每帧更新
		void Update () 
		{
			m_View.Update();

			if (m_View.NeedRepaint)
			{
				Debug.Log("Repaint");
				this.Repaint();
			}

			if (m_Target == null)
			{
				return;
			}
		
		}

		// 拼图分析, 九种情况分析

		// 零向延展

		// 单向延展 三个方向为空的
		// 1.左右:上下无,则左右拼接
		// 2.上下:左右无,则上下拼接

		// 双向延展 

		// 三向延展

		// 四向延展

		//...

		// 八向延展
	}
}