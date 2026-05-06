# DeadInfoVoice

DeadInfoVoice is a plugin for Counter-Strike 2 using CounterStrikeSharp.

The plugin allows players to continue speaking to their teammates for a configurable amount of time after death.

---

# Features

- configurable voice time after death
- chat notification after death
- automatic mute after timer expires
- translation system
- configurable language
- Linux support
- automatic config generation

---

# Requirements

- Metamod:Source
- CounterStrikeSharp
- .NET 8 Runtime

---

# Installation

## 1. Build the plugin

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

## 2. Upload generated files to

```text
game/csgo/addons/counterstrikesharp/plugins/DeadInfoVoice/
```

## 3. Restart the server

---

# Generated Files

After first launch plugin creates:

```text
addons/counterstrikesharp/configs/plugins/DeadInfoVoice/
├── config.json
└── lang/
    ├── pl.json
    └── en.json
```

---

# Configuration

`config.json`

```json
{
  "VoiceTime": 5.0,
  "Language": "pl"
}
```

## Options

| Option | Description |
|---|---|
| VoiceTime | Time in seconds players can speak after death |
| Language | Plugin language (`pl`, `en`) |

---

# Translation System

Translations are stored in:

```text
addons/counterstrikesharp/configs/plugins/DeadInfoVoice/lang/
```

Example:

`pl.json`

```json
{
  "dead_info_start": "{GREEN}● {DEFAULT}Masz {LIGHTGREEN}{TIME} sekund{DEFAULT} na przekazanie informacji drużynie.",
  "dead_info_end": "{RED}● {DEFAULT}Czas na przekazanie informacji minął."
}
```

`en.json`

```json
{
  "dead_info_start": "{GREEN}● {DEFAULT}You have {LIGHTGREEN}{TIME} seconds{DEFAULT} to give information to your team.",
  "dead_info_end": "{RED}● {DEFAULT}Your info time has expired."
}
```

---

# Supported Color Tags

| Tag | Description |
|---|---|
| `{DEFAULT}` | Default chat color |
| `{GREEN}` | Green |
| `{LIGHTGREEN}` | Light green |
| `{RED}` | Red |

---

# Notes

Plugin automatically executes:

```cfg
sv_deadtalk 1
```

to allow temporary dead-player voice communication.

Voice is automatically muted again after configured time expires.

---

# License

MIT License