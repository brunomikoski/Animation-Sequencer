using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        private const int EDITOR_MAX_LOOPS = 3;
        public override string DisplayName => "Tween Target";
        [SerializeField]
        private int loopCount;
        public int LoopCount
        {
            get
            {
                if (Application.isPlaying)
                    return loopCount;
                return loopCount == -1 ? EDITOR_MAX_LOOPS : loopCount;
            }
        }
        [SerializeField]
        private LoopType loopType;
        [SerializeReference]
        private DOTweenActionBase[] actions;
        public DOTweenActionBase[] Actions => actions;

        public override float Duration
        {
            get
            {
                if (LoopCount == 0)
                    return duration;
                if (LoopCount == -1)
                    return float.MaxValue;
                return duration * LoopCount;
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
                Debug.LogError($"Null target on {typeof(DOTweenAnimationStep)}, ignoring it.");
                return;
            }

            if (!Application.isPlaying)
            {
                if (loopCount == -1)
                    Debug.LogWarning($"Infinity Loops are not editor friendly, changing loops into {EDITOR_MAX_LOOPS} for Editor Mode only. Target:{target}");
            }
            
            for (int i = 0; i < actions.Length; i++)
            {
                float targetDuration = duration;
                if (!Application.isPlaying)
                {
                    if (Mathf.Approximately(targetDuration, 0))
                    {
                        //Temporary fix, waiting for a fix from DOTween side: https://github.com/Demigiant/dotween/issues/494
                        targetDuration = float.Epsilon;
                    }
                }
                
                actions[i].CreateTween(target, targetDuration, LoopCount, loopType);
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
