# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [0.1.3]
### Added
- you can know subscribe to events from the SequenceController for when one Step Finished or Started

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


[0.1.3]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.3
[0.1.2]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.2
[0.1.1]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.1
[0.1.0]: https://github.com/brunomikoski/Animation-Sequencer/releases/tag/v0.1.0


