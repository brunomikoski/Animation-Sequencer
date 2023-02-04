#if DOTWEEN_ENABLED
#if TMP_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class TMPAnimation : AnimatedText
    {
        public override Type TargetComponentType => typeof(TMP_Text);
        public override string DisplayName => DisplayNames.AnimateText_TMP;
        
        private TMP_Text textComponent;
        private TMP_Text previousTarget;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (TryToGetComponent(ref textComponent, target)) return null;

            PreviousText = textComponent.text;
            previousTarget = textComponent;
            TweenerCore<string, string, StringOptions> tween = textComponent.DOText(text, duration, richText, scrambleMode);
            return tween;
        }

        public override void Reset()
        {
            if (previousTarget == null) return;
            if (string.IsNullOrEmpty(PreviousText)) return;
            previousTarget.text = PreviousText;
        }
    }
}
#endif
#endif