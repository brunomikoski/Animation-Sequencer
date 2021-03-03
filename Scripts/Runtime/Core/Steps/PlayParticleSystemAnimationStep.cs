using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlayParticleSystemAnimationStep : AnimationStepBase
    {
        [SerializeField]
        private ParticleSystem[] particleSystems;

        [SerializeField]
        private float duration = 1;

        public override float Duration => duration;

        [SerializeField]
        private bool stopEmittingWhenOver;

        public override string DisplayName => "Play Particle System";

        public override void Play()
        {
            base.Play();
            for (int i = 0; i < particleSystems.Length; i++)
                particleSystems[i].Play(false);
        }

        public override void StepFinished()
        {
            base.StepFinished();
            if (stopEmittingWhenOver)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                    particleSystems[i].Stop(false);
            }
        }

        public override string GetDisplayName(int index)
        {
            string display = "NULL";
            if (particleSystems != null)
                display = particleSystems.Length.ToString();
            return $"{index}. Play {display} particle systems";
        }
    }
}
