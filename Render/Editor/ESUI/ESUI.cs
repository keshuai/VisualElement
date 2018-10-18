/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace CXEditor.ESUI
{
	internal class NormalWindow : ScriptableWizard
	{
		internal View view;
		internal bool closedOnLostFocus = false;

		private void OnLostFocus ()
		{
			if (this.closedOnLostFocus)
			{
				base.Close();
			}
		}

		private void OnDestroy ()
		{
			Debug.Log("OnDestroy window");
		}

		void OnGUI ()
		{
			if (this.view != null)
			{
				Rect r = this.position;
				this.view.DrawGUI(0, 0, r.width, r.height);
			}
		}
	}

	internal class WizardWindow : NormalWindow
	{}

	public class Window
	{
		private NormalWindow m_InternalWindow;
		private View m_InternalView;
		System.Action m_OnDestroy;

		public Window ()
		{
			m_InternalWindow = CXEditor.Tools.GetWizardWindowInstance<WizardWindow>();
			m_InternalView = new View();

			m_InternalWindow.view = m_InternalView;
			m_InternalView.Window = m_InternalWindow;
		}

		void OnDestroy ()
		{

		}
	}

	public class View
	{
		public delegate void OnViewSizeChanged (int w, int h);

		int m_X;
		int m_Y;
		int m_Width;
		int m_Height;

		Color m_BackgroundColor = new Color(0.6f, 0.6f, 0.6f, 0f);

		public int Width {get { return m_Width; }}
		public int Height {get { return m_Height; }}

		public EditorWindow m_Window;

		private bool m_DrawGizmos = false;
		List<Element> m_ElementList = new List<Element>();
		private OnViewSizeChanged m_OnViewSizeChanged = null;


		private bool m_NeedRepaint = false;

		public bool NeedRepaint { get { return m_NeedRepaint; } }  

		public void MarkNeedRepaint () { m_NeedRepaint = true; }

		public EditorWindow Window
		{
			get{ return m_Window; }
			set{ m_Window = value; }
		}

		public bool DrawGizmos
		{
			get{ return m_DrawGizmos; }
			set{ m_DrawGizmos = value; }
		}

		public float GetGUIX (float x)
		{
			return m_X + m_Width * 0.5f + x;
		}

		public float GetGUIY (float y)
		{
			return  m_Y + m_Height * 0.5f - y;
		}

		public Rect GetGUIRect (float x, float y, float w, float h)
		{
			x = m_X + m_Width * 0.5f + x - w * 0.5f;
			y = m_Y + m_Height * 0.5f - (y + h * 0.5f);

			return new Rect(x, y, w, h);
		}

		public Rect GetGUIRect (Element e)
		{
			return this.GetGUIRect(e.X, e.Y, e.W, e.H);
		}

		public static Color GetDrawColor (Color c)
		{
			c.r *= 0.5f;
			c.g *= 0.5f;
			c.b *= 0.5f;
			return c;
		}

		// 左键, 右键, 中键
		// 按下, 抬起
		// 单击, 双击, 拖动

		public void AddElement (Element e)
		{
			if (e != null && !m_ElementList.Contains(e))
			{
				m_ElementList.Add(e);
				e.SetView(this);
			}
		}

		internal void RemoveElement (Element e)
		{
			if (m_ElementList.Remove(e))
			{
				e.SetView(null);
			}
		}


		private bool m_NeedUpdate = false;

		public void Update ()
		{
			foreach(Element e in m_ElementList)
			{
				e.Update();
			}

			foreach(Element e in m_ElementList)
			{
				e.LateUpdate();
			}

			m_NeedUpdate = false;
		}

		Rect ScreenRect
		{
			get{ return new Rect(m_X, m_Y, m_Width, m_Height); }
		}

		Vector2 CurrentMousePosition
		{
			get
			{
				Vector2 position = Event.current.mousePosition;

				// to current screen
				position.x -= m_X;
				position.y -= m_Y;

				// to current
				position.x = position.x - m_Width * 0.5f;
				position.y = m_Height * 0.5f - position.y;

				return position;
			}
		}

		Element m_LftBtn_FocusEvent;
		Element m_RgtBtn_FocusEvent;

		// 按下, 抬起, 单击, 双击, 拖动
		void ParseEvent ()
		{
			Event event_ = Event.current;


			if (event_ != null) 
			{
				int button = event_.button;

				switch (event_.type) 
				{
				case EventType.MouseDown:
					//Debug.Log("press down");
					if (button == 0)
					{
						//Debug.Log("lft down");

					}
					else if (button == 1)
					{
						//Debug.Log("rgt down");
					}
					// .ToArray : 时间回调可能会操作该List与Foreach冲突, Copy一份出来Foreach
					foreach(Element element in m_ElementList.ToArray())
					{
						if (element.ReceiveEvent && element.HitMouse(this.CurrentMousePosition))
						{
							element.MouseEvent_Down();
						}
					}
					break;

				case EventType.MouseDrag:
					//Debug.Log("press drag");
					if (button == 0)
					{
						//Debug.Log("lft drag");
					}
					else if (button == 1)
					{
						//Debug.Log("rgt drag");
					}
					break;

				case EventType.MouseMove:
					//Debug.Log("press move");
					if (button == 0)
					{
						//Debug.Log("lft move");
					}
					else if (button == 1)
					{
						//Debug.Log("rgt move");
					}
					break;

				case EventType.MouseUp:
					//Debug.Log(this.CurrentMousePosition);
					if (button == 0)
					{
						//Debug.Log("lft up");
					}
					else if (button == 1)
					{
						//Debug.Log("rgt up");
					}

					foreach(Element element in m_ElementList.ToArray())
					{

						if (element.ReceiveEvent && element.HitMouse(this.CurrentMousePosition))
						{
							element.MouseEvent_Up();
						}
					}
					//Debug.Log("press up");
					break;

				case EventType.ScrollWheel:
					//Debug.Log("scroll wheel");
					break;
				}
			}

	
		}

		public void DrawGUI (float x, float y, float width, float height)
		{
			this.DrawGUI(new Rect(x, y, width, height));
		}
		public void DrawGUI (float width, float height)
		{
			this.DrawGUI(new Rect(0, 0, width, height));
		}
		public void DrawGUI (Rect position)
		{
			m_NeedRepaint = false;

			m_X = Mathf.RoundToInt(position.x);
			m_Y = Mathf.RoundToInt(position.y);

			int width = Mathf.RoundToInt(position.width);
			int height = Mathf.RoundToInt(position.height);
			if (m_Width != width || m_Height != height)
			{
				m_Width = width;
				m_Height = height;

				if (m_OnViewSizeChanged != null)
				{
					m_OnViewSizeChanged(width, height);
				}
			}

			this.ParseEvent();

			//GUI.Button(this.ScreenRect, "");
		

			if (m_NeedUpdate)
			{
				foreach(Element e in m_ElementList)
				{
					e.Update();
				}

				foreach(Element e in m_ElementList)
				{
					e.LateUpdate();
				}

				m_NeedUpdate = false;
			}


			if (m_BackgroundColor.a != 0f)
			{
				Graphics.DrawTexture(new Rect (m_X, m_Y, m_Width, m_Height), CXEditor.EditorUI.WhiteTexture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, GetDrawColor(m_BackgroundColor));
			}

			foreach(Element e in m_ElementList)
			{
				e.Draw(0, 0);
			}

			if (m_DrawGizmos)
			{
				foreach(Element e in m_ElementList)
				{
					e.DrawGizmos(0, 0);
				}
			}

			if (m_Window != null && m_NeedRepaint)
			{
				m_Window.Repaint();
				m_NeedRepaint = false;
			}
		}

		public void DrawHorLine (float x1, float x2, float y0, float lineWidth, Color color)
		{
			x1 = this.GetGUIX(x1);
			x2 = this.GetGUIX(x2);
			y0 = this.GetGUIY(y0);
				
			EditorUI.DrawHorLine(x1, x2, y0, lineWidth, color);
		}

		public void DrawVerLine (float y1, float y2, float x0, float lineWidth, Color color)
		{
			y1 = this.GetGUIY(y1);
			y2 = this.GetGUIY(y2);
			x0 = this.GetGUIX(x0);
			EditorUI.DrawVerLine(y1, y2, x0, lineWidth, color);
		}

		public void Repaint()
		{
			if (m_Window != null)
			{
				m_Window.Repaint();
			}
		}
	}

	public class Element
	{
		internal View m_View;

		protected int m_X = 0;
		protected int m_Y = 0;
		protected int m_W = 100;
		protected int m_H = 100;

		//protected bool m_PositionChanged;
		//protected bool m_SizeChanged;

		public int X { get { return m_X;} set { m_X = value;} }
		public int Y { get { return m_Y;} set { m_Y = value;} }
		public int W { get { return m_W;} set { m_W = value;} }
		public int H { get { return m_H;} set { m_H = value;} }

		protected Color m_Color = Color.white;

		protected bool m_ReceiveEvent = false;

		public bool ReceiveEvent { get { return m_ReceiveEvent; } }

		public void SetPosition (int x, int y)
		{
			//m_PositionChanged = x != m_X || y != m_Y;

			m_X = x;
			m_Y = y;
		}

		public void SetSize (int w, int h)
		{
			//m_SizeChanged = w != m_W || h != m_H;
			m_W = w;
			m_H = h;
		}

		public void SetPosition (int x, int y, int w, int h)
		{
			//m_PositionChanged = x != m_X || y != m_Y;
			//m_SizeChanged = w != m_W || h != m_H;

			m_X = x;
			m_Y = y;
			m_W = w;
			m_H = h;
		}

		internal virtual void SetView (View view)
		{
			m_View = view;
		}

		public Color Color { get {return m_Color;} set {m_Color = value;} }

		protected virtual void virtualUpdate (){}

		protected virtual void virtualDraw(int x, int y){}

		internal void Update ()
		{
			this.virtualUpdate();
		}

		internal void LateUpdate ()
		{
			this.virtualUpdate();
		}

		internal void DrawGizmos (int x, int y)
		{
			// GUI.Button(m_View.GetGUIRect(m_X + x, m_Y + y, m_W, m_H), "");
			float halfW = m_W * 0.5f;
			float halfH = m_H * 0.5f;

			float xMin = m_X - halfW;
			float xMax = m_X + halfW;

			float yMin = m_Y - halfH;
			float yMax = m_Y + halfH;

			const float lineWidht = 1;
			Color col = Color.yellow;

			m_View.DrawHorLine(xMin, xMax, yMin, lineWidht, col);
			m_View.DrawHorLine(xMin, xMax, yMax, lineWidht, col);

			m_View.DrawVerLine(yMin, yMax, xMin, lineWidht, col);
			m_View.DrawVerLine(yMin, yMax, xMax, lineWidht, col);
		}

		internal void Draw (int x, int y)
		{
			this.virtualDraw(x, y);
		}

		public void RemoveFromView ()
		{
			if (m_View != null)
			{
				m_View.RemoveElement(this);
				m_View = null;
			}
		}

		public virtual bool HitMouse (Vector2 mousePosition)
		{
			float halfWidth = m_W * 0.5f;
			float halfHeight = m_H * 0.5f;
			return
				mousePosition.x >= (m_X - halfWidth ) &&
				mousePosition.x <= (m_X + halfWidth ) &&
				mousePosition.y >= (m_Y - halfHeight) &&
				mousePosition.y <= (m_Y + halfHeight) ;
		}

		public virtual void MouseEvent_Down ()
		{
		}

		public virtual void MouseEvent_Drag ()
		{

		}

		public virtual void MouseEvent_Up ()
		{

		}
	}

	public class Label : Element
	{
		public string m_Text;
		GUIStyle m_Style = new GUIStyle();

		public Label () 
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 
		}

		public Label (string text)
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 

			m_Text = text;
		}

		public Label (string text, int fontSize)
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 

			m_Text = text;
			this.FontSize = fontSize;
		}

		public string Text 
		{
			get 
			{
				return m_Text;
			}
			set 
			{
				m_Text = value;
			}
		}

		public int FontSize 
		{
			get 
			{
				return m_Style.fontSize;
			}
			set 
			{
				m_Style.fontSize = value;
			}
		}

	

		protected override void virtualDraw (int x, int y)
		{
			GUI.Label(m_View.GetGUIRect(m_X + x, m_Y +y, m_W, m_H), m_Text, m_Style);
		}
	}


	public class Image : Element
	{
		protected Texture m_Texture;
		protected Rect m_UV;

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

		public Image (){}

		public Image (Texture texture)
		{
			this.Init(texture);
		}

		public Image (Texture texture, Rect uv)
		{
			this.Init(texture, uv);
		}

		public Image (CX.ImageInfo imageInfo)
		{
			if (imageInfo != null)
			{
				this.Init(imageInfo.atlas.Texture, InfoUV(imageInfo));
			}
		}
			
		private void Init (Texture texture)
		{
			this.Init(texture, new Rect(0, 0, 1, 1f));
		}
		private void Init (Texture texture, Rect uv)
		{
			m_Texture = texture;
			m_UV = uv;
		}

		public void SetImageInfo (CX.ImageInfo imageInfo)
		{
			if (imageInfo == null)
			{
				this.Init(null);
			}
			else
			{
				this.Init(imageInfo.atlas.Texture, InfoUV(imageInfo));
			}
		}
	
		static Rect InfoUV (CX.ImageInfo imageInfo)
		{
			return new Rect(imageInfo.uvXMin, imageInfo.uvYMin, imageInfo.uvXMax - imageInfo.uvXMin, imageInfo.uvYMax - imageInfo.uvYMin);
		}

		protected override void virtualDraw (int x, int y)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(m_View.GetGUIRect(m_X + x, m_Y +y, m_W, m_H), m_Texture, m_UV, 0, 0, 0, 0, View.GetDrawColor(m_Color), null, -1);
			}
		}
	}

	public class Button : Element
	{
		Image m_Background;
		Label m_Label;

		object m_Param;
		System.Action<Button> m_OnClick;

		public Image Background { get { return m_Background; } }
		public Label Label { get { return m_Label; } }

		public System.Action<Button> OnClick { get { return m_OnClick; } set { m_OnClick = value;} }

		/// 用于自定义的参数
		public object Param { get { return m_Param; } set { m_Param = value; }}


		public Button ()
		{
			m_ReceiveEvent = true;

			m_Background = new Image();
			m_Label = new Label();
		}

		internal override void SetView (View view)
		{
			m_View = view;
			m_Background.SetView(view);
			m_Label.SetView(view);
		}

		public void SetButtonPosition (int x, int y, int w, int h)
		{
			this.SetPosition(x, y, w, h);
			m_Background.SetSize(w, h);
			m_Label.SetSize(w, h);
		}

		public void SetButtonSize (int w, int h)
		{
			m_W = w;
			m_H = h;
			m_Background.SetSize(w, h);
			m_Label.SetSize(w, h);
		}

		protected override void virtualUpdate ()
		{
			m_Background.Update();
			m_Label.Update();
		}

		protected override void virtualDraw (int x, int y)
		{
			m_Background.Draw(m_X + x, m_Y + y);
			m_Label.Draw(m_X + x, m_Y + y);
		}

		public override void MouseEvent_Down ()
		{
			m_Background.Color = new Color(0.5f, 0.5f, 0.5f, 1f);
			m_View.MarkNeedRepaint();
			//Debug.Log("button down");
		}

		public override void MouseEvent_Up ()
		{
			m_Background.Color = new Color(1f, 1, 1f, 1f);
			m_View.MarkNeedRepaint();

			if (m_OnClick != null)
			{
				Event.current.Use();
				m_OnClick(this);
			}
		}
	}

	public class Input : Element
	{
		public string m_Text = "";
		GUIStyle m_Style = new GUIStyle("TextField");

		public Input () 
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 
		}

		public Input (int fontSize)
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 

			this.FontSize = fontSize;
		}


		public Input (string text)
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 

			m_Text = text;
		}

		public Input (string text, int fontSize)
		{
			m_Style.alignment = TextAnchor.MiddleCenter; 

			m_Text = text;
			this.FontSize = fontSize;
		}

		public string Text 
		{
			get 
			{
				return m_Text;
			}
			set 
			{
				m_Text = value;
			}
		}

		public int FontSize 
		{
			get 
			{
				return m_Style.fontSize;
			}
			set 
			{
				m_Style.fontSize = value;
			}
		}



		protected override void virtualDraw (int x, int y)
		{
			m_Text = EditorGUI.TextField(m_View.GetGUIRect(m_X + x, m_Y +y, m_W, m_H), m_Text, m_Style);
		}
	}
		
}