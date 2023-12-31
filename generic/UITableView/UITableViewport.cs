﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using CX;

public class UITableViewport : CXTouchEvent
{
	public View ViewPanel;
	public UITableView TableView;
	[SerializeField] private BoxCollider Collider;



	CXTouch m_Touch;
	private bool m_Moved;
	private float m_NoMovedYSum = 0f;
	int m_FrameIndex = 0;
	bool m_CellTouchBegin;

	public Vector2 Size
	{
		get
		{
			return this.Collider.size;
		}
		set
		{
			if (this.Collider.size.x != value.x || this.Collider.size.y != value.y)
			{
				this.Collider.size = value;
				if (this.ViewPanel != null)
				{
					//this.ViewPanel.baseClipRegion = new Vector4(0f, 0f, value.x, value.y);
				}

				this.TableView.ResetScrollbarTrans();
			}
		}

	}

	public float Width
	{
		get { return this.Collider.size.x; }
		set { this.Size = new Vector2(value, this.Height); }
	}
	public float Height
	{
		get { return this.Collider.size.y; }
		set { this.Size = new Vector2(this.Width, value); }
	}

	public float MaxFrameMove { get { return this.Height * 0.8f; } }

	// 允许的最大越界距离
	public float MaxCross { get { return this.Height * 0.4f; } }


	public float Top
	{
		get
		{
			return this.transform.localPosition.y + this.Height * 0.5f;
		}
	}
	public float Bottom
	{
		get
		{
			return this.transform.localPosition.y - this.Height * 0.5f;
		}
	}

	void Awake ()
	{
		this.Collider = this.gameObject.AddComponent<BoxCollider>();
	}

	void OnDisable ()
	{
		if (this.m_Touch != null)
		{
			if (this.m_Touch.FocusEvent == this)
			{
				this.m_Touch.FocusEvent = null;
			}
		}
	}


	void Reset ()
	{
		m_Touch = null;
		m_Moved = false;
		m_FrameIndex = 0;
		m_NoMovedYSum = 0;
		m_CellTouchBegin = false;
	}

	public override void OnTouchEvent (CXTouchParser touchParser, CXTouch touch)
	{
		if (this.m_Touch == null)
		{
			bool smoothMove = this.TableView.OnSmoothMove;

			this.TableView.StopSmoothMove();

			if (touch.TouchPhase == TouchPhase.Moved)
			{
				if (this.TableView.DataSource.CanScroll)
				{
					m_Touch = touch;
					touch.FocusEvent = this;
					this.Move( touch.UIDeltaMove.y);
				}
			}
			else if (touch.TouchPhase == TouchPhase.Began && !smoothMove)
			{
				//this.m_Touch = touch;
				//touch.FocusEvent = this;
			}
		}
		else
		{
			if ( touch == this.m_Touch )
			{
				if (touch.TouchPhase == TouchPhase.Moved)
				{
					if (m_Moved)
					{
						this.Move( touch.UIDeltaMove.y);
					}
					else
					{
						/// 在有列表可滑动的情况下 
						if (this.TableView.DataSource.CanScroll)
						{
							m_NoMovedYSum += touch.UIDeltaMove.y;
						}

						/// Y轴累加至 3 像素后才算开始移动 
						if (Mathf.Abs(m_NoMovedYSum) > 3f)
						{
							// 有cellTouch的进行Up
							if (m_CellTouchBegin)
							{
								m_CellTouchBegin = false;
								this.TableView.DoCellTouchUp();
							}

							this.Move(m_NoMovedYSum);
							m_NoMovedYSum = 0f;
						}
						else // 等同于静止解析
						{
							if (!m_CellTouchBegin && ++ m_FrameIndex > 2)
							{
								m_CellTouchBegin = true;
								this.TableView.DoCellTouchBegin(touch.CurrentUIPos.y);
							}
						}
					}
				}
				else if (!this.m_Moved && touch.TouchPhase == TouchPhase.Stationary)
				{
					if (!m_CellTouchBegin && ++ m_FrameIndex > 2)
					{
						m_CellTouchBegin = true;
						this.TableView.DoCellTouchBegin(touch.CurrentUIPos.y);
					}
				}
				else if (touch.TouchPhase == TouchPhase.Ended)
				{
					if (m_Moved)
					{
						this.TableView.StartSmoothMove();
					}
					else if (m_CellTouchBegin)
					{
						m_CellTouchBegin = false;
						touch.ParseDone();
						this.TableView.DoCellTouchUpInside(touch.CurrentUIPos.y);
					}
				}
			}
		}

	}

	public override void OnTouchCancel (CXTouch touch)
	{
		if ( touch == this.m_Touch )
		{
			if (touch.FocusEvent == this)
			{
				if (m_Moved)
				{
					this.TableView.StartSmoothMove();
				}
			}
			if (m_CellTouchBegin)
			{
				this.TableView.DoCellTouchUp();
			}
			this.Reset();
		}
	}

	private void Move (float delta)
	{
		this.m_Moved = true;
		this.TableView.Move(delta);
	}

	public void CancelTouch ()
	{
		if (this.m_Touch != null && this.m_Touch.FocusEvent == this)
		{
			this.m_Touch.ParseDone();
		}
	}

}
