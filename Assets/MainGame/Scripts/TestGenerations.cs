using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestGenerations : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;

    Mesh Mesh;

    private void Awake() {
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        Mesh = mf.mesh;
    }
}
