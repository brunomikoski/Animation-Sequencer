#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PathPositionDOTweenActionBase : PathDOTweenActionBase
    {
        [SerializeField]
        private Vector3[] positions;
        public Vector3[] Positions => positions;

        public override string DisplayName => "Move to Path Positions" ;

        protected override Vector3[] GetPathPositions()
        {
            return positions;
        }
    }
}
#endif