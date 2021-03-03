using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeCanvasGroupDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(CanvasGroup);

        public override string DisplayName => "Fade Canvas Group";

        [SerializeField]
        private float alpha;

        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return false;
            }

            TweenerCore<float, float, FloatOptions> canvasTween = canvasGroup.DOFade(alpha, duration);

            SetTween(canvasTween, loops, loopType);
            return true;
        }
    }
}
