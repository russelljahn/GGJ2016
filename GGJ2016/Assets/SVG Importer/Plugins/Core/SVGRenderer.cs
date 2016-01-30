// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;

namespace SVGImporter
{
    using Utils;
    using Rendering;
    
    [ExecuteInEditMode]
    [AddComponentMenu("Rendering/SVG Renderer", 20)]
    public class SVGRenderer : UIBehaviour, ISVGShape, ISVGRenderer
    {
        /// <summary>
        /// delegate which indicates that the source SVG graphics changed.
        /// </summary>
        public System.Action<SVGAsset> onVectorGraphicsChanged;

        protected System.Action<Mesh, bool> _OnPrepareForRendering;
        /// <summary>
        /// delegate which indicates that the mesh has changed.
        /// You can use it as your custom mesh postprocessor.
        /// </summary>
        public virtual System.Action<Mesh, bool> OnPrepareForRendering
        {
            get {
                return _OnPrepareForRendering;
            }
            set {
                _OnPrepareForRendering = value;
            }
        }

        // Tracking of modified assets
        [FormerlySerializedAs("lastTimeModified")]
        [SerializeField]
        protected long _lastTimeModified;

        // Tracking of mesh change
        protected int _lastFrameChanged;
        public int lastFrameChanged
        {
            get {
                return _lastFrameChanged;
            }
        }

        // Serialized SVG Asset
        [FormerlySerializedAs("vectorGraphics")]
        [SerializeField]
        protected SVGAsset _vectorGraphics;
        protected SVGAsset _lastVectorGraphics;

        /// <summary>
        /// The SVG Asset to render.
        /// </summary>
        public SVGAsset vectorGraphics
        {
            get {
                return _vectorGraphics;
            }
            set {

                if(_vectorGraphics != value)
                {
                    _vectorGraphics = value;
                    if(!meshRenderer.isPartOfStaticBatch)
                        PrepareForRendering(true);
                }
            }
        }

        // Serialized rendering color
        [FormerlySerializedAs("color")]
        [SerializeField]
        protected Color _color = Color.white;
        protected Color _lastColor = Color.white;
        protected Color32[] _cachedColors;

        /// <summary>
        /// Rendering color for the SVG Asset..
        /// </summary>
        public Color color
        {
            get {
                return _color;
            }
            set {
                _color = value;
            }
        }
        
        [FormerlySerializedAs("opaqueMaterial")]
        [SerializeField]
        protected Material _opaqueMaterial;
        protected Material _lastOpaqueMaterial;
        /// <summary>
        /// The opaque override material
        /// </summary>
        public Material opaqueMaterial
        {
            get {
                return _opaqueMaterial;
            }
            set {
                if(_opaqueMaterial != value)
                {
                    _opaqueMaterial = value;
                    UpdateMaterials();
                }
            }
        }

        [FormerlySerializedAs("transparentMaterial")]
        [SerializeField]
        protected Material _transparentMaterial;
        protected Material _lastTransparentMaterial;
        /// <summary>
        /// The opaque override material
        /// </summary>
        public Material transparentMaterial
        {
            get {
                return _transparentMaterial;
            }
            set {
                if(_transparentMaterial != value)
                {
                    _transparentMaterial = value;
                    UpdateMaterials();
                }
            }
        }

