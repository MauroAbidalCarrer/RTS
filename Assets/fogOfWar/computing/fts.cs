﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public struct fts
{
    #region doIf
    public void map<T>(IList<T> list, Predicate<T> match, Action<int, IList<T>> action)
    {
        for (int i = 0; i < list.Count; i++)
            if (match(list[i])) action(i, list);
    }
    #endregion
    #region convert if
    public static List<T> mapif<T, T1>(Func<T1, T> conv, Predicate<T1> cond, IList<T1> list)
    {
        List<T> newList = new List<T>();
        foreach (var item in list)
        {
            if (cond(item))
                newList.Add(conv(item));
        }
        return newList;
    }
    #endregion
    #region convert
    public static T[] map<T, T1>(Func<T1, T> f, IList<T1> list)
    {
        T[] array = new T[list.Count];
        for (int i = 0; i < array.Length; i++)
            array[i] = f(list[i]);
        return array;
    }
    public static T[] map<T, T1, T2>(Func<T1, T2, T> f, IList<T1> t1s, IList<T2> t2s)
    {
        if (t1s.Count != t2s.Count)
            throw new ArgumentException("mapError: List counts differ!");
        if (t1s == null || t2s == null)
            throw new ArgumentException("mapError: onr or more lists are null");
        T[] ts = new T[t1s.Count];
        for (int i = 0; i < t1s.Count; i++)
            ts[i] = f(t1s[i], t2s[i]);
        return ts;
    }
    public static T[] map<T, T1, T2, T3>(Func<T1, T2, T3, T> f, IList<T1> t1s, IList<T2> t2s, IList<T3> t3s)
    {
        if (t1s.Count != t2s.Count)
            throw new ArgumentException("mapError: List counts differ!");
        if (t1s == null || t2s == null)
            throw new ArgumentException("mapError: onr or more lists are null");
        T[] ts = new T[t1s.Count];
        for (int i = 0; i < t1s.Count; i++)
            ts[i] = f(t1s[i], t2s[i], t3s[i]);
        return ts;
    }
    #endregion

    public static bool inRange(float a, float min, float max) => min <= a && a <= max;
    public static bool inRange(Vector2 a, Vector2 min, Vector2 max) => inRange(a.x, min.x, max.x) && inRange(a.y, min.y, max.y);
    public static bool inRange(Vector2 a, (Vector2 min, Vector2 max) bounds) => inRange(a, bounds.min, bounds.max);

    public static float frac(float a) => a - (int)a + ((a < 0) ? 1 : 0);
    public static Vector2 frac(Vector2 a) => new Vector2(frac(a.x), frac(a.y));
    public static float floor(float a) => a - a % 1f - ((a < 0) ? 1 : 0);
    public static Vector2 floor(Vector2 a) => new Vector2(floor(a.x), floor(a.y));
    public static float ceil(float a) => a + frac(a);

    public static Vector2 topDown(Vector3 a) => new Vector2(a.x, a.z);
    public static Vector2 fV2(float a) => new Vector2(a, a);
    public static Vector2 sign(Vector2 a) => new Vector2(sign(a.x), sign(a.y));
    public static float sign(float a) => a >= 0 ? 1 : -1;

    public static float min(float a, float b) => a < b ? a : b;
}
