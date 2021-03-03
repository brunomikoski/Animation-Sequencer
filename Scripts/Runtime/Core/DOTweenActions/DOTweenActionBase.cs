using System;
using DG.DOTweenEditor;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class DOTweenActionBase
    {
        public enum AnimationDirection
        {
            To, 
            From
        }
        
        public abstract string DisplayName { get; }
        
        [SerializeField]
        protected AnimationDirection direction;
        [SerializeField]
        protected CustomEase ease = CustomEase.InOutCirc;
        [SerializeField]
        protected bool isRelative;
        
        protected Tweener tweener;

        public abstract bool CreateTween(GameObject target, float duration, int loops, LoopType loopType);

        public virtual Type TargetComponentType { get; } 

        public void Play()
        {
            if (tweener == null)
                return;
            
            if (Application.isPlaying)
            {
                tweener.Play();
            }
            else
            {
#if UNITY_EDITOR
                DOTweenEditorPreview.PrepareTweenForPreview(tweener);
#endif
            }
        }

        protected void SetTween(Tweener tween, int loops, LoopType loopType)
        {
            if (direction == AnimationDirection.From)
                tween.From();

            tween.SetEase(ease);
            tween.SetRelative(isRelative);
            tween.SetLoops(loops, loopType);
            tween.Pause();
            tweener = tween;
        }

        protected void SetTween(TweenerCore<Vector3,Path,PathOptions> tween)
        {
            if (direction == AnimationDirection.From)
                tween.From();

            tween.SetEase(ease);
            tween.SetRelative(isRelative);
            tween.Pause();
            tweener = tween;
        }
    }
}
