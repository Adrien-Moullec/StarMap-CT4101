using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StarList {

    //Individual & duo star information
    public GameObject[] star; //Star Object itself

    //Information related to the cost of each path and the ship
     //2 stars form key for path cost
    public float[] gravitationCost;

}