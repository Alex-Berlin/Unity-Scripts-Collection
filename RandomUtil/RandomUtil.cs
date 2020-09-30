using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public static class RandomUtil
{
    /// <summary>
    /// Returns random boolean value.
    /// </summary>
    /// <returns></returns>
    public static bool CoinFlip()
    {
        int flip = UnityEngine.Random.Range(0, 2);
        return flip != 0;
    }

    /// <summary>
    /// Returns random T from ICollection(array, list, etc.).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T GetRandomFrom<T>(ICollection<T> array)
    {
        int count = array.Count;
        return array.ElementAt(UnityEngine.Random.Range(0, count));
    }

    /// <summary>
    /// Returns true with chance%.
    /// </summary>
    /// <param name="chance">From 0 to 100</param>
    /// <returns></returns>
    public static bool Percentage(float chance)
    {
        if (chance > 100f || chance < 0f)
            throw new ArgumentOutOfRangeException(nameof(chance));

        return chance >= UnityEngine.Random.Range(0f, 100f);
    }
}