        protected MeshFilter _meshFilter;
        public MeshFilter meshFilter {
            get {
                if(_meshFilter == null)
                {   
                    _meshFilter = GetComponent<MeshFilter>();
                    if(_meshFilter == null) 
                    {
                        _meshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                }
                return _meshFilter;
            }
        }

        protected MeshRenderer _meshRenderer;
        public MeshRenderer meshRenderer {
            get {
                if(_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                    if(_meshRenderer == null)
                    {
                        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    }
                }
                return _meshRenderer;
            }
        }

        // Shared mesh for better gpu instancing
        protected Mesh _sharedMesh;
        // Instanced mesh for custom coloring
        protected Mesh _mesh;
        // Shared Material for better batching
        protected Material[] _sharedMaterials;

        // Unity default transparent sorting ID
        [FormerlySerializedAs("sortingLayerID")]
        [SerializeField]
        protected int _sortingLayerID = 0;
        protected int _lastSortingLayerID = 0;
        /// <summary>
        /// Unique ID of the Renderer's sorting layer.
        /// </summary>
        public int sortingLayerID
        {
            get {
                return meshRenderer.sortingLayerID;
            }
            set {
#if UNITY_EDITOR
                _lastSortingLayerID = value;
#endif
                meshRenderer.sortingLayerID = _sortingLayerID = value;
                _sortingLayerName = meshRenderer.sortingLayerName;
            }
        }

        // Unity default transparent sorting layer name
        [FormerlySerializedAs("sortingLayerName")]
        [SerializeField]
        protected string _sortingLayerName;
        /// <summary>
        /// Name of the Renderer's sorting layer.
        /// </summary>
        public string sortingLayerName
        {
            get {
                return meshRenderer.sortingLayerName;
            }
            set {
                meshRenderer.sortingLayerName = _sortingLayerName = value;
                _lastSortingLayerID = _sortingLayerID = meshRenderer.sortingLayerID;
            }
        }

        // Unity default transparent sorting order
        [FormerlySerializedAs("sortingOrder")]
        [SerializeField]
        protected int _sortingOrder = 0;
        protected int _lastSortingOrder = 0;
        /// <summary>
        /// Renderer's order within a sorting layer.
        /// </summary>
        public int sortingOrder
        {
            get {
                return meshRenderer.sortingOrder;
            }
            set {
#if UNITY_EDITOR
                _lastSortingOrder = value;
#endif
                meshRenderer.sortingOrder = _sortingOrder = value;
            }
        }

        [FormerlySerializedAs("overrideSorter")]
        [SerializeField]
        protected bool _overrideSorter = false;
        protected bool _lastOverrideSorter = false;
        /// <summary>
        /// Override SVG Sorter Default Behaviour
        /// </summary>
        public bool overrideSorter
        {
            get {
                return _overrideSorter;
            }
            set {
                _overrideSorter = value;
            }
        }

        [FormerlySerializedAs("overrideSorterChildren")]
        [SerializeField]
        protected bool _overrideSorterChildren = false;
        protected bool _lastOverrideSorterChildren = false;
        /// <summary>
        /// Override SVG Sorter Default Behaviour
        /// </summary>
        public bool overrideSorterChildren
        {
            get {
                return _overrideSorterChildren;
            }
            set {
                _overrideSorterChildren = value;
            }
        }

        /// <summary>
        /// Get SVG Outline, part of the ISVGPath interface..
        /// </summary>
        public SVGPath[] shape
        {
            get {
                if(_vectorGraphics == null) return null;
                return _vectorGraphics.colliderShape;
            }
        }

        // We have to clear editor data and load runtime data
        // Also it handles duplicating game objects
        protected override void Awake()
        {
            meshFilter.sharedMesh = null;
            meshRenderer.sharedMaterials = new Material[0];

            base.Awake();
            Clear(true);
            PrepareForRendering(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnableMeshRenderer(true);
        }

        public void UpdateRenderer()
        {
            PrepareForRendering(true);
        }

        // This is the main rendering method
        protected void PrepareForRendering(bool force = false)
        {           
#if UNITY_EDITOR
            if(_lastSortingOrder != _sortingOrder){ 
                sortingOrder = _sortingOrder;
            }
            if(_lastSortingLayerID != _sortingLayerID){ 
                sortingLayerID = _sortingLayerID;
            }
#endif
            if(_vectorGraphics == null)
            {
                _lastVectorGraphics = null;
                Clear();
            } else {                
                CacheDynamicMesh();

                bool meshChanged = force;
                bool colorChanged = force || _lastColor != _color;
                bool materialChanged = force || _lastOpaqueMaterial != _opaqueMaterial || _lastTransparentMaterial != _transparentMaterial;

                if(_sharedMesh == null || _lastVectorGraphics != _vectorGraphics)
                {
                    meshChanged = true;
                    colorChanged = true;
                }

                if(colorChanged && _color == Color.white){ 
                    meshChanged = true;
                }

                if(meshChanged)
                {
                    Clear();
                    InitMesh();
                    materialChanged = true;

                    if(onVectorGraphicsChanged != null)
                        onVectorGraphicsChanged(_vectorGraphics);
                }

                if(colorChanged)
                {
                    UpdateColors(force);
                    materialChanged = true;
                }
                
                if(materialChanged)
                {
                    InitMaterials();
                    UpdateMaterials();
                }

                if(meshChanged || colorChanged)
                {
                    _lastFrameChanged = Time.frameCount;
                    if(OnPrepareForRendering != null) 
                        OnPrepareForRendering(this.meshFilter.sharedMesh, force);
                }

                _lastOpaqueMaterial = _opaqueMaterial;
                _lastTransparentMaterial = _transparentMaterial;
                _lastVectorGraphics = _vectorGraphics;
                _lastColor = _color;
            }
        }
        
        #if UNITY_EDITOR
        // Clear SVG Renderer when hit Reset in the Editor
        protected override void Reset()
        {
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                Clear();
                PrepareForRendering(true);
            }

            base.Reset();
        }
        #endif

        // Gradient shape texture is used for the shape of the gradient
        void UpdateGradientShapeTexture(Material[] target)
        {
            if(target == null || target.Length == 0)
                return;

            int targetLength = target.Length;
            for(int i = 0; i < targetLength; i++)
            {
                if(target[i] == null)
                    continue;
                
                if(target[i].HasProperty("_GradientShape"))
                    target[i].SetTexture("_GradientShape", SVGAtlas.gradientShapeTexture);
            }
        }

        void ApplyMaterialGradient(Material source, Material target)
        {
            if(source == null || target == null) return;

            if(source.HasProperty("_GradientShape") && target.HasProperty("_GradientShape"))
                target.SetTexture("_GradientShape", source.GetTexture("_GradientShape"));

            if(source.HasProperty("_GradientColor") && target.HasProperty("_GradientColor"))
                target.SetTexture("_GradientColor", source.GetTexture("_GradientColor"));

            if(source.HasProperty("_Params") && target.HasProperty("_Params"))
                target.SetVector("_Params", source.GetVector("_Params"));
        }

        // This method is invoked by Unity when rendering to Camera
        void OnWillRenderObject()
        {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                if(_vectorGraphics != null)
                {
                    System.Reflection.FieldInfo assetTicksInfo = typeof(SVGAsset).GetField("_lastTimeModified", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    long assetTicks = (long)assetTicksInfo.GetValue(_vectorGraphics);
                    if(_lastTimeModified != assetTicks)
                    {
                        _lastTimeModified = assetTicks;
                        Clear();
                    }
                }
            }
#endif
            if(!meshRenderer.isPartOfStaticBatch)
                PrepareForRendering();
        }

        protected void CacheDynamicMesh()
        {
            if (_color != Color.white || _modifiers.Count > 0)
            {
                if(_mesh == null)
                {
                    _mesh = _vectorGraphics.mesh;
                    #if UNITY_EDITOR
                    SetHideFlags(_mesh, HideFlags.DontSave);
                    #endif
                }
            }
        }

        protected Color32[] _finalColors;
        protected void UpdateColors(bool force = false)
        {
            if(!(_sharedMesh == null))
            {
                if (_color != Color.white)
                {
                    if(_cachedColors == null)
                    {
                        Color32[] originalColors = _sharedMesh.colors32;
                        if(originalColors == null || originalColors.Length == 0)
                            return;

                        _finalColors = new Color32[originalColors.Length];
                        _cachedColors = (Color32[])originalColors.Clone();
                    }

                    int colorsLength = _cachedColors.Length;
                    Color32 tempColor = _color;
                    for(int i = 0; i < colorsLength; i++)
                    {
                        _finalColors[i].r = (byte)((_cachedColors[i].r * tempColor.r) / 255);
                        _finalColors[i].g = (byte)((_cachedColors[i].g * tempColor.g) / 255);
                        _finalColors[i].b = (byte)((_cachedColors[i].b * tempColor.b) / 255);
                        _finalColors[i].a = (byte)((_cachedColors[i].a * tempColor.a) / 255);
                    }

                    _mesh.colors32 = _finalColors;
                    meshFilter.sharedMesh = _mesh;     
                }
            }
        }

        internal bool AtlasContainsMaterial(Material material)
        {
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return SVGAtlas.Instance.ContainsMaterial(material);
            } else {
                return false;
            }
#else
            return SVGAtlas.Instance.ContainsMaterial(material);
#endif
        }

