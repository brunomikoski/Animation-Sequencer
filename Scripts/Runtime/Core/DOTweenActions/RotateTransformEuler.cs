﻿#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class RotateTransformEuler : Rotate
    {
        public override string DisplayName => DisplayNames.RotateTransformEuler;

        [SerializeField] private Transform target;
        [SerializeField] private bool useLocalEulerAngles;
        
        public Transform Target
        {
            get => target;
            set => target = value;
        }
        
        public bool UseLocalEulerAngles
        {
            get => useLocalEulerAngles;
            set => useLocalEulerAngles = value;
        }

        protected override Vector3 GetRotation() => !useLocalEulerAngles ? target.eulerAngles : target.localEulerAngles;
    }
}
#endif