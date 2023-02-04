#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class CanvasGroupAlpha : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(CanvasGroup);
        public override string DisplayName => DisplayNames.CanvasGroupAlpha;

        [SerializeField] private float alpha;

        private CanvasGroup canvasGroup;
        private float previousFade;
        
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (TryToGetComponent(ref canvasGroup, target)) return null;

            previousFade = canvasGroup.alpha;
            var canvasTween = canvasGroup.DOFade(alpha, duration);
            return canvasTween;
        }

        public override void Reset()
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = previousFade;
        }
    }
}
#endif