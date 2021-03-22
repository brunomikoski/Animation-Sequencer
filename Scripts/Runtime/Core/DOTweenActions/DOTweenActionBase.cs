using System;
#if UNITY_EDITOR
using DG.DOTweenEditor;
#endif
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

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
        [SerializeField]
        protected bool autoKill;
        
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

            tween.SetAutoKill(autoKill);
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

            tween.SetAutoKill(autoKill);
            tween.SetEase(ease);
            tween.SetRelative(isRelative);
            tween.Pause();
            tweener = tween;
        }

        public void Complete()
        {
            tweener?.Complete();
        }

        public void Rewind()
        {
            if (autoKill)
            {
                throw new Exception($"Rewind not possible when autoKill enabled. Tween: {DisplayName}");
            }
            
            tweener?.Rewind();
        }
    }
}
