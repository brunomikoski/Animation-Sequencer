using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlayParticleSystemAnimationStep : AnimationStepBase
    {
        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private float duration = 1;

        public override float Duration => duration;

        [SerializeField]
        private bool stopEmittingWhenOver;

        public override string DisplayName => "Play Particle System";

        public ParticleSystem Target => particleSystem;

        public override bool CanBePlayed()
        {
            return particleSystem != null;
        }

        public override void Play()
        {
            base.Play();
            particleSystem.Play();
        }

        public override void StepFinished()
        {
            base.StepFinished();
            if (stopEmittingWhenOver)
            {
                particleSystem.Stop();
            }
        }

        public void SetTarget(ParticleSystem newTarget)
        {
            particleSystem = newTarget;
        }

        public override string GetDisplayName(int index)
        {
            string display = "NULL";
            if (particleSystem != null)
                display = particleSystem.name;
            return $"{index}. Play {display} particle system";
        }
    }
}
