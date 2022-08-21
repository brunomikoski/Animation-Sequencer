#if DOTWEEN_ENABLED
using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class RectTransformSizeDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(RectTransform);
        public override string DisplayName => "RectTransform Size";

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
            var tween = previousTarget.DOSizeDelta(sizeDelta, duration);
            tween.SetOptions(axisConstraint);

            return tween;
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;

            previousTarget.sizeDelta = previousSize;
        }
    }
}
#endif