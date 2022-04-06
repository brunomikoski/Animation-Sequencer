using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class AnchoredPositionMoveToRectTransformPositionDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
    {
        [SerializeField]
        private RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get => rectTransform;
            set => rectTransform = value;
        }

        public override string DisplayName => "Move to RectTransform Anchored Position";

        protected override Vector2 GetPosition()
        {
            return rectTransform.anchoredPosition;
        }
    }
}
