/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(CXAnimationBoard))]
public class CXAnimationBoardInspector : Editor 
{

	/// 替换 系统按钮事件 
	[MenuItem("CONTEXT/CXAnimationBoard/Remove Component")]
	static void CONTEXT_RemoveComponent (MenuCommand command)
	{
		CXAnimationBoard board = command.context as CXAnimationBoard;
		if (board != null)
		{
			EditorApplication.delayCall = delegate 
			{
				foreach (CXAnimationCurve curve in board.CurveList)
				{
					DestroyImmediate(curve);
				}

				DestroyImmediate(board);
			};
		}
	}


	static Type[] animationCurveTypeArray = new Type[]
	{
		typeof(CXAnimationTransLocalPositionCurve),
		typeof(CXAnimationTransLocalRotationCurve),
		typeof(CXAnimationTransLocalScaleCurve),

		typeof(CXAnimationTransShakeLocalPosition),
		typeof(CXAnimationTransShakeLocalRotation),
		typeof(CXAnimationTransShakeLocalScale),

		typeof(CXAnimationElementAlphaCurve),
		typeof(CXAnimationAllElementAlpha),
		typeof(CXAnimationViewAlpha),
		typeof(CXAnimationElementColorCurve),
		typeof(CXAnimationFrameAniCurve),

		typeof(CXAnimationParticleSystemCurve),

		typeof(CXAnimationTimeScaleCurve),
	};
	static Type selectCurveType = typeof(CXAnimationTransLocalPositionCurve);

	CXAnimationBoard _this;


	static CXInspectorLayoutColorFrame animationBoardframe = new CXInspectorLayoutColorFrame("动画面板", new Color(0.5f, 0.5f, 0.5f, 1f));
	static CXInspectorLayoutColorFrame noTargetCurveFrame = new CXInspectorLayoutColorFrame("无目标曲线", new Color(0.5f, 0.5f, 0.5f, 1f));


	#if UNITY_EDITOR
	/// 编辑器资源, 不用理会
	[HideInInspector][System.NonSerialized]
	public Dictionary<object, CXInspectorLayoutColorFrame> _Editor_CurveTargetFrameDic = new Dictionary<object, CXInspectorLayoutColorFrame>();
	#endif

	CXInspectorLayoutColorFrame GetFrame (object o)
	{
		Dictionary<object, CXInspectorLayoutColorFrame> frameDic = this._Editor_CurveTargetFrameDic;//_this._Editor_CurveTargetFrameDic;

		if (frameDic.ContainsKey(o))
		{
			return frameDic[o];
		}

		CXInspectorLayoutColorFrame frame = new CXInspectorLayoutColorFrame("", new Color(0.5f, 0.5f, 0.5f, 1f));
		frameDic.Add(o, frame);
		return frame;
	}

	void Awake ()
	{
		_this = (CXAnimationBoard)this.target;
		animationBoardframe.enable = true;
	}

	/// 新建一个空的动画曲线 
	void NewCurve ()
	{
		_this.CurveList.Add((CXAnimationCurve)_this.gameObject.AddComponent(selectCurveType));
	}

	void ClearCurves ()
	{
		EditorApplication.delayCall = delegate 
		{
			foreach (CXAnimationCurve curve in _this.CurveList)
			{
				DestroyImmediate (curve, true);
			}

			_this.CurveList.Clear();
			this._Editor_CurveTargetFrameDic.Clear();//_this._Editor_CurveTargetFrameDic.Clear();
		};
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		this.ShowAnimationBoard();
		this.ShowAnimationCurveList();
	}

