#if DOTWEEN_ENABLED
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CreateAssetMenu(menuName = "Animation Sequencer/Create Animation Sequencer Default", fileName = "AnimationControllerDefaults")]
    public sealed class AnimationControllerDefaults : EditorDefaultResourceSingleton<AnimationControllerDefaults>
    {
        [SerializeField]
        private CustomEase defaultEasing = CustomEase.InOutQuad;
        public CustomEase DefaultEasing => defaultEasing;

        [SerializeField]
        private bool preferUsingPreviousActionEasing = true;
        public bool PreferUsingPreviousActionEasing => preferUsingPreviousActionEasing;

        [SerializeField]
        private SequencerAnimationBase.AnimationDirection defaultDirection = SequencerAnimationBase.AnimationDirection.To;
        public SequencerAnimationBase.AnimationDirection DefaultDirection => defaultDirection;
        
        [SerializeField]
        private bool preferUsingPreviousDirection = true;
        public bool PreferUsingPreviousDirection => preferUsingPreviousDirection;
        
        [SerializeField]
        private bool useRelative = false;
        public bool UseRelative => useRelative;
        
        [SerializeField]
        private bool preferUsingPreviousRelativeValue = true;
        public bool PreferUsingPreviousRelativeValue => preferUsingPreviousRelativeValue;
        
        [SerializeField]
        private Sequencer.AutoplayType autoplayMode = Sequencer.AutoplayType.Awake;
        public Sequencer.AutoplayType AutoplayMode => autoplayMode;
        
        [SerializeField]
        private bool playOnAwake = false;
        public bool PlayOnAwake => playOnAwake;
        
        [SerializeField]
        private bool pauseOnAwake = false;
        public bool PauseOnAwake => pauseOnAwake;
        
        [SerializeField]
        private bool timeScaleIndependent = false;
        public bool TimeScaleIndependent => timeScaleIndependent;
        
        [SerializeField]
        private Sequencer.PlayType playType = Sequencer.PlayType.Forward;
        public Sequencer.PlayType PlayType => playType;
        
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        public UpdateType UpdateType => updateType;
        
        [SerializeField]
        private bool autoKill = true;
        public bool AutoKill => autoKill;
        
        [SerializeField]
        private int loops = 0;
        public int Loops => loops;
    }
}
#endif