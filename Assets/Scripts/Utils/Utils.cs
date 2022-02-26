using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public static class Utils
{
    public static Vector3 GetVectorFromAngle(int angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public static IEnumerator DelayAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
        yield return null;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 11)
            {
                return false;
            }
        }
        return results.Count > 0;
    }

    public static void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}