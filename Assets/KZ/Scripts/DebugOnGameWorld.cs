using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class DebugOnGameWorld : MonoBehaviour {

    [SerializeField] Text _txtLog;
    [SerializeField] bool _ShowStackTrace;
    [SerializeField] int maxMessages = 50;

    Dictionary<LogType, string> _logColors = new Dictionary<LogType, string>() {
        {LogType.Exception,"red" },
        {LogType.Error,    "red" },
        {LogType.Warning,  "yellow" },
        {LogType.Assert,   "yellow" },
        {LogType.Log,      "silver" }
    };

    void Awake() {
        _txtLog.supportRichText = true;
        Application.logMessageReceived += PrintLog;
    }

    void PrintLog(string condition, string stackTrace, LogType type) {
        //combine all lines
        var l = _txtLog.text.Split('\n');

        //limit amount of lines
        var n = l.Count() - (maxMessages - 1);
        if (n > 0) _txtLog.text = l.Skip(n).Aggregate("", (x, a) => x + "\n" + a);

        //add new line
        _txtLog.text +=
            "\n" + "<color=" + _logColors[type] + ">" + type + "</color>: "
            + condition.Replace("\n", "\n  ")
            + (_ShowStackTrace ?
                stackTrace.Replace("\n", "\n ") + "."
                : "."
              );
    }

    private void OnDestroy() {
        Application.logMessageReceived -= PrintLog;
    }
}