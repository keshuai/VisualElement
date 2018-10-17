using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX
{
	public class UIButton : CXTouchEvent
	{
		[SerializeField] public RectColorVE m_Frame;
		[SerializeField] Label m_Label;
		[SerializeField] RectBorderVE m_Border;

		[SerializeField] BoxCollider m_BoxCollider;

		[SerializeField]private Color m_ColorFrameNormal = new Color(0.80f, 0.80f, 0.80f, 1f);
		[SerializeField]private Color m_ColorFramePressed = Color.gray;

		[SerializeField] float m_LabelWidthDelta = 40;

		[SerializeField] public UnityEngine.Events.UnityEvent ClickedEvent;

		public Color ColorNormal
		{
			get { return m_ColorFrameNormal; }
			set { m_ColorFrameNormal = value; }
		}
		
		public Color ColorPressed
		{
			get { return m_ColorFramePressed; }
			set { m_ColorFramePressed = value; }
		}

		public Vector2 Size
		{
			get { return m_Border.Size;}
			set
			{
				if (value.x < 10) value.x = 10;
				if (value.y < 10) value.y = 10;

				m_Border.Size = value;
				m_Frame.Size = value;
				m_BoxCollider.size = value;
				value.x -= m_LabelWidthDelta;
				m_Label.Size = value;
			}
		}

		public string Title
		{
			get { return m_Label.Text; }
			set { m_Label.Text = value; }
		}

		private void Awake()
		{
			if (m_Frame == null)
			{
				GameObject o = new GameObject();
				o.transform.SetParent(this.transform, false);
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Frame = o.AddComponent<RectColorVE>();
				
				m_Frame.Size = new Vector2(200, 100);
				m_Frame.Color = m_ColorFrameNormal;
				if (Application.isEditor) o.name = "frame";
			}
			
			if (m_Border == null)
			{
				GameObject o = new GameObject();
				o.transform.SetParent(this.transform, false);
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Border = o.AddComponent<RectBorderVE>();
				
				m_Border.Size = new Vector2(200, 100);
				if (Application.isEditor) o.name = "border";
			}
			
			if (m_Label == null)
			{
				GameObject o = new GameObject();
				o.transform.SetParent(this.transform, false);
				o.transform.localPosition = Vector3.zero;
				o.layer = this.gameObject.layer;
				m_Label = o.AddComponent<Label>();
				
				m_Label.lockWidth = true;
				m_Label.lockHeight = true;
				m_Label.Size = new Vector2(200, 100);
				m_Label.alignmentX = LabelAlignmentX.Center;
				m_Label.alignmentY = LabelAlignmentY.Center;
				if (Application.isEditor) o.name = "label";
			}

			if (m_BoxCollider == null)
			{
				m_BoxCollider = this.GetComponent<BoxCollider>();
				if (m_BoxCollider == null) m_BoxCollider = this.gameObject.AddComponent<BoxCollider>();
				m_BoxCollider.size = new Vector3(200, 100, 0);
			}
		}

		private void Start()
		{
			this.widget = m_Frame;
		}
		
		// fields
		CXTouch m_Touch;
	

		void ResetTouch ()
		{
			m_Touch = null;
			m_Frame.Color = m_ColorFrameNormal;
		}
	
		public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
		{
			if ( touch.TouchPhase == TouchPhase.Began )
			{
				this.m_Touch = touch;
				m_Frame.Color = m_ColorFramePressed;
			}
			else if ( touch.TouchPhase == TouchPhase.Ended )
			{
				if ( this.m_Touch == touch )
				{
					touch.ParseDone();
					if (this.ClickedEvent != null)
					{
						this.ClickedEvent.Invoke();
					}
				}
			}
		}
	
		public override void OnTouchCancel (CXTouch touch)
		{
			if ( this.m_Touch == touch )
			{
				this.ResetTouch();
			}
		}
	}
}
