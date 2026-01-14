#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetCanvasGroupPropertiesStep : AnimationStepBase
    {
        [SerializeField]
        private CanvasGroup targetCanvasGroup;

        [SerializeField] 
        private float targetAlpha = 1f;

        private float originalAlpha;
        
        public override string DisplayName => "Set Target Canvas Group Properties";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                originalAlpha = targetCanvasGroup.alpha; 
                targetCanvasGroup.alpha = targetAlpha;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Join(behaviourSequence);
            else
                animationSequence.Append(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            targetCanvasGroup.alpha = originalAlpha;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetCanvasGroup != null)
                display = targetCanvasGroup.name;
            
            return $"{index}. Set {display} Properties";
        } 
    }
}
#endif