using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class IntroVideoStreamingAssetsBuildProcessor : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPreprocessBuild(BuildReport report)
	{
		if (report == null || report.summary.platform != BuildTarget.WebGL)
		{
			return;
		}

		string source = Path.Combine(Application.dataPath, "Game/Resources/IntroVideo/IntroVideo.webm");
		if (!File.Exists(source))
		{
			return;
		}

		string destDir = Path.Combine(Application.dataPath, "StreamingAssets/IntroVideo");
		Directory.CreateDirectory(destDir);

		string dest = Path.Combine(destDir, "IntroVideo.webm");
		File.Copy(source, dest, true);
	}
}
