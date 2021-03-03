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

        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            Tweener tween = target.transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeout);
            
            SetTween(tween, loops, loopType);
            return true;
        }
    }
}
