/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using CX;


/********************************************************************
 * 	UITableView由5个部分构成
 * 
 * 	1.	UITableView			主体
 * 	2.	UITableViewCell			列表对象
 * 	3.	UITableViewDataSource	数据源
 * 	4.	UITableViewCellData		列表数据
 * 	5.	UITableViewport		视口和事件
 * 
 * *******************************************************************/
public class UITableView : MonoBehaviour 
{
	// 渲染Panel
	public View Panel;
	// 数据源
	public UITableViewDataSource DataSource;
	// 显示视口
	public UITableViewport Viewport;
	// 
	public UIScrollBar ScrollBar;

	// 子对象节点
	public Transform CellTrans;
	private Transform CachedTrans;

	// 缓冲cell list
	private List<UITableViewCell> CachedCellList;

	// 可见的顶部cell
	public UITableViewCell TopCell;
	// 可见的底部cell
	public UITableViewCell BottomCell;

	private bool m_OnSmoothMove;
	private float LastPos;
	public float LastFrameMove;
	//private float CurrentPos { get { return this.CellTrans.localPosition.y; } }
	// 是否处于平滑滚动
	public bool OnSmoothMove { get {return m_OnSmoothMove;} }

	// for scroll bar
	private bool ScrollBarAlphaLight;
	private bool ScrollBarAlphaDark;

	/// 背景的显示区域仅仅只有适口的位置和大小，需要更大时不要使用这个。
	public RectColorVE BackgroundSprite;

	public int ScrollBarDepth
	{
		get { return this.ScrollBar == null? 0 : this.ScrollBar.BackgroundVE.depth; }
		set { if (this.ScrollBar != null) { this.ScrollBar.BackgroundVE.depth = value++; this.ScrollBar.ForegroundVE.depth = value; } }
	}

	// colored the click
	RectColorVE TouchDownSprite;
	public Color TouchDownColor;

	public int TouchDownSpriteDepth
	{
		get { return this.TouchDownSprite == null? 0 : this.TouchDownSprite.depth; }
		set { if (this.TouchDownSprite != null) this.TouchDownSprite.depth = value; }
	}
	public bool EnableTouchDownColor
	{
		get { return this.TouchDownSprite != null; }
		set 
		{
			if (value)
			{
				if (this.TouchDownSprite == null)
				{
					//this.TouchDownSprite = CXUIPool.CreateSprite(this.CellTrans);
					GameObject o = new GameObject("TouchDownFrameVE");
					o.transform.SetParent(this.CellTrans, false);
					this.TouchDownSprite = o.AddComponent<RectColorVE>();
					//this.TouchDownSprite.atlas = null;
					//this.TouchDownSprite.spriteName = "blank";
					//this.TouchDownSprite.type = UISprite.Type.Sliced;
					//this.TouchDownSprite.alpha = 0f;
					this.TouchDownSprite.Alpha = 0;
				}
			}
			else
			{
				if (this.TouchDownSprite != null)
				{
					//CXUIPool.CollectSprite(this.TouchDownSprite);
					Destroy(this.TouchDownSprite);
					this.TouchDownSprite = null;
				}
			}
		}

	}

	// used for save and recove the pos
	// 用来存粗位置，并恢复位置。
	// float savedPos = tableView.Pos; ... tableView.Pos = savedPos; tableView.ReloadData();
	public float Pos
	{
		get { return this.CellTrans.localPosition.y; }
		set {this.CellTrans.localPosition = new Vector3(0f, value, 0f);}
	}

	// const value
	const float NoCellMoveRate = 0.5f;
	const float SmoothSpeedReduceRate = 1.5f;
	const float SmoothMoveMin = 0.5f;


