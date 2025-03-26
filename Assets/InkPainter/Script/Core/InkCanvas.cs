using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Es.InkPainter.Effective;

#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Es.InkPainter
{
	/// <summary>
	/// Texture paint to canvas.
	/// To set the per-material.
	/// </summary>
	[RequireComponent(typeof(Renderer))]
	[DisallowMultipleComponent]
	public class InkCanvas : MonoBehaviour
	{
		[Serializable]
		public class PaintSet
		{
			/// <summary>
			/// Applying paint materials.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public Material material;

			[SerializeField, Tooltip("The property name of the main texture.")]
			public string mainTextureName = "_MainTex";

			[SerializeField, Tooltip("Normal map texture property name.")]
			public string normalTextureName = "_BumpMap";

			[SerializeField, Tooltip("The property name of the heightmap texture.")]
			public string heightTextureName = "_ParallaxMap";

			[SerializeField, Tooltip("Whether or not use main texture paint.")]
			public bool useMainPaint = true;

			[SerializeField, Tooltip("Whether or not use normal map paint (you need material on normal maps).")]
			public bool useNormalPaint = false;

			[SerializeField, Tooltip("Whether or not use heightmap painting (you need material on the heightmap).")]
			public bool useHeightPaint = false;

			/// <summary>
			/// In the first time set to the material's main texture.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public Texture MainTexture;

			/// <summary>
			/// Copied the main texture to rendertexture that use to paint.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public RenderTexture PaintMainTexture;

			/// <summary>
			/// In the first time set to the material's normal map.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public Texture NormalTexture;

			/// <summary>
			/// Copied the normal map to rendertexture that use to paint.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public RenderTexture PaintNormalTexture;

			/// <summary>
			/// In the first time set to the material's height map.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public Texture HeightTexture;

			/// <summary>
			/// Copied the height map to rendertexture that use to paint.
			/// </summary>
			[HideInInspector]
			[NonSerialized]
			public RenderTexture PaintHeightTexture;

			#region ShaderPropertyID

			[HideInInspector]
			[NonSerialized]
			public int mainTexturePropertyID;

			[HideInInspector]
			[NonSerialized]
			public int normalTexturePropertyID;

			[HideInInspector]
			[NonSerialized]
			public int heightTexturePropertyID;

			#endregion ShaderPropertyID

			#region Constractor
			/// <summary>
			/// Default constractor.
			/// </summary>
			public PaintSet() { }

			/// <summary>
			/// Setup paint data.
			/// </summary>
			/// <param name="mainTextureName">Shader property name(main texture).</param>
			/// <param name="normalTextureName">Shader property name(normal map).</param>
			/// <param name="heightTextureName">Shader property name(height map)</param>
			/// <param name="useMainPaint">Whether to use main texture paint.</param>
			/// <param name="useNormalPaint">Whether to use normal map paint.</param>
			/// <param name="useHeightPaint">Whether to use height map paint.</param>
			public PaintSet(string mainTextureName, string normalTextureName, string heightTextureName, bool useMainPaint, bool useNormalPaint, bool useHeightPaint)
			{
				this.mainTextureName = mainTextureName;
				this.normalTextureName = normalTextureName;
				this.heightTextureName = heightTextureName;
				this.useMainPaint = useMainPaint;
				this.useNormalPaint = useNormalPaint;
				this.useHeightPaint = useHeightPaint;
			}

			/// <summary>
			/// Setup paint data.
			/// </summary>
			/// <param name="mainTextureName">Shader property name(main texture).</param>
			/// <param name="normalTextureName">Shader property name(normal map).</param>
			/// <param name="heightTextureName">Shader property name(height map)</param>
			/// <param name="useMainPaint">Whether to use main texture paint.</param>
			/// <param name="useNormalPaint">Whether to use normal map paint.</param>
			/// <param name="useHeightPaint">Whether to use height map paint.</param>
			/// <param name="material">Specify when painting a specific material.</param>
			public PaintSet(string mainTextureName, string normalTextureName, string heightTextureName, bool useMainPaint, bool useNormalPaint, bool useHeightPaint, Material material)
				:this(mainTextureName, normalTextureName, heightTextureName, useMainPaint, useNormalPaint, useHeightPaint)
			{
				this.material = material;
			}
			#endregion Constractor
		}

		private static Material _paintMainMaterial = null;
		private static Material _paintNormalMaterial = null;
		private static Material _paintHeightMaterial = null;
		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
		private bool _eraseFlag = false;
		private RenderTexture _debugEraserMainView;
		private RenderTexture _debugEraserNormalView;
		private RenderTexture _debugEraserHeightView;
#pragma warning disable 0649
		private bool _eraserDebug;
#pragma warning restore 0649

		/// <summary>
		/// Access data used for painting.
		/// </summary>
		public List<PaintSet> PaintDatas { get { return paintSet; } set { paintSet = value; } }

		/// <summary>
		/// Called by InkCanvas attached game object.
		/// </summary>
		public event Action<InkCanvas> OnCanvasAttached;

		public EventHandler<RenderTexture> OnRenderTextureUpdated;


		/// <summary>
		/// Called by InkCanvas initialization start times.
		/// </summary>
		public event Action<InkCanvas> OnInitializedStart;

		/// <summary>
		/// Called by InkCanvas initialization completion times.
		/// </summary>
		public event Action<InkCanvas> OnInitializedAfter;

		/// <summary>
		/// Called at paint start.
		/// </summary>
		public event Action<InkCanvas, Brush> OnPaintStart;

		/// <summary>
		/// Called at paint end.
		/// </summary>
		public event Action<InkCanvas> OnPaintEnd;

		#region SerializedField

		[SerializeField]
		private List<PaintSet> paintSet = null;

		#endregion SerializedField

		#region ShaderPropertyID

		private int _paintUVPropertyID;

		private int _brushTexturePropertyID;
		private int _brushScalePropertyID;
		private int _brushRotatePropertyID;
		private int _brushColorPropertyID;
		private int _brushNormalTexturePropertyID;
		private int _brushNormalBlendPropertyID;
		private int _brushHeightTexturePropertyID;
		private int _brushHeightBlendPropertyID;
		private int _brushHeightColorPropertyID;

		#endregion ShaderPropertyID

		#region ShaderKeywords

		private const string COLOR_BLEND_USE_CONTROL = "INK_PAINTER_COLOR_BLEND_USE_CONTROL";
		private const string COLOR_BLEND_USE_BRUSH = "INK_PAINTER_COLOR_BLEND_USE_BRUSH";
		private const string COLOR_BLEND_NEUTRAL = "INK_PAINTER_COLOR_BLEND_NEUTRAL";
		private const string COLOR_BLEND_ALPHA_ONLY = "INK_PAINTER_COLOR_BLEND_ALPHA_ONLY";

		private const string NORMAL_BLEND_USE_BRUSH = "INK_PAINTER_NORMAL_BLEND_USE_BRUSH";
		private const string NORMAL_BLEND_ADD = "INK_PAINTER_NORMAL_BLEND_ADD";
		private const string NORMAL_BLEND_SUB = "INK_PAINTER_NORMAL_BLEND_SUB";
		private const string NORMAL_BLEND_MIN = "INK_PAINTER_NORMAL_BLEND_MIN";
		private const string NORMAL_BLEND_MAX = "INK_PAINTER_NORMAL_BLEND_MAX";
		private const string DXT5NM_COMPRESS_USE = "DXT5NM_COMPRESS_USE";
		private const string DXT5NM_COMPRESS_UNUSE = "DXT5NM_COMPRESS_UNUSE";

		private const string HEIGHT_BLEND_USE_BRUSH = "INK_PAINTER_HEIGHT_BLEND_USE_BRUSH";
		private const string HEIGHT_BLEND_ADD = "INK_PAINTER_HEIGHT_BLEND_ADD";
		private const string HEIGHT_BLEND_SUB = "INK_PAINTER_HEIGHT_BLEND_SUB";
		private const string HEIGHT_BLEND_MIN = "INK_PAINTER_HEIGHT_BLEND_MIN";
		private const string HEIGHT_BLEND_MAX = "INK_PAINTER_HEIGHT_BLEND_MAX";
		private const string HEIGHT_BLEND_COLOR_RGB_HEIGHT_A = "INK_PAINTER_HEIGHT_BLEND_COLOR_RGB_HEIGHT_A";

		#endregion ShaderKeywords

		#region MeshData

		private MeshOperator _meshOperator;
		private Camera _camera;

		private MeshOperator MeshOperator
		{
			get
			{
				if(_meshOperator == null)
					Debug.LogError("To take advantage of the features must Mesh filter or Skinned mesh renderer component associated Mesh.");

				return _meshOperator;
			}
		}

		#endregion MeshData

		#region UnityEventMethod

		private void Awake()
		{
			if(OnCanvasAttached != null)
				OnCanvasAttached(this);
			InitPropertyID();
			SetMaterial();
			SetTexture();
			MeshDataCache();
			
		}

		private void Start()
		{
			_camera = Camera.main;
			if(OnInitializedStart != null)
				OnInitializedStart(this);
			SetRenderTexture();
			if(OnInitializedAfter != null)
				OnInitializedAfter(this);
		}

		private void OnDestroy()
		{
			Debug.Log("InkCanvas has been destroyed.");
			ReleaseRenderTexture();
		}

		private void OnGUI()
		{
			if(_eraserDebug)
			{
				if(_debugEraserMainView)
					GUI.DrawTexture(new Rect(0, 0, 100, 100), _debugEraserMainView);
				if(_debugEraserNormalView)
					GUI.DrawTexture(new Rect(0, 100, 100, 100), _debugEraserNormalView);
				if(_debugEraserHeightView)
					GUI.DrawTexture(new Rect(0, 200, 100, 100), _debugEraserHeightView);
			}
		}

		#endregion UnityEventMethod

		#region PrivateMethod

		/// <summary>
		/// Cach data from the mesh.
		/// </summary>
		private void MeshDataCache()
		{
			var meshFilter = GetComponent<MeshFilter>();
			var skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
			if(meshFilter != null)
				_meshOperator = new MeshOperator(meshFilter.sharedMesh);
			else if(skinnedMeshRenderer != null)
				_meshOperator = new MeshOperator(skinnedMeshRenderer.sharedMesh);
			else
				Debug.LogWarning("Sometimes if the MeshFilter or SkinnedMeshRenderer does not exist in the component part does not work correctly.");
		}

		/// <summary>
		/// To initialize the shader property ID.
		/// </summary>
		private void InitPropertyID()
		{
			foreach(var p in paintSet)
			{
				p.mainTexturePropertyID = Shader.PropertyToID(p.mainTextureName);
				p.normalTexturePropertyID = Shader.PropertyToID(p.normalTextureName);
				p.heightTexturePropertyID = Shader.PropertyToID(p.heightTextureName);
			}
			_paintUVPropertyID = Shader.PropertyToID("_PaintUV");
			_brushTexturePropertyID = Shader.PropertyToID("_Brush");
			_brushScalePropertyID = Shader.PropertyToID("_BrushScale");
			_brushRotatePropertyID = Shader.PropertyToID("_BrushRotate");
			_brushColorPropertyID = Shader.PropertyToID("_ControlColor");
			_brushNormalTexturePropertyID = Shader.PropertyToID("_BrushNormal");
			_brushNormalBlendPropertyID = Shader.PropertyToID("_NormalBlend");
			_brushHeightTexturePropertyID = Shader.PropertyToID("_BrushHeight");
			_brushHeightBlendPropertyID = Shader.PropertyToID("_HeightBlend");
			_brushHeightColorPropertyID = Shader.PropertyToID("_Color");
		}

		/// <summary>
		/// Set and retrieve the material.
		/// </summary>
		private void SetMaterial()
		{
			if(_paintMainMaterial == null)
				_paintMainMaterial = new Material(Resources.Load<Material>("Es.InkPainter.PaintMain"));
			if(_paintNormalMaterial == null)
				_paintNormalMaterial = new Material(Resources.Load<Material>("Es.InkPainter.PaintNormal"));
			if(_paintHeightMaterial == null)
				_paintHeightMaterial = new Material(Resources.Load<Material>("Es.InkPainter.PaintHeight"));
			var m = GetComponent<Renderer>().materials;
			for(int i = 0; i < m.Length; ++i)
			{
                if (paintSet[i].material == null)
                    paintSet[i].material = m[i];
			}
		}

		/// <summary>
		/// Set and retrieve the texture.
		/// </summary>
		private void SetTexture()
		{
			foreach(var p in paintSet)
			{
				if(p.material.HasProperty(p.mainTexturePropertyID))
					p.MainTexture = p.material.GetTexture(p.mainTexturePropertyID);
				if(p.material.HasProperty(p.normalTexturePropertyID))
					p.NormalTexture = p.material.GetTexture(p.normalTexturePropertyID);
				if(p.material.HasProperty(p.heightTexturePropertyID))
					p.HeightTexture = p.material.GetTexture(p.heightTexturePropertyID);
			}
		}

		/// <summary>
		/// Create RenderTexture and return.
		/// </summary>
		/// <param name="baseTex">Base texture.</param>
		/// <param name="propertyID">Shader property id.</param>
		/// <param name="material">material.</param>
		private RenderTexture SetupRenderTexture(Texture baseTex, int propertyID, Material material)
		{
			var rt = new RenderTexture(baseTex.width, baseTex.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			rt.filterMode = baseTex.filterMode;
			Graphics.Blit(baseTex, rt);
			material.SetTexture(propertyID, rt);
			return rt;
		}

		/// <summary>
		/// Creates a rendertexture and set the material.
		/// </summary>
		private void SetRenderTexture()
		{
			foreach(var p in paintSet)
			{
				if(p.useMainPaint)
				{
					if(p.MainTexture)
						p.PaintMainTexture = SetupRenderTexture(p.MainTexture, p.mainTexturePropertyID, p.material);
					else
						Debug.LogWarning("To take advantage of the main texture paint must set main texture to materials.");
				}
				if(p.useNormalPaint)
				{
					if(p.NormalTexture)
						p.PaintNormalTexture = SetupRenderTexture(p.NormalTexture, p.normalTexturePropertyID, p.material);
					else
						Debug.LogWarning("To take advantage of the normal map paint must set normal map to materials.");
				}
				if(p.useHeightPaint)
				{
					if(p.HeightTexture)
						p.PaintHeightTexture = SetupRenderTexture(p.HeightTexture, p.heightTexturePropertyID, p.material);
					else
						Debug.LogWarning("To take advantage of the height map paint must set height map to materials.");
				}
			}
		}

		/// <summary>
		/// Rendertexture release process.
		/// </summary>
		private void ReleaseRenderTexture()
		{
			foreach(var p in paintSet)
			{
				if(RenderTexture.active != p.PaintMainTexture && p.PaintMainTexture && p.PaintMainTexture.IsCreated())
					p.PaintMainTexture.Release();
				if(RenderTexture.active != p.PaintNormalTexture && p.PaintNormalTexture && p.PaintNormalTexture.IsCreated())
					p.PaintNormalTexture.Release();
				if(RenderTexture.active != p.PaintHeightTexture && p.PaintHeightTexture && p.PaintHeightTexture.IsCreated())
					p.PaintHeightTexture.Release();
			}
		}

		/// <summary>
		/// To set the data needed to paint shader.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		private void SetPaintMainData(Brush brush, Vector2 uv)
		{
			_paintMainMaterial.SetVector(_paintUVPropertyID, uv);
			_paintMainMaterial.SetTexture(_brushTexturePropertyID, brush.BrushTexture);
			_paintMainMaterial.SetFloat(_brushScalePropertyID, brush.Scale);
			_paintMainMaterial.SetFloat(_brushRotatePropertyID, brush.RotateAngle);
			_paintMainMaterial.SetVector(_brushColorPropertyID, brush.Color);

			foreach(var key in _paintMainMaterial.shaderKeywords)
				_paintMainMaterial.DisableKeyword(key);
			switch(brush.ColorBlending)
			{
				case Brush.ColorBlendType.UseColor:
					_paintMainMaterial.EnableKeyword(COLOR_BLEND_USE_CONTROL);
					break;

				case Brush.ColorBlendType.UseBrush:
					_paintMainMaterial.EnableKeyword(COLOR_BLEND_USE_BRUSH);
					break;

				case Brush.ColorBlendType.Neutral:
					_paintMainMaterial.EnableKeyword(COLOR_BLEND_NEUTRAL);
					break;

				case Brush.ColorBlendType.AlphaOnly:
					_paintMainMaterial.EnableKeyword(COLOR_BLEND_ALPHA_ONLY);
					break;

				default:
					_paintMainMaterial.EnableKeyword(COLOR_BLEND_USE_CONTROL);
					break;
			}
		}

		/// <summary>
		/// To set the data needed to normal map paint shader
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		/// <param name="erase"></param>
		private void SetPaintNormalData(Brush brush, Vector2 uv, bool erase)
		{
			_paintNormalMaterial.SetVector(_paintUVPropertyID, uv);
			_paintNormalMaterial.SetTexture(_brushTexturePropertyID, brush.BrushTexture);
			_paintNormalMaterial.SetTexture(_brushNormalTexturePropertyID, brush.BrushNormalTexture);
			_paintNormalMaterial.SetFloat(_brushScalePropertyID, brush.Scale);
			_paintNormalMaterial.SetFloat(_brushRotatePropertyID, brush.RotateAngle);
			_paintNormalMaterial.SetFloat(_brushNormalBlendPropertyID, brush.NormalBlend);

			foreach(var key in _paintNormalMaterial.shaderKeywords)
				_paintNormalMaterial.DisableKeyword(key);
			switch(brush.NormalBlending)
			{
				case Brush.NormalBlendType.UseBrush:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_USE_BRUSH);
					break;

				case Brush.NormalBlendType.Add:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_ADD);
					break;

				case Brush.NormalBlendType.Sub:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_SUB);
					break;

				case Brush.NormalBlendType.Min:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_MIN);
					break;

				case Brush.NormalBlendType.Max:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_MAX);
					break;

				default:
					_paintNormalMaterial.EnableKeyword(NORMAL_BLEND_USE_BRUSH);
					break;
			}

			switch(erase)
			{
				case true:
					_paintNormalMaterial.EnableKeyword(DXT5NM_COMPRESS_UNUSE);
					break;
				case false:
					_paintNormalMaterial.EnableKeyword(DXT5NM_COMPRESS_USE);
					break;
			}
		}

		/// <summary>
		/// To set the data needed to height map paint shader.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		private void SetPaintHeightData(Brush brush, Vector2 uv)
		{
			_paintHeightMaterial.SetVector(_paintUVPropertyID, uv);
			_paintHeightMaterial.SetTexture(_brushTexturePropertyID, brush.BrushTexture);
			_paintHeightMaterial.SetTexture(_brushHeightTexturePropertyID, brush.BrushHeightTexture);
			_paintHeightMaterial.SetFloat(_brushScalePropertyID, brush.Scale);
			_paintHeightMaterial.SetFloat(_brushRotatePropertyID, brush.RotateAngle);
			_paintHeightMaterial.SetFloat(_brushHeightBlendPropertyID, brush.HeightBlend);
			_paintHeightMaterial.SetVector(_brushHeightColorPropertyID, brush.Color);

			foreach(var key in _paintHeightMaterial.shaderKeywords)
				_paintHeightMaterial.DisableKeyword(key);
			switch(brush.HeightBlending)
			{
				case Brush.HeightBlendType.UseBrush:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_USE_BRUSH);
					break;

				case Brush.HeightBlendType.Add:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_ADD);
					break;

				case Brush.HeightBlendType.Sub:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_SUB);
					break;

				case Brush.HeightBlendType.Min:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_MIN);
					break;

				case Brush.HeightBlendType.Max:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_MAX);
					break;

				case Brush.HeightBlendType.ColorRGB_HeightA:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_COLOR_RGB_HEIGHT_A);
					break;

				default:
					_paintHeightMaterial.EnableKeyword(HEIGHT_BLEND_ADD);
					break;
			}
		}

		/// <summary>
		/// Get an eraser brush.
		/// </summary>
		/// <param name="brush">A brush that becomes the shape of an eraser.</param>
		/// <param name="paintSet">Paint information per material.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		/// <param name="useMainPaint">Whether paint is effective.</param>
		/// <param name="useNormalPaint">Whether paint is effective.</param>
		/// <param name="useHeightpaint">Whether paint is effective.</param>
		/// <returns></returns>
		private Brush GetEraser(Brush brush, PaintSet paintSet, Vector2 uv, bool useMainPaint, bool useNormalPaint, bool useHeightpaint)
		{
			var b = brush.Clone() as Brush;
			b.Color = Color.white;
			b.ColorBlending = Brush.ColorBlendType.UseBrush;
			b.NormalBlending = Brush.NormalBlendType.UseBrush;
			b.HeightBlending = Brush.HeightBlendType.UseBrush;
			b.NormalBlend = 1f;
			b.HeightBlend = 1f;

			if(useMainPaint)
			{
				var rt = RenderTexture.GetTemporary(brush.BrushTexture.width, brush.BrushTexture.height);
				GrabArea.Clip(brush.BrushTexture, brush.Scale, paintSet.MainTexture, uv, brush.RotateAngle, GrabArea.GrabTextureWrapMode.Clamp, rt);
				b.BrushTexture = rt;
			}
			if(useNormalPaint)
			{
				var rt = RenderTexture.GetTemporary(brush.BrushNormalTexture.width, brush.BrushNormalTexture.height);
				GrabArea.Clip(brush.BrushNormalTexture, brush.Scale, paintSet.NormalTexture, uv, brush.RotateAngle, GrabArea.GrabTextureWrapMode.Clamp, rt, false);
				b.BrushNormalTexture = rt;
			}
			if(useHeightpaint)
			{
				var rt = RenderTexture.GetTemporary(brush.BrushHeightTexture.width, brush.BrushHeightTexture.height);
				GrabArea.Clip(brush.BrushHeightTexture, brush.Scale, paintSet.HeightTexture, uv, brush.RotateAngle, GrabArea.GrabTextureWrapMode.Clamp, rt, false);
				b.BrushHeightTexture = rt;
			}

			if(_eraserDebug)
			{
				if(!_debugEraserMainView && useMainPaint)
					_debugEraserMainView = new RenderTexture(b.BrushTexture.width, b.BrushTexture.height, 0);
				if(!_debugEraserNormalView && useNormalPaint)
					_debugEraserNormalView = new RenderTexture(b.BrushNormalTexture.width, b.BrushNormalTexture.height, 0);
				if(!_debugEraserHeightView && useHeightpaint)
					_debugEraserHeightView = new RenderTexture(b.BrushHeightTexture.width, b.BrushHeightTexture.height, 0);

				if(useMainPaint)
					Graphics.Blit(b.BrushTexture, _debugEraserMainView);
				if(useNormalPaint)
					Graphics.Blit(b.BrushNormalTexture, _debugEraserNormalView);
				if(useHeightpaint)
					Graphics.Blit(b.BrushHeightTexture, _debugEraserHeightView);
			}

			return b;
		}

		/// <summary>
		/// Release the RenderTexture for the eraser.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="useMainPaint">Whether paint is effective.</param>
		/// <param name="useNormalPaint">Whether paint is effective.</param>
		/// <param name="useHeightpaint">Whether paint is effective.</param>
		private void ReleaseEraser(Brush brush, bool useMainPaint, bool useNormalPaint, bool useHeightpaint)
		{
			if(useMainPaint && brush.BrushTexture is RenderTexture)
				RenderTexture.ReleaseTemporary(brush.BrushTexture as RenderTexture);

			if(useNormalPaint && brush.BrushNormalTexture is RenderTexture)
				RenderTexture.ReleaseTemporary(brush.BrushNormalTexture as RenderTexture);

			if(useHeightpaint && brush.BrushHeightTexture is RenderTexture)
				RenderTexture.ReleaseTemporary(brush.BrushHeightTexture as RenderTexture);
		}

		#endregion PrivateMethod

		#region PublicMethod

		/// <summary>
		/// Paint processing that UV coordinates to the specified.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		/// <returns>The success or failure of the paint.</returns>
		public bool PaintUVDirect(Brush brush, Vector2 uv, Func<PaintSet, bool> materialSelector = null)
		{
			#region ErrorCheck

			if(brush == null)
			{
				Debug.LogError("Do not set the brush.");
				_eraseFlag = false;
				return false;
			}

			#endregion ErrorCheck

			if(OnPaintStart != null)
			{
				brush = brush.Clone() as Brush;
				OnPaintStart(this, brush);
			}

			var set = materialSelector == null ? paintSet : paintSet.Where(materialSelector);
			foreach(var p in set)
			{
				var mainPaintConditions = p.useMainPaint && brush.BrushTexture && p.PaintMainTexture && p.PaintMainTexture.IsCreated();
				var normalPaintConditions = p.useNormalPaint && brush.BrushNormalTexture && p.PaintNormalTexture && p.PaintNormalTexture.IsCreated();
				var heightPaintConditions = p.useHeightPaint && brush.BrushHeightTexture && p.PaintHeightTexture && p.PaintHeightTexture.IsCreated();

				if(_eraseFlag)
					brush = GetEraser(brush, p, uv, mainPaintConditions, normalPaintConditions, heightPaintConditions);

                if (mainPaintConditions)
                {
                    var mainPaintTextureBuffer = RenderTexture.GetTemporary(p.PaintMainTexture.width, p.PaintMainTexture.height, 0,
                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                    SetPaintMainData(brush, uv);
                    Graphics.Blit(p.PaintMainTexture, mainPaintTextureBuffer, _paintMainMaterial);
                    Graphics.Blit(mainPaintTextureBuffer, p.PaintMainTexture);
                    p.material.SetTexture(BaseMap, p.PaintMainTexture);
                    OnRenderTextureUpdated?.Invoke(p, p.PaintMainTexture);
                    RenderTexture.ReleaseTemporary(mainPaintTextureBuffer);
                }

                if (normalPaintConditions)
				{
					var normalPaintTextureBuffer = RenderTexture.GetTemporary(p.PaintNormalTexture.width, p.PaintNormalTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					SetPaintNormalData(brush, uv, _eraseFlag);
					Graphics.Blit(p.PaintNormalTexture, normalPaintTextureBuffer, _paintNormalMaterial);
					Graphics.Blit(normalPaintTextureBuffer, p.PaintNormalTexture);
					RenderTexture.ReleaseTemporary(normalPaintTextureBuffer);
				}

				if(heightPaintConditions)
				{
					var heightPaintTextureBuffer = RenderTexture.GetTemporary(p.PaintHeightTexture.width, p.PaintHeightTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					SetPaintHeightData(brush, uv);
					Graphics.Blit(p.PaintHeightTexture, heightPaintTextureBuffer, _paintHeightMaterial);
					Graphics.Blit(heightPaintTextureBuffer, p.PaintHeightTexture);
					RenderTexture.ReleaseTemporary(heightPaintTextureBuffer);
				}

				if(_eraseFlag)
					ReleaseEraser(brush, mainPaintConditions, normalPaintConditions, heightPaintConditions);
			}

			if(OnPaintEnd != null)
				OnPaintEnd(this);

			_eraseFlag = false;
			return true;
		}

		/// <summary>
		/// Paint of points close to the given world-space position on the Mesh surface.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="worldPos">Approximate point.</param>
		/// <param name="renderCamera">Camera to use to render the object.</param>
		/// <returns>The success or failure of the paint.</returns>
		public bool PaintNearestTriangleSurface(Brush brush, Vector3 worldPos, Func<PaintSet, bool> materialSelector = null, Camera renderCamera = null)
		{
			var p = transform.worldToLocalMatrix.MultiplyPoint(worldPos);
			var pd = MeshOperator.NearestLocalSurfacePoint(p);

			return Paint(brush, transform.localToWorldMatrix.MultiplyPoint(pd), materialSelector, renderCamera);
		}

		/// <summary>
		/// Paint processing that use world-space surface position.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="worldPos">Point on object surface (world-space).</param>
		/// <param name="renderCamera">Camera to use to render the object.</param>
		/// <returns>The success or failure of the paint.</returns>
		public bool Paint(Brush brush, Vector3 worldPos, Func<PaintSet, bool> materialSelector = null, Camera renderCamera = null)
		{
			Vector2 uv;

			if(!renderCamera)
				renderCamera = _camera;

			Vector3 p = transform.InverseTransformPoint(worldPos);
			Matrix4x4 mvp = renderCamera.projectionMatrix * renderCamera.worldToCameraMatrix * transform.localToWorldMatrix;
			if(MeshOperator.LocalPointToUV(p, mvp, out uv))
				return PaintUVDirect(brush, uv, materialSelector);
			else
			{
				//Debug.LogWarning("Could not get the point on the surface.");
				return PaintNearestTriangleSurface(brush, worldPos, materialSelector, renderCamera);
			}
		}

		/// <summary>
		/// Paint processing that use raycast hit data.
		/// Must MeshCollider is set to the canvas.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="hitInfo">Raycast hit info.</param>
		/// <returns>The success or failure of the paint.</returns>
		public bool Paint(Brush brush, RaycastHit hitInfo, Func<PaintSet, bool> materialSelector = null)
		{
			if(hitInfo.collider)
			{
				if (hitInfo.collider.GetComponent<RenderTexture>() != null)
				
				if(hitInfo.collider is MeshCollider)
					return PaintUVDirect(brush, hitInfo.textureCoord, materialSelector);
				Debug.LogWarning("If you want to paint using a RaycastHit, need set MeshCollider for object.");
				return PaintNearestTriangleSurface(brush, hitInfo.point, materialSelector);
			}
			return false;
		}

		/// <summary>
		/// Erase processing that UV coordinates to the specified.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="uv">UV coordinates for the hit location.</param>
		/// <returns>The success or failure of the erase.</returns>
		public bool EraseUVDirect(Brush brush, Vector2 uv, Func<PaintSet, bool> materialSelector = null)
		{
			_eraseFlag = true;
			return PaintUVDirect(brush, uv, materialSelector);
		}

		/// <summary>
		/// Erase of points close to the given world-space position on the Mesh surface.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="worldPos">Approximate point.</param>
		/// <param name="renderCamera">Camera to use to render the object.</param>
		/// <returns>The success or failure of the erase.</returns>
		public bool EraseNearestTriangleSurface(Brush brush, Vector3 worldPos, Func<PaintSet, bool> materialSelector = null, Camera renderCamera = null)
		{
			_eraseFlag = true;
			return PaintNearestTriangleSurface(brush, worldPos, materialSelector, renderCamera);
		}

		/// <summary>
		/// Erase processing that use world-space surface position.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="worldPos">Point on object surface (world-space).</param>
		/// <param name="renderCamera">Camera to use to render the object.</param>
		/// <returns>The success or failure of the erase.</returns>
		public bool Erase(Brush brush, Vector3 worldPos, Func<PaintSet, bool> materialSelector = null, Camera renderCamera = null)
		{
			_eraseFlag = true;
			return Paint(brush, worldPos, materialSelector, renderCamera);
		}

		/// <summary>
		/// Erase processing that use raycast hit data.
		/// Must MeshCollider is set to the canvas.
		/// </summary>
		/// <param name="brush">Brush data.</param>
		/// <param name="hitInfo">Raycast hit info.</param>
		/// <returns>The success or failure of the erase.</returns>
		public bool Erase(Brush brush, RaycastHit hitInfo, Func<PaintSet, bool> materialSelector = null)
		{
			_eraseFlag = true;
			return Paint(brush, hitInfo, materialSelector);
		}

		/// <summary>
		/// To reset the paint.
		/// </summary>
		public void ResetPaint()
		{
			ReleaseRenderTexture();
			SetRenderTexture();
			if(OnInitializedAfter != null)
				OnInitializedAfter(this);
		}

		/// <summary>
		/// To get the original main texture.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Original main texture.</returns>
		public Texture GetMainTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.MainTexture;
		}

		/// <summary>
		/// To get the main texture in paint.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Main texture in paint.</returns>
		public RenderTexture GetPaintMainTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.PaintMainTexture;
		}

		/// <summary>
		/// Set paint texture.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <param name="newTexture">New rendertexture.</param>
		public void SetPaintMainTexture(string materialName, RenderTexture newTexture)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
			{
				Debug.LogError("Failed to set texture.");
				return;
			}
			data.PaintMainTexture = newTexture;
			
			data.material.SetTexture(data.mainTextureName, data.PaintMainTexture);
			data.useMainPaint = true;
		}

		/// <summary>
		/// To get the original normal map.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Original normal map.</returns>
		public Texture GetNormalTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.NormalTexture;
		}

		/// <summary>
		/// To get the paint in normal map.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Normal map in paint.</returns>
		public RenderTexture GetPaintNormalTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.PaintNormalTexture;
		}

		/// <summary>
		/// Set paint texture.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <param name="newTexture">New rendertexture.</param>
		public void SetPaintNormalTexture(string materialName, RenderTexture newTexture)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
			{
				Debug.LogError("Failed to set texture.");
				return;
			}
			data.PaintNormalTexture = newTexture;
			data.material.SetTexture(data.normalTextureName, data.PaintNormalTexture);
			data.useNormalPaint = true;
		}

		/// <summary>
		/// To get the original height map.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Original height map.</returns>
		public Texture GetHeightTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.HeightTexture;
		}

		/// <summary>
		/// To get the paint in height map.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <returns>Height map in paint.</returns>
		public RenderTexture GetPaintHeightTexture(string materialName)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
				return null;
			return data.PaintHeightTexture;
		}

		/// <summary>
		/// Set paint texture.
		/// </summary>
		/// <param name="materialName">Material name.</param>
		/// <param name="newTexture">New rendertexture.</param>
		public void SetPaintHeightTexture(string materialName, RenderTexture newTexture)
		{
			materialName = materialName.Replace(" (Instance)", "");
			var data = paintSet.FirstOrDefault(p => p.material.name.Replace(" (Instance)", "") == materialName);
			if(data == null)
			{
				Debug.LogError("Failed to set texture.");
				return;
			}
			data.PaintHeightTexture = newTexture;
			data.material.SetTexture(data.heightTextureName, data.PaintHeightTexture);
			data.useHeightPaint = true;
		}

		#endregion PublicMethod

		#region CustomEditor

