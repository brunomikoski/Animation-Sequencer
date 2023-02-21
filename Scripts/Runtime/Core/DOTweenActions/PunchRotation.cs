#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchRotation : Punch
    {
        public override string DisplayName => DisplayNames.PunchRotation;
        
        private Quaternion previousRotation;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            PreviousTarget = target.transform;
            previousRotation = target.transform.rotation;
            Tweener tween = target.transform.DOPunchRotation(punchMagnitude, duration, vibrato, elasticity);

            return tween;
        }

        public override void Reset()
        {
            if (PreviousTarget == null) 
                return;
            
            PreviousTarget.rotation = previousRotation;
        }
    }
}
#endif