	void Awake ()
	{
		this.CachedCellList = new List<UITableViewCell>();

		this.Panel = this.gameObject.AddComponent<View>();
		//this.Panel.clipping = UIDrawCall.Clipping.SoftClip;

		
		this.BackgroundSprite = this.Panel.NewElement<RectColorVE>();//this.BackgroundSprite = CXUIPool.CreateSprite(this.transform);
		this.BackgroundSprite.depth = 0;//this.BackgroundSprite.depth = -1;
		this.BackgroundSprite.name = "0_viewport";

		this.Viewport = this.BackgroundSprite.gameObject.AddComponent<UITableViewport>();
		this.Viewport.transform.localPosition = Vector3.zero;
		this.Viewport.TableView = this;
		this.Viewport.ViewPanel = this.Panel;
		this.Viewport.element = this.BackgroundSprite;

		this.CellTrans = new GameObject("1_cellList").transform;
		this.CellTrans.parent = this.Panel.ChildRoot;

		this.CachedTrans = new GameObject("3_cached").transform;
		this.CachedTrans.parent = this.Panel.ChildRoot;
		this.CachedTrans.gameObject.SetActive(false);

//		#if UNITY_EDITOR
//		this.Viewport.name = "0_viewport";
//		this.CellTrans.name = "1_cellList";
//		this.CachedTrans.name = "3_cached";
//		#endif
	}

	// 创建内建滚动条
	public void CreateScrollBar ()
	{
		if (this.ScrollBar == null)
		{
			//this.ScrollBar = ComponentCreator.Create<UIScrollBar>(this.transform);
			GameObject o = new GameObject("Scroll Bar");
			o.transform.SetParent(this.Panel.ChildRoot, false);
			this.ScrollBar = o.AddComponent<UIScrollBar>();
//			CXSprite fsprite = CXUIPool.CreateSprite(this.ScrollBar.transform);
//			CXSprite bsprite = CXUIPool.CreateSprite(this.ScrollBar.transform);
//			this.ScrollBar.foregroundWidget = fsprite;
//			this.ScrollBar.backgroundWidget = bsprite;
//			fsprite.transform.localPosition = Vector3.zero;
//			bsprite.transform.localPosition = Vector3.zero;
//			fsprite.type = UISprite.Type.Sliced;
//			bsprite.type = UISprite.Type.Sliced;
//			fsprite.width = 16;
//			bsprite.width = 16;
//			this.ScrollBar.fillDirection = UIProgressBar.FillDirection.TopToBottom;
//			this.ScrollBar.alpha = 0f;
			this.ResetScrollbarTrans();

			#if UNITY_EDITOR
//			fsprite.name = "1_fsprite";
//			bsprite.name = "2_bsprite";
			this.ScrollBar.name = "4_scrollBar";
			#endif
		}
	
	}

	/// 当视口大小调整的时候，自动调整滚动条的位置与尺寸
	public void ResetScrollbarTrans ()
	{
		if (this.ScrollBar != null && this.Viewport != null && this.DataSource != null)
		{
			int height = Mathf.RoundToInt(this.Viewport.Height - (this.DataSource.TopSpace + this.DataSource.BottomSpace));
			this.ScrollBar.ForegroundVE.Height = height;
			this.ScrollBar.BackgroundVE.Height = height;

			this.ScrollBar.transform.localPosition = new Vector3 (this.Viewport.Width * 0.5f - 16f, (this.DataSource.TopSpace - this.DataSource.BottomSpace) * 0.5f, 0f);
		}
	}

	// 设置滚动的NGUI图片
	public void SetScrollBar (Color fColor, Color bColor)
	{
		if (this.ScrollBar != null)
		{
//			CXSprite fsprite = (CXSprite)this.ScrollBar.foregroundWidget;
//			CXSprite bsprite = (CXSprite)this.ScrollBar.backgroundWidget;
//			fsprite.atlas = atlas;
//			bsprite.atlas = atlas;
//			fsprite.spriteName = fSpriteName;
//			bsprite.spriteName = bSpriteName;
//			fsprite.color = new Color(fColor.r, fColor.g, fColor.b, 0f);
//			bsprite.color = new Color(bColor.r, bColor.g, bColor.b, 0f);
		}
	}

	// 当cell被按下时 由视口事件调用
	public void DoCellTouchBegin (float pos)
	{
		this.DataSource.CellTouchBegin(this.HitCell(pos));
		if (this.TouchDownSprite != null)
		{
			this.ShowTouchDownSrite(this.HitCell(pos));
		}
	}

	// 当按下cell后抬起或取消
	public void DoCellTouchUp ()
	{
		this.DataSource.CellTouchUp();
		if (this.TouchDownSprite != null)
		{
			this.TouchDownSprite.Alpha = 0f;
		}
	}

