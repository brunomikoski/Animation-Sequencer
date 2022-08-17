#if DOTWEEN_ENABLED
using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using JetBrains.Annotations;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public partial class CustomEase : IEquatable<CustomEase>
    {
        [SerializeField]
        private Ease ease;
        public Ease Ease => ease;
        [SerializeField]
        private AnimationCurve curve;

        private EaseFunction easeFunction;


        public bool UseCustomCurve => ease == Ease.INTERNAL_Custom;

        public CustomEase(AnimationCurve curve)
        {
            this.curve = curve;
            ease = Ease.INTERNAL_Custom;
            easeFunction = new EaseCurve(curve).Evaluate;
        }

        public CustomEase(Ease ease)
        {
            this.ease = ease;
            easeFunction = null;
            curve = null;
        }

        public CustomEase()
        {
            ease = Ease.InOutCirc;
        }
        
        public float Lerp(float from, float to, float fraction)
        {
            return Mathf.Lerp(from, to, Evaluate(fraction));
        }

        public float LerpUnclamped(float from, float to, float fraction)
        {
            return Mathf.LerpUnclamped(from, to, Evaluate(fraction));
        }

        [Pure]
        public float Evaluate(float time, float duration = 1f,
            float overshootOrAmplitude = 1.70158f)
        {
            if (UseCustomCurve)
            {
                if (easeFunction == null)
                    easeFunction = new EaseCurve(curve).Evaluate;

                return EaseManager.Evaluate(Ease.INTERNAL_Custom, easeFunction, time, duration,
                    overshootOrAmplitude, DOTween.defaultEasePeriod);
            }
            else
            {
                return EaseManager.Evaluate(ease, null, time, duration,
                    overshootOrAmplitude, DOTween.defaultEasePeriod);
            }
        }

        public void ApplyTo(TweenParams tweenParams)
        {
            if (UseCustomCurve)
                tweenParams.SetEase(curve);
            else
                tweenParams.SetEase(ease);

        }

        public void ApplyTo<T>(T tween) where T : Tween
        {
            if (UseCustomCurve)
                tween.SetEase(curve);
            else
                tween.SetEase(ease);
        }

        public bool Equals(CustomEase other)
        {
            return ease == other.ease && (ease != Ease.INTERNAL_Custom || Equals(curve, other.curve));
        }

        public override bool Equals(object obj)
        {
            return obj is CustomEase other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)ease * 397) ^ ((ease == Ease.INTERNAL_Custom && curve != null) ? curve.GetHashCode() : 0);
            }
        }
    }
}

namespace DG.Tweening
{
    public static partial class CustomEaseExtensions
    {
        /// <summary>Sets the ease of the tween using a custom ease function.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static TweenParams SetEase(this TweenParams tweenParams, CustomEase customEase)
        {
            customEase.ApplyTo(tweenParams);
            return tweenParams;
        }

        /// <summary>Sets the ease of the tween.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static T SetEase<T>(this T t, CustomEase customEase) where T : Tween
        {
            customEase.ApplyTo(t);
            return t;
        }
    }
}
#endif