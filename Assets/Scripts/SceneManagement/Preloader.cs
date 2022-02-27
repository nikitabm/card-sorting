using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    [SerializeField] private LevelTransitioner _levelTransitioner;

    private void Start()
    {
        _levelTransitioner.LoadScene(1, 0.4f);
    }
}
