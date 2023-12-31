﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/*****************************************************
 * UITableView DataSource 与 UITableView 配合使用
 * 用于数据代理计算和与代理沟通
 * ***************************************************/


public class UITableViewDataSource
{
	//--------------
	// 必须
	//--------------

	// cell
	private Func<UITableView, int, int, UITableViewCell> DelegateOfCellForRow;
	// number of rows
	private Func<UITableView, int, int> DelegateOfNumberOfRows;
	// height for row
	private Func<UITableView, int, int, float> DelegateOfHeightForRow;

	//--------------
	// 可选
	//--------------

	// 几个段落
	public Func<UITableView, int> DelegateOfNumberOfSections;

	// 高度
	public Func<UITableView, int, float> DelegateOfHeightForHeader;
	public Func<UITableView, int, float> DelegateOfHeightForFooter;

	// 间距
	public Func<UITableView, float> DelegateOfIntervalForSection;
	public Func<UITableView, int, float> DelegateOfIntervalForRow;

	// 头尾
	public Func<UITableView, int, UITableViewCell> DelegateOfHeaderCellForSection;
	public Func<UITableView, int, UITableViewCell> DelegateOfFooterCellForSection;

	// 点击事件
	public Action<UITableViewCell> DelegateOfCellTouchUpInside;
	public Action<UITableViewCell> DelegateOfCellTouchBegin;
	public Action<UITableViewCell> DelegateOfCellTouchUp;


	// 上下空白区域
	public float TopSpace = 0f;
	public float BottomSpace = 0f;

	// 
	public bool HasHeader { get { return DelegateOfHeaderCellForSection != null; } }
	public bool HasFooter { get { return DelegateOfFooterCellForSection != null; } }

	public bool HasCellTouchUpInside { get { return DelegateOfCellTouchUpInside != null; } }
	public bool HasCellTouchBegin { get { return DelegateOfCellTouchBegin != null; } }
	public bool HasCellTouchUp { get { return DelegateOfCellTouchUp != null; } }

	
	private UITableView m_TableView;
	private float m_TotalHeight;
	private float m_MinPos;
	private float m_MaxPos;
	private UITableViewCellData[] m_CellDataArray;

	public UITableView TableView { get {return m_TableView;} }
	public float TotalHeight { get { return m_TotalHeight; } }
	public int NumberOfTotalCells { get {return m_CellDataArray.Length;} }

	public float IntervalForSection { get { return DelegateOfIntervalForSection == null? 0f : DelegateOfIntervalForSection(this.TableView); }}
	public int NumberOfSections { get{return DelegateOfNumberOfSections == null? 1 : DelegateOfNumberOfSections (this.TableView);}}

	public float MinPos { get { return m_MinPos; } }
	public float MaxPos { get { return m_MaxPos; } }
	
	/*顶部越界距离*/public float DistanceOfTopToViewportTop { get { return m_MinPos - m_TableView.CellTrans.localPosition.y; }}
	/*底部越界距离*/public float DistanceOfBottomToViewportBottom { get { return m_TableView.CellTrans.localPosition.y - m_MaxPos;}}


	/*边缘越界极限值*/public float OnCrossEdgeValue;
	/*上边缘越界极限*/public bool OnCrossTopEdge { get { return this.DistanceOfTopToViewportTop > OnCrossEdgeValue; } }
	/*下边缘越界极限*/public bool OnCrossBottomEdge { get { return this.DistanceOfBottomToViewportBottom > OnCrossEdgeValue; } }

	/*顶部越界*/public bool OnCrossTop { get { return m_TableView.CellTrans.localPosition.y < m_MinPos; } }
	/*底部越界*/public bool OnCrossBottom { get { return m_TableView.CellTrans.localPosition.y > m_MaxPos; } }
	/*越界*/public bool OnCross { get { return this.OnCrossTop || this.OnCrossBottom; } }


	public bool ShowScrollBar { get { return this.TotalHeight > m_TableView.Viewport.Height; } }

	public bool CanScroll { get { return m_CanScroll; } }
	private bool m_CanScroll;

