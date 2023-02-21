#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class AnchorPositionToPosition : AnimateAnchorPosition
    {
        public override string DisplayName => DisplayNames.AnchorPositionToPosition;

        [SerializeField]
        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        protected override Vector2 GetPosition()
        {
            return position;
        }
    }
}
#endif