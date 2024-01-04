using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public sealed class AnimationSequencerSettings : ScriptableObjectForPreferences<AnimationSequencerSettings>
    {
        [SerializeField]
        private bool autoHideStepsWhenPreviewing = true;

        [SerializeField]
        private bool drawTimingsWhenPreviewing = true;
        
        public bool AutoHideStepsWhenPreviewing => autoHideStepsWhenPreviewing;
        public bool DrawTimingsWhenPreviewing   => drawTimingsWhenPreviewing;

        [SettingsProvider]
        private static SettingsProvider SettingsProvider()
        {
            return CreateSettingsProvider("Animation Sequencer", null);
        }
    }
}
