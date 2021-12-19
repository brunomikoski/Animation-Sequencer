using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using UnityEngine;

public class RectTransformSizeDOTweenAction : DOTweenActionBase
{
    public override Type TargetComponentType => typeof(RectTransform);
    public override string DisplayName => "RectTransform Size";

    [SerializeField]
    private Vector2 sizeDelta;
    [SerializeField]
    private AxisConstraint axisConstraint;

    private RectTransform previousTarget;
    private Vector2 previousSize;
    
    protected override Tweener GenerateTween_Internal(GameObject target, float duration)
    {
        previousTarget = target.transform as RectTransform;
        previousSize = previousTarget.sizeDelta;
        var tween = previousTarget.DOSizeDelta(sizeDelta, duration);
        tween.SetOptions(axisConstraint);

        return tween;
    }

    public override void ResetToInitialState()
    {
        if (previousTarget == null)
            return;
            
        previousTarget.sizeDelta = previousSize;
    }
}