#if UNITY_EDITOR

		[CustomEditor(typeof(InkCanvas))]
		[CanEditMultipleObjects]
		private class InkCanvasInspectorExtension : Editor
		{
			private Renderer renderer;
			private Material[] materials;
			private List<bool> foldOut;

			public override void OnInspectorGUI()
			{
				var instance = target as InkCanvas;
				if(instance.paintSet == null)
					instance.paintSet = new List<PaintSet>();

				if(renderer == null)
					renderer = instance.GetComponent<Renderer>();
				if(materials == null)
					materials = renderer.sharedMaterials;
				if(foldOut == null)
					foldOut = new List<bool>();

				if(instance.paintSet.Count < materials.Length)
				{
					for(int i = instance.paintSet.Count; i < materials.Length; ++i)
						instance.paintSet.Add(new PaintSet
						{
							mainTextureName = "_MainTex",
							normalTextureName = "_BumpMap",
							heightTextureName = "_ParallaxMap",
							useMainPaint = true,
							useNormalPaint = false,
							useHeightPaint = false,
						});
					foldOut.Clear();
				}

				if(instance.paintSet.Count > materials.Length)
				{
					instance.paintSet.RemoveRange(materials.Length, instance.paintSet.Count - materials.Length);
					foldOut.Clear();
				}

				if(foldOut.Count < instance.paintSet.Count)
					for(int i = foldOut.Count; i < instance.paintSet.Count; ++i)
						foldOut.Add(true);

				EditorGUILayout.Space();

				if(EditorApplication.isPlaying)
				{
					#region PlayModeOperation

					EditorGUILayout.HelpBox("Can not change while playing.\n but you can saved painted texture.", MessageType.Info);
					for(int i = 0; i < instance.paintSet.Count; ++i)
					{
						if(foldOut[i] = Foldout(foldOut[i], string.Format("Material \"{0}\"", materials[i].name)))
						{
							EditorGUILayout.BeginVertical("ProgressBarBack");
							var backColorBuf = GUI.backgroundColor;
							GUI.backgroundColor = Color.green;

							var paintSet = instance.paintSet[i];

							if(paintSet.PaintMainTexture != null && GUILayout.Button("Save main texture"))
								SaveRenderTextureToPNG(paintSet.MainTexture != null ? paintSet.MainTexture.name : "main_texture", paintSet.PaintMainTexture);

							if(instance.paintSet[i].PaintNormalTexture != null && GUILayout.Button("Save normal texture"))
								//TODO:https://github.com/EsProgram/InkPainter/issues/13
								SaveRenderTextureToPNG(paintSet.NormalTexture != null ? paintSet.NormalTexture.name : "normal_texture", paintSet.PaintNormalTexture);

							if(instance.paintSet[i].PaintHeightTexture != null && GUILayout.Button("Save height texture"))
								SaveRenderTextureToPNG(paintSet.HeightTexture != null ? paintSet.HeightTexture.name : "height_texture", paintSet.PaintHeightTexture);

							GUI.backgroundColor = backColorBuf;
							EditorGUILayout.EndVertical();
						}
					}
					EditorGUILayout.Space();
					instance._eraserDebug = EditorGUILayout.Toggle("Eracer debug option", instance._eraserDebug);
					if(instance._eraserDebug)
					{
						if(GUILayout.Button("Save eracer main texture"))
							SaveRenderTextureToPNG("eracer_main", instance._debugEraserMainView);
						if(GUILayout.Button("Save eracer normal texture"))
							SaveRenderTextureToPNG("eracer_normal", instance._debugEraserNormalView);
						if(GUILayout.Button("Save eracer height texture"))
							SaveRenderTextureToPNG("eracer_height", instance._debugEraserHeightView);
					}

					#endregion PlayModeOperation
				}
				else
				{
					#region Property Setting

					for(int i = 0; i < instance.paintSet.Count; ++i)
					{
						if(foldOut[i] = Foldout(foldOut[i], string.Format("Material \"{0}\"", materials[i].name)))
						{
							EditorGUI.indentLevel = 0;
							EditorGUILayout.BeginVertical("ProgressBarBack");

							//MainPaint
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].useMainPaint = EditorGUILayout.Toggle("Use Main Paint", instance.paintSet[i].useMainPaint);
							if(EditorGUI.EndChangeCheck())
								ChangeValue(i, "Use Main Paint", p => p.useMainPaint = instance.paintSet[i].useMainPaint);
							if(instance.paintSet[i].useMainPaint)
							{
								EditorGUI.indentLevel++;
								EditorGUI.BeginChangeCheck();
								instance.paintSet[i].mainTextureName = EditorGUILayout.TextField("MainTexture Property Name", instance.paintSet[i].mainTextureName);
								if(EditorGUI.EndChangeCheck())
									ChangeValue(i, "Main Texture Name", p => p.mainTextureName = instance.paintSet[i].mainTextureName);
								EditorGUI.indentLevel--;
							}

							//NormalPaint
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].useNormalPaint = EditorGUILayout.Toggle("Use NormalMap Paint", instance.paintSet[i].useNormalPaint);
							if(EditorGUI.EndChangeCheck())
								ChangeValue(i, "Use Normal Paint", p => p.useNormalPaint = instance.paintSet[i].useNormalPaint);
							if(instance.paintSet[i].useNormalPaint)
							{
								EditorGUI.indentLevel++;
								EditorGUI.BeginChangeCheck();
								instance.paintSet[i].normalTextureName = EditorGUILayout.TextField("NormalMap Property Name", instance.paintSet[i].normalTextureName);
								if(EditorGUI.EndChangeCheck())
									ChangeValue(i, "Normal Texture Name", p => p.normalTextureName = instance.paintSet[i].normalTextureName);
								EditorGUI.indentLevel--;
							}

							//HeightPaint
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].useHeightPaint = EditorGUILayout.Toggle("Use HeightMap Paint", instance.paintSet[i].useHeightPaint);
							if(EditorGUI.EndChangeCheck())
								ChangeValue(i, "Use Height Paint", p => p.useHeightPaint = instance.paintSet[i].useHeightPaint);
							if(instance.paintSet[i].useHeightPaint)
							{
								EditorGUI.indentLevel++;
								EditorGUI.BeginChangeCheck();
								instance.paintSet[i].heightTextureName = EditorGUILayout.TextField("HeightMap Property Name", instance.paintSet[i].heightTextureName);
								if(EditorGUI.EndChangeCheck())
									ChangeValue(i, "Height Texture Name", p => p.heightTextureName = instance.paintSet[i].heightTextureName);
								EditorGUI.indentLevel--;
							}

							EditorGUILayout.EndVertical();
							EditorGUI.indentLevel = 0;
						}
					}

					#endregion Property Setting
				}
			}

			private void SaveRenderTextureToPNG(string textureName, RenderTexture renderTexture, Action<TextureImporter> importAction = null)
			{
				string path = EditorUtility.SaveFilePanel("Save to png", Application.dataPath, textureName + "_painted.png", "png");
				if(path.Length != 0)
				{
					var newTex = new Texture2D(renderTexture.width, renderTexture.height);
					RenderTexture.active = renderTexture;
					newTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
					newTex.Apply();

					byte[] pngData = newTex.EncodeToPNG();
					if(pngData != null)
					{
						File.WriteAllBytes(path, pngData);
						AssetDatabase.Refresh();
						var importer = AssetImporter.GetAtPath(path) as TextureImporter;
						if(importAction != null)
							importAction(importer);
					}

					Debug.Log(path);
				}
			}

			private void ChangeValue(int paintSetIndex, string recordName, Action<PaintSet> assign)
			{
				Undo.RecordObjects(targets, "Change " + recordName);
				foreach(var t in targets.Where(_t => _t is InkCanvas).Select(_t => _t as InkCanvas))
					if(t.paintSet.Count > paintSetIndex)
					{
						assign(t.paintSet[paintSetIndex]);
						EditorUtility.SetDirty(t);
					}
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

			public bool Foldout(bool foldout, string content)
			{
				var style = new GUIStyle("ShurikenModuleTitle");
				style.font = new GUIStyle(EditorStyles.label).font;
				style.border = new RectOffset(1, 7, 4, 4);
				style.fixedHeight = 28;
				style.contentOffset = new Vector2(20f, -2f);

				var rect = GUILayoutUtility.GetRect(16f, 22f, style);
				GUI.Box(rect, content, style);

				var e = Event.current;

				var toggleRect = new Rect(rect.x + 4f, rect.y + 5f, 13f, 13f);
				if(e.type == EventType.Repaint)
				{
					EditorStyles.foldout.Draw(toggleRect, false, false, foldout, false);
				}

				if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
				{
					foldout = !foldout;
					e.Use();
				}

				return foldout;
			}
		}

#endif

		#endregion CustomEditor
	}
}