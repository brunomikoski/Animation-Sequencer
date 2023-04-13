#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class FillImageSequencerAnimation : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Image);
        public override string DisplayName => "Fill Amount";

        [SerializeField, Range(0, 1)]
        private float fillAmount;
        public float FillAmount
        {
            get => fillAmount;
            set => fillAmount = Mathf.Clamp01(value);
        }

        private Image image;
        private float previousFillAmount;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out image))
                return null;

            previousFillAmount = image.fillAmount;
            TweenerCore<float, float, FloatOptions> tween = image.DOFillAmount(fillAmount, duration);
            return tween;
        }

        public override void Reset()
        {
            if (image == null)
                return;

            image.fillAmount = previousFillAmount;
        }
    }
}
#endif