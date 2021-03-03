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
        

        public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
        {
            Graphic graphic = target.GetComponent<Graphic>();
            if (graphic == null)
                throw new Exception($"{target} does not contain {typeof(Graphic)} component");

            TweenerCore<Color, Color, ColorOptions> graphicTween = graphic.DOFade(alpha, duration);
            SetTween(graphicTween, loops, loopType);
        }
    }
}
