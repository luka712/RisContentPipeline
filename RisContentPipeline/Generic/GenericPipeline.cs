namespace RisContentPipeline.Generic;

/// <summary>
/// Defines a generic pipeline.
/// Use this class to create a custom pipeline.
/// </summary>
public class GenericPipeline : APipeline
{
    private readonly Func<object, object?, PipelineResult> _convertAction;
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="name">The name of the pipeline.</param>
    /// <param name="sourceTypes">The source types.</param>
    /// <param name="targetTypes">The target types.</param>
    /// <param name="convertAction">The convert action.</param>
    public GenericPipeline(string name, string[] sourceTypes, string[] targetTypes,
        Func<object, object?, PipelineResult> convertAction
    )
        : base(name, sourceTypes, targetTypes)
    {
        _convertAction = convertAction ?? throw new ArgumentNullException(nameof(convertAction));
    }

    /// <summary>
    /// Converts the source to the target type.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="options">Teh options.</param>
    /// <returns>The <see cref="PipelineResult"/>.</returns>
    public override PipelineResult Convert(object source, object? options)
    {
        return _convertAction(source, options);
    }
}