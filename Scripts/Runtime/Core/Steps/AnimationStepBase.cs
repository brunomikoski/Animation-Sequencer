using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnimationStepBase
    {
        [SerializeField]
        private float delay;
        [SerializeField]
        private FlowType flowType;

        public float Delay => delay;
        public abstract float Duration { get; }
        public FlowType FlowType => flowType;

        public abstract string DisplayName { get; }
        
        //TODO find a way to discover if one step is playing or not.
        public bool IsPlaying => false;
        
        public abstract void AddTweenToSequence(Sequence animationSequence);

        public virtual string GetDisplayNameForEditor(int index)
        {
            return $"{index}. {this}";
        }
    }
}
