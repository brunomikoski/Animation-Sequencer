using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnchoredPositionMoveDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(RectTransform);

        [SerializeField]
        private AxisConstraint axisConstraint;

        public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
        {
            RectTransform rectTransform = target.transform as RectTransform;

            if (rectTransform == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return false;
            }

            TweenerCore<Vector2, Vector2, VectorOptions> anchorPosTween = rectTransform.DOAnchorPos(GetPosition(), duration);

            anchorPosTween.SetOptions(axisConstraint);

            SetTween(anchorPosTween, loops, loopType);
            return true;
        }

        protected abstract Vector2 GetPosition();
    }
}
