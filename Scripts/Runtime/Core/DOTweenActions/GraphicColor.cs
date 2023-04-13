#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class GraphicColor : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => DisplayNames.GraphicColor;

        [SerializeField]
        private Color color;
        private Color previousColor;

        private Graphic targetGraphic;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out targetGraphic))
                return null;

            previousColor = targetGraphic.color;
            Tweener graphicTween = targetGraphic.DOColor(color, duration);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                graphicTween.OnUpdate(() =>
                {
                    targetGraphic.transform.localScale = new Vector3(1.001f, 1.001f, 1.001f);
                    targetGraphic.transform.localScale = new Vector3(1, 1, 1);
                });
            }
#endif

            return graphicTween;
        }

        public override void Reset()
        {
            if (targetGraphic == null)
                return;
            targetGraphic.color = previousColor;
        }
    }
}

#endif