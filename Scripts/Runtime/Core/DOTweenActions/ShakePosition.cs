#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ShakePosition : Shake
    {
        public override string DisplayName => DisplayNames.ShakePosition;

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
            previousPosition = PreviousTarget.position;
            Tweener tween =
                target.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeout);
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