	// data source create function, input can not be null
	public UITableViewDataSource ( UITableView tableView, Func<UITableView, int, int, UITableViewCell> cellForRow, Func<UITableView, int, int, float> heightForRow, Func<UITableView, int, int> numberOfRowsInSection)
	{
		m_TableView = tableView;
		DelegateOfCellForRow = cellForRow;
		DelegateOfNumberOfRows = numberOfRowsInSection;
		DelegateOfHeightForRow = heightForRow;
	}

	// load data
	public void ReloadData ()
	{
		List<UITableViewCellData> cellDataList = new List<UITableViewCellData>();

		int numberSections = this.NumberOfSections;

		float viewportHalfHeight = this.TableView.Viewport.Height * 0.5f;

		float totalHeight = viewportHalfHeight;
		float cellHeight = 0f;
		float cellPos = 0f;
		float topParting = 0f;
		float bottomParting = 0f;

		float intervalOfSections = this.IntervalForSection;

		int index = 0;
		for (int section = 0; section < numberSections; ++section)
		{
			if (this.HasHeader)
			{
				if (section == 0)
				{
					topParting = totalHeight;
				}
				else
				{
					topParting = totalHeight + intervalOfSections;
				}

				cellHeight = this.HeightForHeader(section);
				cellPos = totalHeight - cellHeight * 0.5f;
				totalHeight -= cellHeight;

				bottomParting = totalHeight;

				cellDataList.Add(new UITableViewCellData(this, section, -1, index++, cellPos, cellHeight, topParting, bottomParting ));
			}

			int numberOfRows = this.NumberOfRowsInSection(section);
			float intervalOfRows = this.IntervalForRow(section);
			for ( int row = 0; row < numberOfRows; ++row )
			{
				if (row == 0)
				{
					if (this.HasHeader)
					{
						topParting = totalHeight;
					}
					else
					{
						topParting = totalHeight + intervalOfSections;
					}
				}
				else
				{
					topParting = totalHeight + intervalOfRows;
				}

				cellHeight = this.HeightForRow(section, row);
				cellPos = totalHeight - cellHeight * 0.5f;
				totalHeight -= cellHeight;

				if (row == numberOfRows -1)
				{
					if (!this.HasFooter && section != numberSections -1)
					{
						totalHeight -= intervalOfSections;
					}
				}
				else
				{
					totalHeight -= intervalOfRows;
				}
				bottomParting = totalHeight;

				cellDataList.Add(new UITableViewCellData(this, section, row, index++, cellPos, cellHeight, topParting, bottomParting ));
			}

			if (this.HasFooter)
			{
				// 段落cell 的个数不为0
				topParting = totalHeight;

				cellHeight = this.HeightForFooter(section);
				cellPos = totalHeight - cellHeight * 0.5f;
				totalHeight -= cellHeight;

				if (section != numberSections -1)
				{
					// 不是最后一个，则下一个分割线需加上段落间距
					totalHeight -=  intervalOfSections;
				}
				bottomParting = totalHeight;

				cellDataList.Add(new UITableViewCellData(this, section, numberOfRows, index++, cellPos, cellHeight, topParting, bottomParting ));
			}
		}

		m_MinPos = -this.TopSpace;
		m_MaxPos = -viewportHalfHeight - totalHeight + this.BottomSpace;
		m_TotalHeight = viewportHalfHeight - totalHeight;
		m_CellDataArray = cellDataList.ToArray();

		// 最大位置
		if (m_MaxPos < 0f)
		{
			m_MaxPos = 0f;
			this.OnCrossEdgeValue = this.TotalHeight * 0.5f;
		}
		else
		{
			this.OnCrossEdgeValue = viewportHalfHeight * 2f * 0.9f;
		}

		// clamp pos
		float cellTransPos = this.ClampPos(m_TableView.CellTrans.localPosition.y);
		m_TableView.CellTrans.localPosition = new Vector3(0f, cellTransPos, 0f);

		if (m_TotalHeight <= (this.TableView.Viewport.Height - this.TopSpace - this.BottomSpace))
		{
			m_CanScroll = false;
		}
		else
		{
			m_CanScroll = true;
		}

		#if UNITY_EDITOR
		//Debug.Log( "一共加载 " + m_CellDataArray.Length + " 个cell数据, tablePos : 0~" + m_MaxPos );
		#endif
	}

