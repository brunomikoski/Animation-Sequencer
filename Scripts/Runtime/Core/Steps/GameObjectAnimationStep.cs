using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class GameObjectAnimationStep : AnimationStepBase
    {
        [SerializeField]
        protected GameObject target;
        public GameObject Target => target;

        [SerializeField]
        protected float duration = 1;
        public override float Duration => duration;

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
        }
    }
}
