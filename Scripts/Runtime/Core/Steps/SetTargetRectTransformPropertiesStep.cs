#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetRectTransformPropertiesStep : AnimationStepBase
    {
        public override string DisplayName => "Set Target RectTransform Properties";
        [SerializeField]
        private RectTransform targetRectTransform;
        
        [SerializeField]
        private bool useLocal;
        [SerializeField]
        private Vector3 position;
        [SerializeField] 
        private Vector3 eulerAngles;
        [SerializeField] 
        private Vector3 scale = Vector3.one;

        [SerializeField] 
        private Vector2 anchorMin =  new Vector2(0.5f, 0.5f);
        [SerializeField] 
        private Vector2 anchorMax =  new Vector2(0.5f, 0.5f);
        [SerializeField] 
        private Vector2 anchoredPosition;
        [SerializeField] 
        private Vector2 sizeDelta;
        [SerializeField] 
        private Vector2 pivot = new Vector2(0.5f, 0.5f);

        private Vector3 originalPosition;
        private Vector3 originalEulerAngles;
        private Vector3 originalScale;
        private Vector2 originalAnchorMin;
        private Vector2 originalAnchorMax;
        private Vector2 originalAnchoredPosition;
        private Vector2 originalSizeDelta;
        private Vector2 originalPivot;
        
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                if (useLocal)
                {
                    originalPosition = targetRectTransform.localPosition;
                    originalEulerAngles = targetRectTransform.localEulerAngles;
                    
                    targetRectTransform.localPosition = position;
                    targetRectTransform.localEulerAngles = eulerAngles;
                }
                else
                {
                    originalPosition = targetRectTransform.position;
                    originalEulerAngles = targetRectTransform.eulerAngles;
                    
                    targetRectTransform.position = position;
                    targetRectTransform.eulerAngles = eulerAngles;
                }


                targetRectTransform.anchorMin = anchorMin;
                targetRectTransform.anchorMax = anchorMax;
                targetRectTransform.anchoredPosition = anchoredPosition;
                targetRectTransform.sizeDelta = sizeDelta;
                targetRectTransform.pivot = pivot;
                
                
                originalAnchorMin = targetRectTransform.anchorMin;
                originalAnchorMax = targetRectTransform.anchorMax;
                originalAnchoredPosition = targetRectTransform.anchoredPosition;
                originalSizeDelta = targetRectTransform.sizeDelta;
                originalPivot = targetRectTransform.pivot;
                
                originalScale = targetRectTransform.localScale; 
                targetRectTransform.localScale = scale;
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
                targetRectTransform.localPosition = originalPosition;
                targetRectTransform.localEulerAngles = originalEulerAngles;
            }
            else
            {
                targetRectTransform.position = originalPosition;
                targetRectTransform.eulerAngles = originalEulerAngles;
            }
            targetRectTransform.localScale = originalScale;
            
            targetRectTransform.anchorMin = originalAnchorMin;
            targetRectTransform.anchorMax = originalAnchorMax;
            targetRectTransform.anchoredPosition = originalAnchoredPosition;
            targetRectTransform.sizeDelta = originalSizeDelta;
            targetRectTransform.pivot = originalPivot;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetRectTransform != null)
                display = targetRectTransform.name;
            
            return $"{index}. Set {display}(RectTransform) Properties";
        }  

    }
}
#endif