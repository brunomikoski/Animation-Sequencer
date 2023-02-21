#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ParticleSystemStep : AnimationStep
    {
        public override string DisplayName => DisplayNames.ParticleSystemStep;

        [SerializeField]
        private ParticleSystem particleSystem;
        public ParticleSystem ParticleSystem
        {
            get => particleSystem;
            set => particleSystem = value;
        }

        [SerializeField]
        private float duration = 1;
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        [SerializeField]
        private bool stopEmittingWhenOver;
        public bool StopEmittingWhenOver
        {
            get => stopEmittingWhenOver;
            set => stopEmittingWhenOver = value;
        }

        public override void AddTween(Sequence animationSequence)
        {
            animationSequence.SetDelay(Delay);
            animationSequence.AppendCallback(() => { particleSystem.Play(); });
            animationSequence.AppendInterval(duration);
            animationSequence.AppendCallback(FinishParticles);
        }

        public override void Reset()
        {
        }

        private void FinishParticles()
        {
            if (stopEmittingWhenOver) 
                particleSystem.Stop();
        }

        public void SetTarget(ParticleSystem newTarget)
        {
            particleSystem = newTarget;
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (particleSystem != null) 
                display = particleSystem.name;
            
            return $"{index}. Play {display} particle system";
        }
    }
}
#endif