	public void DoCellTouchUpInside (float pos)
	{
		if (this.TouchDownSprite != null)
		{
			this.TouchDownSprite.Alpha = 0f;
		}
		this.DataSource.CellTouchUpInside(this.HitCell(pos));
	}

	private void ShowTouchDownSrite (UITableViewCell cell)
	{
		if (cell != null && this.TouchDownSprite != null)
		{
			this.TouchDownSprite.transform.localPosition = new Vector3(0f, cell.Data.LocalPos, 0f);
			this.TouchDownSprite.Size = new Vector2(this.Viewport.Width, cell.Data.Height);
			this.TouchDownSprite.Color = this.TouchDownColor;
		}
	}

	// 获取可点击的cell对象
	private UITableViewCell HitCell (float pos)
	{
		UITableViewCell cell = this.TopCell;
		while (cell != null)
		{
			if (cell.ContainsPos(pos))
			{
				if (cell.Data.IsHeader || cell.Data.IsFooter)
				{
					// 头尾不触发点击事件
					return null;
				}
				return cell;
			}

			cell = cell.NextCell;
		}

		return null;
	}

	void Update ()
	{
		this.DoSmoothMove();
		this.UpdateScrollBarAlpha();
	}


	void DoSmoothMove ()
	{
		if (this.m_OnSmoothMove)
		{
			this.LastFrameMove *= (1f - Time.deltaTime * SmoothSpeedReduceRate);
			this.Move(this.LastFrameMove);
			if (Mathf.Abs(this.LastFrameMove) < SmoothMoveMin)//if (Mathf.Abs(this.LastFrameMove) < SmoothMoveMin && !this.DataSource.OnCross)
			{
				this.StopSmoothMove();
				this.ScrollBarAlphaDark = true;
			}
		}

		// record last frame delta move
		{
			const int MaxFrameMove = 30;

			float currentPos = this.Pos;
			this.LastFrameMove = Mathf.Clamp(currentPos - this.LastPos, -MaxFrameMove, MaxFrameMove);
			this.LastPos = currentPos;
		}
	}

	public void StartSmoothMove ()
	{
		this.m_OnSmoothMove = true;
		if (this.DataSource.OnCrossTop)
		{
			this.LastFrameMove = 1f;
		}
		else if (this.DataSource.OnCrossBottom)
		{
			this.LastFrameMove = -1f;
		}
	}

	public void StopSmoothMove ()
	{
		this.m_OnSmoothMove = false;
		this.LastFrameMove = 0f;
	}

	/// 在reload之前, 必须停止滚动, 目前未实现
	private void StopMove ()
	{

	}
	
	public void ReloadData ()
	{
		this.StopMove();

		this.CachedTrans.gameObject.SetActive(true);
		{
			this.DataSource.ReloadData();
			this.RemoveAllVisibleCell();
			if (this.DataSource.NumberOfTotalCells > 0)
			{
				if (!this.DataSource.CanScroll)
				{
					this.Pos = this.DataSource.MinPos;
				}

				int firstVisibleIndex = this.DataSource.FindFirstVisibleCellIndex();
				UITableViewCell cell = this.DataSource.CellForRow(firstVisibleIndex);
				this.TopCell = cell;
				this.BottomCell = cell;
				this.LoadCellFromBottom();
			}
		}
		this.CachedTrans.gameObject.SetActive(false);
	}


	public UITableViewCell FindCellInVisible (int index)
	{
		UITableViewCell cell = this.TopCell;
		while(cell != null)
		{
			if (cell.Data.Index == index)
			{
				return cell;
			}

			cell = cell.NextCell;
		}

		return null;
	}

	public UITableViewCell FindCellInVisible (int section, int row)
	{
		UITableViewCell cell = this.TopCell;
		while(cell != null)
		{
			if (cell.Data.Section == section && cell.Data.Row == row)
			{
				return cell;
			}

			cell = cell.NextCell;
		}

		return null;
	}

	public UITableViewCell FindHeaderCellInVisible (int section)
	{
		return this.FindCellInVisible(section, -1);
	}

	public UITableViewCell FindFooterCellInVisible (int section)
	{
		UITableViewCell cell = this.TopCell;
		while(cell != null)
		{
			if (cell.Data.Section == section && cell.Data.IsFooter)
			{
				return cell;
			}

			cell = cell.NextCell;
		}

		return null;
	}

