#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class WaitForIntervalStep : AnimationStepBase
    {
        public override string DisplayName => "Wait for Interval";

        [SerializeField]
        private float interval;
        public float Interval
        {
            get => interval;
            set => interval = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(Delay);

            sequence.AppendInterval(interval);
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);        }

        public override void ResetToInitialState()
        {
        }

        public override string GetDisplayNameForEditor(int index)
        {
            return $"{index}. Wait {interval} seconds";
        }
    }
}
#endif