using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [InitializeOnLoad]
    public static class AnimationSequencerSetupHelper
    {
        private static string SCRIPTING_DEFINE_SYMBOL = "DOTWEEN_ENABLED";
        private static string DOTWEEN_ASSEMBLY_NAME = "DOTween.Modules";
        static AnimationSequencerSetupHelper()
        {
            Assembly[] availableAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);

            bool foundDOTween = false;
            for (int i = availableAssemblies.Length - 1; i >= 0; i--)
            {
                if (availableAssemblies[i].name.IndexOf(DOTWEEN_ASSEMBLY_NAME, StringComparison.Ordinal) > -1)
                {
                    foundDOTween = true;
                    break;
                }
            }

            if (foundDOTween)
            {
                AddScriptingDefineSymbol();
            }
            else
            {
                RemoveScriptingDefineSymbol();
                Debug.LogWarning("No DOTween found, animation sequencer will be disabled until DOTween setup is complete and asmdef files are created");
            }
        }

        private static void AddScriptingDefineSymbol()
        {
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (scriptingDefineSymbols.Contains(SCRIPTING_DEFINE_SYMBOL))
                return;

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, $"{scriptingDefineSymbols};{SCRIPTING_DEFINE_SYMBOL}");

            Debug.Log($"Adding {SCRIPTING_DEFINE_SYMBOL} for {EditorUserBuildSettings.selectedBuildTargetGroup}");
        }

        private static void RemoveScriptingDefineSymbol()
        {
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (!scriptingDefineSymbols.Contains(SCRIPTING_DEFINE_SYMBOL))
                return;

            scriptingDefineSymbols = scriptingDefineSymbols.Replace(SCRIPTING_DEFINE_SYMBOL, string.Empty);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, $"{scriptingDefineSymbols}");
        }
    }
}