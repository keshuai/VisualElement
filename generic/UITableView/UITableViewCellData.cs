﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*****************************************************
 * 	UITableViewCellData
 * 
 * ***************************************************/

public class UITableViewCellData
{
	private UITableViewDataSource m_DataSource;
	private int m_Section;
	private int m_Row;
	private int m_Index;
	private float m_LocalPos;
	private float m_Height;
	private float m_TopParting;
	private float m_BottomParting;

	public UITableViewCellData (UITableViewDataSource dataSource, int section, int row, int index, float localPos, float height, float topParting, float bottomParting)
	{
		m_DataSource = dataSource;
		m_Section = section;
		m_Row = row;
		m_Index = index;
		m_LocalPos = localPos;
		m_Height = height;
		m_TopParting = topParting;
		m_BottomParting = bottomParting;
	}

	public UITableViewDataSource DataSource { get {return m_DataSource;} } 
	public int Index {get {return m_Index;}}
	public int Section {get {return m_Section;}}
	public int Row {get {return m_Row;}}
	public float LocalPos{get {return m_LocalPos;}}
	public float Height {get {return m_Height;}}
	public float LocalTop{ get {return m_LocalPos + m_Height * 0.5f;} }
	public float LocalBottom{ get {return m_LocalPos - m_Height * 0.5f;} }
	public float ViewTop { get { return this.LocalTop + this.DataSource.TableView.CellTrans.transform.localPosition.y; } }
	public float ViewBottom { get { return this.LocalBottom + this.DataSource.TableView.CellTrans.transform.localPosition.y; } }
	public float TopParting {get {return m_TopParting;}}
	public float BottomParting{get {return m_BottomParting;}}
	public int NumberOfRows { get{return m_DataSource.NumberOfRowsInSection(m_Section);} }
	public bool IsHeader { get {return m_Row == -1;}}
	public bool IsFooter { get {return m_Row == this.NumberOfRows;} }
	public bool IsFirstCell { get {return m_Index == 0;} }
	public bool IsLastCell { get {return m_Index == m_DataSource.NumberOfTotalCells - 1;} }
	public bool Visible {  get {  return !(this.ViewTop < this.DataSource.TableView.Viewport.Bottom || this.ViewBottom > this.DataSource.TableView.Viewport.Top);} }

}
