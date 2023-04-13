#if DOTWEEN_ENABLED
namespace BrunoMikoski.AnimationsSequencer
{
    public static class AnimationSequenceEditorUtils
    {
        public static string NameOfAnimationSteps => nameof(AnimationSequence.animationSteps);
        public static string NameOfUpdateType => nameof(AnimationSequence.updateType);
        public static string NameOfAutoPlayMode => nameof(AnimationSequence.autoplayMode);
        public static string NameOfPlayType => nameof(AnimationSequence.playType);
        public static string NameOfLoopType => nameof(AnimationSequence.loopType);
        public static string NameOfTimeScaleIndependent => nameof(AnimationSequence.timeScaleIndependent);
        public static string NameOfStartPaused => nameof(AnimationSequence.startPaused);
        public static string NameOfAutoKill => nameof(AnimationSequence.autoKill);
        public static string NameOfPlaybackSpeed => nameof(AnimationSequence.playbackSpeed);
        public static string NameOfLoops => nameof(AnimationSequence.loops);
        public static string NameOfProgress => nameof(AnimationSequence.progress);
        public static string NameOfOnStartEvent => nameof(AnimationSequence.onStartEvent);
        public static string NameOfOnFinishedEvent => nameof(AnimationSequence.onFinishedEvent);
        public static string NameOfOnProgressEvent => nameof(AnimationSequence.onProgressEvent);
    }
}
#endif