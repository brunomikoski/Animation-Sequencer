#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetImagePropertiesStep : AnimationStepBase
    {
        [SerializeField]
        private Image targetGraphic;

        [SerializeField] 
        private Color targetColor = Color.white;
        [SerializeField]
        private Sprite targetSprite;
        [SerializeField] 
        private Material targetMaterial;

        private Color originalColor = Color.white;
        private Material originalMaterial;
        private Sprite originalSprite;

        public override string DisplayName => "Set Target Image Properties";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {

                if (targetColor != targetGraphic.color)
                {
                    originalColor = targetGraphic.color;
                    targetGraphic.color = targetColor;
                }

                if (targetSprite != null)
                {
                    originalSprite = targetGraphic.sprite; 
                    targetGraphic.sprite = targetSprite;
                }

                if (targetMaterial != null)
                {
                    originalMaterial = targetGraphic.material;
                    targetGraphic.material = targetMaterial;
                }


            });
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(behaviourSequence);
            else
                animationSequence.Append(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            targetGraphic.color = originalColor;
            
            if(originalSprite != null)
                targetGraphic.sprite = originalSprite;
            
            if (originalMaterial != null)
                targetGraphic.material = targetMaterial;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetGraphic != null)
                display = targetGraphic.name;
            
            return $"{index}. Set {display}(Image) Properties";
        } 
    }
}
#endif