        protected void SwapMaterials(bool transparent = true)
        {
            if(_vectorGraphics == null || _sharedMesh == null || _sharedMaterials == null || _sharedMaterials.Length == 0)
            {
                meshRenderer.sharedMaterials = new Material[]{};
                return;
            }

            int subMeshCount = _sharedMesh.subMeshCount;

            if(_vectorGraphics.hasGradients)
            {
                if(_transparentMaterial != null)
                    ApplyMaterialGradient(_sharedMaterials[0], _transparentMaterial);
                if(_opaqueMaterial != null)
                    ApplyMaterialGradient(_sharedMaterials[0], _opaqueMaterial);
            }

            if(_vectorGraphics.isOpaque)
            {
                if(transparent)
                {
                    if(_transparentMaterial != null)
                    {
                        SetSharedMaterials(subMeshCount, _transparentMaterial, _transparentMaterial);
                    } else {
                        if(_sharedMaterials.Length > 1)
                        {
                            SetSharedMaterials(subMeshCount, _sharedMaterials[1], _sharedMaterials[1]);
                        } else {
                            SetSharedMaterials(subMeshCount, _sharedMaterials[0], _sharedMaterials[0]);
                        }
                    }
                } else {
                    if(_transparentMaterial == null && _opaqueMaterial == null)
                    {
                        meshRenderer.sharedMaterials = _sharedMaterials;
                    } else if(_transparentMaterial != null && _opaqueMaterial != null)
                    {
                        SetSharedMaterials(subMeshCount, _opaqueMaterial, _transparentMaterial);
                    } else if(_transparentMaterial != null)
                    {
                        SetSharedMaterials(subMeshCount, _sharedMaterials[0], _transparentMaterial);
                    } else if(_opaqueMaterial != null)
                    {
                        if(_sharedMaterials.Length > 1)
                        {
                            SetSharedMaterials(subMeshCount, _opaqueMaterial, _sharedMaterials[1]);
                        } else {
                            SetSharedMaterials(subMeshCount, _opaqueMaterial, _opaqueMaterial);
                        }
                    }
                }
            } else {
                if(_transparentMaterial == null)
                {
                    meshRenderer.sharedMaterials = _sharedMaterials;
                } else {
                    meshRenderer.sharedMaterial = _transparentMaterial;
                }
            }
        }

