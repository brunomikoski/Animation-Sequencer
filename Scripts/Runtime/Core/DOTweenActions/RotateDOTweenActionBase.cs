#if DOTWEEN_ENABLED
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
        public bool Local
        {
            get => local;
            set => local = value;
        }

        [SerializeField]
        private RotateMode rotationMode = RotateMode.Fast;
        public RotateMode RotationMode
        {
            get => rotationMode;
            set => rotationMode = value;
        }


        private Transform previousTarget;
        private Quaternion previousRotation;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            TweenerCore<Quaternion, Vector3, QuaternionOptions> localTween;
            if (local)
            {
                previousRotation = target.transform.localRotation;
                localTween = target.transform.DOLocalRotate(GetRotation(), duration, rotationMode);
            }
            else
            {
                previousRotation = target.transform.rotation;
                localTween = target.transform.DORotate(GetRotation(), duration, rotationMode);
            }

            return localTween;
        }

        
        protected abstract Vector3 GetRotation();

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (!local)
                previousTarget.rotation = previousRotation;
            else
                previousTarget.localRotation = previousRotation;
        }
    }
}
#endif