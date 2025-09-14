//Creates star system and resets _starlist values

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGeneration : MonoBehaviour {

    public static StarGeneration instance;
    public static List<StarController> _starList = new List<StarController>();
    public static Dictionary<Vector2Int, PathList> possibleStarPaths = new Dictionary<Vector2Int, PathList>();
    UIManager canvas;
    CoroutineManager cm;

    [Space]
    [Header("Object/Prefabs")]
    public GameObject starPrefab;
    public GameObject starListParent;
    [SerializeField] GameObject starInfoUI;

    [SerializeField] public GameObject PlanetCentre;
    [SerializeField] public List<GameObject> planetList;

    [HideInInspector] public bool hasGeneratedPaths;

    //Star ints to travel to and from
    public int startStarInt = -1;
    public int starDestinationInt = -1;
    PathList t;
    public List<int> starIntChecker = new List<int>();

    //Information relative to the first selected star
    public float[] cost;      //Cost to get to star from selected first star
    public int[] leadingStar; //Previous cheapest baseCost star to get back to the starting star
    public List<StarController> finalStarPath = new List<StarController>();

    IEnumerator GenerateStars;

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
        cm = CoroutineManager.Instance;
        EvilRegionCenter = GetRandomSpherePoint(canvas.evilRegionRange.Value);
    }

    public static float GetPathCost(List<int> path) {
        float finalCost = 0;
        if (!(path.Count > 1)) 
            return Mathf.Infinity;
        
        for (int i = 0; i < path.Count - 1; i++) {
            finalCost += possibleStarPaths[new Vector2Int(path[i], path[i+1])].cost;
        }

        return finalCost;
    }

    public void GenerateStarList() {

        _starList.Clear();

        for (int i = 0; i < canvas.spawnCount.Value; i++) {
            Vector3 pos = GetRandomSpherePoint(canvas.spawnRange.Value);
            PoolManager.Instance.TrySpawnFromPool<StarController>("star", out StarController tempStar);
            tempStar.starName = UIManager.planetNames[Random.Range(0, UIManager.planetNames.Count)] +"-"+ i.ToString();
            _starList.Add(tempStar);
            tempStar.transform.position = pos;
        }
        PathFinder.instance.OnStarsGenerate();

        cm.ManageStartCoroutine(StarPathsCalc(), canvas.LoadingStarFlash("Loading", true), canvas.ResetLoadStar, "LoadStar");
    }

    //Find the path costs & Finding the closest star to the center :3
    public IEnumerator StarPathsCalc() {
        int tempStarInt = 0;
        float tempCost;
        possibleStarPaths.Clear();
        canvas.setLoadSliderMax = _starList.Count;

        for (int startStar = 0; startStar < _starList.Count; startStar++) {
            canvas.setLoadSliderValue = startStar;

            if (Vector3.Distance(_starList[startStar].transform.position, Vector3.zero) < Vector3.Distance(_starList[tempStarInt].transform.position, Vector3.zero)) {
                tempStarInt = startStar;
            }
            //Find all path lengths and add all working paths to list
            for (int endStar = 0; endStar < _starList.Count; endStar++) {
                tempCost = Vector3.Distance(_starList[startStar].transform.position, _starList[endStar].transform.position) + (_starList[startStar].gravitationCost + _starList[startStar].gravitationCost)/2;

                if (tempCost <= (canvas.leapDistance.Value) && startStar!=endStar) {
                    CheckStarPath(startStar,endStar,tempCost);
                    CheckStarPath(endStar,startStar,tempCost);
                }
            }
            yield return null;
        }

        hasGeneratedPaths = true;

        PathManager.instance.DisplayAllPaths();

        canvas.loadingStarActive = false;
    }

    void CheckStarPath(int start, int end, float costCheck) {
        if (possibleStarPaths.ContainsKey(new Vector2Int(start, end))) {
            if (possibleStarPaths[new Vector2Int(start, end)].baseCost < costCheck)
                SetStarPath(start, end, costCheck);
        } else { SetStarPath(start, end, costCheck); }
    }

    void SetStarPath(int start, int end, float cost) {
        t = new();
        t.startPoint = _starList[start].transform.position;
        t.endPoint = _starList[end].transform.position;
        t.baseCost = cost;
        t.goodPath = !IsEvilStar(start) && !IsEvilStar(end);

        if (possibleStarPaths.ContainsKey(new Vector2Int(start, end))) 
            possibleStarPaths[new Vector2Int(start, end)] = t;
        else
            possibleStarPaths.Add(new Vector2Int(start, end), t);
    }

    bool IsEvilStar(int checkStar) {
        if (Vector3.Distance(EvilRegionCenter, _starList[checkStar].transform.position) < canvas.evilRegionRange.Value) {
            _starList[checkStar].ChangeParticleColor(Color.red);
            return true;
        } else {
            _starList[checkStar].ChangeParticleColor();
            return false;
        }
    }

    Vector3 GetRandomCubePoint(float range) {
        return new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }

    Vector3 randSpherePoint;
    Vector3 GetRandomSpherePoint(float radius) {
        Vector3 randSpherePoint = GetRandomCubePoint(radius);
        if (Vector3.Distance(Vector3.zero, randSpherePoint) > radius) return GetRandomSpherePoint(radius);
        return randSpherePoint;
    }

    public void ResetStarGeneration() {
        PoolManager.Instance.DespawnByTag("star");
        canvas.ResetStars();
        _starList.Clear();
        ClearPathsAndLoadingStarData();
        GenerateStarList();
    }

    public void ResetPathsGeneration() {
        ClearPathsAndLoadingStarData();
        cm.ManageStartCoroutine(StarPathsCalc(), canvas.LoadingStarFlash("Loading", true), canvas.ResetLoadStar, "LoadStar");
    }

    public void ClearPathsAndLoadingStarData() {
        canvas.starSelectAudio.Play();
        CoroutineManager.Instance.ForceStopCoroutine("LoadStar");
        PathManager.instance.ClearPaths();
        finalStarPath.Clear();
        possibleStarPaths.Clear();
        hasGeneratedPaths = false;
        PathFinder.instance.PathFinderReset();
    }
}

[System.Serializable]
public struct PathList {
    public Vector3 startPoint;
    public Vector3 endPoint;
    public bool goodPath;
    public float baseCost;
    public float cost { 
        get { return (baseCost < UIManager.Instance.leapDistance.Value ? baseCost * (goodPath ? 1 : UIManager.Instance.evilRegionMult.Value) : Mathf.Infinity); } 
        set { baseCost = value; }
    }
}

