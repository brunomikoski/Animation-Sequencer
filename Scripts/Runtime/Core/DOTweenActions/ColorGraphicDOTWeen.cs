using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ColorGraphicDOTWeen : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Color Graphic";

        [SerializeField]
        private Color color;
        public Color Color
        {
            get => color;
            set => color = value;
        }

        private Graphic targetGraphic;
        private Color previousColor;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetGraphic == null)
            {
                targetGraphic = target.GetComponent<Graphic>();
                if (targetGraphic == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            previousColor = targetGraphic.color;
            TweenerCore<Color, Color, ColorOptions> graphicTween = targetGraphic.DOColor(color, duration);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                graphicTween.OnUpdate(() =>
                {
                    targetGraphic.enabled = false;
                    targetGraphic.enabled = true;
                });
            }
#endif
            
            return graphicTween;
        }
        
        public override void ResetToInitialState()
        {
            if (targetGraphic == null)
                return;

            targetGraphic.color = previousColor;
        }
    }
}

