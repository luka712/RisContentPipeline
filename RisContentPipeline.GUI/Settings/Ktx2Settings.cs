namespace RisContentPipeline.GUI.Settings
{
    /// <summary>
    /// The enum representing the target encoding format for KTX2 textures.
    /// It indicates whether the textures should be encoded to Basis format during the build process or not.
    /// </summary>
    internal enum Ktx2EncodingTarget
    {
        Basis,
        NoEncoding,
    }

    /// <summary>
    /// This class represents the settings related to KTX2 texture conversion, specifically the choice between UASTC and ETC1S compression formats.
    /// </summary>
    internal class Ktx2Settings
    {
        /// <summary>
        /// If true, the KTX2 textures will be compressed using the UASTC format, 
        /// which provides better quality at the cost of larger file sizes. 
        /// If false, the textures will be compressed using the ETC1S format, which is more efficient but may result in lower quality. The default value is false (ETC1S).
        /// </summary>
        public bool UseUastc { get; set; }

        /// <summary>
        /// The target encoding format for KTX2 textures. 
        /// If set to Basis, the textures will be encoded to Basis format during the build process.
        /// If set to NoEncoding, the textures will not be encoded and will be processed as-is.
        /// The default value is Basis.
        /// </summary>
        public Ktx2EncodingTarget EncodeTarget { get; set; }
    }
}
