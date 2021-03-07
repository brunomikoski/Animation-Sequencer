using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class GameObjectAnimationStep : AnimationStepBase
    {
        [SerializeField]
        protected GameObject target;

        [SerializeField]
        protected float duration = 1;
        public override float Duration => duration;

        public override bool CanBePlayed()
        {
            return target != null;
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
        }
    }
}
