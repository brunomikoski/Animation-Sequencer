#if DOTWEEN_ENABLED
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
        public bool LocalMove
        {
            get => localMove;
            set => localMove = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private Vector3 previousPosition;
        private GameObject previousTarget;

        public override string DisplayName => "Move to Position";

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
            previousTarget = target;
            if (localMove)
            {
                previousPosition = target.transform.localPosition;
                moveTween = target.transform.DOLocalMove(GetPosition(), duration);
                
            }
            else
            {
                previousPosition = target.transform.position;
                moveTween = target.transform.DOMove(GetPosition(), duration);
            }

            moveTween.SetOptions(axisConstraint);
            return moveTween;
        }

        protected abstract Vector3 GetPosition();

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (localMove)
                previousTarget.transform.localPosition = previousPosition;
            else
                previousTarget.transform.position = previousPosition;
        }
    }
}
#endif