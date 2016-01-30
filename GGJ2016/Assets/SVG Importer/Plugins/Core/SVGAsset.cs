// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;

namespace SVGImporter 
{
    using Rendering;
    using Geometry;
    using Utils;

    public enum SVGUseGradients
    {
        Always,
        Auto,
        Never
    }

    public enum SVGMeshCompression
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum SVGAssetFormat
    {
        Opaque = 0,
        Transparent = 1,
        uGUI = 2
    }

    public class SVGAsset : ScriptableObject
    {
        [FormerlySerializedAs("lastTimeModified")]
        [SerializeField]
        protected long _lastTimeModified;

        [FormerlySerializedAs("documentAsset")]
        [SerializeField]
        protected SVGDocumentAsset _documentAsset;

        [FormerlySerializedAs("sharedMesh")]
        [SerializeField]
        protected Mesh _sharedMesh;

        /// <summary>
        /// Returns the shared mesh of the SVG Asset. (Read Only)
        /// </summary>
        public Mesh sharedMesh
        {
            get {
    #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    return runtimeMesh;
                } else {
                    _runtimeMesh = null;
                    return _sharedMesh;
                }
    #else
                return runtimeMesh;
    #endif
            }
        }

        public bool isOpaque
        {
            get {
                if(_format == SVGAssetFormat.Transparent || _format == SVGAssetFormat.uGUI) return false;
                if(_sharedShaders == null || _sharedShaders.Length == 0) return true;
                for(int i = 0; i < _sharedShaders.Length; i++)
                {
                    if(string.IsNullOrEmpty(_sharedShaders[i])) continue;
                    if(_sharedShaders[i].ToLower().Contains("opaque")) return true;
                }

                return false;
            }
        }
        
        /// <summary>
        /// Returns the instanced mesh of the SVG Asset. (Read Only)
        /// </summary>
        public Mesh mesh
        {
            get {
                Mesh sharedMeshReference = sharedMesh;
                if(sharedMeshReference == null)
                    return null;
                Mesh clonedMesh = SVGMeshUtils.Clone(sharedMeshReference);
                if(clonedMesh != null)
                {
                    clonedMesh.name += " Instance "+clonedMesh.GetInstanceID();                    
                }
                return clonedMesh;
            }
        }

        protected Mesh _runtimeMesh;
        protected Mesh runtimeMesh
        {
            get {
                if(_runtimeMesh == null)
                {
                    if(_sharedGradients == null || _sharedGradients.Length == 0)
                    {
                        _runtimeMesh = _sharedMesh;
                    } else {
                        Dictionary<int, int> gradientCache = new Dictionary<int, int>();
                        CCGradient[] gradients = new CCGradient[_sharedGradients.Length];
                        for(int i = 0; i < _sharedGradients.Length; i++)
                        {
                            if(_sharedGradients[i] == null)
                                continue;
                            gradients[i] = SVGAtlas.Instance.AddGradient(_sharedGradients[i].Clone());
                            gradientCache.Add(_sharedGradients[i].index, gradients[i].index);
                        }

                        _runtimeMesh = SVGMeshUtils.Clone(_sharedMesh);
                        if(hasGradients)
                        {
                            if(_runtimeMesh.uv2 != null && _runtimeMesh.uv2.Length > 0)
                            {
                                Vector2[] uv2 = _runtimeMesh.uv2;
                                for(int i = 0; i < uv2.Length; i++)
                                {
                                    uv2[i].x = (float)gradientCache[(int)uv2[i].x];
                                }
                                _runtimeMesh.uv2 = uv2;
                            }
                        }

                        if(SVGAtlas.Instance.atlasTextures != null && SVGAtlas.Instance.atlasTextures.Count > 0)
                        {
                            SVGAtlas.Instance.InitMaterials();
                        }

//                        Vector4 vectorParams = new Vector4((float)SVGAtlas.Instance.atlasTextureWidth, (float)SVGAtlas.Instance.atlasTextureHeight, 
//                                                           (float)SVGAtlas.Instance.gradientWidth, (float)SVGAtlas.Instance.gradientHeight);

                        if(_sharedShaders != null && _sharedShaders.Length > 0)
                        {
                            _runtimeMaterials = new Material[_sharedShaders.Length];
                            string shaderName;
                            for(int i = 0; i < _sharedShaders.Length; i++)
                            {
                                if(_sharedShaders[i] == null)
                                    continue;

                                shaderName = _sharedShaders[i];
                                if(shaderName == SVGShader.SolidColorOpaque.name)
                                {
                                    _runtimeMaterials[i] = SVGAtlas.Instance.opaqueSolid;
                                } else if(shaderName == SVGShader.SolidColorAlphaBlended.name)
                                {
                                    _runtimeMaterials[i] = SVGAtlas.Instance.transparentSolid;
                                } else if(shaderName == SVGShader.GradientColorOpaque.name)
                                {
                                    _runtimeMaterials[i] = SVGAtlas.Instance.opaqueGradient;
                                } else if(shaderName == SVGShader.GradientColorAlphaBlended.name)
                                {
                                    _runtimeMaterials[i] = SVGAtlas.Instance.transparentGradient;
                                }
                            }
                        }
                    }                
                }
                return _runtimeMesh;
            }
        }

