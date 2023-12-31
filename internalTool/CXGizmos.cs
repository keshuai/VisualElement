﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXGizmos 
{
	public static void Draw2DCircle (Transform trans, float radius, float theta , Color color)
	{
		Draw2DCircle(trans, Vector2.zero, radius, theta, color);
	}

	public static void Draw2DCircle (Transform trans, Vector2 offset, float radius, float theta , Color color)
	{
		if (trans == null) return;
		if (theta < 0.0001f) theta = 0.0001f;

		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = trans.localToWorldMatrix;

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		// 绘制圆环
		Vector3 firstPoint = new Vector3(offset.x + radius, offset.y, 0);
		Vector3 prevPoint = firstPoint;
		Vector3 currentPoint = Vector3.zero;
		for (float currentTheta = theta; currentTheta < 2 * Mathf.PI; currentTheta += theta)
		{
			currentPoint = new Vector3(offset.x + radius * Mathf.Cos(currentTheta), offset.y + radius * Mathf.Sin(currentTheta), 0);
			Gizmos.DrawLine(prevPoint, currentPoint);
			prevPoint = currentPoint;
		}
		Gizmos.DrawLine(currentPoint, firstPoint);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw2DRect (Transform trans, float width, float height , Color color)
	{
		Draw2DRect(trans, Vector3.zero, width, height, color);
	}

	public static void Draw2DRect (Transform trans, Vector3 offset, float width, float height , Color color)
	{
		if (trans == null)
		{
			return;
		}

		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Color defaultColor = Gizmos.color;

		Gizmos.matrix = trans.localToWorldMatrix;
		Gizmos.color = color;
		Gizmos.DrawWireCube(offset, new Vector3(width, height, 0));

		Gizmos.color = defaultColor;
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw2DRect (Transform trans, Vector3 position, Vector3 rotation, float width, float height , Color color)
	{
		if (trans == null) return;

		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = trans.localToWorldMatrix;

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		float halfWidth = width * 0.5f;
		float halfHeight = height * 0.5f;
		// 绘制圆环
		Vector3 leftTop = new Vector3(-halfWidth,halfHeight);
		Vector3 rightTop = new Vector3(halfWidth,halfHeight);
		Vector3 leftBottom = new Vector3(-halfWidth,-halfHeight);
		Vector3 rightBottom = new Vector3(halfWidth,-halfHeight);

		Quaternion quaternion = Quaternion.Euler(rotation);
		leftTop = position + quaternion * leftTop;
		rightTop = position + quaternion * rightTop;
		leftBottom = position + quaternion * leftBottom;
		rightBottom = position + quaternion * rightBottom;

		// 绘制线段
		Gizmos.DrawLine(leftTop, rightTop);
		Gizmos.DrawLine(rightTop, rightBottom);
		Gizmos.DrawLine(rightBottom, leftBottom);
		Gizmos.DrawLine(leftBottom, leftTop);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw2DRect (Transform trans, Vector3 position, Vector3 rotation, Vector3 scale, float width, float height , Color color)
	{
		if (trans == null) return;

		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = trans.localToWorldMatrix * Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		float halfWidth = width * 0.5f;
		float halfHeight = height * 0.5f;
		// 绘制圆环
		Vector3 leftTop = new Vector3(-halfWidth,halfHeight);
		Vector3 rightTop = new Vector3(halfWidth,halfHeight);
		Vector3 leftBottom = new Vector3(-halfWidth,-halfHeight);
		Vector3 rightBottom = new Vector3(halfWidth,-halfHeight);

		// 绘制线段
		Gizmos.DrawLine(leftTop, rightTop);
		Gizmos.DrawLine(rightTop, rightBottom);
		Gizmos.DrawLine(rightBottom, leftBottom);
		Gizmos.DrawLine(leftBottom, leftTop);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw2DRect(Matrix4x4 matrix, float width, float height)
	{
		Color color = Color.yellow;
		Draw2DRect(matrix, width, height, color);
	}

	public static void Draw2DRect (Matrix4x4 matrix, float width, float height , Color color)
	{
		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = matrix;

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		float halfWidth = width * 0.5f;
		float halfHeight = height * 0.5f;
		// 绘制圆环
		Vector3 leftTop = new Vector3(-halfWidth,halfHeight);
		Vector3 rightTop = new Vector3(halfWidth,halfHeight);
		Vector3 leftBottom = new Vector3(-halfWidth,-halfHeight);
		Vector3 rightBottom = new Vector3(halfWidth,-halfHeight);

		// 绘制线段
		Gizmos.DrawLine(leftTop, rightTop);
		Gizmos.DrawLine(rightTop, rightBottom);
		Gizmos.DrawLine(rightBottom, leftBottom);
		Gizmos.DrawLine(leftBottom, leftTop);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw2DRect(Matrix4x4 matrix, float width, float height, float scale, Vector2 pivot)
	{
		Color color = Color.yellow;
		Draw2DRect(matrix, width, height, scale, pivot, color);
	}

	public static void Draw2DRect (Matrix4x4 matrix, float width, float height , float scale, Vector2 pivot, Color color)
	{
		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = matrix;

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		scale *= 0.5f;
		float xMin = (-1f - pivot.x) * scale * width;
		float xMax = ( 1f - pivot.x) * scale * width;
		float yMin = (-1f - pivot.y) * scale * height;
		float yMax = ( 1f - pivot.y) * scale * height;

		// 绘制圆环
		Vector3 topLeft = new Vector3(xMin, yMax);
		Vector3 topRight = new Vector3(xMax, yMax);
		Vector3 bottomLeft = new Vector3(xMin, yMin);
		Vector3 bottomRight = new Vector3(xMax, yMin);

		// 绘制线段
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}

	public static void Draw4PointRect (Transform trans, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
	{
		if (trans == null) return;

		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = trans.localToWorldMatrix;

		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = color;

		// 绘制线段
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);

		// 恢复默认颜色
		Gizmos.color = defaultColor;

		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}
}