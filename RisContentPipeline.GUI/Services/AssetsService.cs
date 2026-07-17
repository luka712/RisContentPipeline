using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Models;
using RisContentPipeline.GUI.ViewModels;
using RisContentPipeline.Ktx2;
using RisKtx2;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// The service for managing the asset files.
/// </summary>
public class AssetsService
{
    private readonly IPipelineSystem _pipelineSystem = new PipelineSystem();
    private readonly MessageService _messageService;

    /// <summary>
    /// The constructor.
    /// </summary>
    public AssetsService()
    {
        _messageService = App.Services.GetService<MessageService>()!;
    }

    /// <summary>
    /// The assets.
    /// </summary>
    public ObservableCollection<AssetViewModel> Assets { get; } = [];
    
    /// <summary>
    /// The preferences view model.
    /// </summary>
    public PreferencesViewModel? PreferencesViewModel { get; set; }

    /// <summary>
    /// The queued pipeline items.
    /// </summary>
    public IReadOnlyList<QueuedPipelineItem> QueuedPipelineItems => _pipelineSystem.QueuedItems;

    /// <summary>
    /// Called when a new item is added to the assets.
    /// </summary>
    public event Action<QueuedPipelineItem>? OnItemQueued;
    
    /// <summary>
    /// Called when the build process starts.
    /// </summary>
    public event Action? OnBuildStarted;

    /// <summary>
    /// Add a PNG file to the assets.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private void AddPngFile(string filePath)
    {
        Assets.Add(new AssetViewModel()
        {
            AbsoluteFilePath = filePath,
            Preferences = PreferencesViewModel,
            BuildPath = PreferencesViewModel?.BuildDirectory ?? "",
            Image = new ImageViewModel()
            {
                FilePath = filePath,
                Ktx2Settings = PreferencesViewModel?.Ktx2Settings.Copy() ?? new ()
            }
        });
    }

    /// <summary>
    /// Add a generic file to the assets.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private void AddGenericFile(string filePath)
    {
        Assets.Add(new AssetViewModel
        {
            Preferences = PreferencesViewModel,
            AbsoluteFilePath = filePath
        });
    }

    /// <summary>
    /// Add a file to the assets.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public void AddFile(string filePath)
    {
        // Do not add a file if it's already in the list.
        if (Assets.Any(x => x.AbsoluteFilePath == filePath))
        {
            _messageService.Warning($"File '{filePath}' is already in the list.");
            return;
        }
        
        if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) 
            || filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            )
        {
            AddPngFile(filePath);
        }
        // else if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
        //          filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        // {
        //     AddGenericFile(filePath);
        // }
    }

    private void QueueFileForBuild(AssetViewModel asset)
    {
        var fileName = asset.FileName;
        if (string.IsNullOrEmpty(fileName)) return;

        if (asset.Image != null)
        {
            var settings = asset.Image.Ktx2Settings;
            var outputPath = Path.Combine(asset.BuildPath, Path.GetFileNameWithoutExtension(fileName) + ".ktx2");

            var options = new Ktx2PipelineOptions
            {
                GenerateMipmaps = settings.GenerateMipmaps,
                OutputPath = outputPath,
                Encoding = settings.EncodingTarget,
                FlipY = settings.FlipY
            };

            if (settings.EncodingTarget == Ktx2EncodingTarget.BASIS_UASTC)
            {
                options.UastcFlags = (KtxUastcFlags)settings.GetUastcQualityLevelValue();
            }
            else if (settings.EncodingTarget == Ktx2EncodingTarget.BASIS_ETC1S)
            {
                options.QualityLevel = (uint)settings.GetQualityLevelValue();
            }
            else if (settings.EncodingTarget == Ktx2EncodingTarget.ASTC_4X4)
            {
                options.AstcQuality = (KtxPackAstcQualityLevels)settings.GetQualityLevelValue();
            }

            var queuedItem = _pipelineSystem.StoreSourceAsset("png", "ktx2", new Ktx2PipelineSource { FilePath = asset.AbsoluteFilePath },
                options);
            OnItemQueued?.Invoke(queuedItem);
        }
        else if (asset.IsJson || asset.IsXml)
        {
            var fileType = Path.GetExtension(asset.AbsoluteFilePath).TrimStart('.');
            var queuedItem = _pipelineSystem.StoreSourceAsset(fileType, IPipeline.ANY_TYPE,
                new GenericPipelineSource { FilePath = asset.AbsoluteFilePath },
                new GenericPipelineOptions { OutputPath = Path.Combine(asset.BuildPath, fileName) });
            OnItemQueued?.Invoke(queuedItem);
        }
    }

    /// <summary>
    /// Remove an asset from the assets.
    /// </summary>
    /// <param name="index">The index of asset.</param>
    public void RemoveAsset(int index)
    {
        if (index >= 0 && index < Assets.Count)
        {
            Assets.RemoveAt(index);
        }
    }

    /// <summary>
    /// Remove an asset from the assets.
    /// </summary>
    /// <param name="asset">The <see cref="AssetViewModel"/> to remove.</param>
    public void RemoveAsset(AssetViewModel asset)
    {
        Assets.Remove(asset);
    }

    /// <summary>
    /// Build the assets.
    /// </summary>
    public async Task BuildAsync()
    {
        try
        {
            OnBuildStarted?.Invoke();
            _messageService.Clear();
            _messageService.Info("Building assets   ");
            
            // PipelineSystem.ClearStoredAssets();

            foreach (var asset in Assets)
            {
                QueueFileForBuild(asset);
            }

            await _pipelineSystem.ConvertAllAsync();
           // OnBuildFinished?.Invoke();
           
           _messageService.Info("Build finished   ");
        }
        catch (Exception ex)
        {
            _messageService.Error($"Build failed: {ex.Message}");
            throw;
        }
    }
}