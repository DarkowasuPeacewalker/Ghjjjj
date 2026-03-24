using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class BuildScript
{
    private const string DefaultOutputDir = "Builds";

    public static void BuildWindows()
    {
        BuildForTarget(BuildTarget.StandaloneWindows64, "NazarickEscape.exe");
    }

    public static void BuildLinux()
    {
        BuildForTarget(BuildTarget.StandaloneLinux64, "NazarickEscape.x86_64");
    }

    private static void BuildForTarget(BuildTarget target, string executableName)
    {
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("Нет включённых сцен в Build Settings.");
        }

        string outputDir = Path.Combine(DefaultOutputDir, target.ToString());
        Directory.CreateDirectory(outputDir);
        string outputPath = Path.Combine(outputDir, executableName);

        BuildPlayerOptions options = new()
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Сборка неуспешна: {report.summary.result}");
        }

        UnityEngine.Debug.Log($"Сборка готова: {outputPath}");
    }

    private static string[] GetEnabledScenes()
    {
        var enabledScenes = new System.Collections.Generic.List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }

        return enabledScenes.ToArray();
    }
}
