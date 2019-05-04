using System;
using System.Collections;
using UnityEngine;

public static class Utility {

    public static T Log<T>(T s) {
        Debug.Log(s);
        return s;
    }

    static Nothing _n;
    static Nothing n {
        get {
            return (_n == null) ?
                _n = (new GameObject() { name = "Util" }).AddComponent<Nothing>() : _n;
        }
        set { _n = value; }
    }

    //ExecuteInSeconds
    public static void ExecuteInSeconds(float time, Action operation) {
        n.StartCoroutine(ExecuteCR(time, operation));
    }
    static IEnumerator ExecuteCR(float time, Action operation) {
        yield return new WaitForSeconds(time);
        operation();
    }

    //ExecuteWhen
    public static void ExecuteWhen(Func<bool> condition, Action operation) {
        n.StartCoroutine(ExecuteWhenCR(condition, operation));
    }
    static IEnumerator ExecuteWhenCR(Func<bool> condition, Action operation) {
        while (!condition()) yield return new WaitForEndOfFrame();
        operation();
    }

    //ExecuteInFrames
    public static void ExecuteInFrames(int frames, Action operation) {
        n.StartCoroutine(ExecuteCRFrames(frames, operation));
    }
    static IEnumerator ExecuteCRFrames(int time, Action operation) {
        for (int i = time; i > 0; i--)
            yield return new WaitForEndOfFrame();
        operation();
    }

    public static Nothing GetGo() { return n; }
}
public class Nothing : MonoBehaviour { }