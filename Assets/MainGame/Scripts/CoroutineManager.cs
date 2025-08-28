
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class CoroutineManager : MonoBehaviour {

    public static CoroutineManager Instance;

    IEnumerator currentCoroutine;

    public bool isRunning = false;
    public bool isFinished = false;

    private void Awake() {
        Instance = this;
    }

    public IEnumerator RunCoroutine(IEnumerator Coroutine) {
        currentCoroutine = Coroutine;

        StartCoroutine(currentCoroutine);

        yield break;
    }
    
    public void ActivateFinish() {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
    }
}