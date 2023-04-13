#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    // Fixes "Unknown managed type referenced" error
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Scripting/APIUpdating/UpdatedFromAttribute.cs
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "DOTweenAnimationStep")]
    public abstract class AnimationStep
    {
        [SerializeField]
        private float delay;
        public float Delay => delay;

        [SerializeField]
        private FlowType flowType;
        public FlowType FlowType => flowType;


        public static string NameOfFlowType => nameof(flowType);
        public abstract string DisplayName { get; }
        public abstract void AddTween(Sequence animationSequence);
        public abstract void Reset();
        public virtual string GetDisplayNameForEditor(int index) => $"{index}. {this}";
    }
}
#endif