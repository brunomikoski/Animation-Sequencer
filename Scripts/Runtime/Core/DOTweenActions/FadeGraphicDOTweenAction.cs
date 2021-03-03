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
        

        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            Graphic graphic = target.GetComponent<Graphic>();
            if (graphic == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return false;
            }

            TweenerCore<Color, Color, ColorOptions> graphicTween = graphic.DOFade(alpha, duration);
            SetTween(graphicTween, loops, loopType);
            return true;
        }
    }
}
