﻿/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXAnimationParticleSystemCurve : CXAnimationCurve 
{
	public ParticleSystem Target;

	private Material m_DynamicMat;
	public int RenderQueue = 4000;
	public CX.ImageAtlas Atlas;
	public string SpriteName;
	public Shader DynamicMatShader;
	public float ClearDynamicMatDelay;

	public override object Ani_Target { get { if (!this.Target) { this.Target = null; } return this.Target; } }
	public override bool Ani_Loop { get { return this.Loop; } }
	public override float Ani_Duration { get { return this.Duration; } }

	public float StartDelay 
	{ 
		get 
		{
			return this.Target == null ? 0f : this.Target.startDelay;
		}
		set
		{
			if (this.Target != null) this.Target.startDelay = value;
		}
	}
	public float Duration
	{
		get
		{
			return this.Target == null ? 0f : this.Target.duration;
		}
	}
	public bool Loop
	{
		get
		{
			return this.Target == null ? false : this.Target.loop;
		}
		set
		{
			if (this.Target != null) this.Target.loop = value;
		}
	}

	public bool UseNguiAtlas
	{
		get;
		set;
	}
		

	public void RebuildDynamicMat ()
	{
		if (m_DynamicMat != null)
		{
			UnityEngine.Object.Destroy(m_DynamicMat);
			m_DynamicMat = null;
		}

		if (this.Atlas == null || string.IsNullOrEmpty(this.SpriteName))
		{
			return;
		}

		if (this.DynamicMatShader == null)
		{
			///if (this.Atlas.spriteMaterial != null && this.Atlas.spriteMaterial.shader != null)
			//{
			//	this.DynamicMatShader = this.Atlas.spriteMaterial.shader;
			//}
			//else
			//{
			//	return;
			//}
		}

		//m_DynamicMat = UIAtlasExtend.CreateMeterial(this.Atlas, this.SpriteName, this.DynamicMatShader);
		//m_DynamicMat.renderQueue = this.RenderQueue;


		if (this.Target != null)
		{
			this.Target.GetComponent<Renderer>().sharedMaterial = m_DynamicMat;
		}
	}

	public override void Play ()
	{
		if (this.Target != null)
		{
			if (this.StartDelay > 0f)
			{
				CXTransformValueCenter.Instance.DelayCall(this._Play, this.StartDelay);
			}
			else
			{
				this._Play();
			}
		}
	}



	private void _Play ()
	{
		this.Target.Play();
	}

	public override void Stop ()
	{
		if (Target != null)
		{
			this.Target.Stop();
		}
	}

	void OnDestroy ()
	{
		this.ClearDynamicMat();
	}

	private void ClearDynamicMat ()
	{
		if (m_DynamicMat != null)
		{
			Destroy(m_DynamicMat, ClearDynamicMatDelay);
			m_DynamicMat = null;
		}
	}


	#if UNITY_EDITOR
	public void _Editor_SelectAtlas (UnityEngine.Object o)
	{
		//UIAtlas atlas = o as UIAtlas;
		//if (atlas != null)
		//{
		//	this.Atlas = atlas;
		//	if (this.DynamicMatShader == null && atlas.spriteMaterial != null)
		//	{
		//		this.DynamicMatShader = atlas.spriteMaterial.shader;
		//	}
		//}
	}
	public void _Editor_SelectSprite(string spriteName)
	{
		if (this.SpriteName != spriteName)
		{
			this.SpriteName = spriteName;
			this.RebuildDynamicMat();
		}
	}
	#endif
}