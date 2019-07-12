using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine.Networking;

namespace KZ {
    public static class FilesManager {

        public const string folderName = "GameFiles";


        #region PATHS
#if UNITY_ANDROID && !UNITY_EDITOR
        public static string path = "mnt/sdcard/KZ_Games/" + Application.productName + "_" + folderName + "/";
        public const char SEPARATOR = '/';
#else
        public static string path = Path.GetFullPath(Application.dataPath + "/../" + folderName + "/");
        public const char SEPARATOR = '\\';
#endif

        /// <returns>path of pathfile. Example: "C:\files\pepe.jpg" -> returns -> "C:\files\" )</returns>
        public static string GetPath(string pathFile) {
            string[] ps = pathFile.Split(SEPARATOR);
            return (ps.Length > 1) ?
                ps.Take(ps.Length - 1).MakeString(SEPARATOR) + SEPARATOR :
                "";
        }
        #endregion


        #region TEXT FILES
        static readonly System.Text.Encoding TEXT_ENCODING = System.Text.Encoding.UTF8;

        /// <param name="filename">"file.txt" or "SubFolder/file.txt"</param>
        public static List<string> LoadTextFile(string fileName) {
            return File.Exists(path + fileName)
                ? File.ReadAllLines(path + fileName).ToList()
                : new List<string>();
        }

        public static void SaveTextFile(string fileName, IEnumerable<string> content) {
            string file = Path.GetFullPath(path + fileName);
            Directory.CreateDirectory(GetPath(file));
            File.WriteAllLines(file, content.ToArray(), TEXT_ENCODING);
        }

        public static void AddLineToTextFile(string fileName, string newLine) {
            SaveTextFile(fileName, LoadTextFile(fileName).AddReturn(newLine));
        }
        #endregion


        #region IMAGES
        ///<summary>PNG/JPG files</summary>
        public static Texture2D LoadImage(string fileName) {
            var f = Path.GetFullPath(path + fileName);
            if (File.Exists(f)) {
                var img = new Texture2D(1, 1) { name = fileName };
                img.LoadImage(File.ReadAllBytes(path + fileName));
                return img;
            }
            Debug.Log("Image not found \"" + f + "\"");
            return null;
        }

        ///<summary>Save and onverrides "filename"</summary>
        /// <param name="filename"> pic.jpg, pic.png </param>
        /// <param name="img"></param>
        /// <param name="encoding"></param>
        public static void SaveImage(string filename, Texture2D image, ImageEncoding encoding = ImageEncoding.JPG) {
            string file = Path.GetFullPath(path + SEPARATOR + filename);
            Directory.CreateDirectory(GetPath(file));
            File.WriteAllBytes(file, encodingF[encoding](image));
        }

        public enum ImageEncoding { EXR, JPG, PNG }
        static readonly Dictionary<ImageEncoding, Func<Texture2D, byte[]>> encodingF =
            new Dictionary<ImageEncoding, Func<Texture2D, byte[]>>()
            .AddReturn(ImageEncoding.EXR, GetEXR)
            .AddReturn(ImageEncoding.JPG, GetJPG)
            .AddReturn(ImageEncoding.PNG, GetPNG);

        static byte[] GetEXR(Texture2D image) { return image.EncodeToEXR(); }
        static byte[] GetJPG(Texture2D image) { return image.EncodeToJPG(100); }
        static byte[] GetPNG(Texture2D image) { return image.EncodeToPNG(); }
        #endregion


        #region AUDIO
        public static void LoadAudio(string fileName, AudioType at, Action<AudioClip> OnLoad) {
            var fullPath = Path.GetFullPath(path + SEPARATOR + fileName);
            if (!File.Exists(fullPath)) {
                Debug.LogWarning("Audio not found: \"" + fullPath + "\"");
                return;
            }
            Utility.GetGo().StartCoroutine(LoadAudioCR(fileName, at, OnLoad));
        }
        /// <summary>Remember to use clip.UnloadAudioData() and Destoy(clip)</summary>
        static IEnumerator LoadAudioCR(string fileName, AudioType at, Action<AudioClip> OnLoad) {
            var fullPath = Path.GetFullPath(path + SEPARATOR + fileName);
            var url = "file://" + fullPath;
            using (UnityWebRequest web = UnityWebRequestMultimedia.GetAudioClip(url, at)) {
                yield return web.SendWebRequest();
                if (!web.isNetworkError && !web.isHttpError) {
                    var clip = DownloadHandlerAudioClip.GetContent(web);
                    if (clip) {
                        OnLoad(clip);
                        Debug.Log("AudioClip loaded: " + url);
                    }
                    else
                        throw new Exception("audio can't be loaded: " + url);
                }
                else
                    throw new Exception("Error loading audio:" + (web.isNetworkError ? " NetworkError" : "") + (web.isHttpError ? " HttpError" : ""));
            }
        }
        #endregion


        #region CSV
        const char CSV_SEP = ';';
        const string BOOL_TRUE = "SI", BOOL_FALSE = "NO";

