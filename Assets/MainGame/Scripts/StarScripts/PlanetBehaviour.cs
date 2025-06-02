using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetBehaviour : MonoBehaviour {

    [Header("SpeedSettings")]
    [SerializeField] float minSpeed = 50;
    [SerializeField] float maxSpeed = 140;


    Vector3 rotationSpeed;

    private void Awake() {
        rotationSpeed = Random.Range(minSpeed, maxSpeed) * Time.deltaTime * new Vector3(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            );
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(rotationSpeed);
    }
}