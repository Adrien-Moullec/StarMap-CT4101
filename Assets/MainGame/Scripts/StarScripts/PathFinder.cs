//Using _starlist, check the start & destination then try find a path

using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder : MonoBehaviour {

    [Header("StarList")]
    public static PathFinder instance;
    UIManager _uiManager;
    PathManager _pathManager;    

    [Space]
    [Header("Script References")]
    StarGeneration starGen;
    bool isSearchingForPath = false;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        starGen = StarGeneration.instance;
        _uiManager = UIManager.Instance;

        starGen.startStarInt = -1;
        starGen.starDestinationInt = -1;
    }

    IEnumerator RouteCalculater() {

        //Set up beginning values
        isSearchingForPath = true;
        starGen.cost = new float[StarGeneration._starList.Count];
        for (int i = 0; i < StarGeneration._starList.Count; i++) {
            starGen.cost[i] = Mathf.Infinity;
        }
        starGen.leadingStar = new int[StarGeneration._starList.Count];
        starGen.cost[starGen.startStarInt] = 0;

        //Find the cost paths from LEFT to TOP
        StartCoroutine(UIManager.Instance.LoadingStarFlash("Calculating"));
        yield return PathFindOptions();

        if (StarGeneration._starList.Count > 1 && StarGeneration.instance.finalStarPath.Count > 1) {
            UIManager.Instance.UpdatePathList(true);
            ShipController.instance.ShipSetup();
            PathManager.instance.DisplayBestPath();
        } else {
            UIManager.Instance.UpdatePathList(false);
        }

        UIManager.Instance.continueLoad = false;
        //Sets everything for next run
        StarGeneration._starList[starGen.startStarInt].ChangeColour(Color.black);
        StarGeneration._starList[starGen.starDestinationInt].ChangeColour(Color.black);
        starGen.startStarInt = -1;
        starGen.starDestinationInt = -1;
        isSearchingForPath = false;
    }

    //Select the stars to travel between where the first selected star is the start
    public IEnumerator SelectDestination(StarController selectedStar) {
        PathManager.instance.DisplayAllPaths();
        starGen.finalStarPath.Clear();
        int counter;

        if (StarGeneration.instance.hasGeneratedPaths && !isSearchingForPath) {
            //Selects first star of pathfinder
            if (starGen.startStarInt == -1) {
                counter = 0;
                foreach (StarController star in StarGeneration._starList) {
                    if (star == selectedStar) {
                        _uiManager.starSelectAudio.Play();
                        starGen.startStarInt = counter;
                        StarGeneration._starList[starGen.startStarInt].ChangeColour(Color.green);

                        _uiManager._startStarTextUI.text = star.name;
                        _uiManager._endStarTextUI.text = "---";
                        break;
                    }
                    counter++;
                }
            }
            //Selects 2nd star to travel to
            else if (StarGeneration.instance.startStarInt != -1) {
                counter = 0;
                foreach (StarController star in StarGeneration._starList) {

                    //Selects 2nd star if successful
                    if (star == selectedStar && counter != starGen.startStarInt) {
                        _uiManager.starSelectAudio.Play();
                        starGen.starDestinationInt = counter;
                        _uiManager._endStarTextUI.text = star.name;
                        StarGeneration._starList[starGen.starDestinationInt].ChangeColour(Color.green);
                        yield return RouteCalculater();

                        break;
                    }

                    //If the same star is selected, deselect 1st star
                    else if (star == selectedStar && counter == starGen.startStarInt) {
                        _uiManager.starDeselectAudio.Play();
                        StarGeneration._starList[starGen.startStarInt].ChangeColour(Color.black);
                        starGen.startStarInt = -1;
                        starGen.starDestinationInt = -1;
                        _uiManager._startStarTextUI.text = "---";
                        _uiManager._endStarTextUI.text = "---";
                    }
                    counter++;
                }
            }

        }
    }

    public void EndPathfinding() {
        isSearchingForPath = false;
    }

    IEnumerator PathFindOptions() {
        switch (UIManager.Instance.pathfindingOptions.value) {
            case 0: yield return BackTrack(UIManager.Instance.quickFindToggle.isOn); break;
            case 1: yield return Dijkstra(UIManager.Instance.quickFindToggle.isOn); break;
        }
    }
    void OptimizationOptions(List<int> list) {
        switch (UIManager.Instance.optimizationOptions.value) {
            case 0: starGen.finalStarPath = list.Select(x => StarGeneration._starList[x]).ToList(); break;
            case 1: starGen.finalStarPath = SkipPathWaypoints(list).Select(x => StarGeneration._starList[x]).ToList(); break;
        }
    }

    //Finds best path based on dictionary in _starlist variable
    IEnumerator BackTrack(bool quick) {

        isSearchingForPath = true;
        List<int> checkedStars = new List<int>(); 
        List<int> backtrackList = new List<int>(); 
        List<int> finalList = new List<int>();

        bool foundNextPath;
        float cost;
        float bestCost = Mathf.Infinity;
        checkedStars.Add(starGen.startStarInt);
        backtrackList.Add(starGen.startStarInt);

        //Keep looping until the whole area is backtracked
        while (true) {
            foundNextPath = false;

            //Check end of list to see any potential paths to check
            foreach (int x in GetListOfPaths(backtrackList.Last())) {

                if (!checkedStars.Contains(x)) {
                    checkedStars.Add(x);
                    backtrackList.Add(x);
                    foundNextPath = true;

                    if (backtrackList.Last() == starGen.starDestinationInt) {
                        cost = StarGeneration.GetPathCost(backtrackList);
                        if (cost < bestCost) {
                            bestCost = cost;
                            finalList = new List<int>(backtrackList);
                        }
                    }
                    break;
                }
            }

            //If there are no more next paths to take, remove the final list item
            if (!foundNextPath) {
                backtrackList.RemoveAt(backtrackList.Count - 1);
                if (backtrackList.Count < 1) {
                    break;
                }
            }
            if (backtrackList.Last() == starGen.starDestinationInt && quick) {
                break;
            }
            yield return null;
        }

        if (finalList.Count > 1) {
            OptimizationOptions(finalList);
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
                    && StarGeneration.GetPathCost(res.GetRange(x, (j - x - 1))) < StarGeneration.GetPathCost(new List<int>{ res[x], res[j]})) {
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