        static void DebugUndefinedParserMsg(Type t) {
            Debug.LogWarning("parser not defined for \"" + t + "\", using default(" + t + ")");
        }
        public static List<T> LoadCSV<T>(string fileName) where T : new() {


            string file = Path.GetFullPath(path + fileName);
            Directory.CreateDirectory(GetPath(file));

            if (!File.Exists(file)) return new List<T>();

            var r = new List<T>();
            #region VARS
            string[]
                    _currLine,
                    content = File.ReadAllLines(file),
                    fields = content.First().Split(CSV_SEP);
            Dictionary<string, int> dict = Enumerable.Range(0, int.MaxValue)
                .ZipWith(fields, (i, name) => new { name, i })
                .Aggregate(new Dictionary<string, int>(), (d, x) => d.AddReturn(x.name, x.i));
            FieldInfo
                fi;
            Func<string, object>
                parse;
            #endregion
            foreach (var line in content.Skip(1)) {
                object o = new T();
                _currLine = line.Split(CSV_SEP);
                foreach (var f in fields) {
                    fi = typeof(T).GetField(f);
                    if (fi != null) {
                        if (fi.FieldType.IsEnum) {
                            fi.SetValue(o, GetEnumValue(fi.FieldType, _currLine[dict[f]]));
                        }
                        else {
                            if (converters_fromString.TryGetValue(fi.FieldType, out parse))
                                fi.SetValue(o, parse(_currLine[dict[f]]));
                            else
                                DebugUndefinedParserMsg(fi.FieldType);
                        }
                    }
                }
                r.Add((T)o);
            }


            return r;
        }
        public static void SaveCSV<T>(string fileName, IEnumerable<T> content) {
            string file = Path.GetFullPath(path + fileName);
            Directory.CreateDirectory(GetPath(file));

            string line1 = typeof(T).GetFields().MakeString(x => x.Name, CSV_SEP);
            string[] fields = line1.Split(CSV_SEP);
            Func<object, string> f2;

            string[] l = new string[1] { line1 }
                .Concat(content
                    .Select(e => fields
                        .Select(f => e.GetType().GetField(f))
                        .Select(fi =>
                            fi.FieldType.IsEnum ?
                                fi.GetValue(e).ToString() :
                                (converters_String.TryGetValue(fi.FieldType, out f2) ?
                                    f2(fi.GetValue(e)) :
                                    "NULL VALUE")
                        )
                    .MakeString(CSV_SEP))
                ).ToArray();

            File.WriteAllLines(file, l, TEXT_ENCODING);
        }
        public static void AddElementToCsv<T>(string fileName, T element) where T : new() {
            SaveCSV(fileName, LoadCSV<T>(fileName).AddReturn(element));
        }

        static object GetEnumValue(Type t, string valueName) {
            bool f = true;
            object r = null;
            foreach (var value in Enum.GetValues(t)) {
                if (value.ToString() == valueName)
                    return value;
                if (f) {
                    r = value;
                    f = false;
                }
            }
            return r;
        }

        #region ConvertToString
        static string ConvertToString(int v) {
            return v.ToString();
        }
        static string ConvertToString(bool v) {
            return v ? BOOL_TRUE : BOOL_FALSE;
        }
        static string ConvertToString(float v) {
            return v.ToString("0.0");
        }
        static string ConvertToString(string v) {
            return v;
        }
        
        static readonly Dictionary<Type, Func<object, string>> converters_String =
            new Dictionary<Type, Func<object, string>>()
                .AddReturn(typeof(int), o => ConvertToString((int)o))
                .AddReturn(typeof(bool), o => ConvertToString((bool)o))
                .AddReturn(typeof(float), o => ConvertToString((float)o))
                .AddReturn(typeof(string), o => ConvertToString((string)o));
        #endregion

        
        #region ConvertFromString
        static int ConvertFromString_int(string v) {
            int r;
            return int.TryParse(v, out r) ? r : 0;
        }
        static bool ConvertFromString_bool(string v) {
            return v.ToUpper() == BOOL_TRUE;
        }
        static float ConvertFromString_float(string v) {
            float r;
            return float.TryParse(v, out r) ? r : 0f;
        }
        static string ConvertFromString_string(string v) {
            return v;
        }
        
        static readonly Dictionary<Type, Func<string, object>> converters_fromString =
            new Dictionary<Type, Func<string, object>>()
            .AddReturn(typeof(int), s => ConvertFromString_int(s))
            .AddReturn(typeof(bool), s => ConvertFromString_bool(s))
            .AddReturn(typeof(float), s => ConvertFromString_float(s))
            .AddReturn(typeof(string), s => ConvertFromString_string(s));
        #endregion

        #endregion


        #region XML
        public static class Xml {

            static Dictionary<string, XmlFile> _files = new Dictionary<string, XmlFile>();


