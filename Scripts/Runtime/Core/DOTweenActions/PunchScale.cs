#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchScale : Punch
    {
        public override string DisplayName => DisplayNames.PunchScale;
        
        private Vector3 previousScale;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            PreviousTarget = target.transform;
            previousScale = PreviousTarget.localScale;
            
            Tweener tween = target.transform.DOPunchScale(punchMagnitude, duration, vibrato, elasticity);

            return tween;
        }

        public override void Reset()
        {
            if (PreviousTarget == null) return;
            PreviousTarget.localScale = previousScale;
        }
    }
}
#endif