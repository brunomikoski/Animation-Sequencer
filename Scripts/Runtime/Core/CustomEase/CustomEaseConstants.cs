#if DOTWEEN_ENABLED
using DG.Tweening;

namespace BrunoMikoski.AnimationSequencer
{
    public partial class CustomEase
    {
        public static CustomEase Linear => new CustomEase(Ease.Linear);
        public static CustomEase InSine => new CustomEase(Ease.InSine);
        public static CustomEase OutSine => new CustomEase(Ease.OutSine);
        public static CustomEase InOutSine => new CustomEase(Ease.InOutSine);
        public static CustomEase InQuad => new CustomEase(Ease.InQuad);
        public static CustomEase OutQuad => new CustomEase(Ease.OutQuad);
        public static CustomEase InOutQuad => new CustomEase(Ease.InOutQuad);
        public static CustomEase InCubic => new CustomEase(Ease.InCubic);
        public static CustomEase OutCubic => new CustomEase(Ease.OutCubic);
        public static CustomEase InOutCubic => new CustomEase(Ease.InOutCubic);
        public static CustomEase InQuart => new CustomEase(Ease.InQuart);
        public static CustomEase OutQuart => new CustomEase(Ease.OutQuart);
        public static CustomEase InOutQuart => new CustomEase(Ease.InOutQuart);
        public static CustomEase InQuint => new CustomEase(Ease.InQuint);
        public static CustomEase OutQuint => new CustomEase(Ease.OutQuint);
        public static CustomEase InOutQuint => new CustomEase(Ease.InOutQuint);
        public static CustomEase InExpo => new CustomEase(Ease.InExpo);
        public static CustomEase OutExpo => new CustomEase(Ease.OutExpo);
        public static CustomEase InOutExpo => new CustomEase(Ease.InOutExpo);
        public static CustomEase InCirc => new CustomEase(Ease.InCirc);
        public static CustomEase OutCirc => new CustomEase(Ease.OutCirc);
        public static CustomEase InOutCirc => new CustomEase(Ease.InOutCirc);
        public static CustomEase InElastic => new CustomEase(Ease.InElastic);
        public static CustomEase OutElastic => new CustomEase(Ease.OutElastic);
        public static CustomEase InOutElastic => new CustomEase(Ease.InOutElastic);
        public static CustomEase InBack => new CustomEase(Ease.InBack);
        public static CustomEase OutBack => new CustomEase(Ease.OutBack);
        public static CustomEase InOutBack => new CustomEase(Ease.InOutBack);
        public static CustomEase InBounce => new CustomEase(Ease.InBounce);
        public static CustomEase OutBounce => new CustomEase(Ease.OutBounce);
        public static CustomEase InOutBounce => new CustomEase(Ease.InOutBounce);
        public static CustomEase Flash => new CustomEase(Ease.Flash);
        public static CustomEase InFlash => new CustomEase(Ease.InFlash);
        public static CustomEase OutFlash => new CustomEase(Ease.OutFlash);
        public static CustomEase InOutFlash => new CustomEase(Ease.InOutFlash);
    }
}
#endif