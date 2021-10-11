using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchScaleDOTweenAction : DOTweenActionBase
    {
        public override string DisplayName => "Punch Scale";
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private Vector3 punch;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float elasticity = 1f;

        private Transform previousTarget;
        private Vector3 previousScale;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousScale = previousTarget.localScale;
            
            Tweener tween = target.transform.DOPunchScale(punch, duration, vibrato, elasticity);

            return tween;
        }

        public override void ResetToInitialState()
        {
            previousTarget.localScale = previousScale;
        }
    }
}
