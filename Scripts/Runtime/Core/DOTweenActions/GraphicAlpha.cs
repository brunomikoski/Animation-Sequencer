#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class GraphicAlpha : SequencerAnimationBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => DisplayNames.GraphicAlpha;

        [SerializeField]
        private float alpha;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        private Graphic targetGraphic;
        private float previousAlpha;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out targetGraphic))
                return null;

            previousAlpha = targetGraphic.color.a;
            Tweener graphicTween = targetGraphic.DOFade(alpha, duration);

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

            Color color = targetGraphic.color;
            color.a = previousAlpha;
            targetGraphic.color = color;
        }
    }
}
#endif