	/// 动画面板
	void ShowAnimationBoard ()
	{
		if(animationBoardframe.Begin())
		{
			_this.PlayOnAwake = EditorGUILayout.Toggle("Play On Awake", _this.PlayOnAwake);
			_this.StartDelay = EditorGUILayout.FloatField("Start Delay", _this.StartDelay);

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Play", GUILayout.MinWidth(40), GUILayout.MinHeight(40)))
				{
					_this.Play();
				}

				if (GUILayout.Button("Stop", GUILayout.MinWidth(40), GUILayout.MinHeight(40)))
				{
					_this.Stop();
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			selectCurveType = CXEditorInspector.PopupField<Type>(selectCurveType, animationCurveTypeArray);
			GUILayout.Space(5);

			EditorGUILayout.IntField("曲线个数(read only)", _this.CurveCount);
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("New Curve", GUILayout.MinWidth(40), GUILayout.MinHeight(40)))
				{
					this.NewCurve();
				}
				if (GUILayout.Button("Clear Curves", GUILayout.MinWidth(40), GUILayout.MinHeight(40)))
				{
					this.ClearCurves();
					return;
				}
			}
			GUILayout.EndHorizontal();
		}
		animationBoardframe.End();
	}



	private static bool CurveHasTarget (CXAnimationCurve curve)
	{
		if (curve.Ani_Target != null)
		{
			if (curve.Ani_Target is UnityEngine.Object)
			{
				return (curve.Ani_Target as UnityEngine.Object) != null;
			}

			return true;
		}

		return false;
	}
	private static bool CurveTargetEqual (CXAnimationCurve curve, object targetObject)
	{
		if (targetObject == null)
		{
			return !CurveHasTarget(curve);
		}

		return curve.Ani_Target == targetObject;
	}

	/// 曲线 
	void ShowAnimationCurveList ()
	{
		List<CXAnimationCurve> curveList = _this.CurveList;
		List<object> curveTargetList = new List<object>();

		int noTargetCurveCount = 0;

		foreach (CXAnimationCurve curve in curveList)
		{
			if (CurveHasTarget(curve))
			{
				if (!curveTargetList.Contains(curve.Ani_Target))
				{
					curveTargetList.Add(curve.Ani_Target);
				}
			}
			else
			{
				++noTargetCurveCount;
			}
		}
			
		noTargetCurveFrame.title = "无目标曲线(" + noTargetCurveCount + ")";

		if (noTargetCurveFrame.Begin())
		{
			this.ShowTargetCurves(null);
		}
		noTargetCurveFrame.End();

		foreach (object targetObject in curveTargetList)
		{
			int targetCurveCount = 0;
			foreach (CXAnimationCurve curve in curveList)
			{
				if (curve.Ani_Target == targetObject)
				{
					++targetCurveCount;
				}
			}

			CXInspectorLayoutColorFrame frame = GetFrame(targetObject);
			string targetObjectName = "";
			if (targetObject is UnityEngine.Object)
			{
				if ((targetObject as UnityEngine.Object) == null)
				{
					Debug.Log("null");
				}
				else
				{
					targetObjectName = ((UnityEngine.Object)targetObject).name;
				}

			}
			else
			{
				targetObjectName = targetObject.ToString();
			}

			frame.title = targetObjectName + "(" + targetCurveCount + ")";

			if (frame.Begin())
			{
				this.ShowTargetCurves(targetObject);
			}
			frame.End();
		}

	}

	void ShowTargetCurves (object targetObject)
	{
		List<CXAnimationCurve> curveList = _this.CurveList;
		List<CXAnimationCurve> deleteList = new List<CXAnimationCurve>();

		for (int i = 0; i < curveList.Count;++i)
		{
			CXAnimationCurve curve = curveList[i];
			if (CurveTargetEqual(curve, targetObject))
			{
				bool deleted = this.ShowCurve(curve);
				if (deleted)
				{
					deleteList.Add(curve);
				}
			}
		}

		foreach (CXAnimationCurve curve in deleteList)
		{
			curveList.Remove(curve);
		}

		EditorApplication.delayCall = delegate 
		{
			foreach (CXAnimationCurve curve in deleteList)
			{
				Editor.DestroyImmediate(curve, true);
			}
		};
	}

	bool ShowCurve (CXAnimationCurve curve)
	{
		bool deleted = false;

		CXInspectorLayoutColorFrame frame = GetFrame(curve);

		frame.title = curve.GetType().Name;
		if (frame.Begin())
		{


			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Desc"))
				{

				}
				if (GUILayout.Button("Copy"))
				{

				}
				if (GUILayout.Button("Paste"))
				{

				}
				if (GUILayout.Button("Del"))
				{
					deleted = true;
				}
			}
			GUILayout.EndHorizontal();

			if (!deleted)
			{
				if (curve is CXAnimationTransCurve)
				{
					ShowCurve_Trans((CXAnimationTransCurve)curve);
				}

				else if (curve is CXAnimationTransShake)
				{
					ShowCurve_TransShake((CXAnimationTransShake)curve);
				}

				else if (curve is CXAnimationElementAlphaCurve)
				{
					ShowCurve_NguiWidgetAlpha((CXAnimationElementAlphaCurve)curve);
				}
				else if (curve is CXAnimationAllElementAlpha)
				{
					ShowCurve_AllNguiWidgetAlpha((CXAnimationAllElementAlpha)curve);
				}
				else if (curve is CXAnimationViewAlpha)
				{
					ShowCurve_NguiPanelAlpha((CXAnimationViewAlpha)curve);
				}
				else if (curve is CXAnimationElementColorCurve)
				{
					ShowCurve_NguiWidgetColor((CXAnimationElementColorCurve)curve);
				}
				else if (curve is CXAnimationFrameAniCurve)
				{
					ShowCurve_NguiFrameAni((CXAnimationFrameAniCurve)curve);
				}

				else if (curve is CXAnimationLocalBezierCurve)
				{
					ShowCurve_Bezier((CXAnimationLocalBezierCurve)curve);
				}

				else if (curve is CXAnimationParticleSystemCurve)
				{
					ShowCurve_ParticleSystemAni((CXAnimationParticleSystemCurve)curve);
				}

				else if (curve is CXAnimationTimeScaleCurve)
				{
					ShowCurve_TimeScale((CXAnimationTimeScaleCurve)curve);
				}
			}

		}
		frame.End();

		return deleted;
	}

	private void ShowCurve_Trans (CXAnimationTransCurve curve)
	{
		curve.Name = EditorGUILayout.TextField("Name", curve.Name);
		curve.StartDelay = EditorGUILayout.FloatField("StartDelay", curve.StartDelay);
		curve.Target = (Transform)EditorGUILayout.ObjectField("Target Transform", curve.Target, typeof(Transform), true);
		curve.StartValue = EditorGUILayout.Vector3Field("StartValue", curve.StartValue);
		curve.DestValue = EditorGUILayout.Vector3Field("DestValue", curve.DestValue);
		curve.Duration = EditorGUILayout.FloatField("Duration", curve.Duration);
		curve.U3DCurve = CXEditorInspector.CurveField("Curve", curve.U3DCurve);
		curve.Loop = EditorGUILayout.Toggle("Loop", curve.Loop);
	}

	private void ShowCurve_TransShake(CXAnimationTransShake ani)
	{	
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (Transform)EditorGUILayout.ObjectField("Target Transform", ani.Target, typeof(Transform), true);
		ani.DestValue = EditorGUILayout.Vector3Field("DestValue", ani.DestValue);
		ani.Amplitude = EditorGUILayout.Vector3Field("Amplitude", ani.Amplitude);
		ani.Frequency = EditorGUILayout.FloatField("Frequency", ani.Frequency);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
		ani.ShakeType = (CXTransformShakeType)EditorGUILayout.EnumPopup("ShakeType", ani.ShakeType);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}

	private void ShowCurve_Bezier(CXAnimationLocalBezierCurve ani)
	{	
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (Transform)EditorGUILayout.ObjectField("Target Transform", ani.Target, typeof(Transform), true);
		ani.PowerPoints = CXEditorInspector.Vector3ArrayField("PowerPoints", ani.PowerPoints);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
	}

	private void ShowCurve_NguiWidgetAlpha(CXAnimationElementAlphaCurve ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (CX.VEle)EditorGUILayout.ObjectField("Target UIWidget", ani.Target, typeof(CX.VEle), true);
		ani.StartValue = EditorGUILayout.FloatField("StartValue", ani.StartValue);
		ani.DestValue = EditorGUILayout.FloatField("DestValue", ani.DestValue);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}


	private void ShowCurve_AllNguiWidgetAlpha(CXAnimationAllElementAlpha ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (GameObject)EditorGUILayout.ObjectField("Target GameObject", ani.Target, typeof(GameObject), true);
		ani.StartValue = EditorGUILayout.FloatField("StartValue", ani.StartValue);
		ani.DestValue = EditorGUILayout.FloatField("DestValue", ani.DestValue);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}


	private void ShowCurve_NguiPanelAlpha(CXAnimationViewAlpha ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (CX.View)EditorGUILayout.ObjectField("Target UIPanel", ani.Target, typeof(CX.View), true);
		ani.StartValue = EditorGUILayout.FloatField("StartValue", ani.StartValue);
		ani.DestValue = EditorGUILayout.FloatField("DestValue", ani.DestValue);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}

	private void ShowCurve_NguiWidgetColor(CXAnimationElementColorCurve ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (CX.VEle)EditorGUILayout.ObjectField("Target UIWidget", ani.Target, typeof(CX.VEle), true);
		ani.StartValue = EditorGUILayout.ColorField("StartColor", ani.StartValue);
		ani.DestValue = EditorGUILayout.ColorField("DestColor", ani.DestValue);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}


	private void ShowCurve_NguiFrameAni(CXAnimationFrameAniCurve ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Target = (CX.ImageVE)EditorGUILayout.ObjectField("Target Sprite", ani.Target, typeof(CX.ImageVE), true);
		ani.Prefix = EditorGUILayout.TextField("Name Prefix", ani.Prefix);
		ani.StartIndex = EditorGUILayout.IntField("Start Index", ani.StartIndex);
		ani.EndIndex = EditorGUILayout.IntField("End Index", ani.EndIndex);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.Scale = EditorGUILayout.FloatField("Scale Factor", ani.Scale);
		ani.Loop = EditorGUILayout.Toggle("Loop", ani.Loop);
	}

	private void ShowCurve_ParticleSystemAni(CXAnimationParticleSystemCurve ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);

		ParticleSystem old = ani.Target;
		ani.Target = (ParticleSystem)EditorGUILayout.ObjectField("Target ParticleSystem", old, typeof(ParticleSystem), true);
		ParticleSystem particleSystem = ani.Target;

		if (particleSystem != null)
		{
			particleSystem.startDelay = EditorGUILayout.FloatField("StartDelay", particleSystem.startDelay);

			if (old == null)
			{
				particleSystem.playOnAwake = false;
			}

			int oldRenderQueue = ani.RenderQueue;
			int newRenderQueue = EditorGUILayout.IntField("ParticleRenderQueue", oldRenderQueue);
			if (newRenderQueue != oldRenderQueue)
			{
				ani.RenderQueue = newRenderQueue;

				if (particleSystem.GetComponent<Renderer>().sharedMaterial != null)
				{
					particleSystem.GetComponent<Renderer>().sharedMaterial.renderQueue = newRenderQueue;
				}
			}

			ani.ClearDynamicMatDelay =  EditorGUILayout.FloatField("Clear Dynamic Mat Delay", ani.ClearDynamicMatDelay); 

//			EditorGUILayout.FloatField("Duration (ParticleSystem)", particleSystem.duration); 
//			ani.Duration = particleSystem.duration;

			particleSystem.loop = EditorGUILayout.Toggle("Loop", particleSystem.loop);
			ani.Loop = particleSystem.loop;

			ani.UseNguiAtlas = EditorGUILayout.Toggle("Use NGUI Atlas", ani.UseNguiAtlas);

			if (ani.UseNguiAtlas)
			{
				Shader oldShader = ani.DynamicMatShader;
				ani.DynamicMatShader = (Shader)EditorGUILayout.ObjectField("Shader", oldShader, typeof(Shader), true);
				if (oldShader != ani.DynamicMatShader)
				{
					ani.RebuildDynamicMat();
				}
				//ani.Atlas = (UIAtlas)EditorGUILayout.ObjectField("NGUIAtlas", ani.Atlas, typeof(UIAtlas), true);

				if (GUILayout.Button("Select NGUI Atlas", "DropDownButton"))
				{
					//ComponentSelector.Show<UIAtlas>(ani._Editor_SelectAtlas);
				}
				//NGUIEditorTools.DrawAdvancedSpriteField(ani.Atlas, ani.SpriteName, ani._Editor_SelectSprite, false);
			}
			else
			{
				Material oldMaterial = particleSystem.GetComponent<Renderer>().sharedMaterial;
				Material newMaterial = (Material)EditorGUILayout.ObjectField("material", oldMaterial, typeof(Material), true);
				if (newMaterial != oldMaterial)
				{
					newMaterial.renderQueue = ani.RenderQueue;
					particleSystem.GetComponent<Renderer>().sharedMaterial = newMaterial;
				}
			}
		}
	}

	private void ShowCurve_TimeScale(CXAnimationTimeScaleCurve ani)
	{
		ani.Name = EditorGUILayout.TextField("Name", ani.Name);
		ani.StartDelay = EditorGUILayout.FloatField("StartDelay", ani.StartDelay);
		ani.Duration = EditorGUILayout.FloatField("Duration", ani.Duration);
		ani.U3DCurve = CXEditorInspector.CurveField("Curve", ani.U3DCurve);
	}



}