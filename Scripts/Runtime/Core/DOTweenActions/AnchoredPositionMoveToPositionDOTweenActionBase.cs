#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class AnchoredPositionMoveToPositionDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
    {
        [SerializeField]
        private Vector2 position;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public override string DisplayName => "Move To Anchored Position";

        protected override Vector2 GetPosition()
        {
            return position;
        }
    }
}
#endif