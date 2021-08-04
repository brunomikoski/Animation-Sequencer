using System;
using System.Collections;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        private enum InitializeMode
        {
            None,
            PlayOnAwake
        }
        
        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];
        public AnimationStepBase[] AnimationSteps => animationSteps;

        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private InitializeMode initializeMode = InitializeMode.PlayOnAwake;
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;

        private Sequence playingSequence;
        public bool IsPlaying => playingSequence != null && playingSequence.IsPlaying();

        private void Awake()
        {
            if (initializeMode == InitializeMode.PlayOnAwake)
            {
                Play();
            }
        }

        public void Play(Action callback = null)
        {
            playingSequence = GenerateSequence();
            playingSequence.SetUpdate(updateType, timeScaleIndependent);
            playingSequence.OnComplete(() => { callback?.Invoke(); });
            
#if UNITY_EDITOR
            DOTweenEditorPreview.PrepareTweenForPreview(playingSequence, false, false, false);
#endif
            playingSequence.Play();
        }

        public void Complete()
        {
            if (!IsPlaying)
                return;

            DOTween.Complete(playingSequence, true);
        }

        public void Rewind()
        {
            if (playingSequence == null)
                return;
            
            DOTween.Rewind(playingSequence);
        }

        public void Stop()
        {
            if (!IsPlaying)
                return;

            DOTween.Kill(playingSequence, true);
        }

        public IEnumerator PlayEnumerator()
        {
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        public Sequence GenerateSequence()
        {
            Sequence animationSequence = DOTween.Sequence();
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                Tween tween = animationStepBase.GenerateTween();
                if (animationStepBase.FlowType == FlowType.Append)
                {
                    animationSequence.Append(tween);
                }
                else
                {
                    animationSequence.Join(tween);
                }
            }

            return animationSequence;
        }
    }
}
