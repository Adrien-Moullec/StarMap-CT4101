using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarMaps;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    [SerializeField] GameObject[] allPathsGO;
    [SerializeField] GameObject bestPathGO;

    private void Awake() {
        instance = this;
        bestPathGO.SetActive(false);
        foreach (GameObject pathGen in allPathsGO) {
            pathGen.SetActive(false);
        }
    }

    public void DisplayAllPaths() {
        bestPathGO.SetActive(false);
        foreach (GameObject pathGen in allPathsGO) {
            pathGen.SetActive(true);
            pathGen.GetComponent<IPath>().DisplayPath();
        }
    }

    public void DisplayBestPath() {
        foreach (GameObject pathGen in allPathsGO) {
            pathGen.SetActive(false);
        }
        bestPathGO.SetActive(true);
        bestPathGO.GetComponent<IPath>().DisplayPath();
    }

    public void ClearPaths() {
        foreach (GameObject pathGen in allPathsGO) {
            pathGen.GetComponent<IPath>().ResetPaths();
        }
        bestPathGO.GetComponent<IPath>().ResetPaths();
    }
}
