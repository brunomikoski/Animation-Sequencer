#if DOTWEEN_ENABLED
using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    internal static class DOTweenProxy
    {
        private static FieldInfo sequencedObjects;
        private static FieldInfo sequencedPosition;
        private static FieldInfo sequencedEndPosition;

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            try
            {
                sequencedObjects = typeof(Sequence)
                    .GetField("_sequencedObjs", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                sequencedPosition = typeof(ABSSequentiable)
                    .GetField("sequencedPosition",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                sequencedEndPosition = typeof(ABSSequentiable)
                    .GetField("sequencedEndPosition",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static (float start, float end)[] GetTimings(Sequence sequence, AnimationStepBase[] steps)
        {
            try
            {
                var timings = new (float start, float end)[steps.Length];
                var objs = GetSequencedObjects(sequence);
                var duration = sequence.Duration();

                // take into account two SequenceCallbacks
                // that added in AnimationSequencerController.GenerateSequence method
                if (objs.Count != steps.Length + 2)
                {
                    Debug.LogError("Sequenced object count mismatch for sequence");
                    return null;
                }

                for (var index = 0; index < Math.Min(timings.Length, objs.Count); index++)
                {
                    var obj = objs[index + 1]; // +1 for skip Start Callback
                    var step = steps[index];

                    var start = GetSequencedStartPosition(obj);
                    var end = GetSequencedEndPosition(obj);

                    start += step.Delay;

                    timings[index] = (start / duration, end / duration);
                }

                return timings;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private static List<ABSSequentiable> GetSequencedObjects(Sequence sequence)
        {
            return sequencedObjects?.GetValue(sequence) as List<ABSSequentiable>;
        }

        private static float GetSequencedStartPosition(ABSSequentiable sequenced)
        {
            return (float) sequencedPosition.GetValue(sequenced);
        }

        private static float GetSequencedEndPosition(ABSSequentiable sequenced)
        {
            return (float) sequencedEndPosition.GetValue(sequenced);
        }
    }
}
#endif