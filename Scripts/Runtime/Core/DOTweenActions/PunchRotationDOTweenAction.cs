using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchRotationDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Punch Rotation";

        [SerializeField]
        private Vector3 punch;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float elasticity = 1f;
        

        public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
        {
            Tweener tween = target.transform.DOPunchRotation(punch, duration, vibrato, elasticity);
            
            SetTween(tween, loops, loopType);
        }
    }
}
