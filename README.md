# Animation Sequencer
[![openupm](https://img.shields.io/npm/v/com.brunomikoski.uimanager?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.brunomikoski.uimanager/) 

[![](https://img.shields.io/github/followers/brunomikoski?label=Follow&style=social)](https://github.com/brunomikoski) [![](https://img.shields.io/twitter/follow/brunomikoski?style=social)](https://twitter.com/brunomikoski)




I LOVE Tween, I love DOTween even more! But having to wait for a recompilation every time you tweak a single value on some animation it's frustrating! Even more complicated is properly have to create the entire animation on your head and having to wait until you reach your animation to see what you have done! That's why I created the Animation Sequencer, it is **HEAVILY INSPIRED** (~~cloned~~) by [Space Ape](https://spaceapegames.com/) *UIAnimSequencer*.



## Features
- Allow you to create a complex sequence of Tweens/Actions and play on Editor Mode!
- User Friendly interface with a lot of customization
- Easy to extend with project specific actions
- Chain sequences and control entire animated windows with a single interface
- Can be used on both UI or any other object on the project.
- Searchable actions allowing fast interactions and updates
- Can be used for any type of Objects, UI or anything you want! 

## How to use?
- Animation Sequencer rely on DOTween for now, so it a requirement that you have DOTween on your project with properly created ASMdef for it (Created by the DOTween setup panel)
- Add the Animation Sequencer to any GameObject and start your animation! 

## FAQ

<details>
<summary>How to create a custom Step for you project?</summary> 
Lets say you want to create a new action to play a specific sound from your sound manager on the game, you just need to extend the `AnimationStepBase`
```c#
[Serializable]
public class PlayAudioClipAnimationStep : AnimationStepBase
{
    [SerializeField]
    private AudioClip audioClip;

    //Duration of this step, in this case will return the length of the clip.
    public override float Duration => audioClip.length;
    //This is the name that will be displayed on the + button on the Animation Sequencer
    public override string DisplayName => "Play Audio Clip";

    //Here is actually the action of this step
    public override void Play()
    {
        base.Play();
        AudioManager.Play(audioClip);
    }
}
```  
</details>

### I have my own DOTween extensions, can I use that? 
Absolutely! The same as the step, you can add any new DOTween action by extending `DOTweenActionBase`. In order to avoid any performance issues all the tweens are created on the PrepareToPlay method on Awake, and are paused.

```c#
[Serializable]
public sealed class ChangeMaterialStrengthDOTweenAction : DOTweenActionBase
{
    public override string DisplayName => "Change Material Strength";
        
    public override Type TargetComponentType => typeof(Renderer);

    [SerializeField, Range(0,1)]
    private float materialStrength = 1;

    public override void PrepareForPlay(GameObject target, float duration, int loops, LoopType loopType)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null)
            return;

        TweenerCore<float, float, FloatOptions> materialTween = renderer.sharedMaterial.DOFloat(materialStrength, "Strength", duration);
        
        SetTween(materialTween, loops, loopType);
    }
}
```

![custom-tween-action](https://user-images.githubusercontent.com/600419/109774425-3965a280-7bf8-11eb-9bfe-90b0be8b8617.gif)



## System Requirements
Unity 2018.4.0 or later versions


## Installation

### OpenUPM
The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.brunomikoski.animationsequencer
```

### Manifest
You can also install via git URL by adding this entry in your **manifest.json**
```
"com.brunomikoski.uimanager": "https://github.com/brunomikoski/AnimationSequencer.git"
```

### Unity Package Manager
```
from Window->Package Manager, click on the + sign and Add from git: https://github.com/brunomikoski/AnimationSequencer.git
```
