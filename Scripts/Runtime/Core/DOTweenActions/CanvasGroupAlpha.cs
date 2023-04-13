#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class CanvasGroupAlpha : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(CanvasGroup);
        public override string DisplayName => DisplayNames.CanvasGroupAlpha;

        [SerializeField]
        private float alpha;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        private CanvasGroup canvasGroup;
        private float previousFade;


        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out canvasGroup)) 
                return null;

            previousFade = canvasGroup.alpha;
            Tweener canvasTween = canvasGroup.DOFade(alpha, duration);
            return canvasTween;
        }

        public override void Reset()
        {
            if (canvasGroup == null) 
                return;
            
            canvasGroup.alpha = previousFade;
        }
    }
}
#endif