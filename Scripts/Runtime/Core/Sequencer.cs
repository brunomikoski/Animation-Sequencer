#if DOTWEEN_ENABLED
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class Sequencer : MonoBehaviour
    {
        #region Local Enums

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

        #endregion

        #region Serialized Fields

        [SerializeReference] private AnimationStep[] animationSteps = Array.Empty<AnimationStep>();
        
        [SerializeField] private UpdateType updateType = UpdateType.Normal;
        [SerializeField] private AutoplayType autoplayMode = AutoplayType.Awake;
        [SerializeField] protected PlayType playType = PlayType.Forward;
        [SerializeField] private LoopType loopType = LoopType.Restart;
        
        [SerializeField] private bool timeScaleIndependent;
        [SerializeField] protected bool startPaused;
        [SerializeField] private bool autoKill = true;
        
        [SerializeField] private float playbackSpeed = 1f;
        [SerializeField] private int loops;
        
        [SerializeField] private UnityEvent onStartEvent = new UnityEvent();
        [SerializeField] private UnityEvent onFinishedEvent = new UnityEvent();
        [SerializeField] private UnityEvent onProgressEvent = new UnityEvent();
        
        [SerializeField, Range(0, 1)] private float progress = -1;
        
        #endregion

        #region Privates

        private Sequence playingSequence;
        private PlayType playTypeInternal = PlayType.Forward;
#if UNITY_EDITOR
        private bool requiresReset = false;
#endif

        #endregion

        #region Public Properties

        public Sequence PlayingSequence => playingSequence;
        public float PlaybackSpeed => playbackSpeed;
        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();

        public UpdateType UpdateType
        {
            get => updateType;
            set => updateType = value;
        }

        public AutoplayType AutoplayMode
        {
            get => autoplayMode;
            set => autoplayMode = value;
        }

        public PlayType TypeOfPlay
        {
            get => playType;
            set => playType = value;
        }

        public bool TimeScaleIndependent
        {
            get => timeScaleIndependent;
            set => timeScaleIndependent = value;
        }

        public bool StartPaused
        {
            get => startPaused;
            set => startPaused = value;
        }

        public bool AutoKill
        {
            get => autoKill;
            set => autoKill = value;
        }

        public int Loops
        {
            get => loops;
            set => loops = value;
        }

        #region Events

        public UnityEvent OnProgressEvent => onProgressEvent;
        public UnityEvent OnStartEvent
        {
            get => onStartEvent;
            protected set => onStartEvent = value;
        }
        public UnityEvent OnFinishedEvent
        {
            get => onFinishedEvent;
            protected set => onFinishedEvent = value;
        }

        #endregion

        #endregion

        #region Variable Name Properties

        public static string NameOfAnimationSteps => nameof(animationSteps);
        public static string NameOfUpdateType => nameof(updateType);
        public static string NameOfAutoPlayMode => nameof(autoplayMode);
        public static string NameOfPlayType => nameof(playType);
        public static string NameOfLoopType => nameof(loopType);
        public static string NameOfTimeScaleIndependent => nameof(timeScaleIndependent);
        public static string NameOfStartPaused => nameof(startPaused);
        public static string NameOfAutoKill => nameof(autoKill);
        public static string NameOfPlaybackSpeed => nameof(playbackSpeed);
        public static string NameOfLoops => nameof(loops);
        public static string NameOfProgress => nameof(progress);
        public static string NameOfOnStartEvent => nameof(onStartEvent);
        public static string NameOfOnFinishedEvent => nameof(onFinishedEvent);
        public static string NameOfOnProgressEvent => nameof(onProgressEvent);

        #endregion

        #region Unity Event Functions

        protected virtual void Awake()
        {
            progress = -1;
            if (autoplayMode == AutoplayType.Awake) Autoplay();
        }
        
        protected virtual void OnEnable()
        {
            if (autoplayMode != AutoplayType.OnEnable) Autoplay();
        }
        
        private void Update()
        {
            if (progress == -1.0f) return;
            SetProgress(progress);
        }
        
        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayType.OnEnable) return;
            if (playingSequence == null) return;

            ClearPlayingSequence();
            // Reset the object to its initial state so that if it is re-enabled the start values are correct for
            // regenerating the Sequence.
            ResetToInitialState();
        }

        protected virtual void OnDestroy()
        {
            ClearPlayingSequence();
        }

        #endregion

        #region Playing

        private void Autoplay()
        {
            Play();
            if (startPaused) playingSequence.Pause();
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
            
            if (onCompleteCallback != null) onFinishedEvent.AddListener(onCompleteCallback.Invoke);

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
            if (playingSequence == null) Play();
            
            playTypeInternal = PlayType.Forward;
            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null) onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            if (resetFirst) SetProgress(0);
            
            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null) Play();
            
            playTypeInternal = PlayType.Backward;
            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null) onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            if (completeFirst) SetProgress(1);
            
            playingSequence.PlayBackwards();
        }

        #endregion

        #region Progress And Time

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (playingSequence == null) Play();

            float duration = playingSequence.Duration();
            float finalProgress = Mathf.Clamp01(seconds / duration);
            SetProgress(finalProgress, andPlay);
        }
        
        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            targetProgress = Mathf.Clamp01(targetProgress);
            
            if (playingSequence == null) Play();

            playingSequence.Goto(targetProgress, andPlay);
        }

        #endregion

        #region Play Controls

        public virtual void TogglePause()
        {
            playingSequence?.TogglePause();
        }

        public virtual void Pause()
        {
            if (!IsPlaying) return;
            playingSequence.Pause();
        }

        public virtual void Resume()
        {
            playingSequence?.Play();
        }

        #endregion
        
        #region Controls

        public virtual void Complete(bool withCallbacks = true)
        {
            playingSequence?.Complete(withCallbacks);
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            playingSequence?.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!IsPlaying) return;
            playingSequence.Kill(complete);
        }

        #endregion

        public virtual IEnumerator PlayEnumerator()
        {
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        #region Generation

        public virtual Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();
            
            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward) onStartEvent.Invoke();
                else onFinishedEvent.Invoke();
            });
            
            foreach (var step in animationSteps)
                step.AddTween(sequence);

            sequence.SetTarget(this);
            sequence.SetAutoKill(autoKill);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnUpdate(() => { onProgressEvent.Invoke(); });
            
            // See comment above regarding bookending via AppendCallback.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward) onFinishedEvent.Invoke();
                else onStartEvent.Invoke();
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

        #endregion

        #region Cleaning

        public virtual void ResetToInitialState()
        {
            progress = -1.0f;
            for (int i = animationSteps.Length - 1; i >= 0; i--)
                animationSteps[i].Reset();
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(this);
            DOTween.Kill(playingSequence);
            playingSequence = null;
        }

        #endregion

        #region Only Editor

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
        #endregion

        #region Generics

        public bool TryGetStepAtIndex<T>(int index, out T result) where T : AnimationStep
        {
            if (index < 0 || index > animationSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = animationSteps[index] as T;
            return result != null;
        }

        #endregion
    }
}
#endif