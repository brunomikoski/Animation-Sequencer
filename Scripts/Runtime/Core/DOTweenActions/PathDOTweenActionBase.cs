using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class PathDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        protected bool isLocal;
        [SerializeField]
        private Color gizmoColor;
        [SerializeField]
        private int resolution = 10;
        [SerializeField]
        private PathMode pathMode = PathMode.Full3D;
        [SerializeField]
        private PathType pathType = PathType.CatmullRom;

        private Transform previousTarget;
        private Vector3 previousPosition;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            TweenerCore<Vector3, Path, PathOptions> tween;

            previousTarget = target.transform;
            if (!isLocal)
            {
                tween = target.transform.DOPath(GetPathPositions(), duration, pathType, pathMode, resolution, gizmoColor);
                previousPosition = target.transform.position;
            }
            else
            {
                tween = target.transform.DOLocalPath(GetPathPositions(), duration, pathType, pathMode, resolution, gizmoColor);
                previousPosition = target.transform.localPosition;
            }

            return tween;
        }


        protected abstract Vector3[] GetPathPositions();
        public override void ResetToInitialState()
        {
            if (isLocal)
            {
                previousTarget.transform.localPosition = previousPosition;
            }
            else
            {
                previousTarget.transform.position = previousPosition;
            }
        }
    }
}
