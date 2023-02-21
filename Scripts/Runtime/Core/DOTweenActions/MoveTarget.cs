#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class MoveTarget : Move
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => DisplayNames.MoveTarget;

        [SerializeField] private Transform target;
        
        public Transform Target
        {
            get => target;
            set => target = value;
        }

        protected override Vector3 GetPosition() => IsLocal ? target.localPosition : target.position;
    }
}
#endif