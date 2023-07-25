#if DOTWEEN_ENABLED
using System;
using System.Collections;
#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;
#endif
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
        
        public enum AutoplayType
        {
            Awake,
            OnEnable,
            Nothing
        }

        [SerializeReference]
        private AnimationStepBase[] animationSteps = Array.Empty<AnimationStepBase>();
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;
        [SerializeField]
        private AutoplayType autoplayMode = AutoplayType.Awake;
        [SerializeField]
        protected bool startPaused;
        [SerializeField]
        private float playbackSpeed = 1f;
        public float PlaybackSpeed => playbackSpeed;
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
#if UNITY_EDITOR
        private bool requiresReset = false;
#endif

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();
        public bool IsPaused => playingSequence != null && playingSequence.IsActive() && !playingSequence.IsPlaying();

        [SerializeField, Range(0, 1)] 
        private float progress = -1;

        protected virtual void Awake()
        {
            progress = -1;
            if (autoplayMode != AutoplayType.Awake)
                return;
            
            Autoplay();
        }
        
        protected virtual void OnEnable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            Autoplay();
        }

        private void Autoplay()
        {
            Play();
            if (startPaused)
                playingSequence.Pause();
        }
        
        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
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
        
        public virtual void Play()
        {
            Play(null);
        }

        public virtual void Play(Action onCompleteCallback)
        {
            playTypeInternal = playType;
            
            ClearPlayingSequence();
            
            onFinishedEvent.RemoveAllListeners();
            
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

        public virtual void PlayForward(bool resetFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();
            
            playTypeInternal = PlayType.Forward;
            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            
            if (resetFirst)
                SetProgress(0);
            
            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();
            
            playTypeInternal = PlayType.Backward;
            onFinishedEvent.RemoveAllListeners();

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
            float finalProgress = Mathf.Clamp01(seconds / duration);
            SetProgress(finalProgress, andPlay);
        }
        
        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            targetProgress = Mathf.Clamp01(targetProgress);
            
            if (playingSequence == null)
                Play();

            playingSequence.Goto(targetProgress, andPlay);
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
                {
                    targetLoops = 10;
                    Debug.LogWarning("Infinity sequences on editor can cause issues, using 10 loops while on editor.");
                }
            }

            sequence.SetLoops(targetLoops, loopType);
            sequence.timeScale = playbackSpeed;
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            progress = -1.0f;
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
  
        public void SetAutoplayMode(AutoplayType autoplayType)
        {
            autoplayMode = autoplayType;
        }
        
        public void SetPlayOnAwake(bool targetPlayOnAwake)
        {
        }
        
        public void SetPauseOnAwake(bool targetPauseOnAwake)
        {
            startPaused = targetPauseOnAwake;
        }
        
        public void SetTimeScaleIndependent(bool targetTimeScaleIndependent)
        {
            timeScaleIndependent = targetTimeScaleIndependent;
        }
        
        public void SetPlayType(PlayType targetPlayType)
        {
            playType = targetPlayType;
        }
        
        public void SetUpdateType(UpdateType targetUpdateType)
        {
            updateType = targetUpdateType;
        }
        
        public void SetAutoKill(bool targetAutoKill)
        {
            autoKill = targetAutoKill;
        }
        
        public void SetLoops(int targetLoops)
        {
            loops = targetLoops;
        }

        private void Update()
        {
            if (progress == -1.0f)
                return;
            
            SetProgress(progress);
        }

#if UNITY_EDITOR
        // Unity Event Function called when component is added or reset.
        private void Reset()
        {
            requiresReset = true;
        }

        // Used by the CustomEditor so it knows when to reset to the defaults.
        public bool IsResetRequired()
        {
            return requiresReset;
        }

        // Called by the CustomEditor once the reset has been completed 
        public void ResetComplete()
        {
            requiresReset = false;
        }
#endif
        public bool TryGetStepAtIndex<T>(int index, out T result) where T : AnimationStepBase
        {
            if (index < 0 || index > animationSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = animationSteps[index] as T;
            return result != null;
        }

#if UNITASK_ENABLED
        public async UniTask PlayAsync(CancellationToken cancellationTokenSource = default)
        {
            if (cancellationTokenSource == default)
                cancellationTokenSource = this.GetCancellationTokenOnDestroy();
            
            await PlayEnumerator().ToUniTask(PlayerLoopTiming.Update, cancellationTokenSource);
        }
#endif
    }
}
#endif
