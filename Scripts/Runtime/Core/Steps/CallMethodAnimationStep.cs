using System;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    public class CallMethodAnimationStep : AnimationStepBase
    {
        [SerializeField]
        private UnityEvent methodToCall;
        
        public override string DisplayName => "Method Call Step";
        
        public override float Duration { get; }

        public override bool CanBePlayed() => true;
        
        public override string GetDisplayNameForEditor(int index)
        {
            string persistentTargetNames = String.Empty;
            for (int i = 0; i < methodToCall.GetPersistentEventCount(); i++)
            {
                persistentTargetNames = $"{String.Join(", ", methodToCall.GetPersistentTarget(i).name).Truncate(45)}";
            }
            
            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }

        public override void Complete()
        {
            methodToCall?.Invoke();
        }
    }
}
