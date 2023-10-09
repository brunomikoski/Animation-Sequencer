#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetTransformPropertiesStep : AnimationStepBase
    {
        public override string DisplayName => "Set Target Transform Properties";
        [FormerlySerializedAs("targetGameObject")] [SerializeField]
        private Transform targetTransform;
        
        [SerializeField]
        private bool useLocal;
        [SerializeField]
        private Vector3 position;
        [SerializeField] 
        private Vector3 eulerAngles;
        [SerializeField] 
        private Vector3 scale = Vector3.one;

        private Vector3 originalPosition;
        private Vector3 originalEulerAngles;
        private Vector3 originalScale;
        
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                if (useLocal)
                {
                    originalPosition = targetTransform.localPosition;
                    originalEulerAngles = targetTransform.localEulerAngles;
                    
                    targetTransform.localPosition = position;
                    targetTransform.localEulerAngles = eulerAngles;
                }
                else
                {
                    originalPosition = targetTransform.position;
                    originalEulerAngles = targetTransform.eulerAngles;
                    
                    targetTransform.position = position;
                    targetTransform.eulerAngles = eulerAngles;
                }

                originalScale = targetTransform.localScale; 
                targetTransform.localScale = scale;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Join(behaviourSequence);
            else
                animationSequence.Append(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            if (useLocal)
            {
                targetTransform.localPosition = originalPosition;
                targetTransform.localEulerAngles = originalEulerAngles;
            }
            else
            {
                targetTransform.position = originalPosition;
                targetTransform.eulerAngles = originalEulerAngles;
            }
            targetTransform.localScale = originalScale;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetTransform != null)
                display = targetTransform.name;
            
            return $"{index}. Set {display} Transform Properties";
        }   
    }
}
#endif