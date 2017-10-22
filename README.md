chaturbate-downloader
========

A chaturbate stream downloader using [streamlink](https://github.com/streamlink/streamlink).

# Usage
Make sure you have [streamlink](https://github.com/streamlink/streamlink) installed, and the binary is available from the system path available. Download/compile chaturbate-downloader and run the binary. You can now add models to download (or their URL - only for `chaturbate.com`) to the text box at the bottom, and when they are online they will be downloaded.

# Features
* Loads pre-configured models list (place their names in a file called `models.txt` in the same folder as  the chaturbate-downloader binary)
* Auto-completion of model names (add them to `models_list.txt` and re-compile the project)
* Downloads at the highest quality available to `.flv` format

# Known Bugs
* When you close the program, sometimes dangling streamlink processes may exist.
* When you remove a model, a dangling streamlink process for it may still exist.
* Handle more chaturbate URLs from other regions, when adding a model from their URL.
* Check if streamlink is accessible

# Requirements
* [streamlink](https://github.com/streamlink/streamlink) (make sure the binary is available from the system path variable)
* .NET Framework v4.5.2

# License
The [MIT License](LICENSE).
