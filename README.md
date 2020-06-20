# TerrLauncherPackCreator

Just an app that can help you to create packs for TL Pro: https://play.google.com/store/apps/details?id=com.pixelcurves.terlauncher

## Requirements

You need to have .Net Framework v4.6.1+ installed to run the app. You can find the exact version on the Microsoft site but I recommend installing version 4.8 instead of the lower ones, as it is backward compatible with previous versions but supports newer versions of framework out of the box. You can find an offline installer here: https://support.microsoft.com/en-us/help/4503548/microsoft-net-framework-4-8-offline-installer-for-windows

## Installation

The installation is pretty straightforward:
1. You download the archive from the `Releases` page, extract it somewhere and launch the `installer.exe`
2. The installer will copy all the needed files to `<system drive>:\Users\<current user>\AppData\Roaming\TerrLauncherPackCreator` and create a shortcut in the current folder
3. You can then use the app by either launching it via the shortcut or by launching the `.exe` directly

## Usage

You have several steps you can loop through to create a new pack. You can fill the current page with the details then press `Next`, fill again, press `Next` again, and so on.
Then you finally save the pack and copy it to the target device.

## Importing packs to TL Pro

Resulting packs have `.tl` extension. To import them to TL Pro, click on the file inside an android file explorer (pretty much anyone) and open the file via TL Pro

## Important

This app is not really production-ready as it doesn't have an uninstaller and comprehensive documentation and sometimes it contains bugs. So, pls do not hesitate to raise an issue if you see something is broken or if you have a suggestion
