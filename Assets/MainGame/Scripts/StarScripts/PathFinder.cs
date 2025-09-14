//Using _starlist, check the start & destination then try find a path

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PathFinder : MonoBehaviour {

    [Header("StarList")]
    public static PathFinder instance;
    UIManager canvas;
    PathManager _pathManager;
    CoroutineManager cm;

    IEnumerator pathOptionsIEnum;

    [Space]
    [Header("Script References")]
    StarGeneration starGen;
    bool isSearchingForPath = false;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        starGen = StarGeneration.instance;
        canvas = UIManager.Instance;
        cm = CoroutineManager.Instance;
    }

    public void PathFinderReset() {
        isSearchingForPath = false;
    }

    public void OnStarsGenerate() {
        SetupPathStart(null, Color.black, Color.black, -1, -1, UIManager.empty, UIManager.empty);
    }

    void RouteCalculater() {

        //Set up beginning values
        isSearchingForPath = true;
        starGen.cost = new float[StarGeneration._starList.Count];
        for (int i = 0; i < StarGeneration._starList.Count; i++) {
            starGen.cost[i] = Mathf.Infinity;
        }
        starGen.leadingStar = new int[StarGeneration._starList.Count];
        starGen.cost[starGen.startStarInt] = 0;

        cm.ManageStartCoroutine(PathFindOptions(), canvas.LoadingStarFlash("Loading", true), canvas.ResetLoadStar, "LoadStar");
    
    }

    void PathFinalSetup() {
        if (StarGeneration._starList.Count > 1 && StarGeneration.instance.finalStarPath.Count > 1) {
            UIManager.Instance.UpdatePathList(true);
            ShipController.instance.ShipSetup();
            PathManager.instance.DisplayBestPath();
        } else {
            UIManager.Instance.UpdatePathList(false);
        }
        //Sets everything for next run
        SetupPathStart(null, Color.black, Color.black, -1, -1, UIManager.empty, UIManager.empty);
        isSearchingForPath = false;
    }

    //Select the stars to travel between where the first selected star is the start
    public void SelectDestination(StarController selectedStar) {
        PathManager.instance.DisplayAllPaths();
        starGen.finalStarPath.Clear();
        int counter;

        if (StarGeneration.instance.hasGeneratedPaths && !isSearchingForPath) {
            counter = 0;
            //Selects first star of pathfinder
            if (starGen.startStarInt == -1) {

                foreach (StarController star in StarGeneration._starList) {
                    if (star == selectedStar) {
                        SetupPathStart(canvas.starSelectAudio, Color.green, Color.black, counter, -1, star.name, UIManager.empty);
                        break;
                    }
                    counter++;
                }
            }
            //Selects 2nd star to travel to
            else {

                foreach (StarController star in StarGeneration._starList) {
                    //Selects 2nd star if successful
                    if (star == selectedStar && counter != starGen.startStarInt) {
                        SetupPathStart(canvas.starSelectAudio, Color.green, Color.green, starGen.startStarInt, counter, null, star.name);
                        RouteCalculater();
                        break;
                    }

                    //If the same star is selected, deselect 1st star
                    else if (star == selectedStar && counter == starGen.startStarInt) {
                        SetupPathStart(canvas.starDeselectAudio, Color.black, Color.black, -1, -1, UIManager.empty, UIManager.empty);
                        break;
                    }
                    counter++;
                }
            }

        }
    }

    void SetupPathStart(AudioSource aud, Color colStart, Color colEnd, int start, int end, string startText, string endText) {

        if (aud != null) aud.Play();

        if (starGen.startStarInt != -1) StarGeneration._starList[starGen.startStarInt].ChangeColour(Color.black);
        if (starGen.starDestinationInt != -1) StarGeneration._starList[starGen.starDestinationInt].ChangeColour(Color.black);

        if (start != -1) StarGeneration._starList[start].ChangeColour(colStart);
        if (end != -1) StarGeneration._starList[end].ChangeColour(colEnd);

        starGen.startStarInt = start;
        starGen.starDestinationInt = end;

        if (startText != null) canvas._startStarTextUI.text = startText;
        if (endText != null) canvas._endStarTextUI.text = endText;
    }

    public void EndPathfinding() {
        isSearchingForPath = false;
    }

    IEnumerator PathFindOptions() {
        switch (UIManager.Instance.pathfindingOptions.value) {
            case 0: yield return BackTrack(UIManager.Instance.quickFindToggle.isOn); break;
            case 1: yield return QuickBackTrack(UIManager.Instance.quickFindToggle.isOn); break;
            case 2: yield return Dijkstra(UIManager.Instance.quickFindToggle.isOn); break;
        }
        PathFinalSetup();
    }
    void OptimizationOptions(List<int> list) {
        switch (UIManager.Instance.optimizationOptions.value) {
            case 0: starGen.finalStarPath = list.Select(x => StarGeneration._starList[x]).ToList(); break;
            case 1: starGen.finalStarPath = SkipPathWaypoints(list).Select(x => StarGeneration._starList[x]).ToList(); break;
        }
    }

    float cost;
    float bestCost = Mathf.Infinity;
    List<int> finalList = new List<int>();
    //Finds best path based on dictionary in _starlist variable
    IEnumerator BackTrack(bool quick) {

        isSearchingForPath = true;
        List<int> checkedStars = new List<int>();
        List<int> backtrackList = new List<int>();
        finalList = new List<int>();

        checkedStars.Add(starGen.startStarInt);
        backtrackList.Add(starGen.startStarInt);
        canvas.setLoadSliderMax = 1;
        canvas.setLoadSliderValue = 0;
        yield return CheckPath(new List<int> { starGen.startStarInt }, 1, 0);
        OptimizationOptions(finalList);
    }
    IEnumerator QuickBackTrack(bool quick) {

        List<int> checkedStars = new List<int>();
        List<int> backtrackList = new List<int>();
        finalList = new List<int>();
        checkedStars.Add(starGen.startStarInt);
        backtrackList.Add(starGen.startStarInt);
        canvas.setLoadSliderMax = 1;
        canvas.setLoadSliderValue = 0.5f;

        while (true) {

            //Check end of list to see any potential paths to check
            foreach (int x in GetListOfPaths(backtrackList.Last())) {

                if (!checkedStars.Contains(x)) {
                    checkedStars.Add(x);
                    backtrackList.Add(x);

                    goto FoundPath;
                }
            }

            //Backtrack 1 space
            backtrackList.RemoveAt(backtrackList.Count - 1);
            if (backtrackList.Count < 1)
                break;

            FoundPath:
            if (backtrackList.Last() == starGen.starDestinationInt) {
                finalList = new List<int>(backtrackList);
                break;
            }
            yield return null;
        }
        OptimizationOptions(finalList);
    }

    IEnumerator CheckPath(List<int> currentPathCheck, float currentSectionPercentage, float cumulativePercentage) {

        yield return null;
        if (canvas.quickFindToggle.isOn && finalList.Last() == starGen.starDestinationInt) yield break;

        if (currentPathCheck.Last() == starGen.starDestinationInt) {
            cost = StarGeneration.GetPathCost(currentPathCheck);
            if (cost < bestCost) {
                bestCost = cost;
                finalList = new List<int>(currentPathCheck);
                print("Cost: "+cost+", LIST = " + string.Join(", ", currentPathCheck));
                if (canvas.quickFindToggle.isOn && currentPathCheck.Last()==starGen.starDestinationInt) yield break;
            }
        }

        List<int> paths = GetListOfPaths(currentPathCheck.Last());
        float pathPercent=0;

        foreach (int i in paths) {
            pathPercent += currentSectionPercentage / paths.Count;
            if (!currentPathCheck.Contains(i)) {
                canvas.setLoadSliderValue = (cumulativePercentage + pathPercent);
                //print("Percent = "+ (cumulativePercentage + pathPercent) + ". ("+cumulativePercentage+""+pathPercent+"). Check " + currentPathCheck.Concat(new[] { i }).ToList() + ".");
                yield return CheckPath(currentPathCheck.Concat(new[] {i }).ToList(), currentSectionPercentage/paths.Count, cumulativePercentage+pathPercent);
            }
        }
    }

    IEnumerator Dijkstra(bool quick) {

        DisjktraMap[] shortestPaths = new DisjktraMap[StarGeneration._starList.Count];
        List<int> checkCurrentStarList = new List<int>();
        List<int> nextStarsList = new List<int> { starGen.startStarInt };
        float tempCost = 0;

        for (int i = 0; i < shortestPaths.Count(); i++) shortestPaths[i] = new DisjktraMap(i, Mathf.Infinity);
        shortestPaths[starGen.startStarInt] = new DisjktraMap(starGen.startStarInt, 0, new List<int> { starGen.startStarInt });

        //Loop through checking for cheaper path costs until none are found
        while (true) {
            checkCurrentStarList = new List<int>(nextStarsList);
            nextStarsList.Clear();

            foreach (int star in checkCurrentStarList) {
                foreach (int nextStar in StarGeneration.possibleStarPaths.Where(s => s.Key.x == star).Select(h=>h.Key.y)) {
                    tempCost = shortestPaths[star].currentCost + StarGeneration.possibleStarPaths[new Vector2Int(star, nextStar)].cost;
                    if (tempCost < shortestPaths[nextStar].currentCost) {
                        shortestPaths[nextStar].currentCost = tempCost;
                        shortestPaths[nextStar].path = new List<int>(shortestPaths[star].path);
                        shortestPaths[nextStar].path.Add(nextStar);
                        nextStarsList.Add(nextStar);
                        if (quick && nextStar == starGen.starDestinationInt) { goto GetOut; }
                    }
                }
            }
            if (nextStarsList.Count < 1) break;

            yield return null;
        }        
    GetOut:
        if (shortestPaths[starGen.starDestinationInt].path.Count > 1) {
            OptimizationOptions(new List<int>(shortestPaths[starGen.starDestinationInt].path));
        }
    }

    public List<int> SkipPathWaypoints(List<int> list) {

        List<int> res = new List<int>(list);

        for (int x = 0; x < res.Count; x++) {
            for (int j = res.Count - 1; j > x+1; j--) {
                if (StarGeneration.possibleStarPaths.ContainsKey(new Vector2Int(res[x], res[j]))
                    && StarGeneration.GetPathCost(res.GetRange(x, (j - x + 1))) > StarGeneration.GetPathCost(new List<int>{ res[x], res[j]})) {
                    res.RemoveRange(x + 1, j-x-1);
                    return SkipPathWaypoints(res);
                } 
            }
        }
        return res;
    }

    public List<int> GetListOfPaths(int id) {
        return StarGeneration.possibleStarPaths.Where(x => x.Key.x == id && x.Value.cost < Mathf.Infinity).Select(x => x.Key.y).ToList();
    }
}

[System.Serializable]
public struct DisjktraMap {
    public int id;
    public float currentCost;
    public List<int> path;

    public DisjktraMap(int id, float currentCost) {
        this.id = id;
        this.currentCost = currentCost;
        this.path = new List<int>();
    }
    public DisjktraMap(int id, float currentCost, List<int> path) {
        this.id = id;
        this.currentCost = currentCost;
        this.path = path;
    }
}