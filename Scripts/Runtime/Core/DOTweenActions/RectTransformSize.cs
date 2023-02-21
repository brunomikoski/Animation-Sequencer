#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class RectTransformSize : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(RectTransform);
        public override string DisplayName => DisplayNames.RectTransformSize;

        [SerializeField] private Vector2 sizeDelta;
        [SerializeField] private AxisConstraint axisConstraint;
        
        private RectTransform previousTarget;
        private Vector2 previousSize;
        
        public Vector2 SizeDelta
        {
            get => sizeDelta;
            set => sizeDelta = value;
        }
        
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform as RectTransform;
            previousSize = previousTarget.sizeDelta;
            var tween = previousTarget.DOSizeDelta(sizeDelta, duration);
            tween.SetOptions(axisConstraint);

            return tween;
        }

        public override void Reset()
        {
            if (previousTarget == null) return;
            previousTarget.sizeDelta = previousSize;
        }
    }
}
#endif