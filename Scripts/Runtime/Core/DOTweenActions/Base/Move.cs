#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public abstract class Move : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private bool isLocal;
        public bool IsLocal
        {
            get => isLocal;
            set => isLocal = value;
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

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> tween;
            previousTarget = target;

            if (isLocal)
            {
                previousPosition = target.transform.localPosition;
                tween = target.transform.DOLocalMove(GetPosition(), duration);
            }
            else
            {
                previousPosition = target.transform.position;
                tween = target.transform.DOMove(GetPosition(), duration);
            }

            tween.SetOptions(axisConstraint);
            return tween;
        }

        protected abstract Vector3 GetPosition();

        public override void Reset()
        {
            if (previousTarget == null) 
                return;

            if (isLocal) 
                previousTarget.transform.localPosition = previousPosition;
            else 
                previousTarget.transform.position = previousPosition;
        }
    }
}
#endif