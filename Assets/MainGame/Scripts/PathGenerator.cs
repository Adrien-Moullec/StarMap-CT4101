using StarMaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerating;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class PathGenerator : MonoBehaviour, IPath
{
    [SerializeField] PathType pathType;
    MeshFilter mf;

    private void Awake() {
        mf = GetComponent<MeshFilter>();
    }

    public void DisplayPath() {
        Mesh f = new();
        switch (pathType) {
            case PathType.AllPossible:
                mf.mesh = MeshGeneration.PathGen.GeneratePossiblePaths(StarGeneration.possibleStarPaths.Select(x => x.Value).ToList());
                break;
            case PathType.GoodPaths:
                mf.mesh = MeshGeneration.PathGen.GeneratePossiblePaths(StarGeneration.possibleStarPaths.
                Where(x => x.Value.isNotEvil).Select(x => x.Value).ToList());
                break;
            case PathType.BadPaths:
                mf.mesh = MeshGeneration.PathGen.GeneratePossiblePaths(StarGeneration.possibleStarPaths.
                Where(x => !x.Value.isNotEvil).Select(x => x.Value).ToList());
                break;
            case PathType.Best:
                mf.mesh = MeshGeneration.PathGen.GenerateSinglePath(StarGeneration.instance.positionStarPath);
                break;
        }
    }

    public void ResetPaths() {
        if (mf != null)
            mf.mesh.Clear();
    }
}
[System.Serializable]
 public enum PathType {
    GoodPaths = 0,
    BadPaths = 1,
    Best = 2,
    AllPossible = 3
}