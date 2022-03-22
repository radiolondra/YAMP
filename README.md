# YAMP

### Yet Another Media Player

In general, **YAMP** can play video and audio from local files or remote links, even from Youtube, Vimeo and other links.

#### Why YAMP?

Unlike the WPF version of LibVLCSharp, the Avalonia version is missing several things.

- First of all, it doesn't seem to be possible to use the LibVLC *MediaPlayer* in a control, so *MediaPlayer* has to be placed in a window. 

- Also it doesn't seem to be possible to add elements in a layer on top of *MediaPlayer*.

**YAMP** tries to overcome this last drawback with tricks that still generate a decent result.

There is still a lot of work to do to get a stable result, but this is certainly a good starting point.

Some "*goodies*" have been added, such as the use of [YT-DLP](https://github.com/yt-dlp/yt-dlp) to use videos from Youtube and other sites, generating links playable by **YAMP**.

Your contribution is certainly welcome, in any form.



#### Quick Build Guide

- **First of all get the solution**, in the way you prefere (download the zip, git clone, ...)

- **FFMPeg** static builds are needed (x32 & x64). You can get them from [here](https://web.archive.org/web/20200918014242/https://ffmpeg.zeranoe.com/builds/) or from any other website you know. Choose Windows, 4.3.1 or greater, GPL versions for x32 and x64, download and extract them in the right subfolder (**iffx64** and **iffx32**) of **/bins** solution folder (<u>I said /bins, not /bin</u>).

- **YT-DLP** latest release builds are needed (x32 and x64). You can get them from [here](https://github.com/yt-dlp/yt-dlp/releases). Extract the right version (x32 & x64) in the right subfolder (**iffx32** and **iffx64**) of **/bins** solution folder (<u>I said /bins, not /bin</u>).

Open the **YAMP** solution in **Visual Studio 2019** and build it.
