#if DOTWEEN_ENABLED
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
        public bool IsLocal
        {
            get => isLocal;
            set => isLocal = value;
        }

        [SerializeField]
        private Color gizmoColor;
        public Color GizmoColor
        {
            get => gizmoColor;
            set => gizmoColor = value;
        }

        [SerializeField]
        private int resolution = 10;
        public int Resolution
        {
            get => resolution;
            set => resolution = value;
        }

        [SerializeField]
        private PathMode pathMode = PathMode.Full3D;
        public PathMode PathMode
        {
            get => pathMode;
            set => pathMode = value;
        }

        [SerializeField]
        private PathType pathType = PathType.CatmullRom;
        public PathType PathType
        {
            get => pathType;
            set => pathType = value;
        }

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
            if (previousTarget == null)
                return;
            
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
#endif