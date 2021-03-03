using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnimationStepBase
    {
        [SerializeField]
        private float delay;
        [SerializeField]
        private FlowType flowType;

        public float Delay => delay;
        public abstract float Duration { get; }
        public FlowType FlowType => flowType;
        
        public abstract string DisplayName { get; }
        
        private bool isPlaying;
        public bool IsPlaying => isPlaying;
        
        private float animationTime;
        private float stepTime;

        public bool IsWaitingOnDelay => stepTime < delay;
        public bool IsComplete => stepTime >= delay + Duration;

        public virtual void Play()
        {
            isPlaying = true;
        }

        public virtual void PrepareForPlay()
        {
            animationTime = 0;
            stepTime = 0;
        }

        public abstract bool CanBePlayed();

        public virtual string GetDisplayName(int index)
        {
            return $"{index}. {this}";
        }

        public virtual void StepFinished()
        {
            isPlaying = false;
        }

        public void UpdateStep(float deltaTime)
        {
            stepTime += deltaTime;
            if (isPlaying)
                animationTime += deltaTime;
        }

        public void WillBePlayed()
        {
            stepTime = 0;
        }

        public virtual void Stop()
        {
            isPlaying = false;
        }
    }
}
