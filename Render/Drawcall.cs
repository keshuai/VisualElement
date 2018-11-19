/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace CX
{
	/// 渲染类. 一个渲染实例占一个drawcall, 可能渲染一个或多个元素
	/// 注意事项: drawcall会使用属性this.transform.hasChanged, 如果其他脚本对其进行赋值, 可能会产生冲突, 使得渲染不正常
	public abstract class Drawcall : MonoBehaviour
	{
		private Matrix4x4 m_Matrix;

		public Matrix4x4 WorldToLocalMatrix
		{
			get
			{
				return this.cachedTrans.worldToLocalMatrix;
			}
		}

		/// 子组件的root 
		/// 自身transform的缓存 
		[SerializeField][HideInInspector] internal Transform cachedTrans;

		/// 裁剪
		[SerializeField] [HideInInspector] private bool _clip = false;
		[SerializeField] [HideInInspector] private Vector4 _clipRange = new Vector4(-250, 250, -250, 250);
		public bool Clip
		{
			get { return _clip; }
			set
			{
				if (_clip != value)
				{
					_clip = value;
					if (value)
					{
						if (m_Shader == DefaultShader)
						{
							m_Shader = DefaultClipShader;

							// 在运行时
							if (_mMat != null)
							{
								_mMat.shader = m_Shader;
								_mMat.renderQueue = m_RenderQueue;
								_mMat.SetColor("_Clip", _clipRange);
							}
						}
					}
					else
					{
						if (m_Shader == DefaultClipShader)
						{
							m_Shader = DefaultShader;
							
							// 在运行时
							if (_mMat != null)
							{
								_mMat.shader = m_Shader;
								_mMat.renderQueue = m_RenderQueue;
							}
						}
					}
				}
			}
		}
		
		public Vector4 ClipRange
		{
			get { return _clipRange; }
			set
			{
				if (_clipRange != value)
				{
					_clipRange = value;
					if (_clip)
					{
						if (_mMat != null)
						{
							_mMat.SetColor("_Clip", _clipRange);
						}
					}
				}
			}
		}

		private static Shader s_DefaultShader = null;
		private static Shader s_DefaultClipShader = null;
		public static Shader DefaultShader { get { if (s_DefaultShader == null) s_DefaultShader = Shader.Find("UIView"); return s_DefaultShader;}}
		public static Shader DefaultClipShader { get { if (s_DefaultClipShader == null) s_DefaultClipShader = Shader.Find("UIViewClip"); return s_DefaultClipShader;}}

		/// auto change in run time
		/// 是否需要更新Mesh
		private bool m_NeedUpdateMesh = true;

		/// 是否需要更新三角形索引
		[SerializeField]private bool m_NeedUpdateVertexIndex = false;

		/// 重写一个集合 支持 字典的快速包含判断 List的增删查改 再加双List的双重顺序
		
		/// 两个List必须保持 添加元素和删除元素的一致性
		/// 元素列表 -- 直接按添加顺序排列, 中途不改变顺序
		[SerializeField]protected List<VEle> m_ElementIndexArray = new List<VEle>();
		/// 渲染顺序列表 -- 通过DepthIndex排序, 用来重构渲染顺序
		[SerializeField]protected List<VEle> m_DepthIndexArray = new List<VEle>();
		/// 用来处理非正常运行模式删除的元素
		List<VEle> m_NullList = new List<VEle>();

		/// 返回一个独立的数组
		public VEle[] ElementList { get { return m_DepthIndexArray.ToArray(); } }

		public int ElementCount
		{
			get { return m_DepthIndexArray.Count; }
		}

		public bool HasVEle(VEle e)
		{
			if (e == null) return false;

			//return m_DepthIndexArray.Contains(e);
			int index0 = e.internalElementIndex;
			int index1 = e.internalDepthIndex;

			return 
				index0 >= 0 && index0 < m_ElementIndexArray.Count &&
				index1 >= 0 && index1 < m_DepthIndexArray.Count &&
				m_ElementIndexArray[index0] == e &&
				m_DepthIndexArray[index1] == e;
		}

		[SerializeField]internal List<Vector3> m_VerList = new List<Vector3>();
		[SerializeField]internal List<Vector2> m_UVList = new List<Vector2>();
		[SerializeField]internal List<Color> m_ColList = new List<Color>();
		[SerializeField]internal List<int> m_TriList = new List<int>();

		[SerializeField][HideInInspector] private int m_RenderQueue = 3000;
		[SerializeField][HideInInspector] private Shader m_Shader;

		public bool NeedUpdate
		{
			get
			{
				return m_NeedUpdateMesh;
			}
		}

		public Transform ChildRoot
		{
			get
			{
				return cachedTrans;
			}
		}

		/// 标记为需要更新Mesh 
		public void MarkNeedUpdate ()
		{
			m_NeedUpdateMesh = true;
		}

		/// 标记为需要更新三角形索引 
		public void MarkNeedUpdateVertexIndex ()
		{
			/// 三角形索引 需要更新 
			m_NeedUpdateVertexIndex = true;
			/// mesh 需要更新
			m_NeedUpdateMesh = true;
		}

		/// 当前视图的渲染队列
		public int RenderQueue
		{
			get
			{
				return m_RenderQueue;
			}
			set
			{
				if (m_RenderQueue != value)
				{
					m_RenderQueue = value;

					if (_mMat != null)
					{
						_mMat.renderQueue = value;
					}
				}
			}
		}

		public Shader Shader
		{
			get 
			{
				return m_Shader;
			}
			set
			{
				if (m_Shader != value)
				{
					m_Shader = value;
					if (_mMat != null)
					{
						_mMat.shader = value;
						// 设置shader时会重置mat的渲染队列
						_mMat.renderQueue = m_RenderQueue;
					}

					this.VirtualShaderChanged();
				}
			}
		}

		private Mesh _mMesh;
		private Material _mMat;

		public Mesh Mesh {get { return _mMesh; } }
		public Material Mat {get { return _mMat; } }

		void OnDestroy ()
		{
			this.VirtualOnDestroy();

			if (_mMat != null)
			{
				if (Application.isPlaying)
				{
					Destroy(_mMat);
				}
				else
				{
					DestroyImmediate(_mMat);
				}
				_mMat = null;
			}

			if (_mMesh != null)
			{
				if (Application.isPlaying)
				{
					Destroy(_mMesh);
				}
				else
				{
					DestroyImmediate(_mMesh);
				}
				_mMesh = null;
			}
		}
		/// Awake调用
		protected virtual void VirtualAwake ()
		{
		}
		protected virtual void VirtualOnDestroy ()
		{
		}
		/// Shader改变时调用
		protected virtual void VirtualShaderChanged ()
		{}

		void Awake ()
		{
			/// 缓存transform
			this.cachedTrans = this.transform;

			this.CheckMat();
			this.CheckMesh();

			this.VirtualAwake();
		}

		protected Material mat
		{
			get 
			{
				this.CheckMat();
				return _mMat;
			}
		}

		private void CheckMat ()
		{
			if (_mMat == null)
			{
				if (m_Shader == null)
				{
					m_Shader = _clip? DefaultClipShader : DefaultShader;
				}

				_mMat = new Material(m_Shader);
				if (_clip) _mMat.SetColor("_Clip", _clipRange);
				_mMat.renderQueue = m_RenderQueue;
			}
		}

		private void CheckMesh ()
		{
			if (_mMesh == null)
			{
				_mMesh = new Mesh ();
				_mMesh.MarkDynamic();
			}
		}

		public void SetTexture (Texture tx)
		{
			if (_mMat != null)
			{
				_mMat.mainTexture = tx;
			}
		}

		internal void ElementVertexCountChangedOnUpdate (VEle e, int oldCount, int newCount)
		{
			int deltaCount = newCount - oldCount;
			if (deltaCount != 0)
			{
				this.MarkNeedUpdateVertexIndex();

				int index = e.internalVertexIndex;

				if (deltaCount > 0)
				{
					m_VerList.InsertEmptyRange(index, deltaCount);
					m_UVList .InsertEmptyRange(index, deltaCount);
					m_ColList.InsertEmptyRange(index, deltaCount);
				}
				else
				{
					int removeCount = -deltaCount;
					m_VerList.RemoveRange(index, removeCount);
					m_UVList .RemoveRange(index, removeCount);
					m_ColList.RemoveRange(index, removeCount);
				}

				e.internalVertexCount = newCount;

				for(int i = e.internalElementIndex + 1, len = m_ElementIndexArray.Count; i < len; ++i)
				{
					m_ElementIndexArray[i].internalVertexIndex += deltaCount;
				}
			}
		}
		
		// 每帧执行每个元素的更新
		void EachVELateUpdate ()
		{
			// update each visual element
			VEle e;
			int count = m_ElementIndexArray.Count;

			// 对每个显示的元素进行更新
			int index = 0;
			while(index < count)
			{
				e = m_ElementIndexArray[index];
				// 显示才执行更新操作
				if (e != null && e.Show) e.DoLateUpdate();
				++index;


			}

			// 将每个显示的元素的transform changed 设为false
			// 在此处进行设置 避免多个元素共用一个transform时矩阵判断出错
			index = 0;
			while(index < count)
			{
				e = m_ElementIndexArray[index];
				// 显示才执行更新操作
				if (e != null && e.Show) e.cachedTrans.hasChanged = false;
				++index;
			}
		}

		/// 更新View的三角形顶点索引
		/// 当层级发生变化时 
		/// 当顶点数发生变化时
		/// 当显隐发生变化时
		void UpdateVertexIndexArray ()
		{
			m_TriList.Clear();
			VEle e;
			for (int i = 0; i < m_DepthIndexArray.Count; ++i)
			{
				e = m_DepthIndexArray[i];
				if (e == null)
				{
					//this.RemoveElement(e);
					//--i;
					// destroyed
					Debug.Log("has removed");
					continue;
				}

				if (e.Show)
				{
					e.virtualUpdateVertexIndex(m_TriList);
				}
			}
		}

		void UpdateDepth ()
		{
			VEle[] els = this.GetComponentsInChildren<VEle>();
			int newCount = els.Length;
			if (newCount == this.m_DepthIndexArray.Count)
			{
				// 个数相等 
				// 检查顺序是否变化
				bool needSort = false;
				for (int i = 0; i < newCount; ++i)
				{
					els[i].internalDepthIndex = i;
					if (els[i] != m_DepthIndexArray[i])
					{
						needSort = true;
						//break;
					}
				}

				if (needSort)
				{
					m_DepthIndexArray.Clear();
					m_DepthIndexArray.AddRange(els);
					m_NeedUpdateVertexIndex = true;
				}
			}
			else
			{
				// 个数不等 需要重置
			}
		}

		void LateUpdate ()
		{
			// 每个显示的元素进行自我更行(更新绘制信息)
			this.EachVELateUpdate();

			// 更新层次排序
			this.UpdateDepth();

			// 索引更新(层次及是否显示)
			if (m_NeedUpdateVertexIndex)
			{
				m_NeedUpdateVertexIndex = false;
				m_NeedUpdateMesh = true;// 层次更新后需要更新Mesh

				this.UpdateVertexIndexArray();
			}

			// 矩阵更新
			if (this.cachedTrans.hasChanged)
			{
				m_Matrix = this.cachedTrans.localToWorldMatrix;
				this.cachedTrans.hasChanged = false;
			}
		
			if (m_NeedUpdateMesh)
			{
				Debug.Log("View UpdateMesh");
				// 更新绘制信息到Mesh
				this.UpdateMesh(m_VerList, m_UVList, m_ColList, m_TriList);
				// 更新标识复位
				m_NeedUpdateMesh = false;
			}

			// 进行绘制调用
			// 在基于纯UI模式时可以改为非每帧调用 已节约耗电
			this.DrawMesh();
		}

		private void DrawMesh()
		{
			Graphics.DrawMesh (_mMesh, m_Matrix, _mMat, this.gameObject.layer);
//			Graphics.DrawMesh (_mMesh, m_Matrix, _mMat, this.gameObject.layer, 
//				null, 					// Camera camera, int submeshIndex
//				0, 						// int submeshIndex
//				null, 					// MaterialPropertyBlock properties
//				ShadowCastingMode.On,	// (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, 
//				false, 					// bool receiveShadows
//				null, 					// Transform probeAnchor
//				false					// bool useLightProbes
//			);
		}
			
	

		public void UpdateMesh (Vector3[] vers, Vector2[] uvs, Vector2[] uvs2, Color[] cols, int[] tris)
		{
			this.CheckMesh();

			_mMesh.Clear();
			_mMesh.vertices = vers;
			_mMesh.uv = uvs;
			_mMesh.uv2 = uvs2;
			_mMesh.colors = cols;
			_mMesh.triangles = tris;
		}

		public void UpdateMesh (Vector3[] vers, Vector2[] uvs, Color[] cols, int[] tris)
		{
			this.CheckMesh();

			_mMesh.Clear();
			_mMesh.vertices = vers;
			_mMesh.uv = uvs;
			_mMesh.colors = cols;
			_mMesh.triangles = tris;
		}

		public void UpdateMesh (List<Vector3> vers, List<Vector2> uvs, List<Color> cols, List<int> tris)
		{
			this.CheckMesh();

			_mMesh.Clear();
			_mMesh.SetVertices(vers);
			_mMesh.SetUVs(0, uvs);
			_mMesh.SetColors(cols);
			_mMesh.SetTriangles(tris, 0);
		}

		public void UpdateMesh (Vector3[] vers, Vector2[] uvs, int[] tris)
		{
			this.CheckMesh();

			_mMesh.Clear();
			_mMesh.vertices = vers;
			_mMesh.uv = uvs;
			_mMesh.triangles = tris;
		}

		//--------------------------------------------------------------------------------------------------------------
		//---------  (add/insert/remove) element  Mehtod(s)  -----------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------

		/// 新建VEle
		/// 会自动添加为child
		public VEle NewElement(System.Type type)
		{
			GameObject o = new GameObject(type.Name);
			o.transform.SetParent(cachedTrans, false);

			VEle e = (VEle)o.AddComponent(type);
			this.AddElement(e);
			return e;
		}

		/// 新建VEle
		/// 会自动添加为child
		public T NewElement<T>() where T : VEle
		{
			return (T)this.NewElement(typeof(T));
		}


		/// View 添加元素的入口 1
		public void AddElement(VEle e)
		{
			if (e.Drawcall == null || ((e.Drawcall == this) && (!m_ElementIndexArray.Contains(e))))
			{
				this.AddElementToVertexList(e);
				e.internalDepthIndex = m_DepthIndexArray.Count;
				// 层次List
				m_DepthIndexArray.Add(e);
				// 此处仍可以优化
				this.MarkNeedUpdateVertexIndex();
			}
			else
			{
				Debug.LogError("the element you add has already belong to some drawcall: " + e.name);
				//throw new System.Exception("the element you add has already belong to some drawcall");
			}
		}

		private void AddElementToVertexList (VEle e)
		{
			e.Drawcall = this;
			e.internalVertexIndex = m_VerList.Count;
			e.internalElementIndex = m_ElementIndexArray.Count;
			m_ElementIndexArray.Add(e);
			e.virtualAwake();
			e.virtualInitUnLightVertex(m_VerList, m_UVList, m_ColList);

			this.MarkNeedUpdate();
		}
			
		private void RemoveElementFromVertexList (VEle e)
		{
			int index = e.internalElementIndex;
			int vertexIndex = e.internalVertexIndex; 
			int verTexCount = e.internalVertexCount;

			for (int i = index + 1, len = m_ElementIndexArray.Count; i < len; ++i)
			{
				m_ElementIndexArray[i].internalElementIndex -= 1;
				m_ElementIndexArray[i].internalVertexIndex -= verTexCount;
			}

			m_VerList.RemoveRange(vertexIndex, verTexCount);
			m_UVList.RemoveRange(vertexIndex, verTexCount);
			m_ColList.RemoveRange(vertexIndex, verTexCount);

			m_ElementIndexArray.RemoveAt(index);
		}

		private void RemoveElementFromIndexArray (VEle e)
		{
			int index = e.internalDepthIndex;
			for (int i = index + 1, len = m_DepthIndexArray.Count; i < len; ++i)
			{
				m_DepthIndexArray[i].internalDepthIndex -= 1;
			}

			m_DepthIndexArray.RemoveAt(e.internalDepthIndex);
			e.Drawcall = null;
			e.internalDepthIndex = -1;

			this.MarkNeedUpdateVertexIndex();
		}
		
		/// 删除元素的唯一入口
		/// 正常运行时Destroy的元素会自动调用此接口
		/// 非正常运行时的此接口可能不会触发 就需要进行null检查
		internal void RemoveElement (VEle e)
		{
			if (e.Drawcall == this)
			{
				this.RemoveElementFromVertexList(e);
				this.RemoveElementFromIndexArray(e);
				e.Drawcall = null;
			}
		}
	}
}
