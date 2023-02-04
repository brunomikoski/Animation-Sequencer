using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class Punch : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Transform);
        
        [SerializeField] protected Vector3 punchMagnitude;
        [SerializeField] protected int vibrato = 10;
        [SerializeField] protected float elasticity = 1f;

        public Vector3 PunchMagnitude
        {
            get => punchMagnitude;
            set => punchMagnitude = value;
        }

        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }

        public float Elasticity
        {
            get => elasticity;
            set => elasticity = value;
        }
        
        protected Transform PreviousTarget;
    }
}