#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class ShakeRotation : Shake
    {
        public override string DisplayName => DisplayNames.ShakeRotation;
        
        private Quaternion previousRotation;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            PreviousTarget = target.transform;
            previousRotation = PreviousTarget.rotation;
            Tweener tween = PreviousTarget.DOShakeRotation(duration, strength, vibrato, randomness, fadeout);
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