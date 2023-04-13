#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public abstract class AnimatedText : SequencerAnimationBase
    {
        [SerializeField]
        protected string text;
        public string Text
        {
            get => text;
            set => text = value;
        }

        
        [SerializeField]
        protected bool richText;
        public bool RichText
        {
            get => richText;
            set => richText = value;
        }

       
        [SerializeField]
        protected ScrambleMode scrambleMode = ScrambleMode.None;
        public ScrambleMode ScrambleMode
        {
            get => scrambleMode;
            set => scrambleMode = value;
        }
        
        protected string PreviousText;


    }
}
#endif