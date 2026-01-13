#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class InvokeCallbackAnimationStep : AnimationStepBase
    {
        [SerializeField]
        [Tooltip("Controls whether this callback should be invoked during calls to AnimationSequencerController's Complete() function")]
        private bool invokeOnSkipToEnd = false;
        [SerializeField]
        private UnityEvent callback = new UnityEvent();
        public UnityEvent Callback
        {
            get => callback;
            set => callback = value;
        }

        public override string DisplayName => "Invoke Callback";

        public bool AllowCallbacks { get; set; } = true;

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(Delay);
            sequence.AppendCallback(() =>
            {
                if (AllowCallbacks && !IsSkippingToEnd || invokeOnSkipToEnd)
                {
                    callback.Invoke();
                }
            });
            
            if (FlowType == FlowType.Append)
                animationSequence.Append(sequence);
            else
                animationSequence.Join(sequence);
        }

        public override void ResetToInitialState()
        {
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string[] persistentTargetNamesArray = new string[callback.GetPersistentEventCount()];
            for (int i = 0; i < callback.GetPersistentEventCount(); i++)
            {
                if (callback.GetPersistentTarget(i) == null)
                    continue;
                
                if (string.IsNullOrWhiteSpace(callback.GetPersistentMethodName(i)))
                    continue;
                
                persistentTargetNamesArray[i] = $"{callback.GetPersistentTarget(i).name}.{callback.GetPersistentMethodName(i)}()";
            }
            
            var persistentTargetNames = $"{string.Join(", ", persistentTargetNamesArray).Truncate(45)}";
            
            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}
#endif