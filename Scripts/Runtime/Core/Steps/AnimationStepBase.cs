#if DOTWEEN_ENABLED
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
        public float Delay => delay;

        [SerializeField]
        private FlowType flowType;
        public FlowType FlowType => flowType;

        public abstract string DisplayName { get; }
        
        public abstract void AddTweenToSequence(Sequence animationSequence);

        public abstract void ResetToInitialState();

        public virtual string GetDisplayNameForEditor(int index)
        {
            return $"{index}. {this}";
        }
    }
}
#endif