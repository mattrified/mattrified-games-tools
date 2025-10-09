using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowModeValue : MattrifiedGames.SVData.IntScriptableValue
{
    /*//
    // Summary:
    //     Set your app to maintain sole full-screen use of a display. Unlike Fullscreen
    //     Window, this mode changes the OS resolution of the display to match the app's
    //     chosen resolution. This option is only supported on Windows; on other platforms
    //     the setting falls back to FullScreenMode.FullScreenWindow.
    ExclusiveFullScreen = 0,
        //
        // Summary:
        //     Set your app window to the full-screen native display resolution, covering the
        //     whole screen. This full-screen mode is also known as "borderless full-screen."
        //     Unity renders app content at the resolution set by a script (or the native display
        //     resolution if none is set) and scales it to fill the window. When scaling, Unity
        //     adds black bars to the rendered output to match the display aspect ratio to prevent
        //     content stretching. This process is called <a href="https:en.wikipedia.orgwikiLetterboxing_(filming)">letterboxing</a>.
        //     The OS overlay UI will display on top of the full-screen window (such as IME
        //     input windows). All platforms support this mode.
        FullScreenWindow = 1,
        //
        // Summary:
        //     Set the app window to the operating system’s definition of “maximized”. This
        //     means a full-screen window with a hidden menu bar and dock on macOS. This option
        //     is only supported on macOS; on other platforms, the setting falls back to FullScreenMode.FullScreenWindow.
        MaximizedWindow = 2,
        //
        // Summary:
        //     Set your app to a standard, non-full-screen movable window, the size of which
        //     is dependent on the app resolution. All desktop platforms support this full-screen
        //     mode.
        Windowed = 3*/

    protected override void AssignDefault()
    {
        base.AssignDefault();

        SetupWindow();
    }

    private void SetupWindow()
    {
        Screen.fullScreenMode = (FullScreenMode)Value;
    }
}