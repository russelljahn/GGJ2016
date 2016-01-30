using Assets.OutOfTheBox.Scripts.Utils;
using Assets.OutOfTheBox.Scripts.Extensions;
using Sense.Extensions;
using Sense.PropertyAttributes;
using UnityEngine;

namespace Assets.OutOfTheBox.Scripts
{
    [ExecuteInEditMode]
    public class GenerateSphereArcMesh : MonoBehaviour
    {
        [SerializeField] private Transform _anchor1;
        [SerializeField] private Transform _anchor2;
        [SerializeField] private Transform _anchor3;
        [SerializeField] private Transform _anchor4;

        [SerializeField, Min(0f)] private float _radius;

        [SerializeField, Range(0, 15)] private int _numHorizontalSubdivisions;
        [SerializeField, Range(0, 15)] private int _numVerticalSubdivisions;

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField, Clamp01] private float _gizmoAlpha = 0.75f;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _triangles;

        private int NumGridColumns
        {
            get { return 2 + _numHorizontalSubdivisions; }
        }

        private int NumGridRows
        {
            get { return 2 + _numVerticalSubdivisions; }
        }

        private float RadiusAtMesh
        {
            get
            {
                if (_meshFilter.IsNull())
                {
                    return 0f;
                }
                var lossyScale = _meshFilter.transform.lossyScale;
                return _radius*3f/(lossyScale.x + lossyScale.y + lossyScale.z);
            }
        }

    private void Start()
        {
            if (_anchor1.IsNull() || _anchor2.IsNull() || _anchor3.IsNull() || _anchor4.IsNull())
            {
                return;
            }
            ClampAnchorsToSphere();
            UpdateVerticesAndUVs();
            UpdateTriangles();
            UpdateMesh();
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                ClampAnchorsToSphere();
            }
        }

        private void OnValidate()
        {
            if (_anchor1.IsNull() || _anchor2.IsNull() || _anchor3.IsNull() || _anchor4.IsNull())
            {
                return;
            }
            ClampAnchorsToSphere();
            UpdateVerticesAndUVs();
            UpdateTriangles();
            UpdateMesh();
        }

        private void OnDrawGizmos()
        {
            if (_anchor1.IsNull() || _anchor2.IsNull() || _anchor3.IsNull() || _anchor4.IsNull())
            {
                return;
            }

            var gizmoLineColor1 = ColorUtils.Colors.Yellows.Yellow(_gizmoAlpha);
            Gizmos.color = gizmoLineColor1;
            Gizmos.DrawLine(_anchor1.position, _anchor2.position);
            Gizmos.DrawLine(_anchor2.position, _anchor3.position);
            Gizmos.DrawLine(_anchor3.position, _anchor4.position);
            Gizmos.DrawLine(_anchor4.position, _anchor1.position);
            Gizmos.DrawLine(_anchor3.position, _anchor1.position);
            Gizmos.DrawLine(_anchor4.position, _anchor2.position);
        }

        private void ClampAnchorsToSphere()
        {
            if (_radius < 0.25f)
            {
                return;
            }
            _anchor1.position = (_anchor1.position - transform.position).normalized * _radius;
            _anchor2.position = (_anchor2.position - transform.position).normalized * _radius;
            _anchor3.position = (_anchor3.position - transform.position).normalized * _radius;
            _anchor4.position = (_anchor4.position - transform.position).normalized * _radius;
        }

        private void UpdateVerticesAndUVs()
        {
            _vertices = new Vector3[NumGridColumns * NumGridRows];
            _uvs = new Vector2[NumGridColumns * NumGridRows];

            for (int row = 0; row < NumGridRows; ++row)
            {
                var tRow = row/(NumGridRows - 1.0f);
                for (int column = 0; column < NumGridColumns; ++column)
                {
                    var tColumn = column/(NumGridColumns - 1.0f);
                    var blendedTopAnchors = Vector3.Lerp(_anchor1.position, _anchor2.position, tRow);
                    var blendedBottomAnchors = Vector3.Lerp(_anchor3.position, _anchor4.position, tRow);

                    var unextrudedVertex = Vector3.Lerp(blendedTopAnchors, blendedBottomAnchors, tColumn);
                    var extrudedVertex = unextrudedVertex.normalized*RadiusAtMesh;

                    var index = row*NumGridColumns + column;
                    _vertices[index] = extrudedVertex;
                    _uvs[index] = new Vector2(tRow, 1.0f - tColumn);
                }
            }
        }

        private void UpdateTriangles()
        {
            var numTriangles = 6*NumGridColumns*(NumGridRows - 1);
            _triangles = new int[numTriangles];
            var triangleIndex = 0;

            for (int row = 0; row < NumGridRows-1; ++row)
            {
                for (int column = 0; column < NumGridColumns-1; ++column)
                {
                    var topLeftVertexIndex = row*NumGridColumns + column;
                    var topRightVertexIndex = row * NumGridColumns + column + 1;
                    var bottomLeftVertexIndex = (row + 1) * NumGridColumns + column;
                    var bottomRightVertexIndex = (row + 1) * NumGridColumns + column + 1;

                    _triangles[triangleIndex] = topLeftVertexIndex;
                    _triangles[triangleIndex + 1] = bottomLeftVertexIndex;
                    _triangles[triangleIndex + 2] = topRightVertexIndex;
                    _triangles[triangleIndex + 3] = topRightVertexIndex;
                    _triangles[triangleIndex + 4] = bottomLeftVertexIndex;
                    _triangles[triangleIndex + 5] = bottomRightVertexIndex;

                    triangleIndex += 6;
                }
            }
        }

        private void UpdateMesh()
        {
            if (_mesh == null)
            {
                _mesh = _meshFilter.mesh = new Mesh();
                _mesh.name = "Sphere Patch Mesh";
            }

            _mesh.triangles = null;
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.uv = _uvs;

            _mesh.RecalculateNormals();
        }

    }
}
