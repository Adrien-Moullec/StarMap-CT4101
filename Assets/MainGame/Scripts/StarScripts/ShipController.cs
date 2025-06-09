using BezierSolution;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour, IInteract
{
    public static ShipController instance;

    [Space]
    [Header("Bezier Path")]
    [SerializeField] BezierSpline bezierSpline;
    [SerializeField] BezierPoint bezierPoint;
    [SerializeField] BezierWalkerWithSpeed starShip;

    BezierSpline tempSpline;
    BezierPoint tempSplinePoint;

    private void Awake() {
        instance = this;
        starShip.gameObject.SetActive(false);
        starShip.onPathCompleted.AddListener(ShipJourneyEnd);
        starShip.onPathCompleted.AddListener(delegate { PathFinder.instance.ENDTHING(); });
    }


    public void Interact() {

    }

    //Sets points for bezier curve & starts ship flight
    public void ShipSetup() {
        tempSpline = Instantiate(bezierSpline);
        tempSpline.transform.GetChild(0).transform.position = StarGeneration.instance.finalStarPath[StarGeneration.instance.finalStarPath.Count - 1].transform.position;
        tempSpline.transform.GetChild(1).transform.position = StarGeneration.instance.finalStarPath[StarGeneration.instance.finalStarPath.Count - 2].transform.position;

        for (int i = StarGeneration.instance.finalStarPath.Count - 3; i >= 0; i--) {
            tempSplinePoint = Instantiate(bezierPoint, tempSpline.transform);
            tempSplinePoint.transform.position = StarGeneration.instance.finalStarPath[i].transform.position;
        }

        starShip.gameObject.SetActive(true);
        starShip.NormalizedT = 0;
        starShip.transform.position = StarGeneration.instance.finalStarPath[StarGeneration.instance.finalStarPath.Count - 1].transform.position;
        starShip.spline = tempSpline;
    }

    //Activates when ship reaches destination
    public void ShipJourneyEnd() {
        Destroy(tempSpline); tempSpline = null;
        starShip.gameObject.SetActive(false);
    }
}
