﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTransformFrameAni :  CXTransformValueCurve
{
	private CX.ImageVE Sprite;
	private string Prefix;
	private int StartIndex = 1;
	private int EndIndex;
	private float OneFrameTime;
	private float CurrentTime;
	private int CurrentFrameIndex;
	public float Scale = 1f;
	public bool Loop;

	// 序列帧序号从1开始，结束于endIndex
	public void Init (CX.ImageVE sprite, string prefix, int endIndex, float scale, float duration)
	{
		this.Init(sprite, prefix, 1, endIndex, scale, duration);
	}

	public void Init (CX.ImageVE sprite, string prefix, int startIndex, int endIndex, float scale, float duration)
	{
		this.AttachObject = sprite;
		this.Sprite = sprite;
		this.Prefix = prefix;
		this.StartIndex = startIndex;
		this.EndIndex = endIndex;
		this.Scale = scale;

		this.OneFrameTime = duration/ (endIndex - StartIndex + 1);
		this.CurrentFrameIndex = this.StartIndex;
		this.CurrentTime = 0f;
		this.SetFrame();
	}

	public override void MakeItLifeEnd ()
	{
		this.Alive = false;
		this.CallBack();
	}

	public override void TimeStep (float timeStep)
	{
		this.CurrentTime += timeStep;
		if ( this.CurrentTime >= this.OneFrameTime )
		{
			this.CurrentTime = 0f;
			this.NextFrame();
		}
	}


	private void NextFrame ()
	{
		++this.CurrentFrameIndex;
		if ( this.CurrentFrameIndex > this.EndIndex )
		{
			if (this.Loop)
			{
				this.CurrentFrameIndex = this.StartIndex;
			}
			else
			{
				//this.Sprite.spriteName = null;// 结束时不再显示。
				this.MakeItLifeEnd();
				return;
			}
		}

		this.SetFrame();
	}

	private void SetFrame ()
	{
		//this.Sprite.spriteName = this.Prefix + this.CurrentFrameIndex;
		//this.Sprite.MakePixelPerfect();
		if (this.Scale != 1f)
		{
			this.Sprite.transform.localScale = new Vector3(Scale, Scale, 1f);
		}
	}
}