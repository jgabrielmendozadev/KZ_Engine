using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;

public class LocalizationManager {

    static LocalizationManager _instance;
    
    public static LocalizationManager GetInstance() {
        if (_instance == null) _instance = new LocalizationManager();
        return _instance;
    }
    string _fileName = "localizationData.json";
    Dictionary<UserLanguage, Dictionary<string, string>> _texts;
    LocalizationManager() {

        //Get System Lang
        switch (Application.systemLanguage) {
            case SystemLanguage.English: _currentLanguage = UserLanguage.English; break;
            case SystemLanguage.Spanish: _currentLanguage = UserLanguage.Spanish; break;
            default: _currentLanguage = UserLanguage.English; break;
        }

        //Get file with localization
        string path = Path.GetFullPath(Application.dataPath + "/../GameFiles/");
        Directory.CreateDirectory(path);
        string filePath = Path.GetFullPath(path + "/" + _fileName);

        if (!File.Exists(filePath)) CreateInitialFile(filePath);
        
        var lh = JsonUtility.FromJson<LocalizationHelper>(File.ReadAllText(filePath));
        
        _texts = new Dictionary<UserLanguage, Dictionary<string, string>>();
        _texts = lh.GetTexts();
    }

    void CreateInitialFile(string filePath) {
        File.Create(filePath).Dispose();
        var f = new LocalizationHelper();

        var ts = new Dictionary<UserLanguage, Dictionary<string, string>>();
        foreach (var lang in GetUserLanguages())
            ts[lang] = new Dictionary<string, string>() { { "TITLE", "Recontra" }, };

        f.SetTexts(ts);
        Debug.Log(JsonUtility.ToJson(f, true));
        File.WriteAllText(filePath, JsonUtility.ToJson(f, true));
    }

    UserLanguage _currentLanguage;
    void AddText(UserLanguage lang, string title, string text) {
        foreach (var l in GetUserLanguages())
            if (!_texts[l].ContainsKey(title))
                _texts[l][title] = "text not translated yet";
        
        _texts[lang][title] = text;
    }
    public void ChangeLanguage (UserLanguage lang) {
        _currentLanguage = lang;
        var objs = UnityEngine.Object.FindObjectsOfType<LocalizableText>();
        for (int i = objs.Length - 1; i >= 0; i--) objs[i].SetText();
    }

    public string GetText(string title) {
        string r;
        return (_texts[_currentLanguage].TryGetValue(title, out r)) ? r : "MISSING TEXT";
    }
    public UserLanguage GetCurrentLanguage() {
        return _currentLanguage;
    }

    UserLanguage[] GetUserLanguages() {
        return new UserLanguage[] {
            UserLanguage.English,
            UserLanguage.Spanish
        };
    }
    
}

[Serializable]
public class LocalizationHelper {
    [SerializeField] List<LangTexts> texts;
    
    public Dictionary<UserLanguage, Dictionary<string, string>> GetTexts() {
        var r = new Dictionary<UserLanguage, Dictionary<string, string>>();
        foreach (var lts in texts) {
            r.Add(lts.lang, new Dictionary<string, string>());
            foreach (var KV in lts.titleTexts)
                r[lts.lang][KV.title] = KV.text;
        }
        return r;
    }

    public void SetTexts(Dictionary<UserLanguage, Dictionary<string, string>> dict) {
        texts = new List<LangTexts>();
        foreach (var item in dict) {
            var l = new List<StrStr>();
            foreach (var ss in item.Value)
                l.Add(new StrStr() { title = ss.Key, text = ss.Value });
            texts.Add(new LangTexts() { lang = item.Key, titleTexts = l });
        }
    }

    [Serializable]
    public class LangTexts { public UserLanguage lang; public List<StrStr> titleTexts; }
    [Serializable]
    public class StrStr { public string title, text; }
}

public enum UserLanguage { English, Spanish, }











