﻿#if AR_FOUNDATION_REMOTE_INSTALLED
    using ARFoundationRemote.Runtime;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;


namespace ARFoundationRemote.Editor {
    [UsedImplicitly]
    [InitializeOnLoad]
    public class CompanionAppInstaller: IPreprocessBuildWithReport, IPostprocessBuildWithReport {
        const string appName = "ARCompanion";
        const string arCompanionDefine = "AR_COMPANION";


        [NotNull]
        static InstallerSettings settings {
            get {
                var instance = ARFoundationRemoteInstaller.Instance;
                return instance != null ? instance.installerSettings : new InstallerSettings();
            }
        }

        public int callbackOrder => 0;
        
        /// OnPreprocessBuild is not called after unsuccessful build, so I restore change in static constructor
        /// ARFoundationRemoteInstaller.Instance can be null in static constructor
        static CompanionAppInstaller() {
            if (ARFoundationRemoteInstaller.Instance != null) {
                restoreChanges();
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report) {
            log($"CompanionAppInstaller IPreprocessBuildWithReport.OnPreprocessBuild, isBuildingCompanionApp(report): {isBuildingCompanionApp(report)}, defines: {PlayerSettings.GetScriptingDefineSymbolsForGroup(activeBuildTargetGroup)}, isCloudBuild: {isCloudBuild}");
            if (isCloudBuild) {
                if (!isARCompanionDefineSet) {
                    throw new BuildFailedException($"{ARFoundationRemoteInstaller.displayName}: please add the '{arCompanionDefine}' to the 'Scripting Define Symbols' in Unity Cloud Build console.");
                }
                
                if (!prepareBuild()) {
                    throw new BuildFailedException($"{ARFoundationRemoteInstaller.displayName}: build failed.");
                }

                return;
            }

            if (!isBuildingCompanionApp(report)) {
                if (EditorBuildSettings.scenes.Any(_ => _.path.Contains("ARCompanion.unity"))) {
                    throw new BuildFailedException(
                        $"{ARFoundationRemoteInstaller.displayName}: please build the companion app via Installer object by pressing 'Install AR Companion App' or 'Build AR Companion and show in folder' button.");
                }
            }
        }

        static string applicationIdentifier {
            get => PlayerSettings.GetApplicationIdentifier(activeBuildTargetGroup);
            set => PlayerSettings.SetApplicationIdentifier(activeBuildTargetGroup, value);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report) {
            if (isCloudBuild) {
                Debug.Log(appName + " UCB build succeeded.");
                return;
            }
            
            if (isBuildingCompanionApp(report)) {
                restoreChanges();
                if (report.summary.totalErrors == 0) {
                    Debug.Log(appName + " build succeeded.");
                }
            }
        }

        static void restoreChanges() {
            if (settings.modifyAppId) {
                applicationIdentifier = removeAppName(applicationIdentifier);
            }

            if (settings.modifyAppName) {
                PlayerSettings.productName = removeAppName(PlayerSettings.productName);
            }

            if (settings.removeCompanionAppDefineAfterBuild) {
                toggleDefine(arCompanionDefine, false);
            }
        }

        static void toggleDefine(string define, bool enable) {
            var buildTargetGroup = activeBuildTargetGroup;
            if (enable) {
                if (isDefineSet(define)) {
                    return;
                }

                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                setDefines($"{defines};{define}");
            } else {
                if (!isDefineSet(define)) {
                    return;
                }

                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                setDefines(string.Join(";", defines.Where(d => d.Trim() != define).ToArray()));
            }
            
            void setDefines(string defines) {
                log($"set defines: {defines}");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            }
        }

        static BuildTargetGroup activeBuildTargetGroup => EditorUserBuildSettings.activeBuildTarget.ToBuildTargetGroup();

        static bool isDefineSet(string define) {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.activeBuildTarget.ToBuildTargetGroup()).Contains(define);
        }

        static bool isCloudBuild {
            get {
                #if UNITY_CLOUD_BUILD
                    return true;
                #else
                    return false;
                #endif
            }
        }

