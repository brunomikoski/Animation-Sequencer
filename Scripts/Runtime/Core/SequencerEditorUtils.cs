#if DOTWEEN_ENABLED
namespace BrunoMikoski.AnimationSequencer
{
    public static class SequencerEditorUtils
    {
        public static string NameOfAnimationSteps => nameof(Sequencer.animationSteps);
        public static string NameOfUpdateType => nameof(Sequencer.updateType);
        public static string NameOfAutoPlayMode => nameof(Sequencer.autoplayMode);
        public static string NameOfPlayType => nameof(Sequencer.playType);
        public static string NameOfLoopType => nameof(Sequencer.loopType);
        public static string NameOfTimeScaleIndependent => nameof(Sequencer.timeScaleIndependent);
        public static string NameOfStartPaused => nameof(Sequencer.startPaused);
        public static string NameOfAutoKill => nameof(Sequencer.autoKill);
        public static string NameOfPlaybackSpeed => nameof(Sequencer.playbackSpeed);
        public static string NameOfLoops => nameof(Sequencer.loops);
        public static string NameOfProgress => nameof(Sequencer.progress);
        public static string NameOfOnStartEvent => nameof(Sequencer.onStartEvent);
        public static string NameOfOnFinishedEvent => nameof(Sequencer.onFinishedEvent);
        public static string NameOfOnProgressEvent => nameof(Sequencer.onProgressEvent);
    }
}
#endif