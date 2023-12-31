﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System;

public class UITableViewCell : MonoBehaviour
{
	private UITableViewCellData m_Data;
	// 标志符
	public string Identifier;
	public UITableView TableView;
	public UITableViewCell LastCell;
	public UITableViewCell NextCell;

	public UITableViewCellData Data
	{
		get { return m_Data; }
		set
		{
			m_Data = value;
			this.TableView = value.DataSource.TableView;
			this.transform.parent = this.TableView.CellTrans;
			this.transform.localPosition = new Vector3(0f, value.LocalPos, 0f);
		}
	}

	public float WorldTop { get { return this.transform.position.y + this.Data.Height * 0.5f; } }
	public float WorldBottom { get { return this.transform.position.y - this.Data.Height * 0.5f; } }


	public float ViewPortCenter 
	{ 
		get { return this.Data.LocalPos + this.transform.parent.localPosition.y; } 
	}
	public float ViewPortTop 
	{ 
		get { return this.ViewPortCenter + this.Data.Height * 0.5f; } 
	}
	public float ViewPortBottom 
	{ 
		get { return this.ViewPortCenter - this.Data.Height * 0.5f; } 
	}

	public float ViewPortTopParting
	{
		get { return this.Data.TopParting + this.transform.parent.localPosition.y; }
	}

	public float ViewPortBottomParting
	{
		get { return this.Data.BottomParting + this.transform.parent.localPosition.y; }
	}
	

	// 顶部与视口顶部的距离，大于零表示有距离
	public float DistanceToViewportTop
	{
		get
		{
			return this.TableView.Viewport.Top - this.ViewPortTop;
		}
	}

	public float MinMoveUpDelta
	{
		get
		{
			return  Mathf.Max(0.1f, (this.TableView.Viewport.MaxCross - DistanceToViewportBottom));
		}
	}

	public float MaxMoveDownDelta
	{
		get
		{
			return Mathf.Min(-0.1f, DistanceToViewportTop - this.TableView.Viewport.MaxCross);
		}
	}


	// 底部与视口底部的距离，大于零表示有距离
	public float DistanceToViewportBottom
	{
		get
		{
			return this.ViewPortBottom - this.TableView.Viewport.Bottom;
		}
	}
	

	public bool NeedLoadTopCell
	{
		get
		{
			// 顶部低于视口顶部时需要加载上一个cell
			return this.ViewPortTopParting < this.TableView.Viewport.Top;
		}
	}


	public bool NeedLoadBottomCell
	{
		get
		{
			return this.ViewPortBottomParting > this.TableView.Viewport.Bottom;
		}
	}

	public bool ContainsPos (float pos)
	{
		return pos <= this.WorldTop && pos >= this.WorldBottom;
	}


	#if UNITY_EDITOR
	// Draw Gizmos
	void OnDrawGizmos ()
	{
		bool selected = UnityEditor.Selection.activeGameObject == gameObject;
		if  (selected)
		{
			Color t = Gizmos.color;
			Gizmos.color = new Color ( 0f, 0f, 1f, 1f );
			Vector2 center = this.transform.position;
			Vector2 size = new Vector2(this.TableView.Viewport.Width, this.Data.Height);
			Gizmos.DrawWireCube( center, size );
			Gizmos.color = t;
		}
	}
	#endif
}
