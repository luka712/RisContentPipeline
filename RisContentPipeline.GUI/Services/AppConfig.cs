namespace RisContentPipeline.GUI.Services;

public class AppConfig
{
    /// <summary>
    /// Indicates whether the application is self-contained.
    /// </summary>
    public bool IsSelfContained { get; private set; } = Constants.IS_SELF_CONTAINED;
}