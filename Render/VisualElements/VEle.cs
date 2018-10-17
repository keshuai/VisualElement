/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CX
{
	public abstract class VEle : MonoBehaviour 
	{		
		// 等待删除
		public bool autoResizeBoxCollider;
		//--------------------------------------------------------------------------------------------------------------
		//-----   static Field   ---------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------
		public static readonly Vector2 NoUV = new Vector2(-16f, -16f);

		//--------------------------------------------------------------------------------------------------------------
		//----- instance Field   ---------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------
		/// Drawcall: connot null
		[SerializeField][HideInInspector] protected Drawcall m_DrawCall;
		[SerializeField][HideInInspector] protected ViewAsset m_Asset;
		/// 关联的Asset index 
		[SerializeField][HideInInspector] internal int internalAssetIndex = 0;
		/// Drawcall: id in drawcall
		[SerializeField][HideInInspector] internal int internalDepthIndex = 0;
		/// Drawcall: id in drawcall
		[SerializeField][HideInInspector] internal int internalElementIndex = 0;
		/// Drawcall: vertex id in drawcall
		[SerializeField][HideInInspector] internal int internalVertexIndex;
		/// Drawcall: vertex count
		[SerializeField][HideInInspector] internal int internalVertexCount;
		/// Drawcall: triangle id
		[SerializeField][HideInInspector] internal int internalTriangleIndex;
		/// Drawcall: triangle vertex count
		[SerializeField][HideInInspector] internal int internalTriangleCount;
		/// Drawcall: texture id
		[SerializeField] [HideInInspector] internal int internalTextureIndex = 0;
		/// 不在ChildRoot之中
		[SerializeField][HideInInspector] protected bool m_NotInChildRoot = false;
		[SerializeField] [HideInInspector] protected bool m_Show = true;
		/// Alpha 
		[SerializeField] [HideInInspector] protected float m_Alpha = 1f;
		/// Scale 
		[SerializeField] [HideInInspector] protected float m_Scale = 1f;
		[SerializeField] [HideInInspector] protected bool m_ScaleChanged = false;
		/// 是否接受内置事件 
		[SerializeField] [HideInInspector] protected bool m_ReceiveEvent = false;

		[SerializeField][HideInInspector] private bool m_MatrixChanged = false;
		[SerializeField][HideInInspector] private Matrix4x4 m_Matrix = Matrix4x4.identity;

		[SerializeField][HideInInspector] protected bool m_UVChanged = true;
		[SerializeField][HideInInspector] protected bool m_ColorChanged = true;

		

		//--------------------------------------------------------------------------------------------------------------
		//-----     Property   -----------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------

		public int assetIndex { get { return this.internalAssetIndex; }  set { this.internalAssetIndex = value; }}

		public int depth
		{
			get { return this.internalDepthIndex; }
			set
			{
				if (m_DrawCall != null)
				{
					m_DrawCall.ElementIndexMoveToIndex(this, value);
				}
			}
		}
		

		/// 用来进行绘制与更新的Drawcall 
		public Drawcall Drawcall
		{
			get
			{
				return m_DrawCall;
			}
			internal set
			{
				m_DrawCall = value;
			}
		}

		/// this.enabled && this.gameObject.activeInHierarchy
		public bool Show
		{
			get
			{
				return this.enabled && this.gameObject.activeInHierarchy;
			}
		}

		public bool NotInChildRoot
		{
			get
			{
				return m_NotInChildRoot;
			}
			set
			{
				if (m_NotInChildRoot != value)
				{
					m_NotInChildRoot = value;
					m_MatrixChanged = true;
				}
			}
		}

		public float Alpha
		{
			get
			{
				return m_Alpha;
			}
			set
			{
				value = Mathf.Clamp01(value);
				if (m_Alpha != value)
				{
					m_Alpha = value;
					m_ColorChanged = true;
				}
			}
		}

		/// 此scale的变化不会影响子级的VE, 可用于自身缩放的动画 
		public float Scale
		{
			get
			{
				return m_Scale;
			}
			set
			{
				if (m_Scale != value)
				{
					m_Scale = value;
					m_ScaleChanged = true;
					m_DrawCall.MarkNeedUpdate();
				}
			}
		}

		public bool MatrixChanged
		{
			get
			{
				return m_MatrixChanged;
			}
		}

		public Vector2 TransUVImageInfo (Vector2 uv)
		{
			// < 0 0~1 2~3 4~5 6~7
			uv.x += this.internalAssetIndex * 2;
			return uv;
		}
		public float TransUVImageInfoX (float x)
		{
			// < 0 0~1 2~3 4~5 6~7
			return x + this.internalAssetIndex * 2;
		}

		public Vector2 TransUVLabel (Vector2 uv)
		{
			// < 0 0~1 2~3 4~5 6~7
			uv.x += this.internalAssetIndex * 2;
			uv.y += 2;// 这个数不能太大, 太大在ios上会出现锯齿问题
			return uv;
		}

		/// 调用时间在LateUpdate 
		protected Matrix4x4 GetMatrix ()
		{
			if (m_MatrixChanged)
			{
				m_MatrixChanged = false;
				this.transform.hasChanged = false;

				m_Matrix = this.transform.localToWorldMatrix;

				if (m_NotInChildRoot || Application.isEditor)
				{
					m_Matrix = m_DrawCall.transform.worldToLocalMatrix * m_Matrix;
				}
			}

			return m_Matrix;
		}

		protected Matrix4x4 GetScaleMatrix ()
		{
			return new Matrix4x4 
			{
				m00 = m_Scale,
				m01 = 0,
				m02 = 0,
				m03 = 0,
				m10 = 0,
				m11 = m_Scale,
				m12 = 0,
				m13 = 0,
				m20 = 0,
				m21 = 0,
				m22 = 1,
				m23 = 0,
				m30 = 0,
				m31 = 0,
				m32 = 0,
				m33 = 1
			};
		}

		protected Matrix4x4 GetGizmosMatrix ()
		{
			if (m_NotInChildRoot)
			{
				return this.transform.localToWorldMatrix;
			}

			if (Application.isEditor)
			{
				return this.transform.localToWorldMatrix;
			}

			return m_DrawCall.transform.worldToLocalMatrix * this.transform.localToWorldMatrix;
		}

		//--------------------------------------------------------------------------------------------------------------
		//-----  normal  Mehtod   --------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------
		public void MarkNeedUpdateVertexIndex ()
		{
			if (m_DrawCall != null && this.Show)
			{
				m_DrawCall.MarkNeedUpdateVertexIndex();
			}
		}

		public void MarkNeedUpdate ()
		{
			if (m_DrawCall != null)
			{
				m_DrawCall.MarkNeedUpdate();
			}
		}
			
		public void RemoveFromDrawcall ()
		{
			if (m_DrawCall != null)
			{
				m_DrawCall.RemoveElement(this);
				m_DrawCall = null;
			}
		}

		void Start()
		{
			this.CheckView();
		}

		// 检查View
		public void CheckView ()
		{
			if (m_DrawCall == null)
			{
				View view = this.GetComponentInParent<View>();
				if (view != null) 
				{
					view.AddElement(this);
					m_MatrixChanged = true;
					m_ColorChanged = true;
				}
			}
			else
			{
				View view = this.GetComponentInParent<View>();
				if (!view.HasVEle(this))
				{
					m_DrawCall = null;
					view.AddElement(this);
					m_MatrixChanged = true;
					m_ColorChanged = true;
				}
			}
		}

		void OnDestroy ()
		{
			this.RemoveFromDrawcall();
		}
		
		// 显示
		void OnEnable()
		{
			if (m_DrawCall != null)
			{
				m_DrawCall.MarkNeedUpdateVertexIndex();
			}
		}
		// 隐藏
		void OnDisable()
		{
			if (m_DrawCall != null)
			{
				m_DrawCall.MarkNeedUpdateVertexIndex();
			}
		}

		//--------------------------------------------------------------------------------------------------------------
		//-----  virtual  Mehtod   -------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------

		/// 初始化顶点数据
		public abstract void virtualInitUnLightVertex (List<Vector3> verList, List<Vector2> uvList, List<Color> colList);

		/// 更新顶点索引, 如下情况下回触发 
		/// 当非末尾element被删除时, 在一帧结束时全部重构
		/// 当element排序变化时, 在一帧结束时全部重构
		public abstract void virtualUpdateVertexIndex (List<int> vertexList);

		/// 由drawcall调用
		/// 初始化vertex个数,三角形索引个数
		public virtual void virtualAwake () {}

		/// 状态更新
		protected virtual void virtualLateUpdate () {}

		/// 由drawcall调用
		/// 当show时 drawcall update每帧调用 
		internal void DoLateUpdate ()
		{
			if (!m_MatrixChanged)
			{
				m_MatrixChanged = m_NotInChildRoot ?
					m_DrawCall.transform.hasChanged || this.transform.hasChanged : 
					this.transform.hasChanged;
			}

			//if (this.Show)
			{
				this.virtualLateUpdate();
			}
		}
	}
}