using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    public sealed class AnimationSequencerSettings : ScriptableObjectForPreferences<AnimationSequencerSettings>
    {
        [SerializeField]
        private bool autoHideStepsWhenPreviewing = true;
        public bool AutoHideStepsWhenPreviewing => autoHideStepsWhenPreviewing;

        [SettingsProvider]
        private static SettingsProvider SettingsProvider()
        {
            return CreateSettingsProvider("Preferences/Animation Sequencer", null);
        }
    }
}
