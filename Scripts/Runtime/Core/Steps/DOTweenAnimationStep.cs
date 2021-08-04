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
        public int LoopCount => loopCount;
        
        [SerializeField]
        private LoopType loopType;
        [SerializeReference]
        private DOTweenActionBase[] actions;

        public override float Duration => duration;

        public override Tween GenerateTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(Delay);
            for (int i = 0; i < actions.Length; i++)
            {
                sequence.Join(actions[i].GenerateTween(target, duration));
            }

            sequence.SetLoops(loopCount, loopType);

            return sequence;
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
