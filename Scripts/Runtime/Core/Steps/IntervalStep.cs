#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class IntervalStep : AnimationStep
    {
        public override string DisplayName => DisplayNames.IntervalStep;

        [SerializeField] private float interval;
        
        public float Interval
        {
            get => interval;
            set => interval = value;
        }

        public override void AddTween(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(Delay);

            sequence.AppendInterval(interval);
            
            if (FlowType == FlowType.Join) animationSequence.Join(sequence);
            else animationSequence.Append(sequence);        }

        public override void Reset()
        {
        }

        public override string GetDisplayNameForEditor(int index) => $"{index}. Wait {interval} seconds";
    }
}
#endif