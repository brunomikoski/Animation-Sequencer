using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField]
        private int loopCount;
        
        [SerializeField]
        private LoopType loopType;
        [SerializeReference]
        private DOTweenActionBase[] actions;

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            animationSequence.AppendInterval(Delay);
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < actions.Length; i++)
            {
                Tween tween = actions[i].GenerateTween(target, duration);
                tween.SetLoops(loopCount, loopType);
                sequence.Join(tween);
            }
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);

        }

        public override void ResetToInitialState()
        {
            for (int i = actions.Length - 1; i >= 0; i--)
            {
                actions[i].ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;
            
            return $"{index}. {targetName}: {String.Join(", ", actions.Select(action => action.DisplayName)).Truncate(45)}";
        }
    }
}
