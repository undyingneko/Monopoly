using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    public Coroutine StartTrackedCoroutine(string key, IEnumerator coroutine)
    {
        if (activeCoroutines.ContainsKey(key))
        {
            StopTrackedCoroutine(key);
        }

        Coroutine newCoroutine = StartCoroutine(coroutine);
        activeCoroutines[key] = newCoroutine;
        return newCoroutine;
    }

    public void StopTrackedCoroutine(string key)
    {
        if (activeCoroutines.TryGetValue(key, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(key);
        }
    }

    public void StopAllTrackedCoroutines()
    {
        foreach (var coroutine in activeCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
    }
}
