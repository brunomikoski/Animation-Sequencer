using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlaySequenceAnimationStep : AnimationStepBase
    {
        public override string DisplayName => "Play Sequence";

        [SerializeField]
        private AnimationSequencerController sequencer;

        public override float Duration
        {
            get
            {
                if (sequencer == null)
                    return 0;
                return sequencer.Duration;
            }
        }
        
        public AnimationSequencerController Target => sequencer;

        public override Tween GenerateTween()
        {
            return sequencer.GenerateSequence();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (sequencer != null)
                display = sequencer.name;
            return $"{index}. Play {display} Sequence";
        }

        public void SetTarget(AnimationSequencerController newTarget)
        {
            sequencer = newTarget;
        }
    }
}
