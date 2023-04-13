#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class ShakeScale : Shake
    {
        public override string DisplayName => DisplayNames.ShakeScale;
        
        private Vector3 previousScale;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            PreviousTarget = target.transform;
            previousScale = PreviousTarget.localScale;
            Tweener tween = PreviousTarget.DOShakeScale(duration, strength, vibrato, randomness, fadeout);
            return tween;
        }

        public override void Reset()
        {
            if (PreviousTarget == null) 
                return;
            
            PreviousTarget.localScale = previousScale;
        }
    }
}
#endif