using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TweenManager : MonoBehaviour
{
    public static TweenManager instance;
    public List<Tween> tweens = new List<Tween>();
    public bool debug = true;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneUnloaded += OnSceneUnload;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnSceneUnload(Scene scene)
    {
        //kill all tweens on room change unless we specify otherwise
        for (int i = tweens.Count - 1; i >= 0; i--)
        {
            if (tweens[i].GetDestroyOnLoad())
            {
                tweens[i].Stop();
            }
        }
    }

    private void Update()
    {
        for (int i = tweens.Count - 1; i >= 0; i--)
        {
            tweens[i].Update();
        }
    }
}
