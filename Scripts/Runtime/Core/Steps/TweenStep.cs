#if DOTWEEN_ENABLED
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class TweenStep : GameObjectRelatedStep
    {
        public override string DisplayName => DisplayNames.TweenStep;

        [SerializeReference]
        private SequencerAnimationBase[] actions;
        public SequencerAnimationBase[] Actions
        {
            get => actions;
            set => actions = value;
        }
        
        [SerializeField]
        private int loopCount;
        public int LoopCount
        {
            get => loopCount;
            set => loopCount = value;
        }

        [SerializeField]
        private LoopType loopType;
        public LoopType LoopType
        {
            get => loopType;
            set => loopType = value;
        }

        public static string NameOfActions => nameof(actions);
        public static string NameOfLoopCount => nameof(loopCount);
        public static string NameOfLoopType => nameof(loopType);

        public override void AddTween(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < actions.Length; i++)
            {
                Tween tween = actions[i].GenerateTween(target, duration);
                if (i == 0) 
                    tween.SetDelay(Delay);
                
                sequence.Join(tween);
            }

            sequence.SetLoops(loopCount, loopType);

            if (FlowType == FlowType.Join) 
                animationSequence.Join(sequence);
            else 
                animationSequence.Append(sequence);
        }

        public override void Reset()
        {
            for (int i = actions.Length - 1; i >= 0; i--)
                actions[i].Reset();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null) 
                targetName = target.name;
            return $"{index}. {targetName}: " +
                   $"{string.Join(", ", actions.Select(action => action.DisplayName)).Truncate(45)}";
        }

        public bool TryGetActionAtIndex<T>(int index, out T result) where T : SequencerAnimationBase
        {
            if (index < 0 || index > actions.Length - 1)
            {
                result = null;
                return false;
            }

            result = actions[index] as T;
            return result != null;
        }
    }
}
#endif