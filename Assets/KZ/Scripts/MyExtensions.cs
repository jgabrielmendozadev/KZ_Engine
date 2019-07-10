using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Xml;
using V3 = UnityEngine.Vector3;
using V2 = UnityEngine.Vector2;
using UnityEngine.UI;
using System.Globalization;

namespace KZ {
    public static class MyExtensions {

        const string msgIndexOf = "Element not found";

        //COLLECTIONS
        public static bool Find<T>(this IEnumerable<T> xs, Predicate<T> f, ref T element) {
            foreach (var x in xs)
                if (f(x)) {
                    element = x;
                    return true;
                }
            return false;
        }
        public static int IndexOf<T>(this IEnumerable<T> xs, Predicate<T> f) {
            int i = zeroint;
            foreach (var x in xs)
                if (f(x)) return i;
                else i++;
            return -1;
        }
        public static IEnumerable<T> ConcatSelf<T>(this IEnumerable<T> l, int times) {
            var r = l;
            for (int i = times; i > zeroint; i--)
                r = r.ConcatSelf();
            return r;
        }
        public static List<T> AddReturn<T>(this List<T> l, T elem) {
            l.Add(elem);
            return l;
        }
        public static HashSet<T> AddReturn<T>(this HashSet<T> h, T elem) {
            h.Add(elem);
            return h;
        }
        public static Dictionary<TK, TV> AddReturn<TK, TV>(this Dictionary<TK, TV> d, TK k, TV v) {
            d[k] = v;
            return d;
        }

        public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> l) {
            return l.OrderBy(x => Random.Range(0f, 100f));
        }
        public static T RandomElement<T>(this T[] l) {
            return l.Any() ?
                l[Random.Range(0, l.Length)] :
                default(T);
        }
        /// <summary>Low performance</summary>
        public static T RandomElement<T>(this IEnumerable<T> l) {
            return l.ToArray().RandomElement();
        }

        public static IEnumerable<T> ConcatSelf<T>(this IEnumerable<T> l) {
            return l.Concat(l);
        }

        public static T MaxElement<T>(this IEnumerable<T> l, Func<T, float> valueOf) {
            if (l == null || !l.Any()) return default(T);
            return l.Skip(1).Aggregate(l.First(), (r, e) => valueOf(e) > valueOf(r) ? e : r);
        }
        public static T MinElement<T>(this IEnumerable<T> l, Func<T, float> valueOf) {
            if (l == null || !l.Any()) return default(T);
            return l.Skip(1).Aggregate(l.First(), (r, e) => valueOf(e) < valueOf(r) ? e : r);
        }

        public static T TakeEnqueue<T>(this Queue<T> q) {
            T r = q.Dequeue();
            q.Enqueue(r);
            return r;
        }
        public static Queue<T> EnqueueReturn<T>(this Queue<T> q, T element) {
            q.Enqueue(element);
            return q;
        }

        public static void ToAll<T>(this IEnumerable<T> l, Action<T> operation) {
            foreach (var x in l) operation(x);
        }

        public static string MakeString(this IEnumerable<string> l, string separator = ";") {
            return l.Any() ?
                l.Skip(1).Aggregate(l.First(), (r, x) => r + separator + x) : "";
        }
        public static string MakeString(this IEnumerable<string> l, char separator = ';') {
            return l.Any() ?
                l.Skip(1).Aggregate(l.First(), (r, x) => r + separator + x) : "";
        }
        public static string MakeString<T>(this IEnumerable<T> l, Func<T, string> f, string separator=";") {
            return l.Any() ?
                l.Skip(1).Aggregate(f(l.First()), (r, x) => r + separator + f(x)) : "";
        }
        public static string MakeString<T>(this IEnumerable<T> l, Func<T, string> f, char separator=';') {
            return l.Any() ?
                l.Skip(1).Aggregate(f(l.First()), (r, x) => r + separator + f(x)) : "";
        }

        public static V TryGetValueDefault<K, V>(this Dictionary<K, V> d, K key, V defaultValue) {
            V aux;
            return d.TryGetValue(key, out aux) ? aux : defaultValue;
        }

