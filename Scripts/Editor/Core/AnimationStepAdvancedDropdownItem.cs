#if DOTWEEN_ENABLED
using System;
using UnityEditor.IMGUI.Controls;

namespace BrunoMikoski.AnimationSequencer
{
    public sealed class AnimationStepAdvancedDropdownItem : AdvancedDropdownItem
    {
        private readonly Type animationStepType;
        public Type AnimationStepType => animationStepType;

        public AnimationStepAdvancedDropdownItem(AnimationStep animationStep, string displayName) : base(displayName)
        {
            animationStepType = animationStep.GetType();
        }
    }
}
#endif