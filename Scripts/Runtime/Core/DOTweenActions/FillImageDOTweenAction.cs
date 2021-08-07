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

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            Image component = target.GetComponent<Image>();
            if (component == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return null;
            }
            
            TweenerCore<float, float, FloatOptions> tween = component.DOFillAmount(fillAmount, duration);
            return tween;
        }

    }
}
