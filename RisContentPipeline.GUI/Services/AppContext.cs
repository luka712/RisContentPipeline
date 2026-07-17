using System.Collections.ObjectModel;
using System.Text.Json;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Models;
using RisContentPipeline.GUI.ViewModels;
using RisContentPipeline.Ktx2;
using RisKtx2;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// The application-wide context shared by ViewModels.
/// Manages assets, scripts, preferences, and the build pipeline.
/// </summary>
public class PipelineContext : IDisposable
{
    private const string INTERNAL_SCRIPTS_DIRECTORY = "InternalScripts";
    private const string SESSION_FILE = "session.json";

    

    public event Action? OnBuildStarted;
    public event Action? OnBuildFinished;

    // public PipelineContext()
    // {
    //     LoadInternalScripts();
    //
    //     PipelineSystem.OnConvertAllStarted += (_, args) =>
    //     {
    //         LogInfo($"Build started. Total items: {args.TotalItems}");
    //     };
    //
    //     PipelineSystem.OnConvertAllFinished += (_, args) =>
    //     {
    //         LogInfo($"Build finished. Total items: {args.TotalItems}");
    //     };
    // }








    // public async Task LoadPreferencesAsync()
    // {
    //     try
    //     {
    //         Preferences = await Preferences.LoadAsync();
    //     }
    //     catch (Exception ex)
    //     {
    //         LogError($"Unable to load preferences: {ex.Message}");
    //     }
    // }
    //
    // public Task SavePreferencesAsync() => Preferences.SaveAsync();
    

    public void SaveSession()
    {
        var session = new Session {  };
        var json = JsonSerializer.Serialize(session);
        File.WriteAllText(SESSION_FILE, json);
    }

    private void LoadInternalScripts()
    {
        var directory = ResolveScriptDirectory(INTERNAL_SCRIPTS_DIRECTORY);
        if (directory == null) return;

        foreach (var file in Directory.EnumerateFiles(directory, "*.py", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileName(file);
            if (name.StartsWith("base_", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("__", StringComparison.Ordinal))
                continue;
        }
    }

    private static string? ResolveScriptDirectory(string relativeDirectory)
    {
        var fromCwd = Path.GetFullPath(relativeDirectory);
        if (Directory.Exists(fromCwd)) return fromCwd;

        var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (assemblyDir != null)
        {
            var fromAssembly = Path.Combine(assemblyDir, relativeDirectory);
            if (Directory.Exists(fromAssembly)) return fromAssembly;
        }

        return null;
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
