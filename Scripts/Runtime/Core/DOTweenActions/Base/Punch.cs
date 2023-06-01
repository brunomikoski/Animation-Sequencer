#if DOTWEEN_ENABLED
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public abstract class Punch : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        protected Vector3 punchMagnitude;
        public Vector3 PunchMagnitude
        {
            get => punchMagnitude;
            set => punchMagnitude = value;
        }
        
        [SerializeField]
        protected int vibrato = 10;
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }
        
        [SerializeField]
        protected float elasticity = 1f;
        public float Elasticity
        {
            get => elasticity;
            set => elasticity = value;
        }

        protected Transform PreviousTarget;
    }
}
#endif
