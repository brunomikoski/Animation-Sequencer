using System;
using DG.Tweening;
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

        public override Tween GenerateTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                particleSystem.Play();
            });
            sequence.AppendInterval(duration);
            sequence.AppendCallback(FinishParticles);
            return sequence;
        }

        private void FinishParticles()
        {
            if (stopEmittingWhenOver)
            {
                particleSystem.Stop();
            }
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
