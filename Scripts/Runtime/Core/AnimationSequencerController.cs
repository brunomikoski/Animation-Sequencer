﻿using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum PlayType
        {
            Forward,
            Backward
        }
        
        public enum AutoplayMode
        {
            Awake,
            OnEnable
        }

        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;
        [SerializeField]
        private AutoplayMode autoplayMode = AutoplayMode.Awake;
        public AutoplayMode AutoPlayMode { get { return autoplayMode;} set { autoplayMode = value; } }
        [SerializeField]
        protected bool playOnAwake;
        public bool PlayOnAwake { get { return playOnAwake;} }
        [SerializeField]
        protected bool pauseOnAwake;
		public bool PauseOnAwake { get { return pauseOnAwake;} }
        [SerializeField]
        protected PlayType playType = PlayType.Forward;
        [SerializeField]
        private int loops = 0;
        [SerializeField]
        private LoopType loopType = LoopType.Restart;
        [SerializeField]
        private bool autoKill = true;

        [SerializeField]
        private UnityEvent onStartEvent = new UnityEvent();
        public UnityEvent OnStartEvent { get { return onStartEvent;} protected set {onStartEvent = value;}}
        [SerializeField]
        private UnityEvent onFinishedEvent = new UnityEvent();
        public UnityEvent OnFinishedEvent { get { return onFinishedEvent;} protected set {onFinishedEvent = value;}}
        [SerializeField]
        private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;
        private PlayType playTypeInternal = PlayType.Forward;

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();
        public bool IsPaused => playingSequence != null && playingSequence.IsActive() && !playingSequence.IsPlaying();

        protected virtual void Awake()
        {
            if (autoplayMode != AutoplayMode.Awake)
                return;

            Autoplay();
        }
        
        protected virtual void OnEnable()
        {
            if (autoplayMode != AutoplayMode.OnEnable)
                return;

            Autoplay();
        }

        private void Autoplay()
        {
            if (playOnAwake)
            {
                Play();
                if (pauseOnAwake)
                    playingSequence.Pause();
            }
        }
        
        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayMode.OnEnable)
                return;
            
            if (playingSequence == null)
                return;

            ClearPlayingSequence();
            // Reset the object to its initial state so that if it is re-enabled the start values are correct for
            // regenerating the Sequence.
            ResetToInitialState();
        }

        protected virtual void OnDestroy()
        {
            ClearPlayingSequence();
        }

        public virtual void Play(Action onCompleteCallback = null)
        {
            playTypeInternal = playType;
            
            ClearPlayingSequence();
            
            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            playingSequence = GenerateSequence();

            switch (playTypeInternal)
            {
                case PlayType.Backward:
                    playingSequence.PlayBackwards();
                    break;

                case PlayType.Forward:
                    playingSequence.PlayForward();
                    break;

                default:
                    playingSequence.Play();
                    break;
            }
        }

        public virtual void PlayForward(bool restFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();
            
            playTypeInternal = PlayType.Forward;

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            
            if (restFirst)
                SetProgress(0);
            
            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();
            
            playTypeInternal = PlayType.Backward;

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            
            if (completeFirst)
                SetProgress(1);
            
            playingSequence.PlayBackwards();
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (playingSequence == null)
                Play();

            float duration = playingSequence.Duration();
            float progress = Mathf.Clamp01(seconds / duration);
            SetProgress(progress, andPlay);
        }
        
        public virtual void SetProgress(float progress, bool andPlay = true)
        {
            progress = Mathf.Clamp01(progress);
            
            if (playingSequence == null)
                Play();

            playingSequence.Goto(progress, andPlay);
        }

        public virtual void TogglePause()
        {
            if (playingSequence == null)
                return;

            playingSequence.TogglePause();
        }

        public virtual void Pause()
        {
            if (!IsPlaying)
                return;

            playingSequence.Pause();
        }

        public virtual void Resume()
        {
            if (playingSequence == null)
                return;

            playingSequence.Play();
        }


        public virtual void Complete(bool withCallbacks = true)
        {
            if (playingSequence == null)
                return;

            playingSequence.Complete(withCallbacks);
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (playingSequence == null)
                return;

            playingSequence.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!IsPlaying)
                return;

            playingSequence.Kill(complete);
        }

        public virtual IEnumerator PlayEnumerator()
        {
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        public virtual Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();
            
            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });

            for (int i = 0; i < animationSteps.Length; i++)
            {
                animationSteps[i].AddTweenToSequence(sequence);
            }

            sequence.SetTarget(this);
            sequence.SetAutoKill(autoKill);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnUpdate(() =>
            {
                onProgressEvent.Invoke();
            });
            // See comment above regarding bookending via AppendCallback.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onFinishedEvent.Invoke();
                }
                else
                {
                    onStartEvent.Invoke();
                }
            });

            int targetLoops = loops;

            if (!Application.isPlaying)
            {
                if (loops == -1)
                    targetLoops = 10;
            }

            sequence.SetLoops(targetLoops, loopType);
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                animationSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(this);
            DOTween.Kill(playingSequence);
            playingSequence = null;
        }
    }
}
