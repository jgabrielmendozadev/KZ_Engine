using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using KZ;

public static class KZ_Settings {

    public static void Initialize() {
        _KZSettings = FilesManager.LoadKZSettings();
    }
    
    static Dictionary<string, string> _KZSettings = new Dictionary<string, string>();

    public static T GetValue<T>(string k, T defaultValue) {
        string value;
        if (_KZSettings.TryGetValue(k, out value)) {
            
            var t = typeof(T);

            Func<string, object, object> f;

            if (_parsers.TryGetValue(t, out f))
                return (T)f(value, defaultValue);
            else {
                Debug.LogError("Type not implemented: " + t + ", key: " + k);
                return defaultValue;
            }
        }
        
        return defaultValue;
    }


    static readonly Dictionary<Type, Func<string, object, object>> _parsers =
        new Dictionary<Type, Func<string, object, object>>()
        .AddReturn(typeof(int), (s, d) => { int x; return int.TryParse(s, out x) ? x : LogWarning_Parser(s, d); })
        .AddReturn(typeof(bool), (s, d) => { bool x; return bool.TryParse(s, out x) ? x : LogWarning_Parser(s, d); })
        .AddReturn(typeof(float), (s, d) => { float x; return float.TryParse(s, out x) ? x : LogWarning_Parser(s, d); })
        .AddReturn(typeof(string), (s, d) => s);

    //utility to simplify lambdas in "_parsers"
    static T LogWarning_Parser<T>(string value, T defaultValue) {
        Debug.LogWarning("Cant parse \"" + value + "\" to \"" + typeof(T) + "\", using default value: \"" + defaultValue + "\"");
        return defaultValue;
    }

}