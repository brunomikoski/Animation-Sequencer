# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased
### Changed

## [0.5.2]
### Changed
- Fixed Animation not saving progress changes
- The last renaming caused a lot of unwanted issues, I'm reverting this until I figure out exactly how to handle this


## [0.5.1]
### Changed
- Fixed issue when previewing animation and having the `AutoHideWhenPreviewing` enabled would cause the Steps not expand on the second play
- Fixed issue where FadeCanvas didn't work while previewing on Editor on Unity 2021

## [0.5.0]
### Changed
- Added support to control AnimationSequencer though regular unity animations, in both runtime / playtime
- Fixed issue with color tweens no working on the editor > 2021
- Added a settings menu on `Edit/Preferences/AnimationSequencer`
- You can now disable the auto foldout of the steps while previewing on the Settings

## [0.4.0]
### Changed
- Fixed Unity 2020 compability

## [0.3.9]
### Changed
- Fixed Unity 2021 support
- Added a new setup to help dealing with projects when DOTween is not available. The AnimationSequencer will try to find the DOTween asmdef and add a scripting defining symbol `DOTWEEN_ENABLED`. Keep in mind that if you remove DOTWEEN you will have to remove the asmdef by hand. 

## [0.3.8]
### Changed
- Fixed Unity 2020 foldout issues
- Added other small null checks

## [0.3.5]
### Changed
- Fixed Infinite Loops not working with sequence, well its not fixed but I'm using the workaround described here: https://github.com/Demigiant/dotween/issues/92
- Exposed all the Steps/Actions changeable values as properties so they can be changed by code.
- Added new `TryGetStepAtIndex` and `TryGetActionAtIndex` to be able to get and manipulate values from code.

## [0.3.4]
### Changed
- Fixed InvokeCallbackStep adding the callback to the parent sequence
- Fixed not allowing -1 as loop number for the DOTweenStep

## [0.3.3]
### Changed
- Fixed issue in which the Animation Step's Delay was being applied to each Action individually, this was causing them to execute sequentially rather than simultaneously.
- Fixed an issue in which joined Steps after an Invoke Callback Step with a delay were incorrectly delayed.
- Added general defaults, previously defaults could only be set for Actions.
- Worked around a Unity bug in which updating the colour of a Graphic does not cause any visual change outside of PlayMode (affects "Color Graphic" and "Fade Graphic" actions).
- Improved Step naming for Invoke Callback to include the called function name and also support multiple callbacks.
- Added RectTransformSizeDOTweenAction to tween the sizeDelta of a Graphic.
- Fixed a bug in which exiting the prefab staging view with an active preview would cause a null ref and failure to reset the animation. This was due to the timing of EditorWIndow.OnDisable (it is is called after the prefab objects have already been destroyed) and has been fixed by hooking into PrefabStage.prefabSaving.
- Added runtime playback speed.


## [0.3.2]
### Changed
- Fixed bug with delay and multiple sequences
- Expand the steps panel if started playing with the panel open.~~~~

## [0.3.1]
### Changed
- Added `AutoplayType`, this allows animations to trigger in either `Awake` or `OnEnable` (defaults to Awake to retain backwards compatibility)
- Fixed issues with backward playback in Editor Preview Tool and at runtime.
- Fixed callbacks not firing correctly when playing backward.
- If `AutoplayType` is OnEnable, then `OnDisable` will kill the tweens and reset the animated objects to their initial states, this ensures that they are in the right state for the Tweens to be regenerated the next time the animation is enabled.
- TimeScale slider in Preview Tool can now be used before playback commences.
- Exposed getters for `PlayOnAwake` and `PauseOnAwake`, these can be used for asserts to catch incorrect setup of `AniamtionSequenceControllers` from the code's perspective.
- Fixed an issue with "Move To Anchored Position" when using the IsRelative flag. It must be set when calling tween.From(isRelative) for it to work.
- Fixed delays not working for Callback steps.
- Fixed potential null refs in various DOTweenActions.
- Allowed various parts of AnimationSequenceController and AnimationSequenceControllerCustomEditor to be overridden.
- Play On Awake and Pause On Awake now alter their inspector label dependent on the selected AutoplayType.
- Fixed a bug with the completion callbacks passed to Play, PlayForward and PlayBackwards, old callbacks were executing for subsequent plays. All listeners are now removed each time any playback function is executed.
- Fixed typo in PlayForward bool.

Thanks for @nindim for all the improvements and bug fixes

## [0.3.0]
### Changed
- Exposed `AnimationSequencerController` methods to be virtual, so they can be extended.
- Removed the `sealed` from `AnimationSequencerControllerCustomEditor` so allow custom editors as well.
- Refactor the Play / Forward / Backward setup, to be easier to understand and use
- Added `autoKill` setting on the sequence settings, to allow back and forward play easily;

