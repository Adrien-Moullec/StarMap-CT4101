//Creates star system and resets _starlist values

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarGeneration : MonoBehaviour {

    public static StarGeneration instance;
    public static List<StarController> _starList = new List<StarController>();
    public Dictionary<Vector2, float> possibleStarPaths = new Dictionary<Vector2, float>();
    UIManager canvas;

    [Space]
    [Header("Object/Prefabs")]
    public GameObject starPrefab;
    public GameObject starListParent;
    [SerializeField] GameObject starInfoUI;

    [SerializeField] public GameObject PlanetCentre;
    [SerializeField] public List<GameObject> planetList;

    [Space]
    [Header("Scripts")]
    [SerializeField] MeshGenerator _meshGenAllPaths;
    [SerializeField] MeshGenerator _meshGenBestPaths;
    PathFinder _pathFinder;

    //Colour section
    float tempCost;

    //star duo lists
    List<Vector3> meshPointList1; List<Vector3> meshPointList2;


    //Star ints to travel to and from
    public int startStarInt;
    public int starDestinationInt;
    public List<int> starIntChecker;

    //Information relative to the first selected star
    public float[] cost;      //Cost to get to star from selected first star
    public int[] leadingStar; //Previous cheapest cost star to get back to the starting star
    public List<StarController> finalStarPath; //Final list of planets in path

    private void Awake() {
        instance = this;
        _pathFinder = PathFinder.instance;
        meshPointList1 = new List<Vector3>();
        meshPointList2 = new List<Vector3>();
    }

    public void GenerateStarList() {

        canvas = UIManager.Instance;

        for (int i = 0; i < canvas.spawnCount; i++) {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange), UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange), UnityEngine.Random.Range(-canvas.spawnRange, canvas.spawnRange));
            GameObject tempStar = Instantiate(starPrefab, pos, Quaternion.identity);
            tempStar.transform.SetParent(starListParent.transform, true);
        }
        StartCoroutine(StarPathsCalc());
    }

    //Find the path costs & Finding the closest star to the center :3
    public IEnumerator StarPathsCalc() {
        int tempStarInt = 0;
        meshPointList1.Clear();
        meshPointList2.Clear();
        possibleStarPaths.Clear();

        for (int startStar = 0; startStar < _starList.Count; startStar++) {
            if (Vector3.Distance(_starList[startStar].transform.position, Vector3.zero) < Vector3.Distance(_starList[tempStarInt].transform.position, Vector3.zero)) {
                tempStarInt = startStar;
            }
            //Find all path lengths and add all working paths to list
            for (int endStar = 0; endStar < _starList.Count; endStar++) {
                tempCost = Vector3.Distance(_starList[startStar].transform.position, _starList[endStar].transform.position) + (_starList[startStar].gravitationCost + _starList[startStar].gravitationCost)/2;
                if (tempCost <= (canvas.leapDistance) && startStar!=endStar) {
                    possibleStarPaths.Add(new Vector2(startStar, endStar), tempCost);
                    meshPointList1.Add(_starList[startStar].transform.position);
                    meshPointList2.Add(_starList[endStar].transform.position);
                }
                else {
                    possibleStarPaths.Add(new Vector2(startStar, endStar), Mathf.Infinity);
                }
            }
            yield return null;
        }
        StartCoroutine(_meshGenAllPaths.GeneratePossiblePaths(meshPointList1, meshPointList2));
        _pathFinder.hasGeneratedStars = true;
    }


    //Resets everything to generate new stars
    public void ResetInitiation() {
        foreach (StarController star in _starList) {
            Destroy(star.gameObject);
        }

        _pathFinder.allPathMesh.SetActive(true); _pathFinder.bestPathMesh.SetActive(false);
        _meshGenAllPaths.mesh.Clear();
        _meshGenBestPaths.mesh.Clear();
        finalStarPath.Clear();
        possibleStarPaths.Clear();

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