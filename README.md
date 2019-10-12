chaturbate-downloader
========

A chaturbate stream downloader using [streamlink](https://github.com/streamlink/streamlink).

# Usage
1. Ensure you have [streamlink](https://github.com/streamlink/streamlink) installed, and the binary is available from the system path, or in the current working directory.
2. Run the chaturbate-downloader binary.
3. You can now add models to download (or their URL - only for `chaturbate.com` and its sub-domains) to the text box at the bottom, and when they are online they will be downloaded.
   Models with a tick next to them are actively being listened to.

You can find the chaturbate streamlink plugin in this repository, as `chaturbate.py`.

# Features
* Loads pre-configured models list (place their names in a file called `models.txt` in the same folder as  the chaturbate-downloader binary)
* Auto-completion of model names (add them to `models_list.txt` and re-compile the project)
* Downloads at the highest quality available to `.flv` format

# Known Bugs
* When you close the program, rarely dangling streamlink processes may exist.
* When you remove a model, rarely a dangling streamlink process for it may still exist.

# Requirements
* [streamlink](https://github.com/streamlink/streamlink)
* .NET Framework v4.5.2

# License
The [MIT License](LICENSE).
