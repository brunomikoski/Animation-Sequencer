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
        private UnityEvent callback;
        
        public override string DisplayName => "Invoke Callback Step";
        
        public override float Duration => 0;

        public override Tween GenerateTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => callback.Invoke());
            return sequence;
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
