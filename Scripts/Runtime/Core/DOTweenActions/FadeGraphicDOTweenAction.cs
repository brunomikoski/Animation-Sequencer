using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeGraphicDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Fade Graphic";

        [SerializeField]
        private float alpha;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            Graphic graphic = target.GetComponent<Graphic>();
            if (graphic == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return null;
            }

            TweenerCore<Color, Color, ColorOptions> graphicTween = graphic.DOFade(alpha, duration);
            return graphicTween;
        }

    }
}
