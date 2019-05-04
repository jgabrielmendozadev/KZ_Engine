using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: make static class
public class BezierPath : MonoBehaviour {

    

    [SerializeField] Transform[] bezierPoints;
    public float minDifference = 0.01f;
    [Range(0, 1)] public float p;

    
    
    public Vector3 GetDirection(float value) {
        return (value > (1 - minDifference)) ?
            (GetBezier(1) - GetBezier(1 - minDifference)).normalized :
            (GetBezier(value + minDifference) - GetBezier(value)).normalized;
    }

    public static Vector3 GetBezier(Vector3[] points, float t) {
        return (points.Length == 1) ?
            points.First() :
            (points.Length == 2) ?
                lerp(points[0], points[1], t) :
                GetBezier(points
                    .ZipWith(points.Skip(1), (a, b) => new { a, b })
                    .Select(segment => lerp(segment.a, segment.b, t))
                    .ToArray()
                    , t
                );
    }

    /// <summary> uses internal array of points </summary>
    Vector3 GetBezier(float value) {
        var ss = new List<Segment>();
        for (int i = 0; i < bezierPoints.Length - 1; i++)
            ss.Add(new Segment(bezierPoints[i].position, bezierPoints[i + 1].position));

        return Bz(ss, value);
    }
    Vector3 Bz(List<Segment> ps,float t) {
        return ps.Count == 1 ?
            ps.First().GetLerp(t) :
            Bz(ps.ZipWith(ps.Skip(1), (a, b) => new Segment(a.GetLerp(t), b.GetLerp(t))).ToList(), t);
    }
    public static Vector3 lerp(Vector3 a, Vector3 b, float t) {
        return a + (b - a) * t;
    }

    public static float GetBezier(float[] points, float t) {
        return (points.Length==1) ?
            points.First():
            (points.Length == 2) ?
                Mathf.Lerp(points[0], points[1], t) :
                GetBezier(points
                    .ZipWith(points.Skip(1), (a, b) => new { a, b })
                    .Select(segment => Mathf.Lerp(segment.a, segment.b, t))
                    .ToArray()
                    , t
                );
    }

    public static long GetBezier(long[] points, float t) {
        return (points.Length == 1) ?
            points.First() :
            (points.Length == 2) ?
                LerpLong(points[0], points[1], t) :
                GetBezier(points
                    .ZipWith(points.Skip(1), (a, b) => new { a, b })
                    .Select(segment => LerpLong(segment.a, segment.b, t))
                    .ToArray()
                    , t
                );
    }

    static long LerpLong(long a, long b , float t) {
        long A = b > a ? a : b;
        long B = b > a ? b : a;

        return (long)((B - A) * t) + A;
    }


#if UNITY_EDITOR
    [Header("debug")]
    public int divisions = 2;
    public float ballRadius = 0.5f;
    void OnDrawGizmos() {
        if (bezierPoints == null ||
            bezierPoints.Length <= 1 ||
            bezierPoints.Any(x => x == null)) return;
        var l = new List<Vector3>();
        if (divisions <= 0)
            divisions = 1;
        float div = 1f / divisions;
        for (float i = 0; i < 1; i += div)
            l.Add(GetBezier(i));
        l.Add(GetBezier(1));
        for (int i = l.Count - 2; i >= 0; i--) {
            Gizmos.DrawLine(l[i], l[i + 1]);
            Gizmos.color = Color.Lerp(Color.red, Color.green, (float)i / l.Count);
            Gizmos.DrawSphere(l[i], ballRadius);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(l[l.Count - 1], ballRadius);
    }
#endif
}






class Segment {
    public Vector3 a, b;
    public Segment(Vector3 a, Vector3 b) { this.a = a; this.b = b; }
    public Vector3 GetLerp(float value) {
        return BezierPath.lerp(a, b, value);
    }
}