#if DOTWEEN_ENABLED
#if TMP_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationsSequencer
{
    [Serializable]
    public sealed class LegacyTextAnimation : AnimatedText
    {
        public override Type TargetComponentType => typeof(Text);
        public override string DisplayName => DisplayNames.AnimateText_TMP;
        
        private Text textComponent;
        private Text previousTarget;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (!TryToGetComponent(target, out textComponent))
                return null;

            PreviousText = textComponent.text;
            previousTarget = textComponent;
            TweenerCore<string, string, StringOptions> tween = textComponent.DOText(text, duration, richText, scrambleMode);
            return tween;
        }

        public override void Reset()
        {
            if (previousTarget == null) 
                return;
            
            if (string.IsNullOrEmpty(PreviousText))
                return;
            
            previousTarget.text = PreviousText;
        }
    }
}
#endif
#endif