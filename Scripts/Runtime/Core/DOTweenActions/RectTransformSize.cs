#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class RectTransformSize : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(RectTransform);
        public override string DisplayName => DisplayNames.RectTransformSize;

        [SerializeField]
        private Vector2 sizeDelta;
        public Vector2 SizeDelta
        {
            get => sizeDelta;
            set => sizeDelta = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private RectTransform previousTarget;
        private Vector2 previousSize;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform as RectTransform;
            previousSize = previousTarget.sizeDelta;
            TweenerCore<Vector2, Vector2, VectorOptions> tween = previousTarget.DOSizeDelta(sizeDelta, duration);
            tween.SetOptions(axisConstraint);

            return tween;
        }

        public override void Reset()
        {
            if (previousTarget == null) 
                return;
            
            previousTarget.sizeDelta = previousSize;
        }
    }
}
#endif