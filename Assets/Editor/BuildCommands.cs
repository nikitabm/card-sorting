using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildCommands : MonoBehaviour
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        var scenes = new List<string>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(i);
            scenes.Add(scene.path);
        }
        buildPlayerOptions.scenes = scenes.ToArray();
        buildPlayerOptions.locationPathName = "build/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded {summary.totalSize} bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log($"Build failed");
        }
    }
}
