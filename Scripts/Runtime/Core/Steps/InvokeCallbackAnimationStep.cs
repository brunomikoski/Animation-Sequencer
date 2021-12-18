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
        private UnityEvent callback = new UnityEvent();
        
        public override string DisplayName => "Invoke Callback Step";
        

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(Delay);
            sequence.AppendCallback(() => callback.Invoke());

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
            string persistentTargetNames = String.Empty;
            for (int i = 0; i < callback.GetPersistentEventCount(); i++)
            {
                if (callback.GetPersistentTarget(i) == null)
                    continue;
                
                persistentTargetNames = $"{string.Join(", ", callback.GetPersistentTarget(i).name).Truncate(45)}";
            }
            
            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}
