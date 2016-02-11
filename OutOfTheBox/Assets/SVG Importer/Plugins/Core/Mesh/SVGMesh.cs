// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SVGImporter.Geometry
{
    using Rendering;
    using Data;
    using Utils;

    public class SVGMesh : System.Object
    {        
        public static float meshScale = 1f;        
        public static Vector4 border;
        public static bool sliceMesh = false;
        public static float minDepthOffset = 0.001f;
        public static SVGAssetFormat format = SVGAssetFormat.Opaque;
        public static bool compressDepth = true;
        protected static FILL_BLEND lastBlendType = FILL_BLEND.ALPHA_BLENDED;

        protected int _depth;
        public int depth
        {
            get {
                return _depth;
            }
        }

        protected string _name;

        public string name
        {
            get { return _name; }
        }

        protected SVGFill _fill;
        public SVGFill fill 
        { 
            get { return _fill; } 
        }

        protected Vector3[] _vertices;
        public Vector3[] vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }
               
        protected Vector2[] _uvs;
        public Vector2[] uvs
        {
            get { return _uvs; }
            set { _uvs = value; }
        }

        protected Vector2[] _uvs2;
        public Vector2[] uvs2
        {
            get { return _uvs2; }
            set { _uvs2 = value; }
        }

        protected Color32[] _colors;
        public Color32[] colors
        {
            get { return _colors; }
            set { _colors = value; }
        }

        protected int[] _triangles;
        public int[] triangles
        {
            get { return _triangles; }
            set { _triangles = value; }
        }

        protected Bounds _bounds;
        public Bounds bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }
        
        public SVGMesh(Mesh mesh, SVGFill svgFill, float opacity = 1f)
        {
            if (mesh == null)
                return;

            _name = mesh.name;
            _fill = svgFill.Clone();

            int length = mesh.vertices.Length;
            int trianglesLength = mesh.triangles.Length;

            _vertices = new Vector3[length];

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < length; i++)
            {
                _vertices [i].x = mesh.vertices [i].x * meshScale;
                _vertices [i].y = mesh.vertices [i].y * meshScale;
                _vertices [i].z = _depth * -minDepthOffset;

                if(_vertices [i].x < minX) minX = _vertices [i].x;
                else if(_vertices [i].x > maxX) maxX = _vertices [i].x;

                if(_vertices [i].y < minY) minY = _vertices [i].y;
                else if(_vertices [i].y > maxY) maxY = _vertices [i].y;

                if(_vertices [i].z < minZ) minZ = _vertices [i].z;
                else if(_vertices [i].z > maxZ) maxZ = _vertices [i].z;
            }

            _bounds = new Bounds(new Vector3(Mathf.Lerp(minX, maxX, 0.5f),
                                             Mathf.Lerp(minY, maxY, 0.5f),
                                             Mathf.Lerp(minZ, maxZ, 0.5f))
                                 , new Vector3(maxX - minX,
                                                maxY - minY,
                                                maxZ - minZ
                          ));

            _triangles = new int[trianglesLength];

			// Correct Winding
			int triangle0, triangle1, triangle2;
			Vector2 vector0, vector1, vector2;
			float winding;
            for (int i = 0; i < trianglesLength; i+=3)
            {
				triangle0 = mesh.triangles [i];
				triangle1 = mesh.triangles [i + 1];
				triangle2 = mesh.triangles [i + 2];

				vector0 = _vertices[triangle0];
				vector1 = _vertices[triangle1];
				vector2 = _vertices[triangle2];

				winding = (vector1.x - vector0.x) * (vector1.y + vector0.y);
				winding += (vector2.x - vector1.x) * (vector2.y + vector1.y);
				winding += (vector0.x - vector2.x) * (vector0.y + vector2.y);

				if(winding < 0)
				{				
	                _triangles [i] = mesh.triangles [i];
					_triangles [i + 1] = mesh.triangles [i + 1];
					_triangles [i + 2] = mesh.triangles [i + 2];
				} else {
					_triangles [i] = mesh.triangles [i];
					_triangles [i + 2] = mesh.triangles [i + 1];
					_triangles [i + 1] = mesh.triangles [i + 2];
				}
            }

            if (mesh.colors32 != null && mesh.colors32.Length > 0)
            {
                _colors = new Color32[length];
                for (int i = 0; i < length; i++)
                {        
                    _colors [i] = mesh.colors32 [i];
                    if(opacity != 1f) _colors[i].a = (byte)Mathf.RoundToInt((_colors[i].a / 255 * opacity) * 255);
                }
            } else {
				_colors = new Color32[length];
                Color32 color = new Color32((byte)255, (byte)255, (byte)255, (byte)Mathf.RoundToInt(opacity * 255));
				for (int i = 0; i < length; i++)
				{
                    _colors [i] = color;
				}
			}

            if (mesh.uv != null && mesh.uv.Length > 0)
            {
                _uvs = new Vector2[length];
                for (int i = 0; i < length; i++)
                {        
                    _uvs [i] = mesh.uv [i];
                }
            } else {
				_uvs = new Vector2[length];
			}

            if (mesh.uv2 != null && mesh.uv2.Length > 0)
            {
                _uvs2 = new Vector2[length];
                for (int i = 0; i < length; i++)
                {        
                    _uvs2 [i] = mesh.uv2 [i];
                }
            } else {
				_uvs2 = new Vector2[length];
			}
        }

        public void UpdateDepth()
        {
            if(_vertices == null || _vertices.Length == 0)
                return;

            int length = _vertices.Length;
            for (int i = 0; i < length; i++)
            {
                _vertices [i].z = _depth * -minDepthOffset;
            }
        }

        public static Mesh CreateMesh(out Material[] materials)
        {
            materials = new Material[0];
            SVGAtlas.Instance.InitMaterials();

            if(format == SVGAssetFormat.Opaque)
            {
                return CreateAutomaticMesh(out materials);
            } else if(format == SVGAssetFormat.Transparent || format == SVGAssetFormat.uGUI)
            {
                return CreateTransparentMesh(out materials);
            }

            return null;
        }

        protected static Mesh CreateAutomaticMesh(out Material[] materials)
        {
            materials = new Material[0];
            
            if(sliceMesh) Create9Slice();

            // Z Sort meshes
            if(SVGMesh.format == SVGAssetFormat.Opaque)
            {    
                int meshCount = SVGGraphics.meshes.Count;
                SVGFill fill;

                if(compressDepth)
                {
                    SVGBounds meshBounds = SVGBounds.InfiniteInverse;
                    for (int i = 0; i < meshCount; i++)
                    {
                        if (SVGGraphics.meshes [i] == null) continue;                
                        meshBounds.Encapsulate(SVGGraphics.meshes [i].bounds);
                    }

                    if(!meshBounds.isInfiniteInverse)
                    {
                        SVGGraphics.depthTree.Clear();
                        SVGGraphics.depthTree = new SVGDepthTree(meshBounds);
                    }

                    for (int i = 0; i < meshCount; i++)
                    {
                        if (SVGGraphics.meshes [i] == null)
                            continue;
                        
                        fill = SVGGraphics.meshes [i]._fill;
                        if (fill != null)
                        {
                            SVGMesh[] nodes = SVGGraphics.depthTree.TestDepthAdd(SVGGraphics.meshes [i], new SVGBounds(SVGGraphics.meshes [i]._bounds));
                            int nodesLength = 0;
                            if(nodes == null || nodes.Length == 0)
                            {
                                SVGGraphics.meshes [i]._depth = 0;
                            } else {
                                nodesLength = nodes.Length;
                                int highestDepth = 0;
                                SVGMesh highestMesh = null;
                                for(int j = 0; j < nodesLength; j++)
                                {
                                    if(nodes[j].depth > highestDepth)
                                    {
                                        highestDepth = nodes[j].depth;
                                        highestMesh = nodes[j];
                                    }
                                }
                                
                                if(fill.blend == FILL_BLEND.OPAQUE)
                                {
                                    SVGGraphics.meshes [i]._depth = highestDepth + 1;
                                } else {
                                    if(highestMesh != null && highestMesh.fill.blend == FILL_BLEND.OPAQUE)
                                    {
                                        SVGGraphics.meshes [i]._depth = highestDepth + 1;
                                    } else {
                                        SVGGraphics.meshes [i]._depth = highestDepth;
                                    }
                                }
                            }

                            SVGGraphics.meshes [i].UpdateDepth();
                        }
                    }
                } else {
                    for (int i = 0; i < meshCount; i++)
                    {
                        if (SVGGraphics.meshes [i] == null)
                            continue;
                        
                        fill = SVGGraphics.meshes [i]._fill;
                        if (fill != null)
                        {
                            if (fill.blend == FILL_BLEND.OPAQUE || lastBlendType == FILL_BLEND.OPAQUE)
                            {
                                SVGGraphics.meshes [i]._depth = SVGGraphics.IncreaseDepth();
                            } else 
                            {
                                SVGGraphics.meshes [i]._depth = SVGGraphics.currentDepthOffset;
                            }

                            lastBlendType = fill.blend;
                            SVGGraphics.meshes [i].UpdateDepth();
                        }
                    }
                }
            }

            // Combine Meshes
            List<CombineInstance> combineInstancesOpaque = new List<CombineInstance>();
            List<CombineInstance> combineInstancesTransparent = new List<CombineInstance>();

            GetCombineInstances(combineInstancesOpaque,
                                combineInstancesTransparent);

            int count = combineInstancesOpaque.Count + combineInstancesTransparent.Count;
            
            if (count == 0)
                return null;
            
            List<Material> outputMaterials = new List<Material>(count);
            
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            
            if (combineInstancesOpaque.Count > 0)
            {
                outputMaterials.Add(SVGAtlas.Instance.opaqueGradient);
                combineInstances.Add(GetCombinedInstance(combineInstancesOpaque));
            }

            if (combineInstancesTransparent.Count > 0)
            {
                outputMaterials.Add(SVGAtlas.Instance.transparentGradient);
                combineInstances.Add(GetCombinedInstance(combineInstancesTransparent));
            }
            
            if(outputMaterials.Count != 0)
                materials = outputMaterials.ToArray();
            
            if (combineInstances.Count > 1)
            {
                Mesh output = new Mesh();
                output.CombineMeshes(combineInstances.ToArray(), false, false);
                return output;
            } else if (combineInstances.Count == 1)
            {
                return combineInstances [0].mesh;
            } else
            {
                return null;
            }
        }

        protected static void Create9Slice()
        {
            int meshCount = SVGGraphics.meshes.Count;
            SVGBounds meshBounds = SVGBounds.InfiniteInverse;
            for (int i = 0; i < meshCount; i++)
            {
                if (SVGGraphics.meshes [i] == null) continue;                
                meshBounds.Encapsulate(SVGGraphics.meshes [i].bounds);
            }

            // 9-slice
            if(SVGMesh.border.sqrMagnitude > 0f)
            {
                Vector2 min = meshBounds.min;
                Vector2 max = meshBounds.max;

                for(int i = 0; i < meshCount; i++)
                {
                    if(SVGMesh.border.x > 0)
                        SVGMeshCutter.MeshSplit(SVGGraphics.meshes [i], new Vector2(Mathf.Lerp(min.x, max.x, SVGMesh.border.x), 0f), Vector2.up); 
                    if(SVGMesh.border.y > 0)
                        SVGMeshCutter.MeshSplit(SVGGraphics.meshes [i], new Vector2(0f, Mathf.Lerp(min.y, max.y, 1f - SVGMesh.border.y)), Vector2.right);                     
                    if(SVGMesh.border.z > 0)
                        SVGMeshCutter.MeshSplit(SVGGraphics.meshes [i], new Vector2(Mathf.Lerp(min.x, max.x, 1f - SVGMesh.border.z), 0f), Vector2.up);                     
                    if(SVGMesh.border.w > 0)
                        SVGMeshCutter.MeshSplit(SVGGraphics.meshes [i], new Vector2(0f, Mathf.Lerp(min.y, max.y, SVGMesh.border.w)), Vector2.right); 
                }
            }
        }

        protected static Mesh CreateTransparentMesh(out Material[] materials)
        {
            if(sliceMesh) Create9Slice();

            materials = new Material[]{
                SVGAtlas.Instance.transparentGradient
            };

            List<CombineInstance> combineInstances = GetAllCombineInstances();
            if(combineInstances == null || combineInstances.Count == 0) return null;

            Mesh output = new Mesh();
            output.CombineMeshes(combineInstances.ToArray(), true, false);
            return output;
        }

        protected static CombineInstance GetCombinedInstance(List<CombineInstance> combinedInstances)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combinedInstances.ToArray(), true, false);
            CombineInstance combinedInstance = new CombineInstance();
            combinedInstance.mesh = combinedMesh;
            return combinedInstance;
        }

        protected static void GetCombineInstances(List<CombineInstance> combineInstancesOpaque,                                         
                                            List<CombineInstance> combineInstancesTransparent)
        {       
            int count = SVGGraphics.meshes.Count;
            SVGFill fill;

            for (int i = 0; i < count; i++)
            {
                if (SVGGraphics.meshes [i] == null)
                    continue;

                Mesh mesh = SVGGraphics.meshes [i].mesh;
                if(mesh == null || mesh.vertexCount == 0)
                    continue;

                fill = SVGGraphics.meshes [i]._fill;
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = SVGGraphics.meshes [i].mesh;

                if (fill != null)
                {
                    if (fill.blend == FILL_BLEND.OPAQUE)
                    {
                        combineInstancesOpaque.Add(combineInstance);
                    } else if (fill.blend == FILL_BLEND.ALPHA_BLENDED)
                    {
                        combineInstancesTransparent.Add(combineInstance);
                    }
                }
            }
        }

        protected static List<CombineInstance> GetAllCombineInstances()
        {
            int count = SVGGraphics.meshes.Count;
            CombineInstance combineInstance;
    //        VrFill fill;

            List<CombineInstance> output = new List<CombineInstance>();
            Mesh combineMesh;

            for (int i = 0; i < count; i++)
            {
                if (SVGGraphics.meshes [i] == null)
                    continue;
                
    //            fill = SVGGraphics.meshes [i]._fill;
                
                combineMesh = SVGGraphics.meshes [i].mesh;
                if(combineMesh == null || combineMesh.vertexCount == 0)
                    continue;

                combineInstance = new CombineInstance();
                combineInstance.mesh = combineMesh;
                output.Add(combineInstance);
            }

            return output;
        }

        public Mesh mesh
        { 
            get
            {
    //        Debug.Log(_vertices);
    //        Debug.Log(_triangles);
                if (_vertices == null || _vertices.Length == 0 || _triangles == null || _triangles.Length == 0)
                    return null;

                Mesh output = new Mesh();
                Bounds meshBounds = new Bounds();
                meshBounds.SetMinMax(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));

                int length = _vertices.Length;
                int trianglesLength = _triangles.Length;
            
                Vector3[] finVertices = new Vector3[length];
                for (int i = 0; i < length; i++)
                {
                    finVertices [i] = _vertices [i];
                    meshBounds.Encapsulate(_vertices [i]);
                }
                output.vertices = finVertices;
            
                int[] finTriangles = new int[trianglesLength];
                for (int i = 0; i < trianglesLength; i++)
                {
                    finTriangles [i] = _triangles [i];
                }
                output.triangles = finTriangles;
            
                if (_colors != null && _colors.Length > 0)
                {
                    Color32[] finColors = new Color32[length];
                    for (int i = 0; i < length; i++)
                    {
                        finColors [i] = _colors [i];
                    }
                    output.colors32 = finColors;
                } else {
					Color32[] finColors = new Color32[length];
					for (int i = 0; i < length; i++)
					{
						finColors [i] = Color.white;
					}
					output.colors32 = finColors;
				}

                if (_uvs != null && _uvs.Length > 0)
                {
                    Vector2[] finUvs = new Vector2[length];
                    for (int i = 0; i < length; i++)
                    {
                        finUvs [i] = _uvs [i];
                    }
                    output.uv = finUvs;
                } else {
					output.uv = new Vector2[length];
				}

                if (_uvs2 != null && _uvs2.Length > 0)
                {
                    Vector2[] finUvs2 = new Vector2[length];
                    for (int i = 0; i < length; i++)
                    {
                        finUvs2 [i] = _uvs2 [i];
                    }
                    output.uv2 = finUvs2;
                } else {
					output.uv2 = new Vector2[length];
				}
            
                return output;
            }
        }
    }
}
