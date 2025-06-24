//Creates star system and resets _starlist values

using MeshGenerating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class StarGeneration : MonoBehaviour {

    public static StarGeneration instance;
    public static List<StarController> _starList = new List<StarController>();
    public static Dictionary<Vector2Int, PathList> possibleStarPaths = new Dictionary<Vector2Int, PathList>();
    UIManager canvas;

    [Space]
    [Header("Object/Prefabs")]
    public GameObject starPrefab;
    public GameObject starListParent;
    [SerializeField] GameObject starInfoUI;

    [SerializeField] public GameObject PlanetCentre;
    [SerializeField] public List<GameObject> planetList;

    //Colour section
    float tempCost;

    [HideInInspector] public bool hasGeneratedPaths;

    //Star ints to travel to and from
    public int startStarInt;
    public int starDestinationInt;
    public List<int> starIntChecker = new List<int>();

    //Information relative to the first selected star
    public float[] cost;      //Cost to get to star from selected first star
    public int[] leadingStar; //Previous cheapest cost star to get back to the starting star
    public List<StarController> finalStarPath = new List<StarController>();
    public List<Vector3> positionStarPath {
        get {
            List<Vector3> res = new List<Vector3>();
            foreach (StarController s in finalStarPath) res.Add(s.transform.position);
            return res;
        }
    }

    public Vector3 EvilRegionCenter;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        canvas = UIManager.Instance;
        EvilRegionCenter = new Vector3(UnityEngine.Random.Range(-canvas.evilRegionRange, canvas.evilRegionRange), UnityEngine.Random.Range(-canvas.evilRegionRange, canvas.evilRegionRange), UnityEngine.Random.Range(-canvas.evilRegionRange, canvas.evilRegionRange));
    }

    public static float GetPathCost(List<int> path) {
        float finalCost = 0;
        if (!(path.Count > 1))
            return Mathf.Infinity;
        for (int i = 0; i < path.Count - 1; i++) {
            finalCost += StarGeneration.possibleStarPaths[new Vector2Int(path[i], path[i+1])].cost;
        }
        return finalCost;
    }

    public void GenerateStarList() {

        _starList.Clear();

        for (int i = 0; i < canvas.spawnCount; i++) {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange), UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange), UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange));
            PoolManager.Instance.TrySpawnFromPool<StarController>("star", out StarController tempStar);
            tempStar.starName = StarPetNames.names[UnityEngine.Random.Range(0, StarPetNames.names.Length)] +"-"+ i.ToString();
            _starList.Add(tempStar);
        }
        StartCoroutine(StarPathsCalc());
    }

    //Find the path costs & Finding the closest star to the center :3
    public IEnumerator StarPathsCalc() {
        int tempStarInt = 0;
        bool isEvilPath;
        possibleStarPaths.Clear();

        PathList t;

        for (int startStar = 0; startStar < _starList.Count; startStar++) {
            if (Vector3.Distance(_starList[startStar].transform.position, Vector3.zero) < Vector3.Distance(_starList[tempStarInt].transform.position, Vector3.zero)) {
                tempStarInt = startStar;
            }
            //Find all path lengths and add all working paths to list
            for (int endStar = 0; endStar < _starList.Count; endStar++) {
                tempCost = Vector3.Distance(_starList[startStar].transform.position, _starList[endStar].transform.position) + (_starList[startStar].gravitationCost + _starList[startStar].gravitationCost)/2;
                if (tempCost <= (canvas.leapDistance) && startStar!=endStar) {
                    isEvilPath = Vector3.Distance(EvilRegionCenter, _starList[startStar].transform.position) < canvas.evilRegionRange || Vector3.Distance(EvilRegionCenter, _starList[startStar].transform.position) < canvas.evilRegionRange;
                    if (!possibleStarPaths.ContainsKey(new Vector2Int(startStar, endStar))) {
                        t = new();
                        t.startPoint = _starList[startStar].transform.position;
                        t.endPoint = _starList[endStar].transform.position;
                        t.cost = tempCost * (isEvilPath ? 1 : canvas.evilRegionMult);
                        t.isNotEvil = !isEvilPath;
                        possibleStarPaths.Add(new Vector2Int(startStar, endStar), t);
                    }
                    if (!possibleStarPaths.ContainsKey(new Vector2Int(endStar, startStar))) {
                        t = new();
                        t.endPoint = _starList[startStar].transform.position;
                        t.startPoint = _starList[endStar].transform.position;
                        t.cost = tempCost * (isEvilPath ? 1 : canvas.evilRegionMult);
                        t.isNotEvil = !isEvilPath;
                        possibleStarPaths.Add(new Vector2Int(endStar, startStar), t);
                    }
                    if (isEvilPath) {
                        _starList[startStar].ChangeParticleColor(Color.red);
                        _starList[endStar].ChangeParticleColor(Color.red);
                    }
                }
            }
            yield return null;
        }
        hasGeneratedPaths = true;
        PathManager.instance.DisplayAllPaths();
    }


    //Resets everything to generate new stars
    public void ResetInitiation() {
        PoolManager.Instance.DespawnByTag("star");

        PathManager.instance.ClearPaths();
        UIManager.Instance.ResetStars();
        PathManager.instance.ClearPaths();

        finalStarPath.Clear();
        possibleStarPaths.Clear();
        _starList.Clear();

        hasGeneratedPaths = false;

        GenerateStarList();
    }

    //Quit
    public void ExitGame() {
        Application.Quit();
    }
}

//A list of random names for stars
public class StarPetNames {
    static public string[] names = {
    "Max", "Bella", "Charlie", "Lucy", "Cooper", "Daisy", "Rocky", "Lola", "Buddy", "Sadie", "Jack", "Molly", "Duke", "Lily", "Teddy", "Ruby", "Toby", "Maggie", "Oliver", "Chloe", "Leo", "Sophie", "Winston", "Roxy", "Milo", "Zoey", "Oscar", "Penny", "Riley", "Gracie", "Abby", "Bear", "Coco", "Jackson", "Layla", "Harvey", "Stella", "Bentley", "Willow", "Sammy", "Murphy", "Luna", "Gus", "Daryl", "James", "Olive", "Rosie", "Hazel", "Gizmo", "Nala", "Louie", "Princess", "Dexter", "Maya", "Bruno", "Phoebe", "Jasper", "Piper", "Penelope", "Henry", "Winnie", "Archie", "Ellie", "Zeus", "Millie", "Boomer", "Lulu", "Diesel", "Apollo", "Poppy", "Buster", "Dixie", "Brody", "Finn", "Chase", "Marley", "Kobe", "Baxter", "Beau", "Gunner", "Tucker", "Leo", "Jax"
    };
}

[System.Serializable]
public struct PathList {
    public Vector3 startPoint;
    public Vector3 endPoint;
    public bool isNotEvil;
    public float cost;
}

