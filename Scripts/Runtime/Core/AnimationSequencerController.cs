using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        private enum InitializeMode
        {
            None,
            PlayOnAwake
        }
        
        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];

        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private InitializeMode initializeMode = InitializeMode.PlayOnAwake;
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;
        [SerializeField]
        private bool autoKill = false;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;

        public bool IsPlaying => playingSequence != null && playingSequence.IsPlaying();

        private void Awake()
        {
            if (initializeMode == InitializeMode.PlayOnAwake)
            {
                Play();
            }
        }

        public void Play(Action callback = null)
        {
            playingSequence = GenerateSequence();
            playingSequence.SetUpdate(updateType, timeScaleIndependent);
            playingSequence.OnComplete(() => { callback?.Invoke(); });
            playingSequence.SetAutoKill(autoKill);
            
            playingSequence.Play();
        }

        
        public void TogglePause()
        {
            if (playingSequence == null)
                return;
            
            playingSequence.TogglePause();
        }

        public void Pause()
        {
            if (!IsPlaying)
                return;

            playingSequence.Pause();
        }
        
        public void Resume()
        {
            if (playingSequence == null)
                return;

            playingSequence.Play();
        }

        
        public void Complete(bool withCallbacks = true)
        {
            if (playingSequence == null)
                return;

            playingSequence.Complete(withCallbacks);
        }

        public void Rewind(bool includeDelay = true)
        {
            if (playingSequence == null)
                return;
            
            playingSequence.Rewind(includeDelay);
        }
        
        public void Kill(bool complete = false)
        {
            if (!IsPlaying)
                return;

            playingSequence.Kill(complete);
        }

        public IEnumerator PlayEnumerator()
        {
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        public Sequence GenerateSequence(Sequence sequence = null)
        {
            if (sequence == null)
                sequence = DOTween.Sequence();
            
            for (int i = 0; i < animationSteps.Length; i++)
            {
                animationSteps[i].AddTweenToSequence(sequence);
            }
            return sequence;
        }
    }
}
