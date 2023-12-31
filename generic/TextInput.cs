﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace CX
{
	

	public class TextInput : CXTouchEvent 
	{
		// 手机软键盘
		TouchScreenKeyboard m_Keyboard;
		// 光标位置
		RangeInt m_selection;
		
		[SerializeField][HideInInspector]string m_Text = "";
		
		// 显示的label
		[SerializeField][HideInInspector]Label m_Label; 
		
		[SerializeField][HideInInspector]RectColorVE m_Cursor;
		
		[SerializeField][HideInInspector]RectBorderVE m_Border;
		
		[SerializeField][HideInInspector]BoxCollider m_BoxCollider;
		
		[SerializeField][HideInInspector]Vector2 m_BorderEmptySize = new Vector2(40, 2);
		
		[SerializeField][HideInInspector]bool m_Secure;

		[SerializeField] public UnityEvent OnTextChanged = new UnityEvent();
		
		bool m_OnInput = false;
		bool m_UseTouchKeyboard = false;
		bool m_NeedUpdateCursorPos = true;
		
		public Vector2 Size 
		{
			get { return m_Border.Size; }
			set
			{
				if (value.x < 10) value.x = 10;
				if (value.y < 10) value.y = 10;
			
				m_Label.lockWidth = true;
				m_Label.lockHeight = true;
				
				if (m_Border.Size != null)
				{
					m_Border.Size = value;
				}
				
				Vector2 labelSize = value - m_BorderEmptySize;
				if (m_Label.Size != labelSize)
				{
					m_Label.Size = labelSize;
				}
				
				m_BoxCollider.size = value;
			}
		}
		
		public Vector2 BorderEmptySize 
		{
			get { return m_BorderEmptySize; }
			set
			{
				if (m_BorderEmptySize != value)
				{
					m_BorderEmptySize = value;
					m_Label.Size = this.Size - value;
				}
			}
		}
		
		
		public string Text
		{
			get { return m_Text; }
			set
			{
				if (m_Text != value)
				{
					m_Text = value;
					this.RefreshLabelText();
				}
			}
		}
		
		public RangeInt Selection
		{
			get { return m_selection; }
			set
			{
				m_selection = value;
			}
		}


		public bool OnInputEdit
		{
			get { return m_OnInput; }
			set 
			{
				if (m_OnInput != value)
				{
					m_OnInput = value;
				} 
			}
		}
		
		public bool secure
		{
			get { return m_Secure; }
			set { m_Secure = value; }
		}

		private void Awake()
		{
			if (m_Label == null)
			{
				GameObject o = new GameObject();
				o.transform.parent = this.transform;
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Label = o.AddComponent<Label>();
				
				if (Application.isEditor) o.name = "show label";
			}
			
			if (m_Border == null)
			{
				GameObject o = new GameObject();
				o.transform.parent = this.transform;
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Border = o.AddComponent<RectBorderVE>();
				
				if (Application.isEditor) o.name = "rect border";
			}
			
			if (m_Cursor == null)
			{
				GameObject o = new GameObject();
				o.transform.parent = this.transform;
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Cursor = o.AddComponent<RectColorVE>();
				m_Cursor.Size = new Vector2(2, m_Label.fontSize + 2);
				
				if (Application.isEditor) o.name = "cursor";
			}
			
			if (m_BoxCollider == null)
			{
				m_BoxCollider = this.GetComponent<BoxCollider>();
				if (m_BoxCollider == null)
				{
					m_BoxCollider = this.gameObject.AddComponent<BoxCollider>();
				}
			}

			m_Cursor.Alpha = 0;
		}
		
		void OpenTouchKeyboard ()
		{
			if (m_Text == null) m_Text = "";
			
			if (m_Keyboard == null)
			{
				Debug.Log("new keyboard");
				m_Keyboard = TouchScreenKeyboard.Open(m_Text, TouchScreenKeyboardType.ASCIICapable, false, true, m_Secure);
			}
			
			if (m_Keyboard.status != TouchScreenKeyboard.Status.Visible)
			{
				m_Keyboard.active = true;
			}
			
			m_selection.length = 0;
			m_selection.start = m_Text.Length;
			m_Keyboard.selection = m_selection;
		}
		void CloseTouchKeyboard()
		{
			TouchScreenKeyboard.hideInput = true;
			m_Keyboard = null;
		}
		
		void BeginInput ()
		{
			Debug.Log("BeginInput");
			
			RuntimePlatform platform = Application.platform;
			bool useTouchKeyboard = platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer;
			m_UseTouchKeyboard = useTouchKeyboard;

			// 光标定位到最后位置
			m_selection.length = 0;
			m_selection.start = m_Text.Length;
			
			if (useTouchKeyboard)
			{
				this.OpenTouchKeyboard();
			}
			
			m_OnInput = true;
			m_NeedUpdateCursorPos = true;
		}
		void LeaveInput ()
		{
			m_OnInput = false;
			m_Cursor.Alpha = 0;
			
			if (m_UseTouchKeyboard)
			{
				this.CloseTouchKeyboard();
			}
		}

		void MobileInput ()
		{
			string oldText = this.Text;
			this.Text = m_Keyboard.text;
			this.Selection = m_Keyboard.selection;

			if (oldText != this.Text)
			{
				this.OnTextChanged.Invoke();
			}
		}
			
			
		bool CheckMousePosition ()
		{
			Camera mainCamera = Camera.main;
			if (mainCamera == null) return true;
		
			Vector3 mousePosition = Input.mousePosition;
			int x = (int)mousePosition.x;
			int y = (int)mousePosition.y;

			int minOffset = -100;
			int maxOffset = 100;

			return x >= minOffset && x <= (mainCamera.pixelWidth + maxOffset) && y >= minOffset && y <= (mainCamera.pixelHeight + maxOffset);
		}

		
		// 删除键
		void E_BackDelete ()
		{
			//m_TextArea.DeleteOperator();
			this.PCDeleteKeyPress();
		}

		void E_ForwardDelete ()
		{
			//m_TextArea.DeleteOperator(false);
		}

		void E_MoveToStart (bool shift)
		{

		}

		void E_MoveToEnd (bool shift)
		{

		}

		void E_SelectAll ()
		{
			//m_TextArea.SelectAll();
		}

		void E_Copy ()
		{
			//string copy = m_TextArea.CopySelection();
			//if (copy != null)
			//{
			//	GUIUtility.systemCopyBuffer = copy;
			//}
		}

		void E_Paste ()
		{
			//m_TextArea.AddInput(GUIUtility.systemCopyBuffer);
		}

		void E_Cut ()
		{

		}

		void E_MoveLeft (bool shift, bool ctrl)
		{
			//m_TextArea.CursorMoveLeft();
			if (m_selection.length == 0)
			{
				--m_selection.start;
				int textLength = m_Text == null? 0 : m_Text.Length;
				if (m_selection.start < 0) m_selection.start = 0;
				if (m_selection.start > textLength) m_selection.start = textLength;
			}
			else
			{
				m_selection.length = 0;
			}
			
			m_NeedUpdateCursorPos = true;
		}

		void E_MoveRight (bool shift, bool ctrl)
		{
			//m_TextArea.CursorMoveRight();
			
			if (m_selection.length == 0)
			{
				++m_selection.start;
				int textLength = m_Text == null? 0 : m_Text.Length;
				if (m_selection.start < 0) m_selection.start = 0;
				if (m_selection.start > textLength) m_selection.start = textLength;
			}
			else
			{
				m_selection.length = 0;
			}
			
			m_NeedUpdateCursorPos = true;
		}

		void E_MoveUp (bool shift)
		{
			//m_TextArea.CursorMoveUp();
		}

		void E_MoveDown (bool shift)
		{
			//m_TextArea.CursorMoveDown();
		}

		void E_Enter ()
		{

		}

		void E_Esc ()
		{
		}

		void E_CompositionString (string s)
		{

		}

		void E_InsertChar (char c)
		{
			//m_TextArea.AddInput("" + c);
		}

		char KeyPressed(Event evt)
		{
			var currentEventModifiers = evt.modifiers;
			bool ctrl = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
			bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
			bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
			bool ctrlOnly = ctrl && !alt && !shift;

			switch (evt.keyCode)
			{
				case KeyCode.Backspace:
				{
					E_BackDelete();
					return (char)0;
				}

				case KeyCode.Delete:
				{
					E_ForwardDelete();
					return (char)0;
				}

				case KeyCode.Home:
				{
					E_MoveToStart(shift);
					return (char)0;
				}

				case KeyCode.End:
				{
					E_MoveToEnd(shift);
					return (char)0;
				}

				case KeyCode.A:
				{
					if (ctrlOnly)
					{
						E_SelectAll();
						return (char)0;
					}
					break;
				}

				case KeyCode.C:
				{
					if (ctrlOnly)
					{
						E_Copy();
						return (char)0;
					}
					break;
				}

				case KeyCode.V:
				{
					if (ctrlOnly)
					{
						E_Paste();
						return (char)0;
					}
					break;
				}

				case KeyCode.X:
				{
					if (ctrlOnly)
					{
						E_Cut();
						return (char)0;
					}
					break;
				}

				case KeyCode.LeftArrow:
				{
					E_MoveLeft(shift, ctrl);
					return (char)0;
				}

				case KeyCode.RightArrow:
				{
					E_MoveRight(shift, ctrl);
					return (char)0;
				}

				case KeyCode.UpArrow:
				{
					E_MoveUp(shift);
					return (char)0;
				}

				case KeyCode.DownArrow:
				{
					E_MoveDown(shift);
					return (char)0;
				}

				case KeyCode.Return:
				case KeyCode.KeypadEnter:
				{
					//E_Enter();
					//return;
					break;
				}

				case KeyCode.Escape:
				{
					E_Esc();
					return (char)0;
				}
			}

			return evt.character;
		}

		Event m_Event = new Event();
		void PCInput ()
		{
			if (Application.isEditor)
			{
				if (!this.CheckMousePosition())
				{
					return;
				}
			}

			// // mobile模式下使用touchKeyboard.text

			bool noEvent = true;

			bool pressedESC = false;
			string inputString = "";
			char c;
			while(Event.PopEvent(m_Event))
			{
				noEvent = false;
			
				if (m_Event.rawType == EventType.MouseDown)
				{
					//veci2 position = this.MousePositionToTextAreaPosition(m_Event.mousePosition);
					//m_DrafTag = this.MouseHitTag(m_Event.mousePosition);
					//if (m_DrafTag == 0)
					//{
					//	if (m_Event.button == 0)
					//	{
					//		int clickCount = m_Event.clickCount % 3;
					//		if (clickCount == 1)
					//		{
					//			m_TextArea.MouseClickAtPostion(position);
					//		}
					//		else if (clickCount == 2)
					//		{
					//			m_TextArea.MouseDoubleClickAtPostion(position);
					//		}
					//		else
					//		{
					//			m_TextArea.MouseThriceClickAtPostion(position);
					//		}
					//	}
					//	else if (m_Event.button == 1)
					//	{
					//		if (m_TextArea.HasSelection)
					//		{

					//		}
					//		else
					//		{
					//			m_TextArea.MouseClickAtPostion(position);
					//		}

					//	}
					//	else 
					//	{
					//	}

					//	this.UpdateCursorAndSelectFrame();
					//}
					//else if (m_DrafTag == 1)
					//{
					//	m_HorSlider.MouseDown(this.MousePositionToCurrent(m_Event.mousePosition));
					//}
					//else// if (m_DrafTag == 2)
					//{
					//	m_VerSlider.MouseDown(this.MousePositionToCurrent(m_Event.mousePosition));
					//}
				}
				else if (m_Event.rawType == EventType.MouseDrag)
				{
					if (m_Event.button == 0)
					{
						//if (m_DrafTag == 0)
						//{
						//	m_TextArea.MouseDrag(this.MousePositionToTextAreaPosition(m_Event.mousePosition));
						//	this.UpdateCursorAndSelectFrame();
						//	m_Draged = true;
						//}
						//else if (m_DrafTag == 1)
						//{
						//	m_HorSlider.DragTo(this.MousePositionToCurrent(m_Event.mousePosition));
						//	m_TextArea.HorPos = m_HorSlider.GetCurrentValue();
						//}
						//else
						//{
						//	m_VerSlider.DragTo(this.MousePositionToCurrent(m_Event.mousePosition));
						//	m_TextArea.VerPos = m_VerSlider.GetCurrentValue();
						//}
					}
				}
				else if (m_Event.rawType == EventType.MouseUp)
				{
					//if (m_Draged)
					//{
					//	if (m_DrafTag == 0)
					//	{
					//		if (m_Event.button == 0)
					//		{

					//		}
					//	}
					//	else if (m_DrafTag == 1)
					//	{
					//	}
					//	else
					//	{
					//	}

					//	m_Draged = false;
					//	m_DrafTag = 0;
					//}
				}
				else if (m_Event.rawType == EventType.KeyDown)
				{
					if (m_Event.keyCode == KeyCode.Escape)
					{
						pressedESC = true;
					}
					else
					{
						c = KeyPressed(m_Event);
						if (c != 0)
						{
							inputString += c;
						}
					}
				}
				// 滚动
				//else if (m_Event.rawType == EventType.ScrollWheel)
				//{
				//	if (m_VerSlider.Visiable)
				//	{
				//		m_VerSlider.Drag(m_Event.delta.y * m_MouseScrollFactor);
				//		m_TextArea.VerPos = m_VerSlider.GetCurrentValue();
				//	}
				//}
			}
			


			if (!pressedESC && string.IsNullOrEmpty(Input.compositionString))
			{
				this.PCAddInput(inputString);
				//m_TextArea.AddInput(inputString);
				//this.UpdateCursorAndSelectFrame();
			}

			// 设定PC输入法位置
			if (!string.IsNullOrEmpty(Input.compositionString))
			{
				//veci2 pos = m_TextArea.CurrentCurSorPostion;
				//Vector2 p = this.TextAreaPositionToCompositionPosition(pos);
				//p.y += (2 *(m_TextArea.m_FontSize + m_TextArea.m_SpacingY));
				//Input.compositionCursorPos = p;
			}

			if (noEvent)
			{
				//if (m_Draged && m_DrafTag == 0)
				//{
				//	m_TextArea.MouseDragStatic();
				//}
			}
		}



		void PCAddInput (string inputString)
		{
			if (string.IsNullOrEmpty(inputString)) return;
			if (string.IsNullOrEmpty(m_Text))
			{
				m_Text = inputString;
				m_selection.start = m_Text.Length;
				m_selection.length = 0;
			}
			else
			{
				// remove selected
				if (m_selection.length > 0)
				{
					m_Text.Remove(m_selection.start, m_selection.length);
					m_selection.length = 0;
				}
				
				// 最前面
				if (m_selection.start <= 0)
				{
					m_Text = inputString + m_Text;
					m_selection.start = inputString.Length;
				}
				// 最后面
				else if (m_selection.start >= m_Text.Length)
				{
					m_Text = m_Text + inputString;
					m_selection.start = m_Text.Length;
				}
				// 中间
				else
				{
					m_Text = m_Text.Substring(0, m_selection.start) + inputString + m_Text.Substring(m_selection.start);
					m_selection.start += inputString.Length;
				}
			}
			this.RefreshLabelText();
			
			this.OnTextChanged.Invoke();
		}
		void PCDeleteKeyPress ()
		{
			if (string.IsNullOrEmpty(m_Text)) return;
			
			if (m_selection.length == 0)
			{
				// 最前
				if (m_selection.start <= 0)
				{
					m_selection.start = 0;
					return;
				}
				
				// 最后
				if (m_selection.start >= m_Text.Length)
				{
					
					m_Text = m_Text.Substring(0, m_Text.Length - 1);
					--m_selection.start;
				}
				// 中间
				else
				{
					m_Text = m_Text.Remove(m_selection.start - 1, 1);
					--m_selection.start;
				}
				this.RefreshLabelText();
			}
			else if (m_selection.length > 0)
			{
				m_selection.length = 0;
				m_Text = m_Text.Remove(m_selection.start, m_selection.length);
				this.RefreshLabelText();
			}
		}
		
		void RefreshLabelText ()
		{
			m_Label.Text = m_Text;
			m_NeedUpdateCursorPos = true;
		}
		
		
		private float m_CursorAlpha = 0;
		[SerializeField][HideInInspector]private float m_CursorAlphaIncrease = 4f;
		[SerializeField][HideInInspector]private float m_CursorFlickerOnceTime = 0.5f;
		
		public float CursorFlickerOnceTime
		{
			get { return m_CursorFlickerOnceTime; }
			set 
			{ 
				if (value <= 0) value = 0.05f;
				m_CursorFlickerOnceTime = value;
				m_CursorAlphaIncrease = 2f / value;
			}
		}
		
		void UpdateCursorAlpha ()
		{
			m_CursorAlpha += (Time.deltaTime * m_CursorAlphaIncrease);
			if (m_CursorAlpha > 1)
			{
				m_CursorAlphaIncrease = -m_CursorAlphaIncrease;
				m_CursorAlpha = 1;
			}
			else if (m_CursorAlpha < 0)
			{
				m_CursorAlphaIncrease = -m_CursorAlphaIncrease;
				m_CursorAlpha = 0;
			}

			m_Cursor.Alpha = m_CursorAlpha > 0.3f ? 1 : 0;
		}
		
		void UpdateCursorPos ()
		{
			// 更新光标位置需要在下一帧
			if (m_NeedUpdateCursorPos)
			{
				m_NeedUpdateCursorPos = false;
				m_Cursor.transform.localPosition = m_Label.GetCursorPosition(m_selection.start);
			}
		}

		void Update()
		{
			if (m_OnInput)
			{
				this.UpdateCursorPos();
			
				if (m_UseTouchKeyboard)
				{
					this.MobileInput();
				}
				else
				{
					this.PCInput();
				}
				
				// no selection
				if (this.m_selection.length == 0)
				{
					this.UpdateCursorAlpha();
				}
				

			}
		}

		///  --- touch ---


		CXTouch m_Touch = null;

		void ResetTouch()
		{
			m_Touch = null;
		}

		public override void OnTouchEvent(CXTouchParser touchParser, CXTouch touch)
		{
			if (touch.TouchPhase == TouchPhase.Began)
			{
				if (m_Touch == null)
				{
					m_Touch = touch;
				}
			}
			else if (touch.TouchPhase == TouchPhase.Ended)
			{
				if (m_Touch == touch)
				{
					if (touchParser.FocusEvent == this)
					{
						if (touchParser.CheckTouchHitCollider(touch, m_BoxCollider))
						{
							// 在手机上 继续打开键盘
							if (m_UseTouchKeyboard)
								this.BeginInput();
						}
						// 取消全局焦点事件
						else
						{
							this.ResetTouch();
							touchParser.FocusEvent = null;
							
							this.LeaveInput();
						}
					}
					else
					{
						// 启用全局焦点事件
						touchParser.FocusEvent = this;
						this.BeginInput();
					}
				}
			}
		}

		public override void OnTouchCancel(CXTouch touch)
		{
			if (touch == m_Touch)
			{
				this.ResetTouch();
			}
		}

	}
}