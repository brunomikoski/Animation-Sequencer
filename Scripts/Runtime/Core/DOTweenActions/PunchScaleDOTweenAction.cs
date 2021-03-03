using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchScaleDOTweenAction : DOTweenActionBase
    {
        public override string DisplayName => "Punch Scale";
        
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private Vector3 punch;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float elasticity = 1f;

        public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
        {
            Tweener tween = target.transform.DOPunchScale(punch, duration, vibrato, elasticity);
            
            SetTween(tween, loops, loopType);
        }
    }
}
