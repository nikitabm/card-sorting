using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public Tween()
    {
        //add this to the tween manager on creation, useless without this step
        if (!TweenManager.instance)
        {
            Debug.LogWarning("Attempted to make Tween without a TweenManager, so making one first");
            new GameObject("TweenManager").AddComponent<TweenManager>();
        }
        TweenManager.instance.tweens.Add(this);
    }

    public enum Ease
    {
        None,
        Custom,
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,

        // Bento interpolations
        Exponential,
        Elastic,
        Sin,
        Cos,

        // Robert Penner interpolations
        OutExpo,
        InExpo,
        InOutExpo,
        OutInExpo,
        OutCirc,
        InCirc,
        InOutCirc,
        OutInCirc,
        OutInQuad,
        OutSine,
        InSine,
        InOutSine,
        OutInSine,
        OutInCubic,
        OutQuart,
        InQuart,
        InOutQuart,
        OutInQuart,
        OutQuint,
        InQuint,
        InOutQuint,
        OutInQuint,
        OutElastic,
        InElastic,
        InOutElastic,
        OutInElastic,
        OutBounce,
        InBounce,
        InOutBounce,
        OutInBounce,
        OutBack,
        InBack,
        InOutBack,
        OutInBack
    }

    public delegate void Callback();
    public delegate void UpdateCallback(float v, float t);
    public Callback OnStart;
    public UpdateCallback OnUpdate;
    public Callback OnComplete;

    float start = 0f;
    float end = 1f;
    AnimationCurve customEase;
    Ease ease;
    float delay = 0f;
    float maxTime = 1f;
    float currentTime = 0f;
    bool paused = false;
    bool ignoreGameSpeed = false;
    bool destroyOnLoad = true;
    float alpha = -4f; // -4 was default damping factor in previous version
    float beta = 1;

    //Tween algorithms
    float GetValue(float nTime)
    {
        float delta = end - start;
        // Calculate value from algorithm for tween
        switch (ease)
        {
            case Ease.Custom: // custom animation curve
                if (customEase != null)
                {
                    return start + (customEase.Evaluate(nTime) * delta);
                }
                break;
            case Ease.Linear: // standard linear
                return delta * nTime + start;
            case Ease.InQuad: // quad in
                return delta * nTime * nTime + start;
            case Ease.OutQuad: // quad out
                return -delta * ((nTime) - 2f) * nTime * nTime + start;
            case Ease.InOutQuad: // quad in out
                nTime *= 2f;
                if (nTime < 1f)
                {
                    return (delta / 2f) * nTime * nTime + start;
                }
                else
                {
                    nTime--;
                    return -(delta / 2f) * (nTime * (nTime - 2f) - 1f) + start;
                }
            case Ease.InCubic: // cubic in
                return delta * nTime * nTime * nTime + start;
            case Ease.OutCubic: // cubic out
                return -delta * ((nTime) - 2f) * nTime * nTime * nTime + start;
            case Ease.InOutCubic: // cubic in out
                nTime *= 2f;
                if (nTime < 1f)
                {
                    return (delta / 2f) * nTime * nTime * nTime + start;
                }
                else
                {
                    nTime -= 2;
                    return (delta / 2f) * (nTime * nTime * nTime + 2) + start;
                }
            // case Ease.Exponential: // exponential
            //     return delta / (Mathf.Exp(-4f) - 1f) * Mathf.Exp(-4f * nTime) + start - delta / (Mathf.Exp(-4f) - 1f);

            // Bento
            case Ease.Exponential: return Tween.Interpolations.exponential(start, end, nTime, alpha);
            case Ease.Elastic: return Tween.Interpolations.elastic(start, end, nTime, alpha, beta);
            case Ease.Sin: return Tween.Interpolations.sin(start, end, nTime, alpha);
            case Ease.Cos: return Tween.Interpolations.cos(start, end, nTime, alpha);

            // Robert Penner
            // case Ease.Linear: return Tween.Interpolations.Linear(nTime, start, delta, 1);
            case Ease.OutExpo: return Tween.Interpolations.ExpoEaseOut(nTime, start, delta, 1);
            case Ease.InExpo: return Tween.Interpolations.ExpoEaseIn(nTime, start, delta, 1);
            case Ease.InOutExpo: return Tween.Interpolations.ExpoEaseInOut(nTime, start, delta, 1);
            case Ease.OutInExpo: return Tween.Interpolations.ExpoEaseOutIn(nTime, start, delta, 1);
            case Ease.OutCirc: return Tween.Interpolations.CircEaseOut(nTime, start, delta, 1);
            case Ease.InCirc: return Tween.Interpolations.CircEaseIn(nTime, start, delta, 1);
            case Ease.InOutCirc: return Tween.Interpolations.CircEaseInOut(nTime, start, delta, 1);
            case Ease.OutInCirc: return Tween.Interpolations.CircEaseOutIn(nTime, start, delta, 1);
            // case Ease.OutQuad: return Tween.Interpolations.QuadEaseOut(nTime, start, delta, 1);
            // case Ease.InQuad: return Tween.Interpolations.QuadEaseIn(nTime, start, delta, 1);
            // case Ease.InOutQuad: return Tween.Interpolations.QuadEaseInOut(nTime, start, delta, 1);
            case Ease.OutInQuad: return Tween.Interpolations.QuadEaseOutIn(nTime, start, delta, 1);
            case Ease.OutSine: return Tween.Interpolations.SineEaseOut(nTime, start, delta, 1);
            case Ease.InSine: return Tween.Interpolations.SineEaseIn(nTime, start, delta, 1);
            case Ease.InOutSine: return Tween.Interpolations.SineEaseInOut(nTime, start, delta, 1);
            case Ease.OutInSine: return Tween.Interpolations.SineEaseOutIn(nTime, start, delta, 1);
            // case Ease.OutCubic: return Tween.Interpolations.CubicEaseOut(nTime, start, delta, 1);
            // case Ease.InCubic: return Tween.Interpolations.CubicEaseIn(nTime, start, delta, 1);
            // case Ease.InOutCubic: return Tween.Interpolations.CubicEaseInOut(nTime, start, delta, 1);
            case Ease.OutInCubic: return Tween.Interpolations.CubicEaseOutIn(nTime, start, delta, 1);
            case Ease.OutQuart: return Tween.Interpolations.QuartEaseOut(nTime, start, delta, 1);
            case Ease.InQuart: return Tween.Interpolations.QuartEaseIn(nTime, start, delta, 1);
            case Ease.InOutQuart: return Tween.Interpolations.QuartEaseInOut(nTime, start, delta, 1);
            case Ease.OutInQuart: return Tween.Interpolations.QuartEaseOutIn(nTime, start, delta, 1);
            case Ease.OutQuint: return Tween.Interpolations.QuintEaseOut(nTime, start, delta, 1);
            case Ease.InQuint: return Tween.Interpolations.QuintEaseIn(nTime, start, delta, 1);
            case Ease.InOutQuint: return Tween.Interpolations.QuintEaseInOut(nTime, start, delta, 1);
            case Ease.OutInQuint: return Tween.Interpolations.QuintEaseOutIn(nTime, start, delta, 1);
            case Ease.OutElastic: return Tween.Interpolations.ElasticEaseOut(nTime, start, delta, 1);
            case Ease.InElastic: return Tween.Interpolations.ElasticEaseIn(nTime, start, delta, 1);
            case Ease.InOutElastic: return Tween.Interpolations.ElasticEaseInOut(nTime, start, delta, 1);
            case Ease.OutInElastic: return Tween.Interpolations.ElasticEaseOutIn(nTime, start, delta, 1);
            case Ease.OutBounce: return Tween.Interpolations.BounceEaseOut(nTime, start, delta, 1);
            case Ease.InBounce: return Tween.Interpolations.BounceEaseIn(nTime, start, delta, 1);
            case Ease.InOutBounce: return Tween.Interpolations.BounceEaseInOut(nTime, start, delta, 1);
            case Ease.OutInBounce: return Tween.Interpolations.BounceEaseOutIn(nTime, start, delta, 1);
            case Ease.OutBack: return Tween.Interpolations.BackEaseOut(nTime, start, delta, 1);
            case Ease.InBack: return Tween.Interpolations.BackEaseIn(nTime, start, delta, 1);
            case Ease.InOutBack: return Tween.Interpolations.BackEaseInOut(nTime, start, delta, 1);
            case Ease.OutInBack: return Tween.Interpolations.BackEaseOutIn(nTime, start, delta, 1);
        }
        return 0f;
    }

    //sets
    public Tween SetStart(float value)
    {
        start = value;
        return this;
    }
    public Tween SetEnd(float value)
    {
        end = value;
        return this;
    }
    public Tween SetTime(float value)
    {
        maxTime = value;
        return this;
    }
    public Tween SetDelay(float value)
    {
        delay = value;
        return this;
    }
    public Tween SetEase(Ease value)
    {
        ease = value;
        return this;
    }
    public Tween SetDamping(float value)
    {
        alpha = -value;
        return this;
    }
    public Tween SetOscillations(float value)
    {
        beta = value;
        return this;
    }
    public Tween SetOnStart(Callback callback)
    {
        OnStart = callback;
        return this;
    }
    public Tween SetOnUpdate(UpdateCallback callback)
    {
        OnUpdate = callback;
        return this;
    }
    public Tween SetOnComplete(Callback callback)
    {
        OnComplete = callback;
        return this;
    }
    public Tween SetIgnoreGameSpeed(bool value)
    {
        ignoreGameSpeed = value;
        return this;
    }
    public Tween SetCustomEase(AnimationCurve value)
    {
        customEase = value;
        return this;
    }
    public Tween SetDestroyOnLoad(bool value)
    {
        destroyOnLoad = value;
        return this;
    }

    public bool GetDestroyOnLoad()
    {
        return destroyOnLoad;
    }

    //playback control
    public Tween SetPause(bool value)
    {
        paused = value;
        return this;
    }
    public void Stop()
    {
        //just remove this? 
        TweenManager.instance.tweens.Remove(this);
    }

    //update
    public void Update()
    {
        try
        {
            if (currentTime == 0f)
            {
                OnStart?.Invoke();
            }

            if (!paused)
            {
                currentTime += ignoreGameSpeed ? Time.unscaledDeltaTime : Time.deltaTime;
                float realTime = currentTime - delay;
                if (realTime > 0f)
                {
                    OnUpdate?.Invoke(GetValue(Mathf.Clamp01(realTime / maxTime)), realTime);
                }
            }

            if (currentTime >= maxTime + delay - Mathf.Epsilon)
            {
                OnComplete?.Invoke();
                Stop();
            }
        }
        catch (System.Exception e)
        {
            if (TweenManager.instance.debug)
            {
                Debug.Log($"Caught An Exception in a tween, so it's being stopped. Exception: {e}");
            }
            Stop();
        }
    }


    public class Interpolations
    {
        /**
         * Bento interpolations
         * s = start value
         * e = end value
         * t = time [0, 1]
         */
        static public float linear(float s, float e, float t)
        {
            return (e - s) * t + s;
        }
        static public float quadratic(float s, float e, float t)
        {
            return (e - s) * t * t + s;
        }
        static public float squareroot(float s, float e, float t)
        {
            return (e - s) * Mathf.Pow(t, 0.5f) + s;
        }
        static public float cubic(float s, float e, float t)
        {
            return (e - s) * t * t * t + s;
        }
        static public float cuberoot(float s, float e, float t)
        {
            return (e - s) * Mathf.Pow(t, 1 / 3f) + s;
        }
        static public float exponential(float s, float e, float t, float alpha = 1)
        {
            //takes alpha as growth/damp factor
            return (e - s) / (Mathf.Exp(alpha) - 1) * Mathf.Exp(alpha * t) + s - (e - s) / (Mathf.Exp(alpha) - 1);
        }
        static public float elastic(float s, float e, float t, float alpha = 0, float beta = 0)
        {
            //alpha=growth factor, beta=wavenumber
            return (e - s) / (Mathf.Exp(alpha) - 1) * Mathf.Cos(beta * t * 2 * Mathf.PI) * Mathf.Exp(alpha * t) + s - (e - s) / (Mathf.Exp(alpha) - 1);
        }
        static public float sin(float s, float e, float t, float alpha = 1)
        {
            //s=offset, e=amplitude, alpha=wavenumber
            return s + e * Mathf.Sin(alpha * t * 2 * Mathf.PI);
        }
        static public float cos(float s, float e, float t, float alpha = 1)
        {
            //s=offset, e=amplitude, alpha=wavenumber
            return s + e * Mathf.Cos(alpha * t * 2 * Mathf.PI);
        }


        /**
         * Robert penner easings
         * b = s
         * c = e - s
         * d = end time if t is absolute
         */
        static public float Linear(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        static public float ExpoEaseOut(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
        }
        static public float ExpoEaseIn(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
        }
        static public float ExpoEaseInOut(float t, float b, float c, float d)
        {
            if (t == 0)
                return b;

            if (t == d)
                return b + c;

            if ((t /= d / 2) < 1)
                return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;

            return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
        }
        static public float ExpoEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return ExpoEaseOut(t * 2, b, c / 2, d);

            return ExpoEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float CircEaseOut(float t, float b, float c, float d)
        {
            return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
        }
        static public float CircEaseIn(float t, float b, float c, float d)
        {
            return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
        }
        static public float CircEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;

            return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }
        static public float CircEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return CircEaseOut(t * 2, b, c / 2, d);

            return CircEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float QuadEaseOut(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }
        static public float QuadEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }
        static public float QuadEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t + b;

            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }
        static public float QuadEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return QuadEaseOut(t * 2, b, c / 2, d);

            return QuadEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float SineEaseOut(float t, float b, float c, float d)
        {
            return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
        }
        static public float SineEaseIn(float t, float b, float c, float d)
        {
            return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
        }
        static public float SineEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return c / 2 * (Mathf.Sin(Mathf.PI * t / 2)) + b;

            return -c / 2 * (Mathf.Cos(Mathf.PI * --t / 2) - 2) + b;
        }
        static public float SineEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return SineEaseOut(t * 2, b, c / 2, d);

            return SineEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        static public float CubicEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        static public float CubicEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }
        static public float CubicEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t + b;

            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }

        static public float CubicEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return CubicEaseOut(t * 2, b, c / 2, d);

            return CubicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float QuartEaseOut(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
        static public float QuartEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }
        static public float QuartEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t * t + b;

            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
        static public float QuartEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return QuartEaseOut(t * 2, b, c / 2, d);

            return QuartEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float QuintEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        static public float QuintEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        static public float QuintEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
        static public float QuintEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return QuintEaseOut(t * 2, b, c / 2, d);
            return QuintEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float ElasticEaseOut(float t, float b, float c, float d)
        {
            if ((t /= d) == 1)
                return b + c;

            float p = d * .3f;
            float s = p / 4;

            return (c * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b);
        }
        static public float ElasticEaseIn(float t, float b, float c, float d)
        {
            if ((t /= d) == 1)
                return b + c;

            float p = d * .3f;
            float s = p / 4;

            return -(c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
        }
        static public float ElasticEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) == 2)
                return b + c;

            float p = d * (.3f * 1.5f);
            float s = p / 4;

            if (t < 1)
                return -.5f * (c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
            return c * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + c + b;
        }
        static public float ElasticEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return ElasticEaseOut(t * 2, b, c / 2, d);
            return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float BounceEaseOut(float t, float b, float c, float d)
        {
            if ((t /= d) < (1f / 2.75f))
                return c * (7.5625f * t * t) + b;
            else if (t < (2f / 2.75f))
                return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
            else if (t < (2.5f / 2.75f))
                return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
            else
                return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
        }
        static public float BounceEaseIn(float t, float b, float c, float d)
        {
            return c - BounceEaseOut(d - t, 0, c, d) + b;
        }
        static public float BounceEaseInOut(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return BounceEaseIn(t * 2, 0, c, d) * .5f + b;
            else
                return BounceEaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
        static public float BounceEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return BounceEaseOut(t * 2, b, c / 2, d);
            return BounceEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        static public float BackEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1) + b;
        }
        static public float BackEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * ((1.70158f + 1) * t - 1.70158f) + b;
        }
        static public float BackEaseInOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            if ((t /= d / 2) < 1)
                return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }

        static public float BackEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return BackEaseOut(t * 2, b, c / 2, d);
            return BackEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

    }
}