	public UITableViewCell FindCellInCache (string identifier)
	{
		UITableViewCell cell = null;
		for (int i = 0, max = this.CachedCellList.Count; i < max; ++i)
		{
			if (this.CachedCellList[i].Identifier == identifier)
			{
				cell = this.CachedCellList[i];
				this.CachedCellList.RemoveAt(i);
				break;
			}
		}
		return cell;
	}


	public void Move (float delta)
	{
		if (!this.DataSource.CanScroll)
		{
			this.Pos = this.DataSource.MinPos;
			return;
		}

		if (this.TopCell == null || this.BottomCell == null)
		{
			#if CXDebug
			//Debug.LogError("Error, Auto Reload Data");
			this.Viewport.CancelTouch();
			m_OnSmoothMove = false;
			#endif
			this.ReloadData();
		}
		else
		{
			if ( delta > 0f )
			{
				this.RemoveUnVisibleCellFromTop();
				//delta = Mathf.Min(delta, this.Viewport.MaxFrameMove);
				this.MoveUp(delta);
				this.UpdateScrollBarValue();
			}
			else if ( delta < 0f )
			{
				this.RemoveUnVisibleCellFromBottom();
				//delta = Mathf.Max(delta, -this.Viewport.MaxFrameMove);
				this.MoveDown(delta);
				this.UpdateScrollBarValue();
			}
		}
	}

	private void MoveUp (float delta)
	{
		if (this.BottomCell == null)
		{
			#if UNITY_EDITOR
			Debug.LogError( "BottomCell null" );
			#endif
		}
		else
		{
			if ( delta != 0f )
			{

				if (this.m_OnSmoothMove)// 平滑移动
				{
					//  上滑上越界， 控制上弹速度
					if (this.DataSource.OnCrossTop)
					{
						float distance = this.DataSource.DistanceOfTopToViewportTop;
						delta = distance * Time.deltaTime * 5f;
					}
					// 上滑下越界，衰减上滑速度， 并做反向
					else if (this.DataSource.OnCrossBottom)
					{
						delta -=  delta * Mathf.Min(0.99f, Time.deltaTime * 28f);
						if (delta < 1f)
						{
							delta = -1f;
						}
					}
					else// 未越界, 做预防处理
					{
						//delta = Mathf.Min(delta, this.BottomCell.MinMoveUpDelta);
					}
				}

				else// 触摸向上移动
				{
					if (this.DataSource.OnCrossBottomEdge)// 达到最大临界释放值, 释放触摸
					{
						this.Viewport.CancelTouch();
					}
					else if (this.DataSource.OnCrossBottom)// 越界衰减触摸值
					{
						// 回弹力：下弹
						delta *= NoCellMoveRate;
					}
				}

				this.CellTrans.transform.Translate( 0f, delta, 0f );
				this.LoadCellFromBottom();
			}

		}
	}

	private void MoveDown (float delta)
	{
		if (this.TopCell == null)
		{
			#if UNITY_EDITOR
			Debug.LogError( "TopCell null" );
			#endif
		}
		else
		{
			if ( delta != 0f )
			{
				if (this.m_OnSmoothMove)// 平滑移动
				{
					if (this.DataSource.OnCrossTop)// 下滑上越界， 衰减速度并反向
					{
						delta -=  delta * Mathf.Min(0.99f, Time.deltaTime * 28f);
						if (delta > -1f)
						{
							delta = 1f;
						}
					}
					else if (this.DataSource.OnCrossBottom)// 下滑下越界， 控制速度
					{
						float distance = this.DataSource.DistanceOfBottomToViewportBottom;
						delta = -distance * Time.deltaTime * 5f;
					}
					else// 未越界，做预防处理
					{
						//delta = Mathf.Max(delta, this.TopCell.MaxMoveDownDelta);
					}
				}

				else// 触摸下移
				{
					if (this.DataSource.OnCrossTopEdge)// 临界释放值
					{
						this.Viewport.CancelTouch();
					}
					else if (this.DataSource.OnCrossTop)// 越界衰减
					{
						delta *= NoCellMoveRate;
					}
				}

				this.CellTrans.transform.Translate( 0f, delta, 0f );
				this.LoadCellFromTop();
			}
		
		}
	}



