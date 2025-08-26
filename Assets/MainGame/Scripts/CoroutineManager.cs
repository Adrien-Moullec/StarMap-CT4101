
using UnityEngine;
using System.Collections;

[System.Serializable]
public class CoroutineManager : MonoBehaviour {

    public static CoroutineManager Instance;

    IEnumerator currentCoroutine;

    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isFinished = false;

    private void Awake() {
        Instance = this;
    }

    public IEnumerator RunCoroutine(IEnumerator coroutine) {
        print("RunCoroutine");
        currentCoroutine = coroutine;
        print(currentCoroutine.ToString());

        isRunning = true;
        isFinished = false;

        yield return currentCoroutine;
        print("End of coroutine in manager");
        isRunning = false;
        isFinished = true;
    }
    
    public void ActivateFinish() {
        print("ActivateFinish coroutine in manager");
        if (currentCoroutine != null) isFinished = true;
        ResetCoroutineManager();
    }

    void ResetCoroutineManager() {
        isRunning = false;
        isFinished = false;
    }
}