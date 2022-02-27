using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{
    [SerializeField] private GameObject screenTransition;

    public void LoadScene(int sceneId, float overtime = 0.75f)
    {
        ScreenTransition thisTransition = Instantiate(screenTransition).GetComponent<ScreenTransition>();
        thisTransition.TransitionToScene(sceneId, overtime);
    }

    public void ReloadScene()
    {
        Scene thisLevel = SceneManager.GetActiveScene();
        if (thisLevel != null)
        {
            LoadScene(thisLevel.buildIndex);
        }
    }
}
