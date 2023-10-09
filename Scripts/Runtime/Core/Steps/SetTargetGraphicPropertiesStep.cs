#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetGraphicPropertiesStep : AnimationStepBase
    {
        [SerializeField]
        private Graphic targetGraphic;

        [SerializeField] 
        private Color targetColor = Color.white;

        private Color originalColor;
        
        public override string DisplayName => "Set Target Graphic Properties";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                originalColor = targetGraphic.color; 
                targetGraphic.color = targetColor;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Join(behaviourSequence);
            else
                animationSequence.Append(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            targetGraphic.color = originalColor;
        }
        
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetGraphic != null)
                display = targetGraphic.name;
            
            return $"{index}. Set {display} Properties";
        } 
    }
}
#endif