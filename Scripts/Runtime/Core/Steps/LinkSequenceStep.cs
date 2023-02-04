#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class LinkSequenceStep : AnimationStep
    {
        public override string DisplayName => DisplayNames.LinkSequenceStep;

        [SerializeField] private Sequencer sequencer;
        
        public Sequencer Sequencer
        {
            get => sequencer;
            set => sequencer = value;
        }

        public override void AddTween(Sequence animationSequence)
        {
            Sequence sequence = sequencer.GenerateSequence();
            sequence.SetDelay(Delay);
            
            if (FlowType == FlowType.Join) animationSequence.Join(sequence);
            else animationSequence.Append(sequence);
        }

        public override void Reset()
        {
            sequencer.ResetToInitialState();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (sequencer != null) display = sequencer.name;
            return $"{index}. Play {display} Sequence";
        }

        public void SetTarget(Sequencer target) => sequencer = target;
    }
}
#endif