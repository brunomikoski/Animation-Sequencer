using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class MoveToTargetDOTweenActionBase : MoveDOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private Transform target;

        [SerializeField]
        private bool useLocalPosition;

        public override string DisplayName => "Move To Transform Position";

        protected override Vector3 GetPosition()
        {
            if (useLocalPosition)
                return target.localPosition;
            return target.position;
        }
    }
}
