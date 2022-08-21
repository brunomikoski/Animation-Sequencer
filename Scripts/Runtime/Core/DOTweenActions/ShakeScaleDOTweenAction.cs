#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ShakeScaleDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Shake Scale";

        [SerializeField]
        private Vector3 strength;
        public Vector3 Strength
        {
            get => strength;
            set => strength = value;
        }

        [SerializeField]
        private int vibrato = 10;
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }

        [SerializeField]
        private float randomness = 90;
        public float Randomness
        {
            get => randomness;
            set => randomness = value;
        }

        [SerializeField]
        private bool fadeout = true;
        public bool Fadeout
        {
            get => fadeout;
            set => fadeout = value;
        }

        private Transform previousTarget;
        private Vector3 previousScale;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousScale = previousTarget.localScale;
            
            Tweener tween = previousTarget.DOShakeScale(duration, strength, vibrato, randomness, fadeout);

            return tween;
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            previousTarget.localScale = previousScale;
        }
    }
}
#endif