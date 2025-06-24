namespace MeshGenerating {

    //Script designed to create mesh path between stars
    using System.Collections.Generic;
    using UnityEngine;

    public class MeshGeneration : MonoBehaviour {


        public class PathGen {

            static Vector2[] uvsBase = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1), 
            };

            //Generate mesh for multiple paths
            public static Mesh GeneratePossiblePaths(List<PathList> starPaths) {

                //Sets up for mesh gen
                Vector3 upMult = Vector3.up * 0.5f;
                int triangleVerticeNumber = (starPaths.Count) * 4;
                int singleTriangleVerticeNumber = (starPaths.Count) * 6;
                Vector3[] vertices = new Vector3[triangleVerticeNumber];
                Vector3[] normals = new Vector3[triangleVerticeNumber];
                Vector2[] uvs = new Vector2[triangleVerticeNumber];
                int[] triangles = new int[singleTriangleVerticeNumber];
                Vector3 tempPerpendicular;

                //Sets all the triangle info for 1 quad double side
                for (int i = 0; i < triangleVerticeNumber; i = i + 4) {
                    vertices[i] = starPaths[i / 4].startPoint - upMult;
                    vertices[i + 1] = starPaths[i / 4].startPoint + upMult;
                    vertices[i + 2] = starPaths[i / 4].endPoint - upMult;
                    vertices[i + 3] = starPaths[i / 4].endPoint + upMult;

                    tempPerpendicular = new Vector3(starPaths[i / 4].startPoint.z - starPaths[i / 4].endPoint.z, 0, starPaths[i / 4].endPoint.x - starPaths[i / 4].startPoint.x);

                    normals[i] = tempPerpendicular;
                    normals[i + 1] = tempPerpendicular;
                    normals[i + 2] = tempPerpendicular;
                    normals[i + 3] = tempPerpendicular;

                    uvs[i] = uvsBase[0];
                    uvs[i + 1] = uvsBase[1];
                    uvs[i + 2] = uvsBase[2];
                    uvs[i + 3] = uvsBase[3];
                }

                //Sets all triangles
                for (int i = 0; i < singleTriangleVerticeNumber; i = i + 6) {
                    triangles[i] = 0 + ((4 * i) / 6); triangles[i + 1] = 1 + ((4 * i) / 6); triangles[i + 2] = 2 + ((4 * i) / 6);
                    triangles[i + 3] = 1 + ((4 * i) / 6); triangles[i + 4] = 3 + ((4 * i) / 6); triangles[i + 5] = 2 + ((4 * i) / 6);
                }

                //Final mesh properties
                Mesh pathMesh = new Mesh();
                pathMesh.vertices = vertices;
                pathMesh.normals = normals;
                pathMesh.uv = uvs;
                pathMesh.triangles = triangles;
                return pathMesh;
            }

            //Generate mesh for single path line
            public static Mesh GenerateSinglePath(List<Vector3> starPoints) {

                //Prepare for mesh gen
                Mesh mesh = new();
                Vector3 upMult = Vector3.up * 0.4f;
                int triangleVerticeNumber = (starPoints.Count - 1) * 8;
                int singleTriangleVerticeNumber = (starPoints.Count - 1) * 12;
                Vector3[] vertices = new Vector3[triangleVerticeNumber];
                Vector3[] normals = new Vector3[triangleVerticeNumber];
                Vector2[] uvs = new Vector2[triangleVerticeNumber];
                int[] triangles = new int[singleTriangleVerticeNumber];
                Vector3 tempPerpendicular;

                //Sets all the triangle info for 1 quad double side
                for (int i = 0; i < triangleVerticeNumber; i = i + 8) {
                    vertices[i] = starPoints[i / 8] - upMult;
                    vertices[i + 1] = starPoints[i / 8] + upMult;
                    vertices[i + 2] = starPoints[(i / 8) + 1] - upMult;
                    vertices[i + 3] = starPoints[(i / 8) + 1] + upMult;
                    vertices[i + 4] = starPoints[i / 8] - upMult;
                    vertices[i + 5] = starPoints[i / 8] + upMult;
                    vertices[i + 6] = starPoints[(i / 8) + 1] - upMult;
                    vertices[i + 7] = starPoints[(i / 8) + 1] + upMult;

                    tempPerpendicular = new Vector3(starPoints[i / 8].z - starPoints[(i / 8) + 1].z, 0, starPoints[(i / 8) + 1].x - starPoints[i / 8].x);

                    normals[i] = tempPerpendicular;
                    normals[i + 1] = tempPerpendicular;
                    normals[i + 2] = tempPerpendicular;
                    normals[i + 3] = tempPerpendicular;
                    normals[i + 4] = -tempPerpendicular;
                    normals[i + 5] = -tempPerpendicular;
                    normals[i + 6] = -tempPerpendicular;
                    normals[i + 7] = -tempPerpendicular;

                    uvs[i] = uvsBase[0];
                    uvs[i + 1] = uvsBase[1];
                    uvs[i + 2] = uvsBase[2];
                    uvs[i + 3] = uvsBase[3];
                    uvs[i + 4] = uvsBase[0];
                    uvs[i + 5] = uvsBase[1];
                    uvs[i + 6] = uvsBase[2];
                    uvs[i + 7] = uvsBase[3];
                }
                //Sets all the triangles
                for (int i = 0; i < singleTriangleVerticeNumber; i = i + 12) {
                    triangles[i] = 0 + ((4 * i) / 6);
                    triangles[i + 1] = 1 + ((4 * i) / 6);
                    triangles[i + 2] = 2 + ((4 * i) / 6);
                    triangles[i + 3] = 1 + ((4 * i) / 6);
                    triangles[i + 4] = 3 + ((4 * i) / 6);
                    triangles[i + 5] = 2 + ((4 * i) / 6);
                    triangles[i + 6] = 2 + ((4 * i) / 6);
                    triangles[i + 7] = 1 + ((4 * i) / 6);
                    triangles[i + 8] = 0 + ((4 * i) / 6);
                    triangles[i + 9] = 2 + ((4 * i) / 6);
                    triangles[i + 10] = 3 + ((4 * i) / 6);
                    triangles[i + 11] = 1 + ((4 * i) / 6);
                }

                //Finishing mesh
                mesh.vertices = vertices;
                mesh.normals = normals;
                mesh.uv = uvs;
                mesh.triangles = triangles;
                return mesh;
            }
        }
    }
}