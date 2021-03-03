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
        private int loopCount = 0;
        [SerializeField]
        private LoopType loopType;
        [SerializeReference]
        private DOTweenActionBase[] actions;

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

        public override void PrepareForPlay()
        {
            base.PrepareForPlay();
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].PrepareForPlay(target, duration, loopCount, loopType);
            }
        }

        public override string GetDisplayName(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;
            
            return $"{index}. {targetName}: {String.Join(", ", actions.Select(action => action.DisplayName)).Truncate(45)}";
        }
    }
}
