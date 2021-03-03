using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class RotateDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        public override string DisplayName => "Punch Scale";

        [SerializeField]
        private bool local;
        [SerializeField]
        private RotateMode rotationMode = RotateMode.Fast;

        public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
        {
            TweenerCore<Quaternion, Vector3, QuaternionOptions> localTween;
            if (local)
                localTween = target.transform.DOLocalRotate(GetRotation(), duration, rotationMode);
            else
                localTween = target.transform.DORotate(GetRotation(), duration, rotationMode);

            
            SetTween(localTween, loops, loopType);
        }

        protected abstract Vector3 GetRotation();
    }
}
