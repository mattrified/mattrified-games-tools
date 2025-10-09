using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Quality")]
    public class QualitySettingsScriptableValue : ScriptableObject
    {
        [SerializeField(), Tooltip("If true, this data is saved to player prefs.")]
        bool usePlayerPrefs = true;

        [SerializeField(), Tooltip("The list of values.")]
        List<QualitySettingsSetup> settingsList;

        [SerializeField()]
        private IntScriptableValue mainPresentValue;

        [SerializeField()]
        private IntScriptableValue fullScreenMode;

        private readonly Dictionary<QualityParameter, UnityAction<int>> dict = new Dictionary<QualityParameter, UnityAction<int>>()
        {
            {QualityParameter.AnisotrophicTextures, (int i) => {QualitySettings.anisotropicFiltering = (AnisotropicFiltering)i; } },
            {QualityParameter.AntiAlias, (int i) => {QualitySettings.antiAliasing = i; } },
            {QualityParameter.AsyncUploadBufferSize, (int i) => {QualitySettings.asyncUploadBufferSize = i; } },
            {QualityParameter.AsyncUploadTimeSlice, (int i) => {QualitySettings.asyncUploadTimeSlice = i; } },
            {QualityParameter.BillboardsFaceCameraPosition_Bool, (int i) => {QualitySettings.billboardsFaceCameraPosition = i == 1; } },
            {QualityParameter.BlendWeights, (int i) => {QualitySettings.skinWeights = (SkinWeights)i; } },
            {QualityParameter.LodBias, (int i) => {QualitySettings.lodBias = i / 100f; } },
            {QualityParameter.MaximumLODLevel, (int i) => {QualitySettings.maximumLODLevel = i; } },
            {QualityParameter.ParticleRaycastBudget, (int i) => {QualitySettings.particleRaycastBudget= i; } },
            {QualityParameter.PixelLightCount, (int i) => {QualitySettings.pixelLightCount= i; } },
            {QualityParameter.RealtimeReflectionProbe_Bool, (int i) => {QualitySettings.realtimeReflectionProbes = i == 1; } },
            {QualityParameter.ResolutionScalingFixedDPI, (int i) => {QualitySettings.resolutionScalingFixedDPIFactor = i/100f; } },
            
            {QualityParameter.ShadowCascade, (int i) => {QualitySettings.shadowCascades = i; } },
            {QualityParameter.ShadowCascade2Split, (int i) => {QualitySettings.shadowCascade2Split = i/100f; } },
            {QualityParameter.ShadowCascase4SplitX, (int i) =>
                {
                    Vector3 v = QualitySettings.shadowCascade4Split;
                    v.x = i/100f;
                    QualitySettings.shadowCascade4Split = v;
                }
            },
            {QualityParameter.ShadowCascase4SplitY, (int i) =>
                {
                    Vector3 v = QualitySettings.shadowCascade4Split;
                    v.y = i/100f;
                    QualitySettings.shadowCascade4Split = v;
                }
            },
            {QualityParameter.ShadowCascase4SplitZ, (int i) =>
                {
                    Vector3 v = QualitySettings.shadowCascade4Split;
                    v.z = i/100f;
                    QualitySettings.shadowCascade4Split = v;
                }
            },
            {QualityParameter.ShadowDist, (int i) => {QualitySettings.shadowDistance = i/100f; } },
            {QualityParameter.ShadowMaskMode, (int i) => {QualitySettings.shadowmaskMode = (ShadowmaskMode)i; } },
            {QualityParameter.ShadowMode, (int i) => {QualitySettings.shadows = (ShadowQuality)i; } },
            {QualityParameter.ShadowNearPlaneOffset, (int i) => {QualitySettings.shadowNearPlaneOffset = i /100f; } },
            {QualityParameter.ShadowProj, (int i) => {QualitySettings.shadowProjection = (ShadowProjection)i; } },
            {QualityParameter.ShadowRes, (int i) => {QualitySettings.shadowResolution = (ShadowResolution)i; } },
            {QualityParameter.SoftParticles_Bool, (int i) => {QualitySettings.softParticles = i == 1; } },
            {QualityParameter.TextureQuality, (int i) => {QualitySettings.globalTextureMipmapLimit = i; } },
            {QualityParameter.TextureStreaming_Bool_Ignore, (int i) => {QualitySettings.streamingMipmapsActive = i == 1; } },
            
            {QualityParameter.VSyncCount, (int i) => {QualitySettings.vSyncCount = i; } },

            {QualityParameter.UseFullScreen_Bool, (int i) => {Screen.fullScreenMode = (FullScreenMode)i; } },
            {QualityParameter.ScreenHeight, (int i) => {Screen.SetResolution(Screen.width, i, Screen.fullScreenMode); } },
            {QualityParameter.ScreenWidth, (int i) => {Screen.SetResolution(i, Screen.height, Screen.fullScreenMode); } },
        };

        [System.NonSerialized()]
        private bool init;

        public void Init()
        {
            if (init)
                return;

            //Application.targetFrameRate = 60;

            init = true;

            mainPresentValue.AddOnValueChangedEvent(SetPreset);

            for (int i =0; i < settingsList.Count; i++)
            {
                settingsList[i].scriptableValue.AddOnValueChangedEvent(dict[settingsList[i].parameter]);

                // Attempts to load the parameter.
                if (PlayerPrefs.HasKey(settingsList[i].scriptableValue.name))
                {
                    settingsList[i].scriptableValue.Value = PlayerPrefs.GetInt(settingsList[i].scriptableValue.name,
                        settingsList[i].scriptableValue.Value);
                }

                if (usePlayerPrefs)
                    settingsList[i].scriptableValue.AddOnValueChangedEvent(settingsList[i].SetPlayerPref);
            }

            mainPresentValue.Value = PlayerPrefs.GetInt(mainPresentValue.name, mainPresentValue.Value);
        }

        void SetPreset(int index)
        {
            PlayerPrefs.SetInt(mainPresentValue.name, index);

            if (index < 0)
                return;

            for (int i = 0; i < settingsList.Count; i++)
            {
                switch (settingsList[i].parameter)
                {
                    case QualityParameter.UseFullScreen_Bool:
                    case QualityParameter.ScreenHeight:
                    case QualityParameter.ScreenWidth:
                        continue;

                        
                }
                settingsList[i].scriptableValue.Value = settingsList[i].presets[index];
            }
        }

        private void OnDisable()
        {
            if (!init)
                return;

            init = false;

            for (int i = 0; i < settingsList.Count; i++)
            {
                settingsList[i].scriptableValue.RemoveOnSetEvent(dict[settingsList[i].parameter]);

                if (usePlayerPrefs)
                    settingsList[i].scriptableValue.RemoveOnSetEvent(settingsList[i].SetPlayerPref);
            }

            mainPresentValue.RemoveOnSetEvent(SetPreset);
        }

        private void OnDestroy()
        {
            OnDisable();
        }

        [System.Serializable()]
        public class QualitySettingsSetup
        {
            public string displayName;
            public QualityParameter parameter;
            public IntScriptableValue scriptableValue;

            public bool useRange;

            [Tooltip("If use range is true, values[0] is the min; values[1] is the max")]
            public List<int> values = new List<int>();

            public List<int> presets = new List<int>();

            internal void SetPlayerPref(int arg0)
            {
                PlayerPrefs.SetInt(scriptableValue.name, arg0);
                PlayerPrefs.Save();
            }

#if UNITY_EDITOR

            internal void SetSix()
            {
                scriptableValue.SetDefaultValue(presets[6]);
            }

            internal void SetupRange(Func<int> p)
            {
                useRange = true;
                values = new List<int>() { 0, 0 };

                presets = new List<int>();

                for (int i = 0; i < 9; i++)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    int v = p();
                    values[0] = Mathf.Min(v, values[0]);
                    values[1] = Mathf.Max(v, values[1]);

                    presets.Add(v);
                }
            }

            internal void SetupSet(Func<int> p)
            {
                useRange = false;
                values = new List<int>();
                for (int i = 0; i < 9; i++)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    int v = p();
                    presets.Add(v);

                    if (values.Contains(v))
                        continue;
                    else
                        values.Add(v);
                }
                values.Sort((x, y) => (x - y));
            }

            internal void SetupBool(Func<int> p)
            {
                useRange = false;
                values = new List<int>() { 0, 1 };
                for (int i = 0; i < 9; i++)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    int v = p();
                    presets.Add(v);
                }
            }

            internal void SetupPreDefinedRange(int min, int max, Func<int> p)
            {
                useRange = true;

                values = new List<int>() { min, max };
                for (int i = 0; i < 9; i++)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    int v = p();
                    presets.Add(v);
                }
            }

            internal void SetupPreDefinedRange(Func<int> p, params int[] inValues)
            {
                useRange = true;

                values = new List<int>(inValues);
                for (int i = 0; i < 9; i++)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    int v = p();
                    presets.Add(v);
                }
            }

            
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Set 6")]
        public void SetSixth()
        {
            for (int i = 0; i < settingsList.Count; i++)
                settingsList[i].SetSix();
        }

        [ContextMenu("Clean Up")]
        public void CleanUp()
        {
            for (int i = 0; i < settingsList.Count; i++)
            {
                DestroyImmediate(settingsList[i].scriptableValue, true);
            }
            settingsList.Clear();

            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(this));
        }

        

        [ContextMenu("Setup")]
        public void Setup()
        {
            settingsList = new List<QualitySettingsSetup>();
            int q = QualitySettings.GetQualityLevel();
            for (QualityParameter p = QualityParameter.PixelLightCount; p <= QualityParameter.ScreenHeight; p++)
            {
                QualitySettingsSetup qss = new QualitySettingsSetup();

                qss.parameter = p;
                qss.displayName = p.ToString();
                qss.scriptableValue = CreateInstance<IntScriptableValue>();
                qss.scriptableValue.name = "Int_" + qss.displayName;

                switch (p)
                {
                    // RENDERING
                    case QualityParameter.PixelLightCount:
                        qss.SetupRange(() => { return QualitySettings.pixelLightCount; }); break;
                    case QualityParameter.TextureQuality:
                        qss.SetupSet(() => { return QualitySettings.globalTextureMipmapLimit; }); break;
                    case QualityParameter.AnisotrophicTextures:
                        qss.SetupSet(() => { return (int)QualitySettings.anisotropicFiltering; }); break;
                    case QualityParameter.AntiAlias:
                        qss.SetupSet(() => { return QualitySettings.antiAliasing; }); break;
                    case QualityParameter.SoftParticles_Bool:
                        qss.SetupBool(() => { return QualitySettings.softParticles ? 1 : 0; }); break;
                    case QualityParameter.RealtimeReflectionProbe_Bool:
                        qss.SetupBool(() => { return QualitySettings.realtimeReflectionProbes ? 1 : 0; }); break;
                    case QualityParameter.BillboardsFaceCameraPosition_Bool:
                        qss.SetupBool(() => { return QualitySettings.billboardsFaceCameraPosition ? 1 : 0; }); break;
                    case QualityParameter.TextureStreaming_Bool_Ignore:
                        qss.SetupBool(() => { return QualitySettings.streamingMipmapsActive ? 1 : 0; }); break;
                    case QualityParameter.ResolutionScalingFixedDPI:
                        qss.SetupSet(() => { return Mathf.RoundToInt(QualitySettings.resolutionScalingFixedDPIFactor * 100); }); break;

                    // SHADOW
                    case QualityParameter.ShadowMode:
                        qss.SetupSet(() => { return (int)QualitySettings.shadows; }); break;
                    case QualityParameter.ShadowMaskMode:
                        qss.SetupSet(() => { return (int)QualitySettings.shadowmaskMode; }); break;
                    case QualityParameter.ShadowRes:
                        qss.SetupSet(() => { return (int)QualitySettings.shadowResolution; }); break;
                    case QualityParameter.ShadowProj:
                        qss.SetupSet(() => { return (int)QualitySettings.shadowProjection; }); break;
                    case QualityParameter.ShadowDist:
                        qss.SetupRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowDistance); }); break;
                    case QualityParameter.ShadowNearPlaneOffset:
                        qss.SetupRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowNearPlaneOffset); }); break;
                    case QualityParameter.ShadowCascade:
                        qss.SetupSet(() => { return QualitySettings.shadowCascades; }); break;
                    case QualityParameter.ShadowCascade2Split:
                        qss.SetupPreDefinedRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowCascade2Split); }, 0, 100); break;
                    case QualityParameter.ShadowCascase4SplitX:
                        qss.SetupPreDefinedRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowCascade4Split.x); }, 0, 100); break;
                    case QualityParameter.ShadowCascase4SplitY:
                        qss.SetupPreDefinedRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowCascade4Split.y); }, 0, 100); break;
                    case QualityParameter.ShadowCascase4SplitZ:
                        qss.SetupPreDefinedRange(() => { return Mathf.RoundToInt(100f * QualitySettings.shadowCascade4Split.z); }, 0, 100); break;

                    // OTHER
                    case QualityParameter.BlendWeights:
                        qss.SetupSet(() => { return (int)QualitySettings.skinWeights; }); break;
                    case QualityParameter.VSyncCount:
                        qss.SetupSet(() => { return QualitySettings.vSyncCount; }); break;
                    case QualityParameter.LodBias:
                        qss.SetupRange(() => { return Mathf.RoundToInt(QualitySettings.lodBias * 100); }); break;
                    case QualityParameter.MaximumLODLevel:
                        qss.SetupRange(() => { return QualitySettings.maximumLODLevel; }); break;
                    case QualityParameter.ParticleRaycastBudget:
                        qss.SetupSet(() => { return QualitySettings.particleRaycastBudget; }); break;
                    case QualityParameter.AsyncUploadTimeSlice:
                        qss.SetupSet(() => { return QualitySettings.asyncUploadTimeSlice; }); break;
                    case QualityParameter.AsyncUploadBufferSize:
                        qss.SetupSet(() => { return QualitySettings.asyncUploadBufferSize; }); break;

                    case QualityParameter.UseFullScreen_Bool:
                        qss.SetupBool(() => { return 0; }); break;

                    // Temp just does different aspect ratios for now...
                    case QualityParameter.ScreenWidth:
                        qss.SetupPreDefinedRange(() => { return 1920; }, 640, 960, 1280, 1920); break;
                    case QualityParameter.ScreenHeight:
                        qss.SetupPreDefinedRange(() => { return 1080; }, 360, 540, 720, 1080); break;
                }

                settingsList.Add(qss);
                UnityEditor.AssetDatabase.AddObjectToAsset(qss.scriptableValue, this);
            }

            QualitySettings.SetQualityLevel(q, true);

            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(this));
        }
#endif

        public enum QualityParameter : int
        {
            PixelLightCount = 0,
            TextureQuality = 1,
            AnisotrophicTextures = 2,
            AntiAlias = 3,
            SoftParticles_Bool = 4,
            RealtimeReflectionProbe_Bool = 5,
            BillboardsFaceCameraPosition_Bool = 6,
            ResolutionScalingFixedDPI = 7,
            TextureStreaming_Bool_Ignore = 8,


            ShadowMaskMode = 9,
            ShadowMode = 10,
            ShadowRes = 11,
            ShadowProj = 12,
            ShadowDist = 13,
            ShadowNearPlaneOffset = 14,
            ShadowCascade = 15,
            ShadowCascade2Split = 16,

            ShadowCascase4SplitX = 17,
            ShadowCascase4SplitY = 18,
            ShadowCascase4SplitZ = 19,

            BlendWeights = 20,
            VSyncCount = 21,
            LodBias = 22,

            MaximumLODLevel = 23,
            ParticleRaycastBudget = 24,
            AsyncUploadTimeSlice = 25,
            AsyncUploadBufferSize = 26,

            UseFullScreen_Bool = 27,
            ScreenWidth = 28,
            ScreenHeight = 29,
        }
    }
}