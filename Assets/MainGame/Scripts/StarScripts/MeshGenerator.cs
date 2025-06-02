//Script designed to create mesh path between stars

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    [Space]
    [Header("Mesh properties")]
    public Mesh mesh;
    [SerializeField] Vector3[] vertices;
    [SerializeField] Vector3[] normals;
    [SerializeField] Vector2[] uvs;
    [SerializeField] int[] triangles;

    [Space]
    [Header("Custom path editor")]
    [SerializeField] float pathWidth;

    [Space]
    [Header("Player Camera")]
    [SerializeField] GameObject cam;

    //arrays
    Vector2[] uvsBase;
    List<Vector3> starPoints;

    //Extra triangle info
    int triangleVerticeNumber;
    int singleTriangleVerticeNumber;
    Vector3 upMult;
    Vector3 tempPerpendicular;

    private void Awake() {
        mesh = new Mesh();
        mesh.name = "StarPathMesh";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        uvsBase = new Vector2[] {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1),
        };
    }

    //Generate mesh for multiple paths
    public IEnumerator GeneratePossiblePaths(List<Vector3> starPoints1, List<Vector3> starPoints2) {

        //Sets up for mesh gen
        upMult = Vector3.up * pathWidth;
        triangleVerticeNumber = (starPoints1.Count) * 4;
        singleTriangleVerticeNumber = (starPoints1.Count) * 6;
        vertices = new Vector3[triangleVerticeNumber];
        normals = new Vector3[triangleVerticeNumber];
        uvs = new Vector2[triangleVerticeNumber];
        triangles = new int[singleTriangleVerticeNumber];

        //Sets all the triangle info for 1 quad double side
        for (int i = 0; i < triangleVerticeNumber; i = i + 4) {
            vertices[i] = starPoints1[i/4] - upMult;
            vertices[i + 1] = starPoints1[i/4] + upMult;
            vertices[i + 2] = starPoints2[i/4] - upMult;
            vertices[i + 3] = starPoints2[i/4] + upMult;

            tempPerpendicular = new Vector3(starPoints1[i / 4].z - starPoints2[i / 4].z, 0, starPoints2[i / 4].x - starPoints1[i / 4].x);

            normals[i] = tempPerpendicular;
            normals[i + 1] = tempPerpendicular;
            normals[i + 2] = tempPerpendicular;
            normals[i + 3] = tempPerpendicular;

            uvs[i] = uvsBase[0];
            uvs[i+1] = uvsBase[1];
            uvs[i+2] = uvsBase[2];
            uvs[i+3] = uvsBase[3];
        }

        //Sets all triangles
        for (int i = 0; i < singleTriangleVerticeNumber; i = i + 6) {
            triangles[i] = 0 + ((4 * i) / 6); triangles[i + 1] = 1 + ((4 * i) / 6); triangles[i + 2] = 2 + ((4 * i) / 6);
            triangles[i + 3] = 1 + ((4 * i) / 6); triangles[i + 4] = 3 + ((4 * i) / 6); triangles[i + 5] = 2 + ((4 * i) / 6);
        }

        //Final mesh properties
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        yield return new WaitForSeconds(0.01f);
    }
    
    //Generate mesh for single path line
    public IEnumerator GeneratePath(List<StarController> stars) {

        //Prepare for mesh gen
        starPoints = new();
        mesh.Clear();
        foreach (StarController star in stars) {
            starPoints.Add(star.transform.position);
        }
        upMult = Vector3.up * pathWidth;
        triangleVerticeNumber = (starPoints.Count-1) * 8;
        singleTriangleVerticeNumber = (starPoints.Count-1) * 12;
        vertices = new Vector3[triangleVerticeNumber];
        normals = new Vector3[triangleVerticeNumber];
        uvs = new Vector2[triangleVerticeNumber];
        triangles = new int[singleTriangleVerticeNumber];

        //Sets all the triangle info for 1 quad double side
        for (int i = 0; i < triangleVerticeNumber; i = i + 8) {
            vertices[i] = starPoints[i/8] - upMult;
            vertices[i + 1] = starPoints[i/8] + upMult;
            vertices[i + 2] = starPoints[(i/8)+1] - upMult;
            vertices[i + 3] = starPoints[(i/8)+1] + upMult;
            vertices[i + 4] = starPoints[i/8] - upMult;
            vertices[i + 5] = starPoints[i/8] + upMult;
            vertices[i + 6] = starPoints[(i/8)+1] - upMult;
            vertices[i + 7] = starPoints[(i/8)+1] + upMult;

            tempPerpendicular = new Vector3(starPoints[i/8].z - starPoints[(i/8)+1].z, 0, starPoints[(i/8)+1].x - starPoints[i/8].x);

            normals[i] = tempPerpendicular;
            normals[i + 1] = tempPerpendicular;
            normals[i + 2] = tempPerpendicular;
            normals[i + 3] = tempPerpendicular;
            normals[i + 4] = -tempPerpendicular;
            normals[i + 5] = -tempPerpendicular;
            normals[i + 6] = -tempPerpendicular;
            normals[i + 7] = -tempPerpendicular;

            uvs[i] = uvsBase[0];
            uvs[i+1] = uvsBase[1];
            uvs[i+2] = uvsBase[2];
            uvs[i+3] = uvsBase[3];
            uvs[i+4] = uvsBase[0];
            uvs[i+5] = uvsBase[1];
            uvs[i+6] = uvsBase[2];
            uvs[i+7] = uvsBase[3];
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
        yield return new WaitForSeconds(0.01f);
    }
}
