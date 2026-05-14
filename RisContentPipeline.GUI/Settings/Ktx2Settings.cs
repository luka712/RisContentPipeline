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
    /// The enum representing the encoding quality for the KTX2 textures.
    /// </summary>
    internal enum Ktx2BasisUEncodingQuality : uint
    {
        Lowest = 32,
        Low = 64,
        Medium = 128,
        High = 196,
        Best = 255
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
        /// If <c>true</c>, the KTX2 textures will be generated with mipmaps.
        /// </summary>
        public bool GenerateMipmaps { get; set; }

        /// <summary>
        /// The target encoding format for KTX2 textures. 
        /// If set to Basis, the textures will be encoded to Basis format during the build process.
        /// If set to NoEncoding, the textures will not be encoded and will be processed as-is.
        /// The default value is Basis.
        /// </summary>
        public Ktx2EncodingTarget EncodeTarget { get; set; }
        
        /// <summary>
        /// The encoding quality for the KTX2 textures.
        /// Low values result in smaller file sizes, but may result in lower quality.
        /// High values result in larger file sizes, but may result in better quality.
        /// Common:
        /// Lowest - 32
        /// Low - 64
        /// Medium - 128
        /// High - 196
        /// Best - 255
        /// </summary>
        public Ktx2BasisUEncodingQuality QualityLevel { get; set; } = Ktx2BasisUEncodingQuality.Medium;

        /// <summary>
        /// Copies the current settings.
        /// </summary>
        /// <returns>The copy of settings.</returns>
        public Ktx2Settings Copy()
        {
            return new Ktx2Settings()
            {
                UseUastc = UseUastc,
                EncodeTarget = EncodeTarget,
                GenerateMipmaps = GenerateMipmaps,
                QualityLevel = QualityLevel,
            };
        }
    }
}
