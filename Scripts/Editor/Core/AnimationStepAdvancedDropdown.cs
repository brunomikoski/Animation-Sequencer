#if DOTWEEN_ENABLED
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public sealed class AnimationStepAdvancedDropdown : AdvancedDropdown
    {
        private Action<AnimationStepAdvancedDropdownItem> callBack;

        public AnimationStepAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {
            this.minimumSize = new Vector2(200, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Animation Step");

            TypeCache.TypeCollection availableTypesOfAnimationStep = TypeCache.GetTypesDerivedFrom(typeof(AnimationStep));
            foreach (Type animatedItemType in availableTypesOfAnimationStep)
            {
                if (animatedItemType.IsAbstract)
                    continue;
                
                AnimationStep animationStep = Activator.CreateInstance(animatedItemType) as AnimationStep;

                string displayName = animationStep.GetType().Name;
                if (!string.IsNullOrEmpty(animationStep.DisplayName))
                    displayName = animationStep.DisplayName;
                
                root.AddChild(new AnimationStepAdvancedDropdownItem(animationStep, displayName));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            callBack?.Invoke(item as AnimationStepAdvancedDropdownItem);
        }

        public void Show(Rect rect, Action<AnimationStepAdvancedDropdownItem> onItemSelectedCallback)
        {
            callBack = onItemSelectedCallback;
            base.Show(rect);
        }
    }
}
#endif