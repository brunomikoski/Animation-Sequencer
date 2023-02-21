using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class Shake : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        protected Vector3 strength;
        public Vector3 Strength
        {
            get => strength;
            set => strength = value;
        }

        
        [SerializeField]
        protected int vibrato = 10;
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }

        
        [SerializeField]
        protected float randomness = 90;
        public float Randomness
        {
            get => randomness;
            set => randomness = value;
        }

       
        [SerializeField]
        protected bool fadeout = true;
        public bool Fadeout
        {
            get => fadeout;
            set => fadeout = value;
        }
        
        protected Transform PreviousTarget;


    }
}