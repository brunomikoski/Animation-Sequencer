#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public abstract class AnimateAnchorPosition : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(RectTransform);

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private RectTransform rectTransform;
        private Vector2 previousAnchorPosition;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out rectTransform))
                return null;

            previousAnchorPosition = rectTransform.anchoredPosition;
            var anchorPosTween = rectTransform.DOAnchorPos(GetPosition(), duration);
            anchorPosTween.SetOptions(axisConstraint);

            return anchorPosTween;
        }

        protected abstract Vector2 GetPosition();

        public override void Reset()
        {
            if (rectTransform == null) return;
            rectTransform.anchoredPosition = previousAnchorPosition;
        }
    }
}
#endif