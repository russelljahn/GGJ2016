using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SVGImporter;
using SVGImporter.Utils;

public class EdgeTest : MonoBehaviour {

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if(meshFilter != null && meshFilter.sharedMesh != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            UnityEditor.Handles.matrix = transform.localToWorldMatrix;

            if(meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh sharedMesh = meshFilter.sharedMesh;
                Vector3[] vertices = sharedMesh.vertices;
                int[] triangles = sharedMesh.triangles;

                /*
                Edge[] edges = SVGMeshUtils.BuildManifoldEdges(vertices, triangles);

                for(int i = 1; i < edges.Length; i++)
                {
                    Gizmos.DrawLine(vertices[edges[i].vertexIndex[0]], 
                                    vertices[edges[i].vertexIndex[1]]);
                    UnityEditor.Handles.Label(vertices[edges[i].vertexIndex[0]], i.ToString());
                }
                */

                List<int[]> paths = SVGMeshUtils.BuildManifoldPoints(vertices, triangles);
                for(int i = 0; i < paths.Count; i++)
                {
                    Vector3 lastPoint = vertices[paths[i][0]];
                    Vector3 currentPoint;
                    for(int j = 0; j < paths[i].Length; j++)
                    {
                        currentPoint = vertices[paths[i][j]];
                        Gizmos.DrawLine(lastPoint, currentPoint);
                        UnityEditor.Handles.Label(currentPoint, i.ToString()+"_"+j.ToString());
                        lastPoint = currentPoint;
                    }
                }
            }
            
            Gizmos.matrix = oldMatrix;
        }                
    }
#endif
}
