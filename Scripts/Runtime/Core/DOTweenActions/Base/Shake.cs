using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class Shake : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Transform);
        
        [SerializeField] protected Vector3 strength;
        [SerializeField] protected int vibrato = 10;
        [SerializeField] protected float randomness = 90;
        [SerializeField] protected bool fadeout = true;

        protected Transform PreviousTarget;
        
        public Vector3 Strength
        {
            get => strength;
            set => strength = value;
        }
        
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }
        
        public float Randomness
        {
            get => randomness;
            set => randomness = value;
        }
        
        public bool Fadeout
        {
            get => fadeout;
            set => fadeout = value;
        }
    }
}