        static bool isARCompanionDefineSet {
            get {
                #if AR_COMPANION
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        /// return false in UCB
        static bool isBuildingCompanionApp(BuildReport report) {
            var result = report.summary.outputPath.Contains(companionAppFolder);
            log("isBuildingCompanionApp " + result);
            return result;
        }

        static string removeAppName(string s) {
            return s.Replace(appName, "");
        }

        public static void Build() {
            build(buildOptions | BuildOptions.ShowBuiltPlayer);
        }

        public static void BuildAndRun() {
            build(buildOptions | BuildOptions.AutoRunPlayer);
        }

        static bool prepareBuild() {
            var listRequest = Client.List(true, true);
            ARFoundationRemoteInstaller.runPackageManagerRequestBlocking(listRequest);
            if (listRequest.Status != StatusCode.Success) {
                Debug.LogError("ARFoundationRemoteInstaller can't check installed packages.");
                return false;
            }

            if (!isPresent(ARFoundationRemoteInstaller.pluginId)) {
                Debug.LogError("Please install " + ARFoundationRemoteInstaller.displayName);
                return false;
            }

            #if AR_FOUNDATION_REMOTE_INSTALLED
            var instance = Settings.Instance;
            instance.packages = PackageVersionData.Create(listRequest.Result);
            instance.inputHandlingData = InputHandlingData.Create();
            EditorUtility.SetDirty(instance);
            
            if (Defines.isAndroid) {
                if (!isPresent("com.unity.xr.arcore")) {
                    logPluginNotInstalledError("ARCore XR Plugin");
                    return false;
                }
            } else if (Defines.isIOS) {
                if (!isPresent("com.unity.xr.arkit") && !isPresent("com.unity.xr.arkit-face-tracking")) {
                    logPluginNotInstalledError("ARKit XR Plugin");
                    return false;
                }
            }

            void logPluginNotInstalledError(string pluginName) {
                Debug.LogError($"Please install '{pluginName}' via Package Manager and ENABLE IT in 'XR Plug-in Management' window.");
            }
            #endif
            
            bool isPresent(string packageId) {
                return listRequest.Result.SingleOrDefault(_ => _.name == packageId) != null;
            }

            if (settings.modifyAppId && !isCloudBuild) {
                applicationIdentifier = removeAppName(applicationIdentifier) + appName;
            }

            if (settings.modifyAppName) {
                PlayerSettings.productName = appName + removeAppName(PlayerSettings.productName);
            }

            toggleDefine(arCompanionDefine, true);
            return true;
        }

        static void build(BuildOptions options) {
            if (!prepareBuild()) {
                return;
            }

            var scenes = getSenderScenePaths().Select(_ => _.ToString()).ToArray();
            BuildPipeline.BuildPlayer(scenes, getInstallDirectory() + EditorUserBuildSettings.activeBuildTarget + getExtension(settings.optionalCompanionAppExtension), EditorUserBuildSettings.activeBuildTarget, options);
        }

        static string getExtension(string optionalCompanionAppExtension) {
            switch (EditorUserBuildSettings.activeBuildTarget) {
                case BuildTarget.iOS:
                    return "";
                case BuildTarget.Android:
                    return ".apk";
                default:
                    if (string.IsNullOrEmpty(optionalCompanionAppExtension)) {
                        Debug.LogWarning("Please specify optionalCompanionAppExtension if your target platform needs one. For example, Android target requires .apk extension for the builds.");
                    } else {
                        Debug.Log("Using optionalCompanionAppExtension: " + optionalCompanionAppExtension);
                    }
                    return optionalCompanionAppExtension;
            }
        }

        /// Adding BuildOptions.AcceptExternalModificationsToPlayer will produce gradle build for android instead of apk
        /// Enable BuildOptions.Development for assertions to work
        static BuildOptions buildOptions => BuildOptions.Development;

        static string getInstallDirectory() {
            var directoryInfo = Directory.GetParent(Application.dataPath);
            Assert.IsNotNull(directoryInfo);
            return directoryInfo.FullName + "/" + companionAppFolder + "/";
        }

        static string companionAppFolder => "ARFoundationRemoteCompanionApp";

        static IEnumerable<FileInfo> getSenderScenePaths() {
            return new DirectoryInfo(Application.dataPath + "/Plugins/ARFoundationRemoteInstaller/Resources")
                .GetFiles("*.unity");
        }

        public static void DeleteCompanionAppBuildFolder() {
            var path = getInstallDirectory();
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
        }
        
        
        [Conditional("_")]
        static void log(string msg) {
            Debug.Log(msg);
        }
    }
}