            public static void CreateFileReader<T>(string fileName, Func<Dictionary<string, string>, T> toT, Func<T, List<XmlLine>> toElem, string tagName = "ELEMENT") {
                if (_files.ContainsKey(fileName))
                    throw new Exception("File already created");
                string path = FilesManager.path;
                _files[fileName] = new XmlFile<T>(path, fileName, toT, toElem, tagName);
                Debug.Log("file loaded: " + _files[fileName]._filePath);
            }

            public static List<T> ReadFile<T>(string fileName) {
                if (!_files.ContainsKey(fileName)) throw new Exception("File not loaded");
                return ((XmlFile<T>)_files[fileName]).Load();
            }

            public static void SaveFile<T>(string fileName, IEnumerable<T> list) {
                if (!_files.ContainsKey(fileName)) throw new Exception("File not loaded");
                ((XmlFile<T>)_files[fileName]).Save(list);
            }

            public static List<string> FilesLoaded() { return _files.Keys.ToList(); }


            #region EXAMPLE
            static void TestFunction() {
                //FILE 00 - string
                Func<Dictionary<string, string>, string> toStr =
                    (x) => x["NAME"];
                Func<string, List<XmlLine>> toElem =
                    (s) => new List<XmlLine>().AddReturn(new XmlLine("NAME", s));
                Xml.CreateFileReader("TestFile00.xml", toStr, toElem, "SOMEONE");
                Xml.SaveFile("TestFile00.xml", new List<string>() { "PEPE ARGENTO", "MONI ARGENTO" });

                //FILE 01 - PlayerData
                Func<Dictionary<string, string>, PlayerData> toPlayerData =
                    (x) => new PlayerData(
                        x["NAME"],
                        int.Parse(x["AGE"])
                    );
                Func<PlayerData, List<XmlLine>> toElem2 =
                    (pd) => new List<XmlLine>()
                        .AddReturn(new XmlLine("NAME", pd.name))
                        .AddReturn(new XmlLine("AGE", pd.age.ToString()));
                Xml.CreateFileReader("TestFile01.xml", toPlayerData, toElem2, "SOMEONE");
                Xml.SaveFile("TestFile01.xml", new List<PlayerData>()
                    .AddReturn(new PlayerData("PEPE ARGENTO", 40))
                    .AddReturn(new PlayerData("MONI ARGENTO", 38))
                );
            }
            class PlayerData {
                public string name;
                public int age;
                public PlayerData(string name, int age) {
                    this.name = name; this.age = age;
                }
            }
            #endregion


        }

        class XmlFile {
            public string _filePath { get; protected set; }
            protected string _tagName = "ELEMENT";
        }

        class XmlFile<T> : XmlFile {
            Func<Dictionary<string, string>, T> _toT;
            Func<T, List<XmlLine>> _toElem;
            #region CONSTRUCTOR
            public XmlFile(string path, string fileName, Func<Dictionary<string, string>, T> toT, Func<T, List<XmlLine>> toElem, string tagName) {
                _filePath = Path.GetFullPath(path + SEPARATOR + fileName);

                Directory.CreateDirectory(GetPath(_filePath));

                if (!File.Exists(_filePath)) CreateFile();

                _toT = toT;
                _toElem = toElem;
                _tagName = tagName;
            }
            #endregion

            void CreateFile() { Save(new List<T>()); }

            public List<T> Load() {
                XmlReaderSettings _xmlSettings = new XmlReaderSettings() {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };

                var l = new List<Dictionary<string, string>>();

                using (XmlReader r = XmlReader.Create(_filePath, _xmlSettings)) {

                    var d = new Dictionary<string, string>();
                    var elem = "";

                    while (r.Read()) {
                        switch (r.NodeType) {
                            case XmlNodeType.Element: //<_tagName> or <name>
                                if (r.Name == _tagName)
                                    d = new Dictionary<string, string>();
                                else
                                    d[elem = r.Name] = "";//initial value needed for empty values(no text node)
                                break;
                            case XmlNodeType.Text: //"pepe argento"
                                d[elem] = r.Value;
                                break;
                            case XmlNodeType.EndElement: //</_tagName> or </name>
                                if (r.Name == _tagName) l.Add(d);
                                break;
                        }
                    };
                }
                return l.Select(x => _toT(x))
                    .ToList();
            }
            public void Save(IEnumerable<T> list) {
                XmlWriterSettings _xmlSettings = new XmlWriterSettings() {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter w = XmlWriter.Create(_filePath, _xmlSettings)) {
                    w.WriteStartDocument();
                    w.WriteStartElement("ROOT");
                    foreach (var x in list.Select(l => _toElem(l))) {
                        w.WriteStartElement(_tagName);
                        foreach (var elem in x)
                            w.WriteElementString(elem.localName, elem.value);
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                    w.WriteEndDocument();
                }
            }
        }

        public class XmlLine {
            public string localName;
            public string value;
            public XmlLine(string localName, string value) {
                this.localName = localName;
                this.value = value;
            }
        }

        #endregion

    }
}