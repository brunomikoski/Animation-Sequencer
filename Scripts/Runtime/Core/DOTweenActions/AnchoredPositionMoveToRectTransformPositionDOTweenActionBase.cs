#if DOTWEEN_ENABLED
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class AnchoredPositionMoveToRectTransformPositionDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
    {
        [SerializeField]
        private RectTransform target;

        public override string DisplayName => "Move to RectTransform Anchored Position";

        protected override Vector2 GetPosition()
        {
            return target.anchoredPosition;
        }
    }
}
#endif
