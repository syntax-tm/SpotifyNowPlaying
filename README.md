# Spotify Now Playing

## Overview

This is based on the *OBSCurrentSong* program that creates a file for the currently playing artist and song. This program was created to give a bit more flexability and functionality over *OBSCurrentSong*.

## Features

- Ability to specify the output folder
- Ability to customize the update delay
- Ability to create multiple outputs
- Ability to create dynamic output file formats
    - Current and previous song info
    - Support for date/time
    - Support for special characters
- Ability to enable/disable outputs
- Easy to use config file (example below)

## Config

The first time _SpotifyNowPlaying_ is ran a default config file named '_config.json_' will be created (if one does not exist). This can be edited as needed and is __only__ loaded on startup.

### Example Config File

#### config.json
```json
{
  "delay": 2000,
  "outputFolder": "C:/test",
  "outputs": [
    {
      "name": "Artist Output",
      "enabled": true,
      "fileName": "artist.txt",
      "fileFormat": "{artist}"
    },
    {
      "name": "Song Output",
      "enabled": true,
      "fileName": "song.txt",
      "fileFormat": "{song}"
    },
    {
      "name": "Current Song Output",
      "enabled": true,
      "fileName": "currentSong.txt",
      "fileFormat": "{song}{newline}by {artist}"
    }
  ]
}
```

### Tokens

These tokens can be using in the config file's *fileFormat* string and are replaced with the specified values during output.

#### Track Info

| Token | Alias | Value |
|:-----:|:-----:|:------|
| \{song} |     | The name of the current song |
| \{artist} |   | The artist of the current song |
| \{previous_song} | | The name of the previous song |
| \{previous_artist} | | The artist of the previous song |

#### Date/Time

| Token | Alias | Value |
|:-----:|:-----:|:------|
| \{longdate} | | Long date string |
| \{longtime} | | Long time string |
| \{date} | | Date string |
| \{time} | | Time string |
| \{dayofweek} | | Current day of the week |
| \{hour} | | Current hour |
| \{minutes} | | Current minute |
| \{month} | | Current month |
| \{year} | | Current year |

#### Static

| Token | Alias | Value |
|:-----:|:-----:|:------|
| \{newline} | \{nl} | Environment-specific line ending (*crlf* on Windows) |
| \{tab} | \{t} | ```\t``` |
| \{s} |        | Space |
| \{cr} |       | ```\r``` |
| \{lf} |       | ```\n``` |
| \{crlf} |     | ```\r\n``` |
| \{quote} | \{q} | ```'``` |
| \{dquote} | \{dq} | ```"``` |
| \{unicode} | \{u} | ```\0``` |
| \{alert} | \{a} | ```\a``` |
| \{form} | \{f} | ```\f``` |
| \{vtab} | | ```\v``` |
| \{bs} | | ```\``` |

## Limitations

- Only works with spotify desktop app (not web player)
- Pausing the current song will remove the previous song

## TODO

- Switch to .NET Core to support non-Windows OS
- Change the previously playing to be a Stack (instead of only storing one item)
- Support date/time tokens in output file names
- Ability to not create output when current song is not set
- Ability to not create output when previous song is not set
- Support pausing and resuming music without losing previously playing track
