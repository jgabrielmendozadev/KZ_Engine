using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultKZValues : MonoBehaviour {


    public bool allow_dev_console = false;
    public bool skip_intro = false;


    public static bool allowDevConsole { get; private set; }
    public static bool skipIntro { get; private set; }

    private void Awake() {
        allowDevConsole = allow_dev_console;
        skipIntro = skip_intro;
    }


}