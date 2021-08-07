using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CreateAssetMenu(menuName = "Animation Sequencer/Create Animation Sequencer Default", fileName = "AnimationControllerDefaults")]
    public sealed class AnimationControllerDefaults : EditorDefaultResourceSingleton<AnimationControllerDefaults>
    {
        [SerializeField]
        private CustomEase defaultEasing = CustomEase.InOutQuad;
        public CustomEase DefaultEasing => defaultEasing;

        [SerializeField]
        private bool preferUsingPreviousActionEasing = true;
        public bool PreferUsingPreviousActionEasing => preferUsingPreviousActionEasing;

        [SerializeField]
        private DOTweenActionBase.AnimationDirection defaultDirection = DOTweenActionBase.AnimationDirection.To;
        public DOTweenActionBase.AnimationDirection DefaultDirection => defaultDirection;
        
        [SerializeField]
        private bool preferUsingPreviousDirection = true;
        public bool PreferUsingPreviousDirection => preferUsingPreviousDirection;
        
        
        [SerializeField]
        private bool useRelative = false;
        public bool UseRelative => useRelative;
        [SerializeField]
        private bool preferUsingPreviousRelativeValue = true;
        public bool PreferUsingPreviousRelativeValue => preferUsingPreviousRelativeValue;
        
        
    }
}
