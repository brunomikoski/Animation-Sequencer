#if DOTWEEN_ENABLED
using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PathTransformPositionsDOTweenActionBase : PathDOTweenActionBase
    {
        [SerializeField]
        private Transform[] pointPositions;
        public Transform[] PointPositions
        {
            get => pointPositions;
            set => pointPositions = value;
        }

        public override string DisplayName => "Move to Path Transform Positions";
        
        protected override Vector3[] GetPathPositions()
        {
            Vector3[] result = new Vector3[pointPositions.Length];

            for (int i = 0; i < pointPositions.Length; i++)
            {
                Transform pointTransform = pointPositions[i];

                if (isLocal)
                    result[i] = pointTransform.localPosition;
                else
                    result[i] = pointTransform.position;
            }

            return result;
        }
    }
}
#endif