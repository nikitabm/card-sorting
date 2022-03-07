using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public static class Utils
{
    public static IEnumerator DelayAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
        yield return null;
    }
}