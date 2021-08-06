using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        private enum InitializeMode
        {
            None,
            PlayOnAwake,
            PauseOnAwake
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

        [SerializeField]
        private UnityEvent onStartEvent = new UnityEvent();
        public UnityEvent OnStartEvent => onStartEvent;
        [SerializeField]
        private UnityEvent onFinishedEvent = new UnityEvent();
        public UnityEvent OnFinishedEvent => onFinishedEvent;
        [SerializeField]
        private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;
        

        private void Awake()
        {
            if (initializeMode == InitializeMode.PlayOnAwake || initializeMode == InitializeMode.PauseOnAwake)
            {
                Play();
                if (initializeMode == InitializeMode.PauseOnAwake)
                    playingSequence.Pause();
            }
        }

        public void Play(Action callback = null)
        {
            DOTween.Kill(this);
            playingSequence?.Kill();

            if (callback != null) 
                onStartEvent.AddListener(callback.Invoke);

            playingSequence = GenerateSequence();
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

            sequence.SetTarget(this);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnComplete(() =>
            {
                onStartEvent.Invoke();
            });
            sequence.OnUpdate(() =>
            {
                onProgressEvent.Invoke();
            });
            sequence.OnComplete(() =>
            {
                onFinishedEvent.Invoke();
            });
            
            sequence.SetAutoKill(autoKill);
            
            return sequence;
        }
    }
}