	private void LoadCellFromTop ()
	{
		if (this.TopCell.Data.IsFirstCell || !this.TopCell.NeedLoadTopCell)
		{
			this.TopCell.LastCell = null;
			return;
		}

		UITableViewCell lastCell = this.DataSource.CellForRow(this.TopCell.Data.Index - 1);
		lastCell.NextCell = this.TopCell;
		this.TopCell.LastCell = lastCell;
		this.TopCell = lastCell;

		this.LoadCellFromTop();
	}

	private void LoadCellFromBottom ()
	{
		if (this.BottomCell.Data.IsLastCell || !this.BottomCell.NeedLoadBottomCell)
		{
			this.BottomCell.NextCell = null;
			return;
		}

		UITableViewCell nextCell = this.DataSource.CellForRow(this.BottomCell.Data.Index + 1);
		nextCell.LastCell = this.BottomCell;
		this.BottomCell.NextCell = nextCell;
		this.BottomCell = nextCell;

		this.LoadCellFromBottom();
	}
	private void RemoveCellToCache (UITableViewCell cell)
	{
		if (cell == null)
		{
			return;
		}
		if (!this.CachedCellList.Contains(cell))
		{
			cell.transform.parent = this.CachedTrans;
			this.CachedCellList.Add(cell);
		}
	}

	private void RemoveUnVisibleCellFromTop ()
	{
		if (this.TopCell != null && !this.TopCell.Data.Visible)
		{
			this.RemoveCellToCache(this.TopCell);
			this.TopCell = this.TopCell.NextCell;

			if (this.TopCell == null)
			{
				#if UNITY_EDITOR
				Debug.LogError("TopCell null");
				#endif
			}
			else
			{
				this.TopCell.LastCell = null;
			}

			this.RemoveUnVisibleCellFromTop();
		}
	}

	private void RemoveUnVisibleCellFromBottom ()
	{
		if (this.BottomCell != null && !this.BottomCell.Data.Visible)
		{
			this.RemoveCellToCache(this.BottomCell);
			this.BottomCell = this.BottomCell.LastCell;

			if (this.BottomCell == null)
			{
				#if UNITY_EDITOR
				Debug.LogError("BottomCell null");
				#endif
			}
			else
			{
				this.BottomCell.NextCell = null;
			}

			this.RemoveUnVisibleCellFromBottom();
		}
	}

	private void RemoveAllVisibleCell()
	{
		UITableViewCell cell = this.TopCell;
		while (cell != null)
		{
			this.RemoveCellToCache(cell);
			cell = cell.NextCell;
		}

		this.TopCell = null;
		this.BottomCell = null;
	}

	private void UpdateScrollBarAlpha ()
	{
		if (ScrollBar != null)
		{
			if (this.ScrollBarAlphaLight)
			{
				if (ScrollBar.Alpha < 1f)
				{
					ScrollBar.Alpha += Time.deltaTime * 3f;
				}
				else
				{
					this.ScrollBarAlphaLight = false;
				}
			}
			else if (this.ScrollBarAlphaDark)
			{
				if (ScrollBar.Alpha > 0f)
				{
					ScrollBar.Alpha -= Time.deltaTime * 3f;
				}
				else
				{
					this.ScrollBarAlphaDark = false;
				}
			}
		}

	}

	private void UpdateScrollBarValue ()
	{
		if (ScrollBar == null)
		{
			return;
		}

		if (!this.DataSource.ShowScrollBar)// 当总高度低于视口高度
		{
			return;
		}

		if (ScrollBar.Alpha < 1f)
		{
			this.ScrollBarAlphaLight = true;
		}

		// center lenght
		float centerLenght = this.Viewport.Height;

		float distanceOfTop = this.DataSource.DistanceOfTopToViewportTop;
		if (distanceOfTop > 0f)
		{
			centerLenght -= distanceOfTop;
		}
		else
		{
			float distanceOfBottom = this.DataSource.DistanceOfBottomToViewportBottom;
			if (distanceOfBottom > 0)
			{
				centerLenght -= distanceOfBottom;
			}
		}

		// size
		float size = centerLenght / this.DataSource.TotalHeight;

		// value
		float value = Mathf.Max(0f, -distanceOfTop) / (this.DataSource.TotalHeight - this.Viewport.Height);

		ScrollBar.Value = value;
		ScrollBar.BarSize = size;
	}

}