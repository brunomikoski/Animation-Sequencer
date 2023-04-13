#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class RotateEuler : Rotate
    {
        public override string DisplayName => DisplayNames.RotateEuler;

        [SerializeField] 
        private Vector3 eulerAngles;
        public Vector3 EulerAngles
        {
            get => eulerAngles;
            set => eulerAngles = value;
        }

        protected override Vector3 GetRotation() => eulerAngles;
    }
}
#endif