        //STRING

        public static string Fill(this string s, int amount, string fill = " ") {
            string r = s;
            for (int i = Mathf.Max(amount - s.Length, zeroint); i > zeroint; i--) r = r + fill;
            return r;
        }

        public static string Colorize(this string s, Color c) {
            return "<color=#"
                + ((byte)(c.r * 255)).ToString("X2")
                + ((byte)(c.g * 255)).ToString("X2")
                + ((byte)(c.b * 255)).ToString("X2")
                + ((byte)(c.a * 255)).ToString("X2") + ">"
                + s + "</color>";
        }
        public static string SetRichSize(this string s, int size) {
            return "<size=" + size.ToString("0") + ">" + s + "</size>";
        }

        //INT
        const int zeroint = 0;
        public static int Clamp(this int value, int min, int max) {
            return value < min ? min : (value > max ? max : value);
        }
        public static int Cycle(this int v,int min, int max) {

            return v <= max ?
                (v >= min ?
                    v :
                    (v + max - min+1).Cycle(min, max)
                ) :
                    (v - max + min-1).Cycle(min, max);
        }


        //FLOAT
        const float zerof = 0f, onef = 1f;

        public static float Abs(this float x) { return Mathf.Abs(x); }
        public static float Clamp(this float value, float min, float max) {
            return value < min ? min : (value > max ? max : value);
        }
        public static float Clamp(this float value) {
            return value < zerof ? zerof : (value > onef ? onef : value);
        }
        public static float Truncate(this float value, int digits) {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
        public static float Round(this float value, int digits) {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Round(mult * value) / mult;
            return (float)result;
        }
        public static float Remap(this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }


        public static float Cycle(this float v, float min, float max) {
            var d = max - min;
            var n = Mathf.Sign(v);
            var m = 1 - (Math.Abs(n) - n) / 2;
            return (d * (1 - m) + ((Mathf.Abs(v % d)) * Mathf.Sign(m - 0.5f))) + min;
        }
        
        public static float CycleRecursive(this float v, float min, float max) {
            return v <= max ?
                (v >= min ?
                    v :
                    (v + max - min).CycleRecursive(min, max)
                ) :
                    (v - max + min).CycleRecursive(min, max);
        }





        //USHORT
        public static ushort Clamp(this ushort value, ushort min, ushort max) {
            return value < min ? min : (value > max ? max : value);
        }

        //DATETIME
        /// <summary>Better for filenames</summary>
        public static string ToKzFormat(this DateTime d) {
            return
                "[" + d.Year.ToString("0000") + "-" + d.Month.ToString("00") + "-" + d.Day.ToString("00") + "]" +
                "[" + d.Hour.ToString("00") + "-" + d.Minute.ToString("00") + "-" + d.Second.ToString("00") + "]";
        }
        public static string ToKzFormat2(this DateTime d) {
            return
                "[" + d.Year.ToString("0000") + "/" + d.Month.ToString("00") + "/" + d.Day.ToString("00") + "]" +
                "[" + d.Hour.ToString("00") + ":" + d.Minute.ToString("00") + ":" + d.Second.ToString("00") + "]";
        }
        public static string ToKzFormatNoSec(this DateTime d) {
            return
                "[" + d.Year.ToString("0000") + "/" + d.Month.ToString("00") + "/" + d.Day.ToString("00") + "]" +
                "[" + d.Hour.ToString("00") + ":" + d.Minute.ToString("00") + "]";
        }
        public static DateTime LerpWith(this DateTime dt0, DateTime target, float time) {
            long ticksA = target > dt0 ? dt0.Ticks : target.Ticks;
            long ticksB = target > dt0 ? target.Ticks : dt0.Ticks;
            return new DateTime((long)((ticksB - ticksA) * time) + ticksA);
        }
        public static bool IsContainedIn(this DateTime d0, DateTime from, DateTime to) {
            long ticksA = to > from ? from.Ticks : to.Ticks;
            long ticksB = to > from ? to.Ticks : from.Ticks;
            long ticks0 = d0.Ticks;
            return ticks0 >= ticksA && ticks0 <= ticksB;
        }
        public static TimeSpan Multiply(this TimeSpan ts, float value) {
            return new TimeSpan((long)(ts.Ticks * value));
        }

        //GAMEOBJECT
        public static void ToggleActive(this GameObject go) { go.SetActive(!go.activeSelf); }
        public static float PosX(this MonoBehaviour m) { return m.transform.position.x; }
        public static float PosY(this MonoBehaviour m) { return m.transform.position.y; }
        public static float PosZ(this MonoBehaviour m) { return m.transform.position.z; }

        //TRANSFORM
        public static void CopyTransform(this Transform t, Transform other) {
            t.SetPositionAndRotation(other.position, other.rotation);
        }

        //VECTOR 2
        public static V2 SetX(this V2 v, float newX) { v.x = newX; return v; }
        public static V2 SetY(this V2 v, float newY) { v.y = newY; return v; }

        //VECTOR 3
        public static V3 SetX(this V3 v, float newX) { v.x = newX; return v; }
        public static V3 SetY(this V3 v, float newY) { v.y = newY; return v; }
        public static V3 SetZ(this V3 v, float newZ) { v.z = newZ; return v; }
        public static float DistanceTo(this Transform from, Transform to) {
            return V3.Distance(from.position, to.position);
        }

        //COLOR
        public static Color GetComplementary(this Color c) {
            return new Color(onef - c.r, onef - c.g, onef - c.b, c.a);
        }
        /// <summary> check similarity between colors. Returns: equal colors -> 1, complementary colors -> 0</summary>
        public static float CompareWith(this Color c, Color other) {
            var a = (c - other);
            return 1 - (Mathf.Abs(a.r) + Mathf.Abs(a.g) + Mathf.Abs(a.b)) / 3;
        }
        
        public static Color SetR(this Color c, float r) { c.r = r; return c; }
        public static Color SetG(this Color c, float g) { c.g = g; return c; }
        public static Color SetB(this Color c, float b) { c.b = b; return c; }
        public static Color SetA(this Color c, float a) { c.a = a; return c; }

        public static bool HexToColor(this string hexValue, out Color color) {
            color = Color.clear;
            var h = hexValue;
            if (h.Length == 6) h += "ff";
            if (h.Length != 8) {
                Debug.Log("Cant convert from HexColor: " + hexValue);
                return false;
            }

            var r_str = h.Remove(2);
            var g_str = h.Remove(0,2).Remove(2);
            var b_str = h.Remove(0,4).Remove(2);
            var a_str = h.Remove(0,6);

            int auxR = 0, auxG = 0, auxB = 0, auxA = 0;

            bool correct =
                int.TryParse(r_str, NumberStyles.AllowHexSpecifier, null, out auxR) &&
                int.TryParse(g_str, NumberStyles.AllowHexSpecifier, null, out auxG) &&
                int.TryParse(b_str, NumberStyles.AllowHexSpecifier, null, out auxB) &&
                int.TryParse(a_str, NumberStyles.AllowHexSpecifier, null, out auxA);
            if (!correct)
                Debug.LogError("Error trying to convert Hex to Color: " + hexValue);
            color.r = auxR / 255f;
            color.g = auxG / 255f;
            color.b = auxB / 255f;
            color.a = auxA / 255f;
            return correct;
        }

        //RenderTexture
        public static Texture2D ToTexture2D(this RenderTexture rt, TextureFormat format = TextureFormat.RGB24) {
            var aux = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D t2d = new Texture2D(rt.width, rt.height, format, false);
            t2d.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = aux;
            return t2d;
        }
        
        //Dropdown
        public static Dropdown.OptionData GetCurrentOption(this Dropdown dd) {
            return dd.options[dd.value];
        }

        //XML
        [Obsolete("not used anymore")]
        public static string ReadValue(this XmlElement elem, string valueName) {
            return elem.GetElementsByTagName(valueName)[0].InnerText;
        }

        public static List<FilesManager.XmlLine> AddElement(this List<FilesManager.XmlLine> l, string localName, string value) {
            return l.AddReturn(new FilesManager.XmlLine(localName, value));
        }
    }
}

