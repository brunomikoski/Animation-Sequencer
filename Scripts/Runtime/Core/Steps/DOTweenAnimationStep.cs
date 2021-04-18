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

        public override float Duration
        {
            get
            {
                if (loopCount == 0)
                    return duration;
                if (loopCount == -1)
                    return float.MaxValue;
                return duration * loopCount;
            }
        }

        public override void Play()
        {
            base.Play();
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Play();
            }
        }

        public override void Rewind()
        {
            base.Rewind();
            for (int i = 0; i < actions.Length; ++i)
            {
                actions[i].Rewind();
            }
        }

        public override void Complete()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Complete();
            }
        }

        public override void PrepareForPlay()
        {
            base.PrepareForPlay();
            if (target == null)
            {
                Debug.LogError($"{target} is null on {typeof(DOTweenAnimationStep)}");
                return;
            }
            
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].CreateTween(target, duration, loopCount, loopType);
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
