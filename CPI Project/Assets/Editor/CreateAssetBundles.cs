using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Disney.Kelowna.Common;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Project/AssetBundles/Generated/Generate client side AssetBundles")]
    static void BuildAllAssetBundles()
    {
        // Detect platform and switch it
        string platform = DetectAndSwitchPlatform();

        if (platform == "unknown")
        {
            Debug.LogError("Unknown platform, aborting Asset Bundle generation.");
            return; // Exit if platform is unknown
        }

        // Modify necessary assets (client_info.asset, embedded_content_manifest.txt)
        ModifyClientInfoAsset(platform);
        ModifyTextFile(platform);

        // Now generate the Asset Bundles
        List<AssetBundleBuild> validAssetBundles = new List<AssetBundleBuild>();

        foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
        {
            if (!assetBundleName.StartsWith("CDN/", System.StringComparison.OrdinalIgnoreCase))
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = assetBundleName;
                build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                validAssetBundles.Add(build);
            }
        }

        string assetBundleDirectory = "";

#if UNITY_ANDROID
        assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/android";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, validAssetBundles.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);

#elif UNITY_STANDALONE_OSX
        assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standaloneosx";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, validAssetBundles.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);

#elif UNITY_STANDALONE_WIN
        assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standalonewindows64";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, validAssetBundles.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

#elif UNITY_STANDALONE_LINUX
        assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standalonelinux64";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, validAssetBundles.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);

#elif UNITY_WEBGL
        assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/webgl";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, validAssetBundles.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
#endif

        CleanupPlatformBundles(assetBundleDirectory);

        // Save and refresh the project
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Project saved and refreshed after cleanup.");
    }

    // Platform detection logic
    private static string DetectAndSwitchPlatform()
    {
        string platform = "";

#if UNITY_STANDALONE_WIN
        platform = "standalonewindows64";
#elif UNITY_STANDALONE_OSX
        platform = "standaloneosx";
#elif UNITY_STANDALONE_LINUX
        platform = "standalonelinux64";
#elif UNITY_ANDROID
        platform = "android";
#elif UNITY_WEBGL
        platform = "webgl";
#else
        platform = "unknown";
#endif

        if (platform != "unknown")
        {
            Debug.Log("Detected platform: " + platform + ". Switching platform...");
        }

        return platform;
    }

    // Modify the client_info.asset file based on the platform
    private static void ModifyClientInfoAsset(string platform)
    {
        var clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>("Assets/Generated/Resources/Configuration/client_info.asset");

        if (clientInfo != null)
        {
            SerializedObject serializedObject = new SerializedObject(clientInfo);
            SerializedProperty platformProperty = serializedObject.FindProperty("Platform");
            platformProperty.stringValue = platform;

            serializedObject.ApplyModifiedProperties();
            Debug.Log("Platform set to: " + platform + " in client_info.asset");
        }
        else
        {
            Debug.LogError("client_info.asset not found.");
        }
    }

    // Modify the embedded_content_manifest.txt file with the platform name
    private static void ModifyTextFile(string platform)
    {
        string txtFilePath = "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt";

        if (File.Exists(txtFilePath))
        {
            string[] lines = File.ReadAllLines(txtFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("standalonewindows64") ||
                    lines[i].Contains("standalonelinux64") ||
                    lines[i].Contains("standaloneosx") ||
                    lines[i].Contains("android") ||
                    lines[i].Contains("webgl"))
                {
                    // Skip specific lines that break the game.
                    if (lines[i].Contains("asset:worldtraynodeprefabs/androidcontainer?dl=res&x=prefab") ||
                        lines[i].Contains("asset:mockauthproject/assetbundles/android/test_cube?dl=res&x=unity3d") ||
						lines[i].Contains("asset:swrveassets.xcassets/app_icons.imageset/1.0.0_android?dl=res&x=png") ||
						lines[i].Contains("asset:swrveassets.xcassets/app_icons.imageset/androidgo?dl=res&x=png") ||
						lines[i].Contains("asset:swrveassets.xcassets/app_icons.imageset/round_android?dl=res&x=png"))
                    {
                        continue; // Skip these lines
                    }

                    lines[i] = lines[i].Replace("standalonewindows64", platform)
                                       .Replace("standalonelinux64", platform)
                                       .Replace("standaloneosx", platform)
                                       .Replace("android", platform)
                                       .Replace("webgl", platform);
                }
            }

            File.WriteAllLines(txtFilePath, lines);
            Debug.Log($"Text file updated with platform: {platform}");
        }
        else
        {
            Debug.LogError(".txt file not found.");
        }
    }

    // Ensure the directory exists and clear its contents if it does
    static void EnsureAndClearDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            // Create directory if it doesn't exist
            Directory.CreateDirectory(path);
        }
        else
        {
            // Clear directory if it exists
            ClearDirectory(path);
        }
    }

    // Function to clear the directory
    static void ClearDirectory(string path)
    {
        // Delete all files in the directory
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        // Delete all subdirectories in the directory
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }

    // Function to clean up platform-specific bundles
    static void CleanupPlatformBundles(string rootPath)
    {
        if (!Directory.Exists(rootPath)) return;

        string[] unwantedKeywords = new string[]
        {
            "android",
            "standalonelinux64",
            "standaloneosx",
            "standalonewindows64",
            "webgl"
        };

        string[] allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);
        foreach (string file in allFiles)
        {
            string fileName = Path.GetFileName(file).ToLower();

            bool shouldDelete = fileName.EndsWith(".manifest");
            if (!shouldDelete)
            {
                foreach (string keyword in unwantedKeywords)
                {
                    if (fileName.Contains(keyword))
                    {
                        shouldDelete = true;
                        break;
                    }
                }
            }

            // Delete the Asset Bundle files and their corresponding .meta files
            if (shouldDelete)
            {
                // Delete the corresponding .meta file
                string metaFilePath = file + ".meta";
                if (File.Exists(metaFilePath))
                {
                    File.Delete(metaFilePath);
                    Debug.Log($"Deleted .meta: {metaFilePath}");
                }

                // Delete the actual Asset Bundle (or manifest)
                File.Delete(file);
                Debug.Log($"Deleted: {file}");

                string platformMetaFile = Path.Combine(rootPath, Path.GetFileNameWithoutExtension(file) + ".meta");
                if (File.Exists(platformMetaFile))
                {
                    File.Delete(platformMetaFile);
                    Debug.Log($"Deleted platform .meta: {platformMetaFile}");
                }
            }
        }
    }
}
