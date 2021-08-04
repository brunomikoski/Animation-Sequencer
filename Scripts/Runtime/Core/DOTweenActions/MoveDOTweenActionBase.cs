using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class MoveDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private bool localMove;
        [SerializeField]
        private AxisConstraint axisConstraint;

        public override string DisplayName => "Move to Position";

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
            if (localMove)
                moveTween = target.transform.DOLocalMove(GetPosition(), duration);
            else
                moveTween = target.transform.DOMove(GetPosition(), duration);

            moveTween.SetOptions(axisConstraint);
            return moveTween;
        }

        protected abstract Vector3 GetPosition();
    }
}
