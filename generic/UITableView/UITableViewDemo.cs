﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/


#if UNITY_EDITOR
/********************************************************
 * UITableView Demo 编译成程序时，此段代码会被剔除
 * 
 * ******************************************************/

using UnityEditor.Connect;
using UnityEngine;

public class UITableViewDemo : MonoBehaviour 
{
	// Demo
	// ==> 将下面三个参数设为该工程拥有的资源 ==> ==> ==> ==> ==> ==> ==> ==> ==> 
	//

	/// NGUI Atlas Resources 路径
	//public const string __DemoAtlasPath = "Atlas/UI";
	/// TTF Font Resources 路径
	//public const string __DemoFontPath = "Font/FZDHTJW";
	/// NGUI Sliced sprite 名称
	//public const string __DemoSlicedSpriteName = "Dark";

	//public static readonly UIAtlas __DemoAtlas = Resources.Load<UIAtlas>(__DemoAtlasPath);
	//public static readonly Font __DemoFont = Resources.Load<Font>(__DemoFontPath);


	// <==  <==  <==  <==  <==  <==  <==  <==  <==  <==  <==  <==  <==  

	public UITableView TableView;
	//public UIAtlas Atlas;
	void Awake ()
	{
		//Atlas = AtlasRes.Main;

		// 创建 TableView
		//this.TableView = ComponentCreator.Create<UITableView>(this.transform);
		GameObject o = new GameObject("UITableView");
		o.transform.SetParent(this.transform, false);
		this.TableView = o.AddComponent<UITableView>();
		
		this.TableView.transform.localPosition = Vector3.zero;// tableView的中心位置
		this.TableView.Viewport.Size = new Vector2(500f, 500f);// tableView的视口大小

		// data source  数据代理
		// 三个必须==> 每行加载的cell，每行的高度， 每段有多少行(大于0行)
		UITableViewDataSource dataSource = new UITableViewDataSource(this.TableView, this.CellForRow, this.HeightForRow, this.NumberOfRowsInSection);
		this.TableView.DataSource = dataSource;
		// 段落个数, 不设置时为1
		dataSource.DelegateOfNumberOfSections = this.NumberOfSections;
		// 行间距，不设置时为0
		dataSource.DelegateOfIntervalForRow = this.IntervalForRow;
		// 段落间距，不设置时为0
		dataSource.DelegateOfIntervalForSection = this.IntervalForSection;
		// 标头cell，不设置时表示没有, (设置后不能返回null, 可高度为0)
		dataSource.DelegateOfHeaderCellForSection = this.CellForHeader;
		// 尾部cell，不设置时表示没有, (设置后不能返回null, 可高度为0)
		dataSource.DelegateOfHeightForHeader = this.HeightForHeader;
		// 单击事件, 不设置不触发
		dataSource.DelegateOfCellTouchUpInside = this.CellTouchUpInside;


		// Scroll Bar 滚动条
		this.TableView.CreateScrollBar();// 创建内置滚动条
		Color fc = new Color(31f/255f, 167f/255f, 173f/255f, 0f);//滚动条前景色
		Color bc = new Color(255f/255f, 88f/255f, 88f/255f, 0f);//滚动条背景色
		this.TableView.SetScrollBar(fc, bc);//滚动条的图片设置，sliced 类型//this.TableView.SetScrollBar(__DemoAtlas, __DemoSlicedSpriteName, __DemoSlicedSpriteName, fc, bc);//滚动条的图片设置，sliced 类型
		this.TableView.ScrollBarDepth = 5;//滚动条depth设置，保持在显示在cell前面

		// 点击颜色变化
		this.TableView.EnableTouchDownColor = true;// 开启按下颜色变化
		this.TableView.TouchDownColor = new Color(0f, 0f, 1f, 0.3f);// 按下颜色
		this.TableView.TouchDownSpriteDepth = 5;// 颜色sprite depth 保持在cell前面

		// 重载数据
		float savedPos = 1000f;// read from your saved
		this.TableView.Pos = savedPos;
		this.TableView.ReloadData();
	}

	// cell 被点击回调代理
	public void CellTouchUpInside (UITableViewCell cell)
	{
		UITableViewCellData data = cell.Data;
		Debug.Log( "demo ==> CellTouchUpInside, index: " + data.Index + ", section: " + data.Section + ", row: " + data.Row );
	}

	// cell头部高度
	public float HeightForHeader (UITableView tableView, int section)
	{
		return 50f;
	}

