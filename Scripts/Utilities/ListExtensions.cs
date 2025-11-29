using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void MoveFirstToLast<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            T firstItem = list[0];
            list.RemoveAt(0);
            list.Add(firstItem);
        }
    }

    public static void ReduceTo<T>(this IList<T> list, int number)
    {
        if (list.Count > number)
        {
            while (list.Count > number)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
