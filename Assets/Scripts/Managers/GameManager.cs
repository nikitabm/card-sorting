using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent GameStarted;

    private void Awake()
    {
    }

    private void Start()
    {
        GameStarted.Invoke();
    }

    private void Update()
    {

    }
}