	// cell row高度
	public float HeightForRow (UITableView tableView, int section, int row)
	{
		if (section == 0)
		{
			return 40f;
		}
		if (section == 1)
		{
			return 90f;
		}
		return 100f;
	}

	// 段落内部 行间距
	public float IntervalForRow (UITableView tableView, int section)
	{
		if (section == 0)
		{
			return 10f;
		}

		if (section == 1)
		{
			return 25;
		}

		return 30f;
	}

	// 段落 间距
	public float IntervalForSection (UITableView tableView)
	{
		return 100f;
	}

	// 段落数
	public int NumberOfSections (UITableView tableView)
	{
		return 3;
	}

	// 每个段落的行数
	public int NumberOfRowsInSection (UITableView tableView,int section)
	{
		if ( section == 0 )
		{
			return 20;
		}
		else if (section == 1)
		{
			return 30;
		}

		return 50;
	}

	// 段落内的cell加载
	public UITableViewCell CellForRow (UITableView tableView, int section, int row)
	{
		// 分段不同类型的cell分开写， identifier 标识为不同
		// 如果不同分段但类型一样，只是尺寸不同，便写在一起， 使用同一个  identifier 标识
		if (section == 0)
		{
			return CellForRowSection0(tableView, row);
		}

		if (section == 1)
		{
			return CellForRowSection1(tableView, row);
		}


		return CellForRowSection2(tableView, row);
	}

	// 段落头加载
	public UITableViewCell CellForHeader (UITableView tableView, int section)
	{
		string identifier = "demoHeader";// 头部本身也是一个cell对象
		UITableViewDemoCell cell = (UITableViewDemoCell)tableView.FindCellInCache(identifier);
		if (cell == null)
		{
			//	cell = ComponentCreator.Create<UITableViewDemoCell>();
			GameObject o = new GameObject();
			cell = o.AddComponent<UITableViewDemoCell>();
			cell.Identifier = identifier;
		}
		cell.Sprite.Size = new Vector2(300, 40);//20 * (row + 1));
		cell.Label.Text = "section: " + section + ", header";
		cell.Sprite.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		cell.Label.color = Color.black;
		return cell;
	}


	public UITableViewCell CellForRowSection0 (UITableView tableView, int row)
	{
		string identifier = "demoCell0";
		// 先从缓冲里查找cell对象
		UITableViewDemoCell cell = (UITableViewDemoCell)tableView.FindCellInCache(identifier);
		if (cell == null)//没有则创建,并赋予 identifier
		{
			//cell = ComponentCreator.Create<UITableViewDemoCell>();
			GameObject o = new GameObject();
			cell = o.AddComponent<UITableViewDemoCell>();
			cell.Identifier = identifier;
		}

		//  设置cell的特定属性
		cell.Sprite.Size = new Vector2(500, 50);//20 * (row + 1));
		cell.Label.Text = "section: 0, row: " + row;
		cell.Sprite.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		cell.Label.color = Color.black;
		return cell;
	}

	public UITableViewCell CellForRowSection1 (UITableView tableView, int row)
	{
		string identifier = "demoCell1";
		UITableViewDemoCell cell = (UITableViewDemoCell)tableView.FindCellInCache(identifier);
		if (cell == null)
		{
			//cell = ComponentCreator.Create<UITableViewDemoCell>();
			GameObject o = new GameObject();
			cell = o.AddComponent<UITableViewDemoCell>();
			cell.Identifier = identifier;
		}

		cell.Sprite.Size = new Vector2(500, 100);
		cell.Label.Text = "section: 1, row: " + row;
		cell.Sprite.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		cell.Label.color = Color.black;
		return cell;
	}

	public UITableViewCell CellForRowSection2 (UITableView tableView, int row)
	{
		string identifier = "demoCell2";
		UITableViewDemoCell cell = (UITableViewDemoCell)tableView.FindCellInCache(identifier);
		if (cell == null)
		{
			//cell = ComponentCreator.Create<UITableViewDemoCell>();
			GameObject o = new GameObject();
			cell = o.AddComponent<UITableViewDemoCell>();
			cell.Identifier = identifier;
		}

		cell.Sprite.Size = new Vector2(500, 100);
		cell.Label.Text = "section: 2, row: " + row;
		cell.Sprite.Color = new Color(0.8f, 0.8f, 0.8f, 1f);
		cell.Label.color = Color.black;
		return cell;
	}
}

#endif