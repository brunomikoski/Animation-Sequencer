#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class CallbackStep : AnimationStep
    {
        public override string DisplayName => DisplayNames.CallbackStep;
        
        [SerializeField] 
        private UnityEvent callback = new UnityEvent();
        public UnityEvent Callback
        {
            get => callback;
            set => callback = value;
        }

        public override void AddTween(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(Delay);
            sequence.AppendCallback(() => callback.Invoke());
            
            if (FlowType == FlowType.Append) 
                animationSequence.Append(sequence);
            else 
                animationSequence.Join(sequence);
        }

        public override void Reset() { }

        public override string GetDisplayNameForEditor(int index)
        {
            string[] persistentTargetNamesArray = new string[callback.GetPersistentEventCount()];
            int persistentEventCount = callback.GetPersistentEventCount();
            for (int i = 0; i < persistentEventCount; i++)
            {
                if (callback.GetPersistentTarget(i) == null) 
                    continue;
                
                if (string.IsNullOrWhiteSpace(callback.GetPersistentMethodName(i))) 
                    continue;
                
                persistentTargetNamesArray[i] = $"{callback.GetPersistentTarget(i).name}.{callback.GetPersistentMethodName(i)}()";
            }
            
            string persistentTargetNames = $"{string.Join(", ", persistentTargetNamesArray).Truncate(45)}";
            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}
#endif