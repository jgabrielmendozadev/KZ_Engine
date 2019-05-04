using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using KZ.GameObjectMoveExtensions;

public class TestScript : MonoBehaviour {

    public GameObject goTest;
    public Transform target;
    public float time;
    public AnimationCurve curve;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            goTest.MoveTo(target.position, target.rotation, time);
    }

}