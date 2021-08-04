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
        public DOTweenActionBase[] Actions => actions;

        public override float Duration => duration;

        public override Tween GenerateTween()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetAutoKill(false);
            for (int i = 0; i < actions.Length; i++)
            {
                Tween generateTween = actions[i].GenerateTween(target, duration);
                generateTween.SetDelay(Delay);
                sequence.Join(generateTween);
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
