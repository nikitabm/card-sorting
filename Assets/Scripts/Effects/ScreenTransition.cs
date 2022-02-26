using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    public Image mainImage;

    private Tween _transitionTween;

    public void TransitionToScene(int targetScene, float transitionAnimTime = 0.75f)
    {
        DontDestroyOnLoad(gameObject);
        // transition out
        _transitionTween = gameObject.AddComponent<Tween>();
        _transitionTween.ease = Tween.Ease.OutQuad;
        _transitionTween.overTime = transitionAnimTime;
        _transitionTween.delay = 0.125f;
        _transitionTween.ignoreTimeScale = true;
        _transitionTween.startValue = 2000f;
        _transitionTween.endValue = 0f;
        _transitionTween.onUpdate = delegate (float v, float t)
        {
            mainImage.rectTransform.sizeDelta = new Vector2(v, v);
        };
        _transitionTween.onComplete = delegate ()
        {
            mainImage.rectTransform.sizeDelta = new Vector2(0f, 0f);

            // change scene
            SceneManager.LoadScene(targetScene);

            // transition in
            _transitionTween = gameObject.AddComponent<Tween>();
            _transitionTween.ease = Tween.Ease.InQuad;
            _transitionTween.overTime = transitionAnimTime;
            _transitionTween.delay = 0.25f;
            _transitionTween.ignoreTimeScale = true;
            _transitionTween.startValue = 0f;
            _transitionTween.endValue = 2000f;
            _transitionTween.onUpdate = delegate (float v, float t)
            {
                mainImage.rectTransform.sizeDelta = new Vector2(v, v);
            };
            _transitionTween.onComplete = delegate ()
            {
                Destroy(gameObject);
            };
        };
    }
}
