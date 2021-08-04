using System;
using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum InitializeMode
        {
            None,
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

        private Sequence playingSequence;
        public bool IsPlaying => playingSequence != null && playingSequence.IsPlaying();

        public event Action OnSequenceFinishedPlayingEvent;

        public event Action<int> OnAnimationStepBeginEvent;
        public event Action<int> OnAnimationStepFinishedEvent;

        private void Awake()
        {
            if (initializeMode == InitializeMode.PlayOnAwake)
            {
                Play();
            }
        }

        public void Play()
        {
            Stop();
            
            playingSequence = GenerateSequence();
#if UNITY_EDITOR
            DOTweenEditorPreview.PrepareTweenForPreview(playingSequence, true, false, false);
#endif
            playingSequence.Play();
        }

        public void Complete()
        {
            if(!IsPlaying)
                return;
            
            DOTween.Complete(playingSequence, true);
        }

        public void Stop()
        {
            if (!IsPlaying)
                return;

            DOTween.Kill(playingSequence, true);
        }

        public IEnumerator PlayEnumerator()
        {
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        public Sequence GenerateSequence()
        {
            Sequence animationSequence = DOTween.Sequence();
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                Tween tween = animationStepBase.GenerateTween();
                if (animationStepBase.FlowType == FlowType.Append)
                {
                    animationSequence.Append(tween);
                }
                else
                {
                    animationSequence.Join(tween);
                }
            }

            return animationSequence;
        }
    }
}
