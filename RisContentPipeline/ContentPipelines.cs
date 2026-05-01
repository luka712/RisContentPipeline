using RisContentPipeline.Container;
using RisKtx2;

namespace RisContentPipeline
{
    /// <summary>
    /// The main API class for the content pipeline. 
    /// </summary>
    public class ContentPipelines
    {
        private readonly ImageToKtx2Pipeline _ktx2Pipeline = new();

        /// <summary>
        /// Converts an ImageContainer to a Ktx2Texture using the provided KtxBasisParams.
        /// </summary>
        /// <param name="imageContainer">The <see cref="ImageContainer"/>.</param>
        /// <param name="basisParams">The <see cref="KtxBasisParams"/>.</param>
        /// <returns>The <see cref="Ktx2Texture"/>.</returns>
        public Ktx2Texture CovertToKtx2(ImageContainer imageContainer, KtxBasisParams? basisParams, string? outputFilePath = null)
        {
            var ktxTexture = _ktx2Pipeline.Convert(imageContainer, basisParams);
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                ktxTexture.WriteToNamedFile(outputFilePath);
            }
            return ktxTexture;
        }

        /// <summary>
        /// Converts an ImageContainer to a Ktx2Texture using the provided quality level (0-255).
        /// </summary>
        /// <param name="imageContainer"></param>
        /// <param name="quality"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public Ktx2Texture CovertToKtx2(ImageContainer imageContainer, uint quality, string? outputFilePath = null)
        {
            var ktxTexture = _ktx2Pipeline.Convert(imageContainer, quality);
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                ktxTexture.WriteToNamedFile(outputFilePath);
            }
            return ktxTexture;
        }
    }
}
