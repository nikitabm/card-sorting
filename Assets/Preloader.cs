using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    private void Awake()
    {

    }

    private void Start()
    {
        LevelManager.Instance.LoadScene(1);
    }
}
