#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class GameObjectRelatedStep : AnimationStep
    {
        [SerializeField] protected GameObject target;
        [SerializeField] protected float duration = 1;

        public static string NameOfTarget => nameof(target);

        public GameObject Target
        {
            get => target;
            set => target = value;
        }
        
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public void SetTarget(GameObject target) => this.target = target;
    }
}
#endif