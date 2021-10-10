using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchPositionDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Punch Position";

        [SerializeField]
        private Vector3 punch;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float elasticity = 1f;
        [SerializeField]
        private bool snapping;

        private Transform previousTarget;
        private Vector3 previousPosition;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousPosition = target.transform.position;
            Tweener tween = target.transform.DOPunchPosition(punch, duration, vibrato, elasticity, snapping);

            return tween;
        }

        public override void ResetToInitialState()
        {
            previousTarget.position = previousPosition;
        }
    }
}
