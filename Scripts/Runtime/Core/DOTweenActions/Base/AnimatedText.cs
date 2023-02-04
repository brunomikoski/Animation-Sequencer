#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnimatedText : SequencerAnimationBase
    {
        [SerializeField] protected string text;
        [SerializeField] protected bool richText;
        [SerializeField] protected ScrambleMode scrambleMode = ScrambleMode.None;
        
        protected string PreviousText;
        
        public string Text
        {
            get => text;
            set => text = value;
        }
        
        public bool RichText
        {
            get => richText;
            set => richText = value;
        }

        public ScrambleMode ScrambleMode
        {
            get => scrambleMode;
            set => scrambleMode = value;
        }
    }
}
#endif