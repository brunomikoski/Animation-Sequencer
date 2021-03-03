using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ShakePositionDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Shake Position";

        [SerializeField]
        private Vector3 strength;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float randomness = 90;
        [SerializeField]
        private bool snapping;
        [SerializeField]
        private bool fadeout = true;

        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            Tweener tween = target.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeout);
            
            SetTween(tween, loops, loopType);
            return true;
        }
    }
}
