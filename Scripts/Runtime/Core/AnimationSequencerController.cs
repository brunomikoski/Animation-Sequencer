using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public class AnimationSequencerController : MonoBehaviour
    {
        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];
        public AnimationStepBase[] AnimationSteps => animationSteps;

        [SerializeField]
        private float duration;
        public float Duration => duration;

        private bool isPlaying;
        public bool IsPlaying => isPlaying;

        private readonly List<AnimationStepBase> stepsToBePlayed = new List<AnimationStepBase>();
        private readonly List<AnimationStepBase> stepsQueue = new List<AnimationStepBase>();
        private bool preparedToPlay;

        public event Action OnSequenceFinishedPlayingEvent;

        private void Awake()
        {
            PrepareForPlay();
        }

        public void Play()
        {
            PrepareForPlay();
            isPlaying = true;
        }

        public void Stop()
        {
            if (!isPlaying)
                return;
            
            isPlaying = false;
            preparedToPlay = false;
            for (int i = 0; i < animationSteps.Length; i++)
                animationSteps[i].Stop();
        }

        public IEnumerator PlayEnumerator()
        {
            Play();
            while (isPlaying)
                yield return null;
        }

        public void PrepareForPlay()
        {
            if (preparedToPlay)
                return;

            for (int i = 0; i < animationSteps.Length; i++)
                animationSteps[i].PrepareForPlay();
            
            stepsQueue.AddRange(animationSteps);
            preparedToPlay = true;
        }

        private void AnimationFinished()
        {
            Stop();
            OnSequenceFinishedPlayingEvent?.Invoke();
        }

        public void UpdateStep(float deltaTime)
        {
            if (!isPlaying)
                return;

            for (int i = stepsToBePlayed.Count - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = stepsToBePlayed[i];
                animationStepBase.UpdateStep(deltaTime);
                
                if (animationStepBase.IsWaitingOnDelay)
                    continue;

                if (!animationStepBase.IsPlaying)
                {
                    animationStepBase.Play();
                }
                else
                {
                    if (!animationStepBase.IsComplete)
                        continue;

                    animationStepBase.StepFinished();
                    stepsToBePlayed.Remove(animationStepBase);
                }
            }

            if (stepsToBePlayed.Count == 0)
            {
                if (stepsQueue.Count == 0)
                {
                    AnimationFinished();
                }
                else
                {
                    UpdateNextSteps();
                }
            }
        }

        private void UpdateNextSteps()
        {
            for (int i = 0; i < stepsQueue.Count; i++)
            {
                AnimationStepBase possibleStepToPlay = stepsQueue[i];
                if (possibleStepToPlay.FlowType == FlowType.Append && stepsToBePlayed.Count == 0
                    || possibleStepToPlay.FlowType == FlowType.Join && stepsToBePlayed.Count > 0)
                {
                    stepsToBePlayed.Add(possibleStepToPlay);
                }
                else
                    break;
            }

            for (int i = 0; i < stepsToBePlayed.Count; i++)
            {
                AnimationStepBase animationStepBase = stepsToBePlayed[i];
                animationStepBase.WillBePlayed();
                stepsQueue.Remove(animationStepBase);
            }
        }

        private void Update()
        {
            UpdateStep(Time.deltaTime);
        }
    }
}
