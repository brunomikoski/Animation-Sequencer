#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class PathPosition : Path
    {
        public override string DisplayName => DisplayNames.PathPosition;

        [SerializeField]
        private Vector3[] positions;
  public Vector3[] Positions
        {
            get => positions;
            set => positions = value;
        }

        protected override Vector3[] GetPathPositions() => positions;
    }
}
#endif