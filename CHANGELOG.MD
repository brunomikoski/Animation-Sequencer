# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

### [Unreleased]

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


