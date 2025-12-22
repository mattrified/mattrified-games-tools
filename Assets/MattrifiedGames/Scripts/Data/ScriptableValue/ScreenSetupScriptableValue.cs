using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Screen Setup")]
    public class ScreenSetupScriptableValue : ScriptableValue<ScreenSetup, UnityEventScreenSetup>
    {


        public override ScreenSetup Value
        {
            get => base.Value;
            set
            {
                base.Value = value;

                
                Screen.SetResolution(_value.width, _value.height, _value.fullScreenMode, _value.preferredRefreshRate);
                QualitySettings.vSyncCount = _value.vSyncValue;

                // TODO:  Mayve have this set here or set somewhere else?  Not 100% sure.
                if (QualitySettings.vSyncCount == 0)
                    Application.targetFrameRate = _value.targetFramerate;
                else
                {
                    // I think in theory, when vsync is used, this should be -1;
                    Application.targetFrameRate = -1;
                }
            }
        }

        public override string Save()
        {
            return JsonUtility.ToJson(Value);
        }

        public override void Load(string s)
        {
            if (string.IsNullOrEmpty(s))
                Value = defaultValue;
            else
                Value = JsonUtility.FromJson<ScreenSetup>(s);
        }

        public void AssignWidth(int newWidth)
        {
            var ss = Value;
            ss.width = newWidth;
            Value = ss;
        }

        public void AssignHeight(int newHeight)
        {
            var ss = Value;
            ss.height = newHeight;
            Value = ss;
        }

        public void AssignFullscreenMode(FullScreenMode newFullScreenMode)
        {
            var ss = Value;
            ss.fullScreenMode = newFullScreenMode;
            Value = ss;
        }

        public void AssignVSync(int newVSync)
        {
            var ss = Value;
            ss.vSyncValue = newVSync;
            Value = ss;
        }
        public string GetVSyncString()
        {
            switch (Value.vSyncValue)
            {
                case 0: return "Off";
                default: return (Screen.currentResolution.refreshRateRatio.value).ToString() + " Hz";
            }
        }

        public int GetResolutionIndex(Resolution[] resolutions)
        {
            var v = Value;

            // We go through each resolution; the closest will give us what we want.
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (v.width == resolutions[i].width && v.height == resolutions[i].height)
                {
                    return i;
                }
            }

            // We return the largest one
            return resolutions.Length - 1;
        }

        public void AssignResolution(Resolution r, bool assignRefreshRate)
        {
            var v = Value;
            v.width = r.width;
            v.height = r.height;

            if (assignRefreshRate)
                v.preferredRefreshRate = r.refreshRateRatio;

            Value = v;
        }

        public void AdjustVSync(bool increase)
        {
            var v = Value;

            if (increase)
            {
                if (v.vSyncValue == 4)
                    v.vSyncValue = 0;
                else
                    v.vSyncValue++;
            }
            else
            {
                if (v.vSyncValue == 0)
                    v.vSyncValue = 4;
                else
                    v.vSyncValue--;
            }


            Value = v;
        }

        public void AdjustFullscreenMode(bool increase)
        {
            var v = Value;

            if (increase)
            {
                if (v.fullScreenMode == FullScreenMode.Windowed)
                    v.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                else
                {
                    v.fullScreenMode++;
                    if (v.fullScreenMode == FullScreenMode.MaximizedWindow)
                        v.fullScreenMode = FullScreenMode.Windowed;
                }   
            }
            else
            {
                if (v.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
                {
                    v.fullScreenMode = FullScreenMode.Windowed;
                }
                else
                {
                    v.fullScreenMode--;
                    if (v.fullScreenMode == FullScreenMode.MaximizedWindow)
                        v.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
            }

            Value = v;
        }
    }

    [System.Serializable()]
    public struct ScreenSetup
    {
        public int width;
        public int height;

        public RefreshRate preferredRefreshRate;

        public FullScreenMode fullScreenMode;
        public int vSyncValue;

        public int targetFramerate;
    }

    [System.Serializable()]
    public class UnityEventScreenSetup : UnityEvent<ScreenSetup> { }
}