        void SetSharedMaterials(int subMeshCount, Material firstMaterial, Material secondMaterial)
        {
            if(subMeshCount < 2)
            {
                meshRenderer.sharedMaterials = new Material[]{ firstMaterial };
            } else {
                meshRenderer.sharedMaterials = new Material[]{ firstMaterial, secondMaterial };
            }
        }
        
        public void UpdateMaterials()
        {
            SwapMaterials(_color.a != 1);
        }

        /// <summary>
        /// Mark the Graphic as dirty and prepare it for rendering.
        /// </summary>
        /// <param name="force">Force re-updating the whole object</param>
        public void SetAllDirty()
        {
            //Debug.Log("UpdateMesh: "+name+" instance: "+GetInstanceID());
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                PrepareForRendering(true);
            } else {
                if(!meshRenderer.isPartOfStaticBatch)
                    PrepareForRendering(true);
            }
#else
            if(!meshRenderer.isPartOfStaticBatch)
                PrepareForRendering(true);
#endif
        }
        
        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if(_vectorGraphics != null && _vectorGraphics.sharedMesh != null)
            {
                Bounds bounds = _vectorGraphics.bounds;
                Matrix4x4 gizmoMatrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
                Gizmos.DrawWireCube(bounds.center, bounds.size);
                Gizmos.matrix = gizmoMatrix;
            }
        }
        #endif
        
        protected override void OnDisable()
        {
            EnableMeshRenderer(false);
            base.OnDisable();
        }

        void EnableMeshRenderer(bool value)
        {
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlaying)
            {
                if(!meshRenderer.isPartOfStaticBatch)
                    meshRenderer.enabled = value;
            } else {
                meshRenderer.enabled = value;
            }
#else
            if(!meshRenderer.isPartOfStaticBatch)
                meshRenderer.enabled = value;
