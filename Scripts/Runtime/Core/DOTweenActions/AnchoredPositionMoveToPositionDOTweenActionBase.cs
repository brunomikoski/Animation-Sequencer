using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class AnchoredPositionMoveToPositionDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
    {
        [SerializeField]
        private Vector2 position;

        public override string DisplayName => "Move To Anchored Position";

        protected override Vector2 GetPosition()
        {
            return position;
        }
    }
}
