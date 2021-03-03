using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FillImageDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Image);
        public override string DisplayName => "Fill Amount";

        [SerializeField, Range(0, 1)]
        private float fillAmount;
        
        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            Image component = target.GetComponent<Image>();
            if (component == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return false;
            }
            
            TweenerCore<float, float, FloatOptions> tween = component.DOFillAmount(fillAmount, duration);
            SetTween(tween, loops, loopType);
            return true;
        }
    }
}
