namespace RisContentPipeline.Exceptions;

/// <summary>
/// Indicates that an error occurred during conversion.
/// </summary>
public class ConversionException : InvalidOperationException
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    public ConversionException(Exception innerException) 
        : base(innerException.Message, innerException)
    {
        
    }
    
    /// <summary>
    /// The source type.
    /// </summary>
    public string SourceType { get; set; } = string.Empty;
    
    /// <summary>
    /// The target type.
    /// </summary>
    public string TargetType { get; set; } = string.Empty;
    
    /// <summary>
    /// The source file path.
    /// </summary>
    public string SourceFilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// The target file path.
    /// </summary>
    public string TargetFilePath { get; set; } = string.Empty;
}