## [0.2.8]
### Changed
- Added the SetProgress to be able to set the position of the sequencer by code.
- Set the `Play` method to virtual so can be overwritten 
- Added [DOTWeen from the UPM](https://openupm.com/packages/com.demigiant.dotween/) package dependency 
- Fixed Timescale slider greyed out before editor was playing
- Added direction as a parameter of the `Play` method, so you can trigger to play backwards and forward by code.

## [0.2.7]
### Changed
- [Fixed issue](https://github.com/brunomikoski/Animation-Sequencer/pull/29) when `OnStart` event was been triggered inside the `OnComplete` event. [JohnDesley](https://github.com/JohnDesley)
- Automatically hide the Steps Panel when previewing to increase performance.

## [0.2.6]
### Changed
 - Removed the `Play` from the playtype, now is only `Backward` and `Forward` and `Forward` is the default one
 - Implement manual reset of editor changes to avoid issues while previewing animation and stop
 - Refactored the preview controllers to allow all the controllers to be able to play the animation when needed
 - Improved inspector performance 

### Added
 - Implemented a bunch of feedback from [nindim](https://github.com/nindim)

## [0.2.5]
### Changed
- Replaced TypeUtility for [Unity TypeCache](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/TypeCache.html)

## [0.2.4]
### Changed
- Fixed issue of Join sequences 

## [0.2.3]
### Changed
- Fixed issue with Append / Join steps

## [0.2.2]
### Changed
- Updated Readme
- Update Delay implementation 
- Removed Duration variable.
- Small cleanups

## [0.2.1]
### Added
- Added ColorGraphic DOTween step.
- While previewing if you press play again with a finished tween, will rewind it first
- Added loops settings for the entire sequence.
- More warnings and help information boxes while previewing
- Added Sequence Settings Panel to control DOTween Sequence Settings only 
 
### Changed
- Disabled infinity loops to be added to DOTween Steps, [More Information](https://github.com/brunomikoski/Animation-Sequencer/issues/19#issuecomment-895668338)
- Cached components on Tween Step to improve performance.
- The Sequence will be killed if the animation controller is destroyed

## [0.2.0]
Thanks for all the [suggestions](https://github.com/brunomikoski/Animation-Sequencer/issues/16) @nindim
### Changed
- Refactored the system to use Sequences as core, instead of proprietary implementation
- Changed the preview controls
- Fixed chain of sequences to be played properly on editor.
- Changed how the DOTweenEditorPreview works

### Added
- Added UnityEvents callback to be exposed on the editor.
- Added TimeScale slider to control preview
- Added progress slider to scrubing through the sequence
- Added new exposed settings (`UpdateType` `TimeScale Independent`, `Auto Kill`)
- Added new play type
- Added `AnimationControllerDefaults` where you can set some default values and behaviours

### Requirement
- Requires latest DOTween version (v1.2.632)

## [0.1.5]
### Changed
- Fixed one issue that prevent you from playing the same animation twice on editor
- Limited the `-1 (Infinity lopps)` on editor playback, this was causing some issues so will show a Warning and limit to 3 loops if was set to -1
- Fixed issue when trying to complete the Sequence on editor, now only Stop is available, and will complete all the sequence.


## [0.1.4]
### Added
- Added the `Invoke Callback Step` that uses `Unity Events` to trigger callbacks inside one sequence! Thanks @VladmirCSouza

## [0.1.3]
### Added
- Added Step Finished / Step Begin events that can be subscribed on the `Animation Controller`

### Changed
- Fixed issue when a custom step would have a null `DisplayName`

## [0.1.2]
### Added
- Added `Rewind()` functionality, thanks @qwe321

### Changed
- Fixed DOTween editor been used on runtime platform

## [0.1.1]
### Added 
- Added Complete method for the Sequence, so you can force one sequence to finished instantaneous
- Added the `force` parameter on the `PrepareToPlay` method, to execute the preparation of a step even if is already generated
- Added initialization settings for the `Animation Sequencer`

### Fixed
- Fixed issue where the duration of a sequence composed of multiple sequences would not sore the correct duration

## [0.1.0]
### Added 
- First initial working version

[0.5.2]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.5.2
[0.5.1]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.5.1
[0.5.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.5.0
[0.4.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.4.0
[0.3.9]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.9
[0.3.8]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.8
[0.3.6]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.6
[0.3.5]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.5
[0.3.4]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.4
[0.3.3]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.3
[0.3.2]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.2
[0.3.1]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.1
[0.3.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.3.0
[0.2.8]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.8
[0.2.7]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.7
[0.2.6]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.6
[0.2.5]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.5
[0.2.4]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.4
[0.2.3]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.3
[0.2.2]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.2
[0.2.1]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.1
[0.2.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.2.0
[0.1.5]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.5
[0.1.4]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.4
[0.1.3]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.3
[0.1.2]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.2
[0.1.1]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.1
[0.1.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.0
