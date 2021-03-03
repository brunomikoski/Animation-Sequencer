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
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float randomness = 90;
        [SerializeField]
        private bool fadeout = true;
        
        public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
        {
            Tweener tween = target.transform.DOShakeScale(duration, strength, vibrato, randomness, fadeout);
            
            SetTween(tween, loops, loopType);
        }
    }
}
