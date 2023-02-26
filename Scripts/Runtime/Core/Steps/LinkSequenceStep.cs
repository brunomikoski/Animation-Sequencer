#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class LinkSequenceStep : AnimationStep
    {
        public override string DisplayName => DisplayNames.LinkSequenceStep;

        [FormerlySerializedAs("sequencer")]
        [SerializeField]
        private AnimationSequence animationSequence;

        public AnimationSequence AnimationSequence
        {
            get => animationSequence;
            set => animationSequence = value;
        }

        public override void AddTween(Sequence animationSequence)
        {
            Sequence sequence = this.animationSequence.GenerateSequence();
            sequence.SetDelay(Delay);

            if (FlowType == FlowType.Join) 
                animationSequence.Join(sequence);
            else 
                animationSequence.Append(sequence);
        }

        public override void Reset()
        {
            animationSequence.ResetToInitialState();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (animationSequence != null) 
                display = animationSequence.name;
            
            return $"{index}. Play {display} Sequence";
        }

        public void SetTarget(AnimationSequence target) => animationSequence = target;
    }
}
#endif