using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameObject screenTransition;

    public void LoadScene(int sceneId)
    {
        ScreenTransition thisTransition = Instantiate(screenTransition).GetComponent<ScreenTransition>();
        thisTransition.TransitionToScene(sceneId);
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