	// 段落中有几行, 必须
	public int NumberOfRowsInSection (int section)
	{
		return DelegateOfNumberOfRows (this.TableView, section);
	}

	// 加载特定某行cell, 必须
	public UITableViewCell CellForRow (int index)
	{
		if (index < 0 || index >= this.NumberOfTotalCells)
		{
			return null;
		}

		UITableViewCellData data = m_CellDataArray[index];
		UITableViewCell cell = null;

		if (data.IsHeader)
		{
			if (DelegateOfHeaderCellForSection != null)
			{
				cell = DelegateOfHeaderCellForSection(this.TableView, data.Section);
				#if UNITY_EDITOR
				cell.name = data.Index + ". section: " + data.Section + ", row: " + data.Row + ", Header, identifier: " + cell.Identifier;
				#endif
			}
		}
		else if (data.IsFooter)
		{
			if (DelegateOfFooterCellForSection != null)
			{
				cell = DelegateOfFooterCellForSection(this.TableView, data.Section);
				#if UNITY_EDITOR
				cell.name = data.Index + ". section: " + data.Section + ", row: " + data.Row + ", Footer, identifier: " + cell.Identifier;
				#endif
			}
		}
		else
		{
			cell = DelegateOfCellForRow (this.TableView, data.Section, data.Row);
			#if UNITY_EDITOR
			cell.name = data.Index + ". section: " + data.Section + ", row: " + data.Row + ", identifier: " + cell.Identifier;
			#endif
		}

		if (cell != null)
		{
			cell.Data = data;
		}

		return cell;
	}

	public float HeightForHeader (int section)
	{
		return DelegateOfHeightForHeader == null? 0f : DelegateOfHeightForHeader(this.TableView, section);
	}
	public float HeightForFooter (int section)
	{
		return DelegateOfHeightForFooter == null? 0f : DelegateOfHeightForFooter(this.TableView, section);
	}
	public float HeightForRow (int section, int row)
	{
		return DelegateOfHeightForRow == null? 0f : DelegateOfHeightForRow(this.TableView, section, row);
	}

	public float IntervalForRow (int section)
	{
		return DelegateOfIntervalForRow == null? 0f : DelegateOfIntervalForRow(this.TableView, section);
	}

	private UITableViewCell m_TmpTouchBeginCell = null;
	/// Up, 同时通知点击事件 
	public void CellTouchUpInside (UITableViewCell cell)
	{
		UITableViewCell touchBeginCell = m_TmpTouchBeginCell;
		m_TmpTouchBeginCell = null;

		// must up
		if (touchBeginCell != null && DelegateOfCellTouchUp != null)
		{
			DelegateOfCellTouchUp(touchBeginCell);
		}

		// up inside
		if (cell != null && cell == touchBeginCell)
		{
			if (DelegateOfCellTouchUpInside != null)
			{
				DelegateOfCellTouchUpInside(touchBeginCell);
			}
		}
	}

	/// touch begin
	public void CellTouchBegin (UITableViewCell cell)
	{
		m_TmpTouchBeginCell = cell;

		if (DelegateOfCellTouchBegin != null && cell != null)
		{
			DelegateOfCellTouchBegin(cell);
		}
	}

	/// 仅仅Up，无点击  
	public void CellTouchUp ()
	{
		if (m_TmpTouchBeginCell != null && DelegateOfCellTouchUp != null)
		{
			DelegateOfCellTouchUp (m_TmpTouchBeginCell);
		}

		m_TmpTouchBeginCell = null;
	}

	public int FindFirstVisibleCellIndex ()
	{
		foreach (UITableViewCellData data in m_CellDataArray)
		{
			if (data.Visible)
			{
				return data.Index;
			}
		}
		return 0;
	}

	public float ClampPos (float pos)
	{
		return Mathf.Clamp(pos, this.MinPos, this.MaxPos);
	}


}
