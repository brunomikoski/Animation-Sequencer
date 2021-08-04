using System;
#if UNITY_EDITOR
using DG.DOTweenEditor;
#endif
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class DOTweenActionBase
    {
        public enum AnimationDirection
        {
            To, 
            From
        }
        
        public abstract string DisplayName { get; }
        
        [SerializeField]
        protected AnimationDirection direction;
        [SerializeField]
        protected CustomEase ease = CustomEase.InOutCirc;
        [SerializeField]
        protected bool isRelative;
        [SerializeField]
        protected bool autoKill;
        
        public abstract Tweener CreateTweenInternal(GameObject target, float duration);
        
        public virtual Type TargetComponentType { get; }


        public Tween GenerateTween(GameObject target, float duration)
        {
            var tween = CreateTweenInternal(target, duration);
            if (direction == AnimationDirection.From)
                tween.From();

            tween.SetAutoKill(autoKill);
            tween.SetEase(ease);
            tween.SetRelative(isRelative);
            return tween;
        }
    }
}