#endif
        }

        void InitMesh()
        {            
            if(_vectorGraphics == null)
            {
                CleanMesh();
            } 
            // Set mesh for rendering
            if(_vectorGraphics != null)
            {
                #if UNITY_EDITOR
                if(!UnityEditor.EditorApplication.isPlaying)
                {
                    if(_sharedMesh == null)
                    {
                        _sharedMesh = _vectorGraphics.mesh;
                        SetHideFlags(_sharedMesh, HideFlags.DontSave);
                    }
                } else {
                    if(_sharedMesh != _vectorGraphics.sharedMesh)
                        _sharedMesh = _vectorGraphics.sharedMesh;
                }
                #else
                if(_sharedMesh != _vectorGraphics.sharedMesh)
                    _sharedMesh = _vectorGraphics.sharedMesh;
                #endif

                if(_color == Color.white && _modifiers.Count == 0)
                {
                    if(meshFilter.sharedMesh != _sharedMesh) 
                        meshFilter.sharedMesh = _sharedMesh;
                } else {
                    CacheDynamicMesh();
                    if(meshFilter.sharedMesh != _mesh) 
                        meshFilter.sharedMesh = _mesh;
                }                             
            } else {
                Clear();
                _lastVectorGraphics = null;
            }
        }

        void InitMaterials()
        {
            if(_vectorGraphics == null)
            {
                CleanMaterials();
            } else {
                #if UNITY_EDITOR
                if(!UnityEditor.EditorApplication.isPlaying)
                {
                    if(_sharedMaterials == null)
                    {
                        _sharedMaterials = _vectorGraphics.materials;
                        SetHideFlags(_sharedMaterials, HideFlags.DontSave);
                        if(_vectorGraphics.hasGradients)
                        {
                            UpdateGradientShapeTexture(_sharedMaterials);
                        }
                    }
                } else {
                    if(_sharedMaterials == null)
                    {
                        _sharedMaterials = _vectorGraphics.sharedMaterials;
                    }
                }
                #else
                if(_sharedMaterials != _vectorGraphics.sharedMaterials)
                    _sharedMaterials = _vectorGraphics.sharedMaterials;
                #endif
            }
        }

        public void AddModifier(ISVGModify modifier)
        {
            if(_modifiers.Contains(modifier)) return;
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(ISVGModify modifier)
        {
            if(!_modifiers.Contains(modifier)) return;
            _modifiers.Remove(modifier);
        }

        /// <summary>
        /// Is this renderer visible in any camera? (Read Only)
        /// </summary>
        public bool isVisible
        {
            get {
                return _meshRenderer.isVisible;
            }
        }

        protected List<ISVGModify> _modifiers = new List<ISVGModify>();
        public List<ISVGModify> modifiers
        {
            get {
                return _modifiers;
            }
        }

        protected void Clear(bool force = false)
        {
            bool clearAll = true;
#if UNITY_EDITOR
            if(!force)
            {
                if(UnityEditor.EditorApplication.isPlaying && meshRenderer.isPartOfStaticBatch){ clearAll = false; }
            }
#else
            if(!meshRenderer.isPartOfStaticBatch){ clearAll = false; }
#endif
            if(clearAll)
            {
                ClearMeshFilter();
                ClearMeshRenderer();
                CleanMesh();
                CleanMaterials();
            }
            
            if(_mesh != null)
            {
                DestroyObjectInternal(_mesh);
                _mesh = null;
            }

            if(_cachedColors != null)
            {
                _cachedColors = null;
            }
        }
        
        void CleanMesh()
        {
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                DestroyObjectInternal(_sharedMesh);
                _sharedMesh = null;
            }
            #endif            
            if(_mesh != null)
            {
                DestroyObjectInternal(_mesh);
                _mesh = null;
            }
        }

        void CleanMaterials()
        {
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                DestroyArray<Material>(_sharedMaterials);
            }
            #endif
            if(_sharedMaterials != null)
                _sharedMaterials = null;
        }

        void ClearMeshFilter()
        {
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                if(meshFilter != null)
                {
                    DestroyObjectInternal(meshFilter.sharedMesh);
                    meshFilter.sharedMesh = null;
                }
            }
            #else            
            if(meshFilter != null)
                meshFilter.sharedMesh = null;
            #endif
        }
        
        void ClearMeshRenderer()
        {
            //Debug.Log("ClearMeshRenderer");
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
            {
                if(meshRenderer != null)
                {
                    meshRenderer.sharedMaterials = new Material[]{};
                }
            }
            #else
            if(meshRenderer != null)
            {
                meshRenderer.sharedMaterials = new Material[]{};
            }
            #endif
        }
        
        void DestroyArray<T>(T[] array) where T : UnityEngine.Object
        {
            if(array == null)
                return;
            foreach(T item in array)
            {
                if(item == null)
                    continue;
                DestroyObjectInternal(item);
            }
        }

        void DestroyObjectInternal(Object obj)
        {
            if(obj == null)
                return;
            
            #if UNITY_EDITOR
            if(!UnityEditor.AssetDatabase.Contains(obj))
            {
                if(UnityEditor.EditorApplication.isPlaying)
                {
                    Destroy(obj);
                } else {
                    DestroyImmediate(obj);
                }
            }
            #else
            Destroy(obj);
            #endif
        }

        #if UNITY_EDITOR
        void SetHideFlags(UnityEngine.Object target, HideFlags hideFlags)
        {
            if(target == null) return;
            target.hideFlags = hideFlags;
        }

        void SetHideFlags(UnityEngine.Object[] target, HideFlags hideFlags)
        {
            if(target == null || target.Length == 0) return;
            for(int i = 0; i < target.Length; i++)
            {
                target[i].hideFlags = hideFlags;
            }
        }            
        #endif
    }
}
