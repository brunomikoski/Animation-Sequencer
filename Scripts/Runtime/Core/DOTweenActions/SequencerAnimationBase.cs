#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class SequencerAnimationBase
    {
        public enum AnimationDirection
        {
            To, 
            From
        }
        
        public virtual Type TargetComponentType { get; }
        public abstract string DisplayName { get; }

        [SerializeField] protected AnimationDirection direction;
        [SerializeField] protected CustomEase ease = CustomEase.InOutCirc;
        [SerializeField] protected bool isRelative;

        public static string NameOfDirection => nameof(direction);
        public static string NameOfEase => nameof(ease);
        public static string NameOfIsRelative => nameof(isRelative);
        
        public AnimationDirection Direction
        {
            get => direction;
            set => direction = value;
        }
        
        public CustomEase Ease
        {
            get => ease;
            set => ease = value;
        }
        
        public bool IsRelative
        {
            get => isRelative;
            set => isRelative = value;
        }

        protected abstract Tweener GenerateTween_Internal(GameObject target, float duration);

        public Tween GenerateTween(GameObject target, float duration)
        {
            Tweener tween = GenerateTween_Internal(target, duration);
            if (direction == AnimationDirection.From)
                // tween.SetRelative() does not work for From variant of "Move To Anchored Position", it must be set
                // here instead. Not sure if this is a bug in DOTween or expected behaviour...
                tween.From(isRelative: isRelative);

            tween.SetEase(ease);
            tween.SetRelative(isRelative);
            return tween;
        }

        public abstract void Reset();
        
        protected bool TryToGetComponent<T>(ref T source, GameObject target, Type cast = null)
        {
            if (source == null)
            {
                source = target.GetComponent<T>();
                if (source == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return false;
                }
            }

            if (cast != null) source = (T) Convert.ChangeType(source, cast);
            return source == null;
        }
    }
}
#endif