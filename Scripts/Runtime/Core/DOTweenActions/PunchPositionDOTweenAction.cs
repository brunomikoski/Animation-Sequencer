using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchPositionDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Punch Position";

        [SerializeField]
        private Vector3 punch;
        [SerializeField]
        private int vibrato = 10;
        [SerializeField]
        private float elasticity = 1f;
        [SerializeField]
        private bool snapping;
        
        public override Tweener CreateTweenInternal(GameObject target, float duration)
        {
            Tweener tween = target.transform.DOPunchPosition(punch, duration, vibrato, elasticity, snapping);

            return tween;
        }

    }
}
