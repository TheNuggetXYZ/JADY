# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Modern text box, combo box, date picker and checkbox styling

### Changed
- Welcome window structure

## [0.3.0] - 2026-5-30

### Added
- Linking notes to events
- Ending an event by linking an end note

### Changed
- Edit window entry type names

## [0.2.1] - 2026-5-17

### Fixed
- App not creating the JADY folder - crashes when trying to load into the app

## [0.2.0] - 2026-5-17

### Added
- Welcome window with optional password
- Save encryption and decryption with AES GCM and pbkdf2 key derivation
- Entries have a color strip

### Changed
- .NET 9 to .NET 10
- Save structure (saves from 0.1.0 are incompatible but easily portable)
- App theme has the "System" option - uses the theme of your OS
- Scroll bar style
- Save file extensions

## [0.1.0] - 2026-5-7

### Added
- Basic diary management (add, edit, delete diaries)
- Basic entry management (add, edit, end, delete entries)
- Searching through diary entries
- Auto save
- Select culture (for dates)
- Select dark/light theme