        protected UIVertex[] _runtimeUIMesh;
        protected UIVertex[] runtimeUIMesh
        {
            get {
                if(_runtimeUIMesh == null)
                    _runtimeUIMesh = CreateUIMesh(sharedMesh);
                return _runtimeUIMesh;
            }
        }

        /// <summary>
        /// Returns the shared UI Mesh of the SVG Asset. (Read Only)
        /// </summary>
        public UIVertex[] sharedUIMesh
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    _runtimeMesh = runtimeMesh;
                    return runtimeUIMesh;
                } else {
                    _runtimeUIMesh = null;
                    return CreateUIMesh(sharedMesh);
                }
                #else
                _runtimeMesh = runtimeMesh;
                return runtimeUIMesh;
                #endif
            }
        }

        //[FormerlySerializedAs("sharedUIMaterial")]
        //[SerializeField]
        protected Material _sharedUIMaterial;

        /// <summary>
        /// Returns the shared UI Material of the SVG Asset. (Read Only)
        /// </summary>
        public Material sharedUIMaterial
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    _runtimeMesh = runtimeMesh;
                    return SVGAtlas.Instance.ui;
                } else {
                    _runtimeMesh = null;
					return _editor_UIMaterial;
                }

                #else
                _runtimeMesh = runtimeMesh;
                return SVGAtlas.Instance.ui;
                #endif
            }
        }

        /// <summary>
        /// Returns the instanced UI Material of the SVG Asset. (Read Only)
        /// </summary>
        public Material uiMaterial
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    _runtimeMesh = runtimeMesh;
                    return CloneMaterial(SVGAtlas.Instance.ui);
                } else {
                    _runtimeMesh = null;
					return CloneMaterial(_editor_UIMaterial);
                }
                
                #else
                _runtimeMesh = runtimeMesh;
                return CloneMaterial(SVGAtlas.Instance.ui);
                #endif
            }
        }

        //[FormerlySerializedAs("sharedUIMaskMaterial")]
        //[SerializeField]
        protected Material _sharedUIMaskMaterial;

        /// <summary>
        /// Returns the shared UI Mask Material of the SVG Asset. (Read Only)
        /// </summary>
        public Material sharedUIMaskMaterial
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    _runtimeMesh = runtimeMesh;
                    return SVGAtlas.Instance.uiMask;
                } else {
                    _runtimeMesh = null;
					return _editor_UIMaskMaterial;
                }                
                #else
                _runtimeMesh = runtimeMesh;
                return SVGAtlas.Instance.uiMask;
                #endif
            }
        }

        /// <summary>
        /// Returns the instanced UI Mask Material of the SVG Asset. (Read Only)
        /// </summary>
        public Material uiMaskMaterial
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    _runtimeMesh = runtimeMesh;
                    return CloneMaterial(SVGAtlas.Instance.uiMask);
                } else {
                    _runtimeMesh = null;
					return CloneMaterial(_editor_UIMaskMaterial);
                }
                #else
                _runtimeMesh = runtimeMesh;
                return CloneMaterial(SVGAtlas.Instance.uiMask);
                #endif
            }
        }

        //[FormerlySerializedAs("sharedMaterials")]
        //[SerializeField]
        protected Material[] _sharedMaterials;

        /// <summary>
        /// Returns the shared materials of the SVG Asset. (Read Only)
        /// </summary>
        public Material[] sharedMaterials
        {
            get {
                #if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    return _runtimeMaterials;
                } else {
					_runtimeMaterials = null;
					return _editor_sharedMaterials;
                }                
                #else
                return _runtimeMaterials;
                #endif
            }
        }

        /// <summary>
        /// Returns the instanced materials of the SVG Asset. (Read Only)
        /// </summary>
        public Material[] materials
        {
            get {
                if(sharedMaterials == null)
                    return null;
                
                int sharedMaterialsLength = sharedMaterials.Length;
                Material[] materials = new Material[sharedMaterialsLength];
                for(int i = 0; i < sharedMaterialsLength; i++)
                {
                    materials[i] = CloneMaterial(sharedMaterials[i]);                    
                }
                return materials;
            }
        }

		const string _GradientColorKey = "_GradientColor";
		const string _GradientShapeKey = "_GradientShape";
		const string _ParamsKey = "_Params";
		public static void AssignMaterialGradients(Material material, Texture2D gradientAtlas, Texture2D gradientShape, int gradientWidth, int gradientHeight)
		{
			if(material == null)
				return;

			if(material.HasProperty(_GradientColorKey))
			{
				material.SetTexture(_GradientColorKey, gradientAtlas);
			}
			if(material.HasProperty(_GradientShapeKey))
			{
				material.SetTexture(_GradientShapeKey, gradientShape);
			}
			if(material.HasProperty(_ParamsKey) && gradientAtlas != null)
			{
				Vector4 materialParams = new Vector4(gradientAtlas.width, gradientAtlas.height, gradientWidth, gradientHeight);
				material.SetVector(_ParamsKey, materialParams);
			}
		}

		public static void AssignMaterialGradients(Material[] materials, Texture2D gradientAtlas, Texture2D gradientShape, int gradientWidth, int gradientHeight)
		{
			if(materials == null || materials.Length == 0)
				return;

			for(int i = 0; i < materials.Length; i++)
			{
				AssignMaterialGradients(materials[i], gradientAtlas, gradientShape, gradientWidth, gradientHeight);
			}
		}

        protected Material[] _runtimeMaterials;   

        //[FormerlySerializedAs("atlasTextures")]
        //[SerializeField]
        protected Texture2D[] _atlasTextures;

        /// <summary>
        /// Returns the references to the used gradient textures of the SVG Asset. (Read Only)
        /// </summary>
        public Texture2D[] atlasTextures
        {
            get {
#if UNITY_EDITOR
				if(UnityEditor.EditorApplication.isPlaying)
                {
					return _atlasTextures = SVGAtlas.Instance.atlasTextures.ToArray();
				} else {
					if(_atlasTextures == null || _atlasTextures.Length == 0)
					{
						Texture2D atlasTexture = SVGAtlas.GenerateGradientAtlasTexture(_sharedGradients, 64, 4);
						if(atlasTexture != null)
						{
							atlasTexture.hideFlags = HideFlags.DontSave;
							_atlasTextures = new Texture2D[]{atlasTexture};
						} else {
							_atlasTextures = new Texture2D[]{SVGAtlas.whiteTexture};
						}
					}
				}
#else
                return _atlasTextures = SVGAtlas.Instance.atlasTextures.ToArray();
#endif
                return _atlasTextures;
            }
        }

        /// <summary>
        /// Use antialiasing (Read Only)
        /// </summary>
        [FormerlySerializedAs("antialiasing")]
        [SerializeField]
        protected bool _antialiasing = false;
        public bool antialiasing
        {
            get {
                return _antialiasing;
            }
        }

        /// <summary>
        /// Antialiasing width, zero value turns antialiasing off (Read Only)
        /// </summary>
        [FormerlySerializedAs("antialiasingWidth")]
        [SerializeField]
        protected float _antialiasingWidth = 0f;
        public float antialiasingWidth
        {
            get {
                return _antialiasingWidth;
            }
        }

        [FormerlySerializedAs("generateCollider")]
        [SerializeField]
        protected bool _generateCollider = false;
        
        /// <summary>
        /// Returns if the asset has generated collider shape. (Read Only)
        /// </summary>
        public bool generateCollider
        {
            get {
                return _generateCollider;
            }
        }

        /// <summary>
        /// Keep the SVG file in the final build (Read Only)
        /// </summary>
        [FormerlySerializedAs("keepSVGFile")]
        [SerializeField]
        protected bool _keepSVGFile = true;
        public bool keepSVGFile
        {
            get {
                return _keepSVGFile;
            }
        }

        /// <summary>
        /// Trim the document canvas to object bounding box (Read Only)
        /// </summary>
        [FormerlySerializedAs("ignoreSVGCanvas")]
        [SerializeField]
        protected bool _ignoreSVGCanvas = true;
        public bool ignoreSVGCanvas
        {
            get {
                return _ignoreSVGCanvas;
            }
        }

        [FormerlySerializedAs("colliderShape")]
        [SerializeField]
        protected SVGPath[] _colliderShape;

        /// <summary>
        /// Returns the collider shape. (Read Only)
        /// </summary>
        public SVGPath[] colliderShape
        {
            get {
                return _colliderShape;
            }
        }

        [FormerlySerializedAs("format")]
        [SerializeField]
        protected SVGAssetFormat _format = SVGAssetFormat.Transparent;
        /// <summary>
        /// Returns the rendering format of the SVG Asset. (Read Only)
        /// </summary>
        public SVGAssetFormat format
        {
            get {
                return _format;
            }
        }
        
        [FormerlySerializedAs("useGradients")]
        [SerializeField]
        protected SVGUseGradients _useGradients = SVGUseGradients.Always;
        /// <summary>
        /// Returns if the mesh was compressed. (Read Only)
        /// </summary>
        public SVGUseGradients useGradients
        {
            get {
                return _useGradients;
            }
        }

        [FormerlySerializedAs("meshCompression")]
        [SerializeField]
        protected SVGMeshCompression _meshCompression = SVGMeshCompression.Off;
        /// <summary>
        /// Returns if the mesh was compressed. (Read Only)
        /// </summary>
        public SVGMeshCompression meshCompression
        {
            get {
                return _meshCompression;
            }
        }

        [FormerlySerializedAs("optimizeMesh")]
        [SerializeField]
        protected bool _optimizeMesh = true;
        
        /// <summary>
        /// Returns if the mesh is optimised for GPU. (Read Only)
        /// </summary>
        public bool optimizeMesh
        {
            get {
                return _optimizeMesh;
            }
        }

        [FormerlySerializedAs("generateNormals")]
        [SerializeField]
        protected bool _generateNormals = false;

        /// <summary>
        /// Returns if the mesh contains normals. (Read Only)
        /// </summary>
        public bool generateNormals
        {
            get {
                return _generateNormals;
            }
        }

        [FormerlySerializedAs("generateTangents")]
        [SerializeField]
        protected bool _generateTangents = false;
        
        /// <summary>
        /// Returns if the mesh contains tangents. (Read Only)
        /// </summary>
        public bool generateTangents
        {
            get {
                return _generateTangents;
            }
        }

        [FormerlySerializedAs("scale")]
        [SerializeField]
        protected float _scale = 0.01f;

        /// <summary>
        /// Returns the scale of the mesh relative to the SVG Asset. (Read Only)
        /// </summary>
        public float scale {
            get {
                return _scale;
            }
        }

        [FormerlySerializedAs("vpm")]
        [SerializeField]
        protected float _vpm = 1000f;

        /// <summary>
        /// Returns the number of vertices in the SVG Asset that correspond to one unit in world space. (Read Only)
        /// </summary>
        public float vpm
        {
            get {
                return _vpm;
            }
        }

        [FormerlySerializedAs("depthOffset")]
        [SerializeField]
        protected float _depthOffset = 0.01f;

        /// <summary>
        /// Returns the minimal z-offset in WorldSpace for Opaque Rendering. (Read Only)
        /// </summary>
        public float depthOffset
        {
            get {
                return _depthOffset;
            }
        }

        [FormerlySerializedAs("compressDepth")]
        [SerializeField]
        protected bool _compressDepth = true;

        /// <summary>
        /// Returns the compress overlapping objects to reduce z-offset requirements. (Read Only)
        /// </summary>
        public bool compressDepth
        {
            get {
                return _compressDepth;
            }
        }

        [FormerlySerializedAs("pivotPoint")]
        [SerializeField]
        protected Vector2 _pivotPoint = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Returns the location of the SVG Asset center point in the original Rect, specified in percents. (Read Only)
        /// </summary>
        public Vector2 pivotPoint
        {
            get {
                return _pivotPoint;
            }
        }

        [FormerlySerializedAs("customPivotPoint")]
        [SerializeField]
        protected bool _customPivotPoint = false;

        /// <summary>
        /// Returns the use of predefined pivot point or custom pivot point. (Read Only)
        /// </summary>
        public bool customPivotPoint
        {
            get {
                return _customPivotPoint;
            }
        }

		[FormerlySerializedAs("border")]
		[SerializeField]
		protected Vector4 _border = new Vector4(0f, 0f, 0f, 0f);
		
		/// <summary>
		/// Returns the 9-slice border. (Read Only)
        /// LEFT, BOTTOM, RIGHT, TOP
		/// </summary>
		public Vector4 border
		{
			get {
				return _border;
			}
		}

        [FormerlySerializedAs("sliceMesh")]
        [SerializeField]
        protected bool _sliceMesh = false;
        
        /// <summary>
        /// Returns if the mesh is sliced. (Read Only)
        /// </summary>
        public bool sliceMesh
        {
            get {
                return _sliceMesh;
            }
        }

		// temporary holder
        protected string _svgFile;

        /// <summary>
        /// Returns the original SVG text content available only in the Editor. (Read Only)
        /// </summary>
        public string svgFile
        {
            get {
                if(!string.IsNullOrEmpty(_svgFile))
                {
                    return _svgFile;
                } else {
                    if(_documentAsset != null)
                    {
                        return _documentAsset.svgFile;
                    } else {
                        return null;
                    }
                }
            }
        }

        [FormerlySerializedAs("sharedGradients")]
        [SerializeField]
        protected CCGradient[] _sharedGradients;

        /// <summary>
        /// Returns all the used gradients in the SVG Asset. (Read Only)
        /// </summary>
        public CCGradient[] sharedGradients {
            get {
                return _sharedGradients;
            }
        }

        [FormerlySerializedAs("sharedShaders")]
        [SerializeField]
        protected string[] _sharedShaders;

        /// <summary>
        /// Returns all the used shader names in the SVG Asset. (Read Only)
        /// </summary>
        public string[] sharedShaders {
            get {
                return _sharedShaders;
            }
        }

        /// <summary>
        /// Returns the bounding volume of the mesh of the SVG Asset. (Read Only)
        /// </summary>
        public Bounds bounds
        {
            get {
                if(_sharedMesh == null)
                    return new Bounds();

                return _sharedMesh.bounds;
            }
        }

        [FormerlySerializedAs("canvasRectangle")]
        [SerializeField]
        protected Rect _canvasRectangle;
        /// <summary>
        /// Returns the Original Canvas rectangle of the SVG Asset. (Read Only)
        /// </summary>
        public Rect canvasRectangle
        {
            get {
                return _canvasRectangle;
            }
        }

        /// <summary>
        /// Returns if the SVG Asset contains any gradients. (Read Only)
        /// </summary>
        public bool hasGradients
        {
            get {
                if(_sharedGradients == null || _sharedGradients.Length == 0) return false;
                return true;
            }
        }

        protected Material CloneMaterial(Material original)
        {
            if(original == null)
                return null;

            Material material = new Material(original.shader);
            material.CopyPropertiesFromMaterial(original);
            return material;
        }

        /// <summary>
        /// Returns the number of vertices in the mesh of the SVG Asset. (Read Only)
        /// </summary>
        public int uiVertexCount
        {
            get {
                if(_sharedMesh == null || _sharedMesh.triangles == null)
                    return 0;

                int trianglesCount = _sharedMesh.triangles.Length;
                return trianglesCount + (trianglesCount / 3);
            }
        }

        protected static UIVertex[] CreateUIMesh(Mesh inputMesh)
        {
            if(inputMesh == null)
                return new UIVertex[0];
            
            Vector3[] vertices = inputMesh.vertices;
            Color32[] colors = inputMesh.colors32;
            Vector2[] uv = inputMesh.uv;
            Vector2[] uv2 = inputMesh.uv2;
            
            int colorsLength = 0;
            if(colors != null)
                colorsLength = colors.Length;
            
            int uvLength = 0;
            if(uv != null)
                uvLength = uv.Length;
            
            int uv2Length = 0;
            if(uv2 != null)
                uv2Length = uv2.Length;
            
            int[] triangles = inputMesh.triangles;
            int trianglesCount = triangles.Length;
            UIVertex[] sharedUIMesh = new UIVertex[trianglesCount + (trianglesCount / 3)];
            
            UIVertex vertex = new UIVertex();        
            int currentQuad = 0;
            int currentTriangle = 0;
            for(int i = 0; i < trianglesCount; i += 3)
            {
                currentTriangle = triangles[i];
                vertex.position = vertices[currentTriangle];
                vertex.color = Color.white;
                
                if(currentTriangle < colorsLength)
                    vertex.color = colors[currentTriangle];
                if(currentTriangle < uvLength)
                    vertex.uv0 = uv[currentTriangle];
                if(currentTriangle < uv2Length)
                    vertex.uv1 = uv2[currentTriangle];
                sharedUIMesh[currentQuad++] = vertex;
                
                currentTriangle = triangles[i + 1];
                vertex.position = vertices[currentTriangle];
                if(currentTriangle < colorsLength)
                    vertex.color = colors[currentTriangle];
                if(currentTriangle < uvLength)
                    vertex.uv0 = uv[currentTriangle];
                if(currentTriangle < uv2Length)
                    vertex.uv1 = uv2[currentTriangle];
                sharedUIMesh[currentQuad++] = vertex;
                
                currentTriangle = triangles[i + 2];
                vertex.position = vertices[currentTriangle];
                if(currentTriangle < colorsLength)
                    vertex.color = colors[currentTriangle];
                if(currentTriangle < uvLength)
                    vertex.uv0 = uv[currentTriangle];
                if(currentTriangle < uv2Length)
                    vertex.uv1 = uv2[currentTriangle];
                sharedUIMesh[currentQuad++] = sharedUIMesh[currentQuad++] = vertex;                    
            }
            
            return (UIVertex[])sharedUIMesh.Clone();
        }

        void OnDisable()
        {
    		
        }
    	
        void OnDestroy()
        {
    		
        }	

    #if UNITY_EDITOR

        internal SVGDocumentAsset _editor_documentAsset
        {
            get {
                return _documentAsset;
            }
        }

        internal void _editor_ApplyChanges(bool importMultipleFiles = false)
        {
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
                return;

            if(_documentAsset != null)
            {
                _documentAsset.errors = null;
            }

            if(_sharedShaders != null)
                _sharedShaders = null;

            if(_sharedMesh != null)
            {
                Object.DestroyImmediate(_sharedMesh, true);
                _sharedMesh = null;
            }

            if(_atlasTextures != null && _atlasTextures.Length > 0)
            {
                for( int i = 0; i < _atlasTextures.Length; i++)
                {
                    if(_atlasTextures[i] == null)
                        continue;
                    
                    Object.DestroyImmediate(_atlasTextures[i], true);
                    _atlasTextures[i] = null;
                }
                _atlasTextures = null;
            }

            if(_sharedUIMaterial != null)
            {
                Object.DestroyImmediate(_sharedUIMaterial, true);
                _sharedUIMaterial = null;
            }

            if(_sharedUIMaskMaterial != null)
            {
                Object.DestroyImmediate(_sharedUIMaskMaterial, true);
                _sharedUIMaskMaterial = null;
            }

            if(_sharedMaterials != null && _sharedMaterials.Length > 0)
            {
                for( int i = 0; i < sharedMaterials.Length; i++)
                {
                    if(_sharedMaterials[i] == null)
                        continue;
                    
                    Object.DestroyImmediate(_sharedMaterials[i], true);
                    _sharedMaterials[i] = null;
                }
                _sharedMaterials = null;
            }

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if(assets != null && assets.Length > 0)
            {
                for(int i = 0; i < assets.Length; i++)
                {
                    if(assets[i] == null)
                        continue;
                    if(assets[i] == this)
                        continue;

                    if(assets[i] is SVGDocumentAsset)
                        continue;

                    DestroyImmediate(assets[i], true);
                }
            }

            _editor_LoadSVG();        

            // Create Document Asset
            if(_documentAsset == null)
            {
                _documentAsset = AddObjectToAsset<SVGDocumentAsset>(ScriptableObject.CreateInstance<SVGDocumentAsset>(), this, HideFlags.HideInHierarchy);
            }

            var svgAssetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var svgAssetImporter = UnityEditor.AssetImporter.GetAtPath(svgAssetPath);

            if(!string.IsNullOrEmpty(svgFile))
            {
                svgAssetImporter.userData = svgFile;
            }

            if(keepSVGFile)
            {
                _documentAsset.svgFile = svgAssetImporter.userData;
            } else {
                _documentAsset.svgFile = null;
            }
            
            //_documentAsset.infoString = _editor_UpdateInfo();

            // Save SVG Errors in Documet Asset
            if(SVGAssetImport.errors != null && SVGAssetImport.errors.Count > 0)
            {
                _documentAsset.errors = SVGAssetImport.errors.ToArray();

				bool critical = false;
				string errors = "";
				int errorsLength = _documentAsset.errors.Length;
				for(int i = 0; i < errorsLength; i++)
				{
					if(i < errorsLength - 1)
					{
						errors += _documentAsset.errors[i].ToString() +", ";
					} else {
						errors += _documentAsset.errors[i].ToString() +".";
					}

					if(_documentAsset.errors[i] == SVGError.CorruptedFile || 
					   _documentAsset.errors[i] == SVGError.Syntax)
					{
						critical = true;
					}
				}

				if(critical)
				{
					Debug.LogError ("SVGAsset: "+this.name+"\nerrors: "+errors+"\npath: "+UnityEditor.AssetDatabase.GetAssetPath(this)+"\n", this);
				} else {
					Debug.LogWarning ("SVGAsset: "+this.name+"\nerrors: "+errors+"\npath: "+UnityEditor.AssetDatabase.GetAssetPath(this)+"\n", this);
				}
            }

            UnityEditor.EditorUtility.SetDirty(_documentAsset);

            // Clean old values
            _svgFile = null;

            if(SVGAssetImport.errors != null)
            {
                SVGAssetImport.errors.Clear();
                SVGAssetImport.errors = null;
            }

            if(_sharedMesh != null && _sharedMesh.vertexCount > 0)
            {
                _sharedMesh.name = this.name;

                int vertexCount = _sharedMesh.vertexCount;
                UnityEditor.MeshUtility.SetMeshCompression(_sharedMesh, GetModelImporterMeshCompression(_meshCompression));
                if(_optimizeMesh) _sharedMesh.Optimize();
                if(_generateNormals)
                {
                    Vector3[] normals = new Vector3[vertexCount];
                    for(int i = 0; i < vertexCount; i++)
                    {
                        normals[i] = -Vector3.forward;
                    }
                    _sharedMesh.normals = normals;
                    if(_generateTangents)
                    {
                        Vector4[] tangents = new Vector4[vertexCount];
                        for(int i = 0; i < vertexCount; i++)
                        {
                            tangents[i] = new Vector4(-1f, 0f, 0f, -1f);
                        }
                        _sharedMesh.tangents = tangents;
                    }
                }
            }

            _lastTimeModified = System.DateTime.UtcNow.Ticks;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        internal UnityEditor.ModelImporterMeshCompression GetModelImporterMeshCompression(SVGMeshCompression meshCompression)
        {
            switch(meshCompression)
            {
                case SVGMeshCompression.Low:
                return UnityEditor.ModelImporterMeshCompression.Low;
                case SVGMeshCompression.Medium:
                    return UnityEditor.ModelImporterMeshCompression.Medium;
                case SVGMeshCompression.High:
                    return UnityEditor.ModelImporterMeshCompression.High;
            }

            return UnityEditor.ModelImporterMeshCompression.Off;
        }

        internal void _editor_SetGradients(CCGradient[] gradients)
        {
            if(gradients == null || gradients.Length == 0)
            {
                _sharedGradients = null;
                return;
            }
            
            _sharedGradients = new CCGradient[gradients.Length];
            for(int i = 0; i < gradients.Length; i++)
            {
                _sharedGradients[i] = gradients[i].Clone();
            }
        }

        internal void _editor_SetColliderShape(SVGPath[] shape)
        {
            _colliderShape = shape;
        }

        internal void _editor_SetCanvasRectangle(Rect rectangle)
        {
            _canvasRectangle = rectangle;
        }

        internal void _editor_LoadSVG()
        {        
            SVGAssetImport assetImport;
            if (svgFile != null)
            {
                SVGMesh.format = _format;
                SVGMesh.meshScale = _scale;
                SVGMesh.border = _border;
                SVGMesh.sliceMesh = _sliceMesh;
                SVGMesh.minDepthOffset = _depthOffset;
                SVGMesh.compressDepth = _compressDepth;

                assetImport = new SVGAssetImport(svgFile, _vpm);            
                SVGAssetImport.useGradients = _useGradients;

                if(_antialiasing)
                {
                    SVGAssetImport.antialiasingWidth = _antialiasingWidth;
                } else {
                    SVGAssetImport.antialiasingWidth = 0f;
                }

                SVGAssetImport.ignoreSVGCanvas = _ignoreSVGCanvas;

                assetImport.StartProcess(this);
            }
        }

        internal T AddObjectToAsset<T>(T obj, SVGAsset asset, HideFlags hideFlags) where T : UnityEngine.Object
        {
            if(obj == null)
                return null;
            
            obj.hideFlags = hideFlags;
            UnityEditor.AssetDatabase.AddObjectToAsset(obj, asset);
            return obj;
        }

        internal Mesh _editor_sharedMesh
        {
            get {
                return _sharedMesh;
            }
        }

		internal Material[] _editor_sharedMaterials
		{
			get {
				if(_sharedMaterials == null || _sharedMaterials.Length == 0)
				{
					_sharedMaterials = new Material[_sharedShaders.Length];
					for(int i = 0; i < _sharedShaders.Length; i++)
					{
						_sharedMaterials[i] = new Material(Shader.Find(_sharedShaders[i]));
						_sharedMaterials[i].hideFlags = HideFlags.DontSave;
					}
                    if(hasGradients)
                    {
					    AssignMaterialGradients(_sharedMaterials, atlasTextures[0], SVGAtlas.gradientShapeTexture, 64, 4);
                    }
	            }
	            return _sharedMaterials;
			}
        }
		
		internal Material _editor_UIMaterial
		{
			get {
				if(_sharedUIMaterial == null)
				{
					_sharedUIMaterial = new Material(SVGShader.UI);
					_sharedUIMaterial.hideFlags = HideFlags.DontSave;
					AssignMaterialGradients(_sharedUIMaterial, atlasTextures[0], SVGAtlas.gradientShapeTexture, 64, 4);
				}
				return _sharedUIMaterial;
			}
		}

		internal Material _editor_UIMaskMaterial
		{
			get {
				if(_sharedUIMaskMaterial == null)
				{
					_sharedUIMaskMaterial = new Material(SVGShader.UIMask);
					_sharedUIMaskMaterial.hideFlags = HideFlags.DontSave;
					AssignMaterialGradients(_sharedUIMaskMaterial, atlasTextures[0], SVGAtlas.gradientShapeTexture, 64, 4);
				}
				return _sharedUIMaskMaterial;
			}
		}

        internal string _editor_Info
        {
            get {
                if(_sharedMesh == null)
                {
                    return "No info available";
                }
                
                string output;
                int totalVertices = _sharedMesh.vertexCount;
                int totalTriangles = _sharedMesh.triangles.Length / 3;
                
                output = string.Format("{0} Vertices, {1} Triangles", totalVertices, totalTriangles);
                /*
                Color32[] colors32 = _sharedMesh.colors32;
                Vector2[] uv = _sharedMesh.uv;
                Vector2[] uv2 = _sharedMesh.uv2;
                Vector2[] uv3 = _sharedMesh.uv3;
                Vector2[] uv4 = _sharedMesh.uv4;
                Vector3[] normals = _sharedMesh.normals;
                Vector4[] tangents = _sharedMesh.tangents;

                if(colors32 != null && colors32.Length > 0)
                    output += ", Color";
                if(uv != null && uv.Length > 0)
                    output += ", UV";
                if(uv2 != null && uv2.Length > 0)
                    output += ", UV2";
                if(uv3 != null && uv3.Length > 0)
                    output += ", UV3";
                if(uv4 != null && uv4.Length > 0)
                    output += ", UV4";
                if(normals != null && normals.Length > 0)
                    output += ", Normals";
                if(tangents != null && tangents.Length > 0)
                    output += ", Tangents";
                */
                
                var fileInfo = new System.IO.FileInfo(UnityEditor.AssetDatabase.GetAssetPath(this));
                if(fileInfo != null)
                {
                    output += ", FileSize: "+string.Format(new FileSizeFormatProvider(), "{0:fs}", fileInfo.Length);
                }
                return output;
            }
        }

        internal SVGError[] _editor_errors
        {
            get {
                if(_documentAsset == null)
                    return null;

                return _documentAsset.errors;
            }
        }
    #endif
    }
}
