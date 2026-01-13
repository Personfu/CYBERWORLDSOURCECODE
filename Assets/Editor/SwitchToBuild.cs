using System.IO;
using UnityEditor;
using UnityEngine;
using Disney.Kelowna.Common;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SwitchToBuild : MonoBehaviour
{
    [MenuItem("Project/Run Before Build")]
    public static void SwitchPlatform()
    {
        string platform = "";

#if UNITY_STANDALONE_WIN
        platform = "standalonewindows64";
#elif UNITY_STANDALONE_OSX
        platform = "standaloneosx";
#elif UNITY_STANDALONE_LINUX || UNITY_STANDALONE_LINUX64
        platform = "standalonelinux64";
#elif UNITY_ANDROID
        platform = "android";
#elif UNITY_WEBGL
        platform = "webgl";
#else
        platform = "unknown";
#endif

        if (platform == "unknown")
        {
            Debug.LogError("Unknown platform, aborting switch.");
            return;
        }

        // Modify the client_info.asset file
        ModifyClientInfoAsset(platform);

        // Modify the embedded_content_manifest.txt file
        ModifyTextFile(platform);

        // Save assets
        AssetDatabase.SaveAssets();

        Debug.Log("SwitchToBuild: Platform switch completed for " + platform);
    }

    private static void ModifyClientInfoAsset(string platform)
    {
        var clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>("Assets/Generated/Resources/Configuration/client_info.asset");

        if (clientInfo != null)
        {
            SerializedObject serializedObject = new SerializedObject(clientInfo);
            SerializedProperty platformProperty = serializedObject.FindProperty("Platform");
            platformProperty.stringValue = platform;

            // Save the updated asset
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
                                       .Replace("webgl", platform)
                                       .Replace("android", platform);
                }
            }

            File.WriteAllLines(txtFilePath, lines);
            Debug.Log("Text file updated with platform: " + platform);
        }
        else
        {
            Debug.LogError(".txt file not found.");
        }
    }
}

public class SwitchToBuildHook : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        SwitchToBuild.SwitchPlatform();
    }
}
