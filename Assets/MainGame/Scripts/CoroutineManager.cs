
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CoroutineManager : MonoBehaviour {

    public static CoroutineManager Instance;

    [SerializeField] List<SubroutineSet> subroutineSets;

    private void Awake() {
        Instance = this;
        subroutineSets = new List<SubroutineSet>(5);
    }

    public void ManageStartCoroutine(IEnumerator main, IEnumerator other, Action endAction, string id) {

        if (CheckIds(id) != -1) return;

        int availableInt;

        availableInt = FindAvailableCoroutine();
        if (availableInt == -1) {
            subroutineSets.Add(new SubroutineSet());
            availableInt = subroutineSets.Count - 1;
        }

        ManageStopCoroutine(availableInt);
        subroutineSets[availableInt].runCoroutines = RunCoroutine(availableInt);
        subroutineSets[availableInt].main = main;
        subroutineSets[availableInt].other = other;
        subroutineSets[availableInt].endTask = endAction;
        subroutineSets[availableInt].id = id;        
        StartCoroutine(subroutineSets[availableInt].runCoroutines);
    }
    public void ForceStopCoroutine(string id) => ManageStopCoroutine(CheckIds(id));

    public IEnumerator RunCoroutine(int availableInt) {
        StartCoroutine(subroutineSets[availableInt].other);
        yield return subroutineSets[availableInt].main;
        ManageStopCoroutine(availableInt);
    }

    int FindAvailableCoroutine() {
        for (int i = 0; i < subroutineSets.Count; i++)
            if (CheckAvailability(i))
                { return i; }
        return -1;
    }

    bool CheckAvailability(int subRoutineInt) {
        if (subroutineSets[subRoutineInt].main == null && subroutineSets[subRoutineInt].other == null)
            return true;
        return false;
    }

    int CheckIds(string id) {
        if (id == "" || id == null) return -1;
        for (int i = 0; i < subroutineSets.Count; i++)
            if (subroutineSets[i].id==id)
                return i;
        return -1;
    }
    void ManageStopCoroutine(int n) {
        if (n == -1) return;
        SubroutineSet ss = subroutineSets[n];
        if (ss.runCoroutines != null) StopCoroutine(ss.runCoroutines);
        ss.runCoroutines = null;
        ss.runCoroutines = null;
        ss.main = null;
        if (ss.other != null) StopCoroutine(ss.other);
        ss.other = null;
        if (ss.endTask != null) ss.endTask.Invoke();
        ss.endTask = null;
        ss.id = null;
    }
}

public class SubroutineSet {
    public IEnumerator runCoroutines;
    public IEnumerator main;
    public IEnumerator other;
    public Action endTask;
    public string id;
}