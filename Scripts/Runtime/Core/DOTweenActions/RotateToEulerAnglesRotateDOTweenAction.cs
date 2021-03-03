using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class RotateToEulerAnglesRotateDOTweenAction : RotateDOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        public override string DisplayName => "Rotate to Euler Angles";

        [SerializeField]
        private Vector3 eulerAngles;
        
        protected override Vector3 GetRotation()
        {
            return eulerAngles;
        }
    }
}
