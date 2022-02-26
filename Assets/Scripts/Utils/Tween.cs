using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Tweens are used for rapidly creating short animations or waits, 
	They are components, so must be added, and are ran the next update frame.


	Typically adding a tween will look something like this:

Tween newTween = gameObject.AddComponent<Tween>();
newTween.ease = Tween.Ease.Linear;
newTween.overTime = 1f;
newTween.delay = 0f;
newTween.ignoreTimeScale = false;
newTween.startValue = 0f;
newTween.endValue = 1f;
newTween.onUpdate = delegate (float v, float t) {
    // work with value and time
};
newTween.onComplete = delegate () {
    // finish up
};


	You can also use them to wait for things like this:

Tween newTween = gameObject.AddComponent<Tween>();
newTween.overTime = 1f;
newTween.onComplete = delegate () {
    // do whatever you were waiting for
};

*/
public class Tween : MonoBehaviour
{
    public enum Ease
    {
        None,
        CustomCurve,
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        Exponential
    }

    public delegate void updateAction(float v, float t);
    public updateAction onUpdate;
    public delegate void completeAction();
    public completeAction onComplete;
    public Ease ease = Ease.None;
    public AnimationCurve customEaseCurve;
    public bool ignoreTimeScale = false;

    float totalTime = 0f;
    public float overTime = 0f;
    public float delay = 0f;

    float value = 0f;
    public float startValue = 0f;
    public float endValue = 1f;

    // Update is called once per frame
    public void Update()
    {
        // Increment time
        totalTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        // Do nothing until the delay is passed
        if ((totalTime - delay) < 0f)
        {
            return;
        }

        // Get time and value
        float time = totalTime - delay;
        float timePercent = time / overTime;
        float deltaVal = endValue - startValue;

        // Calculate value from algorithm for tween
        switch (ease)
        {

            case Ease.CustomCurve: // custom animation curve
                if (customEaseCurve != null)
                {
                    value = startValue + (customEaseCurve.Evaluate(timePercent) * deltaVal);
                }
                break;

            case Ease.Linear: // standard linear
                value = deltaVal * timePercent + startValue;
                break;

            case Ease.InQuad: // quad in
                value = deltaVal * timePercent * timePercent + startValue;
                break;

            case Ease.OutQuad: // quad out
                value = -deltaVal * ((timePercent) - 2f) * timePercent * timePercent + startValue;
                break;

            case Ease.InOutQuad: // quad in out
                timePercent = time / (overTime / 2f);
                if (timePercent < 1f)
                {
                    value = (deltaVal / 2f) * timePercent * timePercent + startValue;
                }
                else
                {
                    timePercent--;
                    value = -(deltaVal / 2f) * (timePercent * (timePercent - 2f) - 1f) + startValue;
                }
                break;

            case Ease.InCubic: // cubic in
                value = deltaVal * timePercent * timePercent * timePercent + startValue;
                break;

            case Ease.OutCubic: // cubic out
                value = -deltaVal * ((timePercent) - 2f) * timePercent * timePercent * timePercent + startValue;
                break;

            case Ease.InOutCubic: // cubic in out
                timePercent = time / (overTime / 2f);
                if (timePercent < 1f)
                {
                    value = (deltaVal / 2f) * timePercent * timePercent * timePercent + startValue;
                }
                else
                {
                    timePercent -= 2;
                    value = (deltaVal / 2f) * (timePercent * timePercent * timePercent + 2) + startValue;
                }
                break;

            case Ease.Exponential: // exponential
                value = deltaVal / (Mathf.Exp(-4f) - 1f) * Mathf.Exp(-4f * timePercent) + startValue - deltaVal / (Mathf.Exp(-4f) - 1f);
                break;
                //TODO: Add more Tween algorithms
        }

        // do update delegate if it exists
        onUpdate?.Invoke(value, time);

        // do complete delegate on end if existing
        if (time >= overTime)
        {
            onComplete?.Invoke();
            Destroy(this);
        }
    }
}