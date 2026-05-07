namespace RisContentPipeline;

/// <summary>
/// The base class for all pipelines in the content pipeline.
/// </summary>
public abstract class APipeline : IPipeline
{
    private readonly string[] _sourceTypes;
    private readonly string[] _targetTypes;
    
    /// <summary>
    /// The constructor takes the source and target types that this pipeline can process.
    /// </summary>
    /// <param name="name">The name of the pipeline. Used as unique identifier of pipeline.</param>
    /// <param name="sourceTypes">The source types.</param>
    /// <param name="targetTypes">The target types.</param>
    protected APipeline(string name, string[] sourceTypes, string[] targetTypes)
    {
        Name = name;
        _sourceTypes = sourceTypes;
        _targetTypes = targetTypes;
    }
    
    /// <inheritdoc/>
    public string Name { get; } 
    
    /// <inheritdoc/>
    public IReadOnlyList<string> SourceTypes => _sourceTypes;
    
    /// <inheritdoc/>
    public IReadOnlyList<string> TargetTypes => _targetTypes;

    /// <inheritdoc/>
    public bool CanConvert(string sourceType, string targetType)
        => _sourceTypes.Contains(sourceType) && _targetTypes.Contains(targetType);

    /// <inheritdoc/>
    public abstract PipelineResult Convert(object source, object? options);
}