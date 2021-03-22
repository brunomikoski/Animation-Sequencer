using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum InitializeMode
        {
            None,
            PrepareToPlayOnAwake,
            PlayOnAwake
        }
        
        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];
        public AnimationStepBase[] AnimationSteps => animationSteps;

        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private InitializeMode initializeMode = InitializeMode.PlayOnAwake;

        private bool isPlaying;
        public bool IsPlaying => isPlaying;

        private readonly List<AnimationStepBase> stepsToBePlayed = new List<AnimationStepBase>();
        private readonly List<AnimationStepBase> stepsQueue = new List<AnimationStepBase>();
        private bool preparedToPlay;

        public event Action OnSequenceFinishedPlayingEvent;

        private void Awake()
        {
            if (initializeMode == InitializeMode.PlayOnAwake || initializeMode == InitializeMode.PrepareToPlayOnAwake)
            {
                PrepareForPlay();
                if (initializeMode == InitializeMode.PlayOnAwake)
                    Play();
            }
        }

        public void Play()
        {
            PrepareForPlay();
            isPlaying = true;
        }

        public void Rewind()
        {
            for (int i = 0; i < animationSteps.Length; ++i)
            {
                animationSteps[i].Rewind();
            }
        }

        public void Complete()
        {
            if (!isPlaying)
                return;
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase.IsComplete)
                    continue;
                
                animationStepBase.Complete();
            }
            AnimationFinished();
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

        public void PrepareForPlay(bool force = false)
        {
            if (!force && preparedToPlay)
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


        public List<T> GetStepsOfType<T>() where T : AnimationStepBase
        {
            List<T> results = new List<T>();
            for (int i = 0; i < animationSteps.Length; i++)
            {
                if (animationSteps[i] is T castedStep)
                    results.Add(castedStep);
            }

            return results;
        }

        public List<DOTweenActionBase> GetDOTweenActionsThatUseComponent<T>() where T : Component
        {
            List<DOTweenAnimationStep> dotweenSteps = GetStepsOfType<DOTweenAnimationStep>();
            List<DOTweenActionBase> results = new List<DOTweenActionBase>();
            for (int i = 0; i < dotweenSteps.Count; i++)
            {
                DOTweenAnimationStep doTweenAnimationStep = dotweenSteps[i];
                for (int j = 0; j < doTweenAnimationStep.Actions.Length; j++)
                {
                    DOTweenActionBase actionBase = doTweenAnimationStep.Actions[j];
                    if (actionBase.TargetComponentType == typeof(T))
                        results.Add(actionBase);
                }
            }

            return results;
        }
    }
}
