using CommunityToolkit.Mvvm.ComponentModel;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for the KTX2 settings.
/// </summary>
public partial class Ktx2SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private Ktx2EncodingTarget _encodingTarget = Ktx2EncodingTarget.BASIS_ETC1S;
    
    [ObservableProperty]
    private bool _generateMipmaps = true;
    
    [ObservableProperty]
    private bool _flipY;

    [ObservableProperty]
    private int _qualityLevel = 3;
    
    
    /// <summary>
    /// Gets the quality level value for ETC1S encoding.
    /// </summary>
    public int GetQualityLevelValue()
    {
        return QualityLevel switch
        {
            0 => 1,
            1 => 64,
            2 => 128,
            3 => 192,
            4 => 224,
            5 => 255,
            _ => 128
        };
    }

    /// <summary>
    /// Gets the UASTC quality level flags.
    /// </summary>
    public uint GetUastcQualityLevelValue()
    {
        return (uint) QualityLevel;
    }
    
    /// <summary>
    /// Creates a copy of these settings.
    /// </summary>
    public Ktx2SettingsViewModel Copy()
    {
        return new Ktx2SettingsViewModel
        {
            EncodingTarget = EncodingTarget,
            GenerateMipmaps = GenerateMipmaps,
            FlipY = FlipY,
            QualityLevel = QualityLevel,
        };
    }
    
}