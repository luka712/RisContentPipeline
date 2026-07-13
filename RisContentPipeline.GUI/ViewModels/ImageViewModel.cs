using CommunityToolkit.Mvvm.ComponentModel;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for any image file.
/// </summary>
public partial class ImageViewModel : ViewModelBase
{
    /// <summary>
    /// The file path of the file.
    /// </summary>
    [ObservableProperty] 
    private string _filePath = string.Empty;

    /// <summary>
    /// The KTX2 settings to apply when converting the image to KTX2 format.
    /// </summary>
    [ObservableProperty]
    private Ktx2SettingsViewModel _ktx2Settings = new();
    
    /// <summary>
    /// The file name of the file.
    /// </summary>
    public string FileName => Path.GetFileName(FilePath);
    

}
