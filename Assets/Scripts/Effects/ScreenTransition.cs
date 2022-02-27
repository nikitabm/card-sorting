using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Image mainImage;

    private Tween _transitionTween;

    public void TransitionToScene(int sceneId, float overTime)
    {
        DontDestroyOnLoad(gameObject);
        //transition out
        _transitionTween = new Tween()
        .SetEase(Tween.Ease.OutQuad)
        .SetTime(overTime)
        .SetDelay(0.125f)
        .SetIgnoreGameSpeed(true)
        .SetStart(2000)
        .SetEnd(0)
        .SetOnUpdate(delegate (float v, float t)
        {
            mainImage.rectTransform.sizeDelta = new Vector2(v, v);
        })
        .SetOnComplete(delegate ()
        {
            mainImage.rectTransform.sizeDelta = new Vector2(0f, 0f);

            //change scene
            SceneManager.LoadScene(sceneId);

            //transition in
            _transitionTween = new Tween()
            .SetEase(Tween.Ease.InQuad)
            .SetTime(overTime)
            .SetDelay(0.25f)
            .SetIgnoreGameSpeed(true)
            .SetStart(0f)
            .SetEnd(2000)
            .SetDestroyOnLoad(false)
            .SetOnUpdate(delegate (float v, float t)
            {
                mainImage.rectTransform.sizeDelta = new Vector2(v, v);
            })
            .SetOnComplete(delegate ()
            {
                Destroy(gameObject);
            });
        });
    }
}