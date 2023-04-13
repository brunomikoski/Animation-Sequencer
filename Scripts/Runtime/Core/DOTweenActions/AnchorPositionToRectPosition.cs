#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class AnchorPositionToRectPosition : AnimateAnchorPosition
    {
        public override string DisplayName => DisplayNames.AnchorPositionToRectPosition;

        [SerializeField]
        private RectTransform target;

        protected override Vector2 GetPosition()
        {
            return target.anchoredPosition;
        }
    }
}
#endif
