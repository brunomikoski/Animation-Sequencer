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
        private enum PlayType
        {
            Forward,
            Backward
        }
        
        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];

        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;
        [SerializeField]
        private bool playOnAwake;
        [SerializeField]
        private bool pauseOnAwake;
        [SerializeField]
        private PlayType playType = PlayType.Forward;

        [SerializeField]
        private int loops = 0;
        [SerializeField]
        private LoopType loopType = LoopType.Restart;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();

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
            if (playOnAwake)
            {
                Play();
                if (pauseOnAwake)
                    playingSequence.Pause();
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            playingSequence?.Kill();
        }

        public void Play(Action onCompleteCallback = null)
        {
            DOTween.Kill(this);
            DOTween.Kill(playingSequence);

            if (onCompleteCallback != null) 
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            playingSequence = GenerateSequence();
            
            switch (playType)
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

        public Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                
                animationStepBase.AddTweenToSequence(sequence);
            }

            sequence.SetTarget(this);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnComplete(() =>
            {
                if (playType == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });
            sequence.OnUpdate(() =>
            {
                onProgressEvent.Invoke();
            });
            sequence.OnComplete(() =>
            {
                if (playType == PlayType.Forward)
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

        public void ResetToInitialState()
        {
            for (var i = 0; i < animationSteps.Length; i++)
            {
                animationSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(playingSequence);
            playingSequence = null;
        }
    }
}
