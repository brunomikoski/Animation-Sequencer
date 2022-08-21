#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class MoveToPositionDOTweenActionBase : MoveDOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public override string DisplayName => "Move To Position";

        protected override Vector3 GetPosition()
        {
            return position;
        }
    }
}
#endif