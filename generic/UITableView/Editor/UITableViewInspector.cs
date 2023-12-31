﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UITableView))]
public class UITableViewInspector : Editor 
{
	UITableView _this;

	void Awake ()
	{
		_this = (UITableView)this.target;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUILayout.Button("Reload Data", GUILayout.Height(40f)))
		{
			_this.ReloadData();
		}

		// data source
		GUILayout.Space(10f);
		EditorGUILayout.LabelField("---Data Source---");
		EditorGUILayout.IntField("Cells Count", _this.DataSource.NumberOfTotalCells);
		EditorGUILayout.Toggle("OnSmoothMove", _this.OnSmoothMove);
		EditorGUILayout.Toggle("OnCross", _this.DataSource.OnCross);
		EditorGUILayout.LabelField("Pos: " + _this.CellTrans.localPosition.y + "(" +  _this.DataSource.MinPos + " => " + _this.DataSource.MaxPos + ")");
		EditorGUILayout.LabelField("Total Height: " + _this.DataSource.TotalHeight);
		EditorGUILayout.LabelField("Distance To Top: " + _this.DataSource.DistanceOfTopToViewportTop);
		EditorGUILayout.LabelField("Distance To Bottom: " + _this.DataSource.DistanceOfBottomToViewportBottom);
		EditorGUILayout.LabelField("OnCrossEdgeValue: " + _this.DataSource.OnCrossEdgeValue);
	}
}