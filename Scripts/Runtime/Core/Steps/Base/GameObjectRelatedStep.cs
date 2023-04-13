#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public abstract class GameObjectRelatedStep : AnimationStep
    {
        [SerializeField]
        protected GameObject target;
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        protected float duration = 1;
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public static string NameOfTarget => nameof(target);

        public void SetTarget(GameObject target) => this.target = target;
    }
}
#endif