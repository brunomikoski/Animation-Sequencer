using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlaySequenceAnimationStep : AnimationStepBase
    {
        public override string DisplayName => "Play Sequence";

        [SerializeField]
        private AnimationSequencerController sequencer;
        public AnimationSequencerController Sequencer => sequencer;

        public override float Duration
        {
            get
            {
                if (sequencer == null)
                    return 0;
                return sequencer.Duration;
            }
        }

        public override void PrepareForPlay()
        {
            base.PrepareForPlay();
            sequencer.PrepareForPlay();
        }

        public override void Play()
        {
            base.Play();
            sequencer.Play();
        }

        public override string GetDisplayName(int index)
        {
            string display = "NULL";
            if (sequencer != null)
                display = sequencer.name;
            return $"{index}. Play {display} Sequence";
        }
    }
}
