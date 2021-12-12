using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ShakeRotationDOTweenAction : DOTweenActionBase
    {

        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Shake Rotation";

        [SerializeField]
        private Vector3 strength;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float randomness = 90;
        [SerializeField]
        private bool fadeout = true;

        private Transform previousTarget;
        private Quaternion previousRotation;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousRotation = previousTarget.rotation;
            
            Tweener tween = previousTarget.DOShakeRotation(duration, strength, vibrato, randomness, fadeout);

            return tween;
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            previousTarget.rotation = previousRotation;
        }
    }
}
