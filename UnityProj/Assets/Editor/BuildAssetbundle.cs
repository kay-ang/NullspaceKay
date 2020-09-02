using Nullspace;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 参数设置见 ProjectSettings.asset
/// </summary>
public class BuildAssetbundle
{
    private static string PropertiesPath = "build.property";
    private static Properties BuildConfig;
    private static Properties TargetConfig;
    private static BuildTargetGroup BuildTargetGroup;
    private static BuildTarget BuildTarget;
    private static string BuildPath;
    private static string BuildName;
    private static BuildOptions BuildOptions;
    private static BuildAssetBundleOptions BuildAssetBundleOptions;
    private class BuildTargetChoose
    {
        public const string PC = "PC";
        public const string Android = "Android";
        public const string IPhone = "IPhone";
    }

    [MenuItem("AssetBundle/BuildAndroid")]
    public static void BuildAndroid()
    {
        BuildAssetbundleTarget(BuildTargetChoose.Android);
    }

    [MenuItem("AssetBundle/BuildIOS")]
    public static void BuildIOS()
    {
        BuildAssetbundleTarget(BuildTargetChoose.IPhone);
    }

    [MenuItem("AssetBundle/BuildPC")]
    public static void BuildPC()
    {
        BuildAssetbundleTarget(BuildTargetChoose.PC);
    }

    public static void BuildAssetbundleTarget(string targetName)
    {
        BuildConfig = Properties.Create(PropertiesPath);
        TargetConfig = BuildConfig.GetNamespace(targetName, true, true);
        switch (targetName)
        {
            case BuildTargetChoose.PC:
                BuildTargetGroup = BuildTargetGroup.Standalone;
                BuildTarget = BuildTarget.StandaloneWindows;
                SetPCVariables();
                break;
            case BuildTargetChoose.Android:
                BuildTarget = BuildTarget.Android;
                BuildTargetGroup = BuildTargetGroup.Android;
                SetAndroidVariables();
                break;
            case BuildTargetChoose.IPhone:
                BuildTarget = BuildTarget.iOS;
                BuildTargetGroup = BuildTargetGroup.iOS;
                SetIOSVariables();
                break;
            default:
                break;
        }
        SetAssetbundleOpt();
        SetScriptingBackend();
        SetBuildOptions();
        SetVersion();
        SetScriptingDefineSymbols();
        SetPathAndName();
        Build();
    }

    private static void SetAssetbundleOpt()
    {
        string assetbundleOpt = BuildConfig.GetString("BuildAssetBundleOptions", "UncompressedAssetBundle");
        BuildAssetBundleOptions = EnumUtils.StringToEnum<BuildAssetBundleOptions>(assetbundleOpt);
    }

    private static string GetFilePath()
    {
        string preffix = BuildConfig.GetString("suffix", "local");
        return string.Format("{0}/{1}{2}", BuildPath, BuildName, preffix);
    }

    private static void SetPathAndName()
    {
        BuildPath = TargetConfig.GetString("buildPath", "");
        BuildName = TargetConfig.GetString("projectName", "");
    }

    private static void SetScriptingDefineSymbols()
    {
        string privateScriptSymbols = System.Environment.GetEnvironmentVariable("ScriptingDefineSymbols");
        if (null != privateScriptSymbols)
        {
            privateScriptSymbols = privateScriptSymbols.Replace('|', ';');
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, privateScriptSymbols);
    }

    private static void SetScriptingBackend()
    {
        if (BuildTarget == BuildTarget.iOS)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup, ScriptingImplementation.IL2CPP);
        }
        else
        {
            bool isProfiler = BuildConfig.GetBool("profiler", true);
            if (isProfiler)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup, ScriptingImplementation.Mono2x);
            }
            else
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup, ScriptingImplementation.IL2CPP);
            }
        }
    }

    private static void SetBuildOptions()
    {
        bool isProfiler = BuildConfig.GetBool("profiler", true);
        BuildOptions = BuildOptions.None;
        if (isProfiler)
        {
            BuildOptions |= BuildOptions.Development;
            BuildOptions |= BuildOptions.AllowDebugging;
            BuildOptions |= BuildOptions.ConnectWithProfiler;
        }
    }

    private static void SetVersion()
    {
        string majorVersion = System.Environment.GetEnvironmentVariable("MajorVersion");
        string minorVersion = System.Environment.GetEnvironmentVariable("MinorVersion");
        string fixVersion = System.Environment.GetEnvironmentVariable("FixVersion");
        string version = string.Format("{0}.{1}.{2}", majorVersion, minorVersion, fixVersion);
        PlayerSettings.bundleVersion = version;
        if (BuildTarget == BuildTarget.Android)
        {
            int result = 0;
            int.TryParse(fixVersion, out result);
            if (result < 10)
            {
                version = string.Format("{0}{1}0{2}", majorVersion, minorVersion, fixVersion);
            }
            if (int.TryParse(version, out result))
            {
                PlayerSettings.Android.bundleVersionCode = result;
            }
        }
        else
        {
            if (BuildTarget == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = version;
            }
        }
    }

    private static void Build()
    {
        AssetbundleTree.SetAssetABNames();
        BuildPipeline.BuildAssetBundles(BuildPath, BuildAssetBundleOptions, BuildTarget);
        // BuildPipeline.BuildPlayer(EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes), BuildPath, BuildTarget, BuildOptions);
    }

    private static void SetPCVariables()
    {

    }

    private static void SetAndroidVariables()
    {
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
    }

    private static void SetIOSVariables()
    {
        PlayerSettings.useAnimatedAutorotation = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
        PlayerSettings.iOS.iOSManualProvisioningProfileID = "";
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
    }

    private static void SetPlayerSettings()
    {
        string privateScriptSymbols = System.Environment.GetEnvironmentVariable("ScriptingDefineSymbols");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, privateScriptSymbols);
        PlayerSettings.applicationIdentifier = BuildConfig.GetString("applicationIdentifier", "com.nullspace.test");
        PlayerSettings.productName = BuildConfig.GetString("productName", "nullspace");
        PlayerSettings.companyName = BuildConfig.GetString("companyName", "nullspace");
    }

}
