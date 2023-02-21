#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchPosition : Punch
    {
        public override string DisplayName => DisplayNames.PunchPosition;

        [SerializeField]
        private bool snapping;
        public bool Snapping
        {
            get => snapping;
            set => snapping = value;
        }

        private Vector3 previousPosition;


        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            PreviousTarget = target.transform;
            previousPosition = target.transform.position;
            Tweener tween = target.transform.DOPunchPosition(punchMagnitude, duration, vibrato, elasticity, snapping);

            return tween;
        }

        public override void Reset()
        {
            if (PreviousTarget == null) 
                return;
            
            PreviousTarget.position = previousPosition;
        }
    }
}
#endif