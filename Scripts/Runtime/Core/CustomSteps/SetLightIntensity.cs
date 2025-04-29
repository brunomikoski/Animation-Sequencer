using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    public class SetLightIntensity : AnimationStepBase
    {
        public override string DisplayName => "Set Light Intensity";
        [SerializeField]
        Light light;
        [SerializeField]
        float duration = 1;
        [SerializeField]
        float targetIntensity;

        float originValue = 0;

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            originValue = light.intensity;
            var tweener = DOTween.To(() => light.intensity, (x) => light.intensity = x, targetIntensity, duration);
            

            if (FlowType == FlowType.Join)
                animationSequence.Join(tweener);
            else
                animationSequence.Append(tweener);
        }

        public override void ResetToInitialState()
        {
            light.intensity = originValue;
        }
    }
}