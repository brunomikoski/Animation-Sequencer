#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class MovePosition : Move
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => DisplayNames.MovePosition;

        [SerializeField]
        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        protected override Vector3 GetPosition() => position;
    }
}
#endif