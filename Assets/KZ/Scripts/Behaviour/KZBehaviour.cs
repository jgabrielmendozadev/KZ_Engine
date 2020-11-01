using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KZ.Managers;

public abstract class KZBehaviour : MonoBehaviour {



    public KZBehaviour() {
        Locator.behaviourManager.OnFixedUpdate += KZFixedUpdate;
        Locator.behaviourManager.OnUpdate += KZUpdate;
        Locator.behaviourManager.OnLateUpdate += KZLateUpdate;
    }
    ~KZBehaviour() {
        Locator.behaviourManager.OnFixedUpdate -= KZFixedUpdate;
        Locator.behaviourManager.OnUpdate -= KZUpdate;
        Locator.behaviourManager.OnLateUpdate -= KZLateUpdate;
    }



    protected abstract void KZFixedUpdate();
    protected abstract void KZUpdate();
    protected abstract void KZLateUpdate();

}


public class Kzkzkz : KZBehaviour {

    protected override void KZFixedUpdate() { }
    protected override void KZLateUpdate() { }
    protected override void KZUpdate() { }

}