using System.Collections.ObjectModel;
using System.Text.Json;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Models;
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

    public IPipelineSystem PipelineSystem { get; } = new PipelineSystem();

    public ObservableCollection<AssetFileOrFolder> Assets { get; } = [];
    public ObservableCollection<Script> BuildScripts { get; } = [];
    public ObservableCollection<Script> InternalScripts { get; } = [];
    public ObservableCollection<LogMessage> Messages { get; } = [];

    public Preferences Preferences { get; private set; } = new();

    public event Action? OnBuildStarted;
    public event Action? OnBuildFinished;

    public PipelineContext()
    {
        LoadInternalScripts();

        PipelineSystem.OnConvertAllStarted += (_, args) =>
        {
            LogInfo($"Build started. Total items: {args.TotalItems}");
        };

        PipelineSystem.OnConvertAllFinished += (_, args) =>
        {
            LogInfo($"Build finished. Total items: {args.TotalItems}");
        };
    }

    public void LogInfo(string message) => Messages.Add(LogMessage.Info(message));
    public void LogWarning(string message) => Messages.Add(LogMessage.Warning(message));
    public void LogError(string message) => Messages.Add(LogMessage.Error(message));
    public void ClearMessages() => Messages.Clear();

    public void AddFile(string filePath)
    {
        if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            AddPngFile(filePath);
        }
        else if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                 filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            AddGenericFile(filePath);
        }
    }

    private void AddPngFile(string filePath)
    {
        Assets.Add(new AssetFileOrFolder
        {
            AbsolutePathOrFileName = filePath,
            Image = new ImageContainer
            {
                FilePath = filePath,
                Ktx2ExportSettings = Preferences.Ktx2GlobalSettings.Copy()
            }
        });
    }

    private void AddGenericFile(string filePath)
    {
        Assets.Add(new AssetFileOrFolder { AbsolutePathOrFileName = filePath });
    }

    public void RemoveAsset(int index)
    {
        if (index >= 0 && index < Assets.Count)
            Assets.RemoveAt(index);
    }

    public void AddBuildScript(Script script)
    {
        if (!BuildScripts.Any(s => s.FilePath == script.FilePath))
            BuildScripts.Add(script);
    }

    public void RemoveBuildScript(Script script) => BuildScripts.Remove(script);

    public async Task BuildAsync()
    {
        var buildDirectory = Preferences.BuildDirectory;

        try
        {
            if (!Directory.Exists(buildDirectory))
                Directory.CreateDirectory(buildDirectory);

            OnBuildStarted?.Invoke();
            ClearMessages();
            PipelineSystem.ClearStoredAssets();

            foreach (var asset in Assets)
            {
                QueueFileForBuild(asset, buildDirectory);
            }

            await PipelineSystem.ConvertAllAsync();
            OnBuildFinished?.Invoke();
        }
        catch (Exception ex)
        {
            LogError($"Build failed: {ex.Message}");
            throw;
        }
    }

    private void QueueFileForBuild(AssetFileOrFolder asset, string buildDirectory)
    {
        var fileName = asset.PathOrFileName;
        if (string.IsNullOrEmpty(fileName)) return;

        if (asset.Image != null)
        {
            var settings = asset.Image.Ktx2ExportSettings;
            var outputPath = Path.Combine(buildDirectory, Path.GetFileNameWithoutExtension(fileName) + ".ktx2");

            var options = new Ktx2PipelineOptions
            {
                GenerateMipmaps = settings.GenerateMipmaps,
                OutputPath = outputPath,
                Encoding = settings.EncodeTarget,
                FlipY = settings.FlipY
            };

            if (settings.EncodeTarget == Ktx2EncodingTarget.BASIS_UASTC)
                options.UastcFlags = (KtxUastcFlags)settings.GetUastcQualityLevelValue();
            else if (settings.EncodeTarget == Ktx2EncodingTarget.BASIS_ETC1S)
                options.QualityLevel = (uint)settings.GetQualityLevelValue();
            else if (settings.EncodeTarget == Ktx2EncodingTarget.ASTC_4X4)
                options.AstcQuality = (KtxPackAstcQualityLevels)settings.GetQualityLevelValue();

            PipelineSystem.StoreSourceAsset("png", "ktx2", new Ktx2PipelineSource { FilePath = asset.AbsolutePathOrFileName }, options);
        }
        else if (asset.IsJson || asset.IsXml)
        {
            var fileType = Path.GetExtension(asset.AbsolutePathOrFileName).TrimStart('.');
            PipelineSystem.StoreSourceAsset(fileType, IPipeline.ANY_TYPE,
                new GenericPipelineSource { FilePath = asset.AbsolutePathOrFileName },
                new GenericPipelineOptions { OutputPath = Path.Combine(buildDirectory, fileName) });
        }
    }

    public async Task LoadPreferencesAsync()
    {
        try
        {
            Preferences = await Preferences.LoadAsync();
        }
        catch (Exception ex)
        {
            LogError($"Unable to load preferences: {ex.Message}");
        }
    }

    public Task SavePreferencesAsync() => Preferences.SaveAsync();

    public void LoadSession()
    {
        if (!File.Exists(SESSION_FILE)) return;

        try
        {
            var json = File.ReadAllText(SESSION_FILE);
            var session = JsonSerializer.Deserialize<Session>(json);
            if (session != null)
            {
                foreach (var scriptPath in session.BuildScripts.Distinct())
                {
                    if (File.Exists(scriptPath))
                        AddBuildScript(new Script(scriptPath));
                    else
                        LogWarning($"Build script '{scriptPath}' not found.");
                }
            }
        }
        catch (Exception ex)
        {
            LogError($"Failed to load session: {ex.Message}");
        }
    }

    public void SaveSession()
    {
        var session = new Session { BuildScripts = BuildScripts.Select(s => s.FilePath).Distinct().ToList() };
        var json = JsonSerializer.Serialize(session);
        File.WriteAllText(SESSION_FILE, json);
    }

    private void LoadInternalScripts()
    {
        InternalScripts.Clear();
        var directory = ResolveScriptDirectory(INTERNAL_SCRIPTS_DIRECTORY);
        if (directory == null) return;

        foreach (var file in Directory.EnumerateFiles(directory, "*.py", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileName(file);
            if (name.StartsWith("base_", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("__", StringComparison.Ordinal))
                continue;

            InternalScripts.Add(new Script(file));
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
