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
    public sealed class TMP_TextDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(TMP_Text);
        public override string DisplayName => "TMP Text";

        [SerializeField]
        private string text;
        public string Text
        {
            get => text;
            set => text = value;
        }

        [SerializeField]
        private bool richText;
        public bool RichText
        {
            get => richText;
            set => richText = value;
        }

        [SerializeField]
        private ScrambleMode scrambleMode = ScrambleMode.None;
        public ScrambleMode ScrambleMode
        {
            get => scrambleMode;
            set => scrambleMode = value;
        }
        
        private TMP_Text tmpTextComponent;
        
        private string previousText;
        private TMP_Text previousTarget;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (tmpTextComponent == null)
            {
                tmpTextComponent = target.GetComponent<TMP_Text>();
                if (tmpTextComponent == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            previousText = tmpTextComponent.text;
            previousTarget = tmpTextComponent;
            TweenerCore<string, string, StringOptions> tween = tmpTextComponent.DOText(text, duration, richText, scrambleMode);
            return tween;
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (string.IsNullOrEmpty(previousText))
                return;

            previousTarget.text = previousText;
        }
    }
}
#endif
#endif