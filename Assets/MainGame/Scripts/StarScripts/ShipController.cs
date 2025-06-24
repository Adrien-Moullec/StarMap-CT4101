using BezierSolution;
using System.Collections.Generic;
using UnityEngine;
using StarMaps;
using System.Linq;
public class ShipController : MonoBehaviour
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
        starShip.onPathCompleted.AddListener(delegate { /*PathFinder.instance.EndPathfinding();*/ });
    }

    //Sets points for bezier curve & starts ship flight
    public void ShipSetup() {
        if (tempSpline != null) Destroy(tempSpline);

        tempSpline = Instantiate(bezierSpline);
        tempSpline.transform.GetChild(0).transform.position = StarGeneration.instance.finalStarPath[0].transform.position;
        tempSpline.transform.GetChild(1).transform.position = StarGeneration.instance.finalStarPath[1].transform.position;

        for (int i = 2 ; i < StarGeneration.instance.finalStarPath.Count; i++) {
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
