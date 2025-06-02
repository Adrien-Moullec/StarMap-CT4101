//Using _starlist, check the start & destination then try find a path

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathFinder : MonoBehaviour {

    [Header("StarList")]
    UIManager _uiManager;
    public static PathFinder instance;
    

    [Space]
    [Header("Script References")]
    StarGeneration _starGenInstance;
    [HideInInspector] public bool hasGeneratedStars;
    bool hasGeneratedPaths = true;

    [Space]
    [Header("Text and Object References")]
    [SerializeField] GameObject playerCamera;
    [SerializeField] LoadingStar _loadingStar;

    [Space]
    [Header("Mesh Generator")]
    public GameObject allPathMesh;
    public GameObject bestPathMesh;
    [SerializeField] MeshGenerator _meshGenBestPath;


    //Temp variables
    int tempSecondStarInt;
    float tempSecondStarCost;
    int pathCheckFirstStar;
    int counter;
    bool continuePathfinderLoop;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        _starGenInstance = StarGeneration.instance;
        _uiManager = UIManager.Instance;

        _starGenInstance.startStarInt = -1;
        _starGenInstance.starDestinationInt = -1;
        allPathMesh.SetActive(true); bestPathMesh.SetActive(false);
    }

    async Task RouteCalculater() {

        //Set up beginning values
        _meshGenBestPath.mesh.Clear();
        hasGeneratedPaths = false;
        for (int i = 0; i < StarGeneration._starList.Count; i++) {
            _starGenInstance.cost[i] = Mathf.Infinity;
        }
        _starGenInstance.leadingStar = new int[StarGeneration._starList.Count];
        _starGenInstance.cost[_starGenInstance.startStarInt] = 0;
        pathCheckFirstStar = _starGenInstance.startStarInt; //LeftVar

        //Find the cost paths from LEFT to TOP
        StartCoroutine(UIManager.Instance.LoadingStarFlash("Calculating"));
        await CalculatePath();
        UIManager.Instance.continueLoad = false;

        //Make a star path list if the planet is reachable
        if (_starGenInstance.cost[_starGenInstance.starDestinationInt] != Mathf.Infinity) {
            allPathMesh.SetActive(false); bestPathMesh.SetActive(true);
            //Making path of stars list
            _starGenInstance.finalStarPath.Clear();
            int currentListInt = _starGenInstance.starDestinationInt;

            //Creates list of stars in the best path
            while (_starGenInstance.startStarInt != currentListInt) {
                await Task.Delay((int)0);
                if (_starGenInstance.finalStarPath.Contains(StarGeneration._starList[currentListInt])) { break; }
                _starGenInstance.finalStarPath.Add(StarGeneration._starList[currentListInt]);
                currentListInt = _starGenInstance.leadingStar[currentListInt];

            }
            _starGenInstance.finalStarPath.Add(StarGeneration._starList[currentListInt]);
            StartCoroutine(_meshGenBestPath.GeneratePath(_starGenInstance.finalStarPath));

            //Update the star list on the right
            UIManager.Instance.UpdatePathList(true);
            ShipController.instance.ShipSetup();
        }

        //If no path can be found
        else {
            UIManager.Instance.UpdatePathList(false);
            hasGeneratedPaths = true;
        }

        //Sets everything for next run
        StarGeneration._starList[_starGenInstance.startStarInt].ChangeColour(Color.black);
        StarGeneration._starList[_starGenInstance.starDestinationInt].ChangeColour(Color.black);
        _starGenInstance.startStarInt = -1;
        _starGenInstance.starDestinationInt = -1;
    }

    //Select the stars to travel between where the first selected star is the start
    public async void SelectDestination() {

        if (hasGeneratedStars && hasGeneratedPaths && false) {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                print("HIT "+hit.collider.name);

                //Selects first star of pathfinder
                if (_starGenInstance.startStarInt == -1 && hit.transform.gameObject.tag == "Star") {
                    counter = 0;
                    foreach (StarController star in StarGeneration._starList) {
                        if (star == hit.transform.gameObject) {
                            _uiManager.starSelectAudio.Play();
                            allPathMesh.SetActive(true); bestPathMesh.SetActive(false);
                            _starGenInstance.startStarInt = counter;
                            StarGeneration._starList[_starGenInstance.startStarInt].ChangeColour(Color.green);
                            _uiManager._startStarTextUI.text = star.name;
                            _uiManager._endStarTextUI.text = "---";
                            return;
                        }
                        counter++;
                    }
                }

                //Selects 2nd star to travel to
                else if (StarGeneration.instance.startStarInt != -1 && hit.transform.gameObject.tag == "Star") {
                    counter = 0;
                    foreach (StarController star in StarGeneration._starList) {

                        //Selects 2nd star if successful
                        if (star == hit.transform.gameObject && counter != _starGenInstance.startStarInt) {
                            _uiManager.starSelectAudio.Play();
                            allPathMesh.SetActive(true); bestPathMesh.SetActive(false);
                            _starGenInstance.starDestinationInt = counter;
                            _uiManager._endStarTextUI.text = star.name;
                            StarGeneration._starList[_starGenInstance.starDestinationInt].ChangeColour(Color.green);
                            await RouteCalculater();

                            return;
                        }

                        //If the same star is selected, deselect 1st star
                        else if (star == hit.transform.gameObject && counter == _starGenInstance.startStarInt) {
                            _uiManager.starDeselectAudio.Play();
                            if (_meshGenBestPath.mesh.subMeshCount == 0) {
                                allPathMesh.SetActive(false); bestPathMesh.SetActive(true);
                            }
                            else {
                                allPathMesh.SetActive(true); bestPathMesh.SetActive(false);
                            }
                            StarGeneration._starList[_starGenInstance.startStarInt].ChangeColour(Color.black);
                            _starGenInstance.startStarInt = -1;
                            _starGenInstance.starDestinationInt = -1;
                            _uiManager._startStarTextUI.text = "---";
                            _uiManager._endStarTextUI.text = "---";
                        }
                        counter++;
                    }
                }
            }
        }
    }

    public void ENDTHING() {
        hasGeneratedPaths = true;
    }

    //Finds best path based on dictionary in _starlist variable
    async Task CalculatePath() {
        _starGenInstance.starIntChecker = new(_starGenInstance.startStarInt);
        counter = 0;
        continuePathfinderLoop = true;

        //Continue loop until all paths have been checked
        while (continuePathfinderLoop) {
            pathCheckFirstStar = _starGenInstance.starIntChecker[counter];

            //All the stars to check against from the first
            for (int i = 0; i < StarGeneration._starList.Count; i++) {

                if (_starGenInstance.cost[pathCheckFirstStar] != Mathf.Infinity && _starGenInstance.possibleStarPaths[new Vector2(pathCheckFirstStar, i)] != Mathf.Infinity) {
                    tempSecondStarInt = i;
                    tempSecondStarCost = _starGenInstance.possibleStarPaths[new Vector2(pathCheckFirstStar, tempSecondStarInt)] + _starGenInstance.cost[pathCheckFirstStar]; //cost = distance between 2 stars + the current cost of the star the cost is being calculated to
                    if (_starGenInstance.cost[tempSecondStarInt] > tempSecondStarCost) {
                        _starGenInstance.cost[tempSecondStarInt] = tempSecondStarCost;
                        _starGenInstance.leadingStar[tempSecondStarInt] = pathCheckFirstStar;

                        //If the Second Star int has already been added to the list it won't need to be checked
                        if (!_starGenInstance.starIntChecker.Contains(tempSecondStarInt)) {
                            _starGenInstance.starIntChecker.Add(tempSecondStarInt);
                        }
                    }
                }

                await Task.Delay((int)0);
            }

            //The while loop ends if the end has been found or all alternatives have been checked
            if (counter + 1 > _starGenInstance.starIntChecker.Count - 1 || _starGenInstance.cost[_starGenInstance.starDestinationInt] != Mathf.Infinity) {
                continuePathfinderLoop = false;
            }
            counter++;
        }
    }    
}