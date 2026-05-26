namespace RisContentPipeline.GUI.Settings
{
    /// <summary>
    /// The enum representing the target encoding format for KTX2 textures.
    /// It indicates whether the textures should be encoded to Basis format during the build process or not.
    /// </summary>
    public enum Ktx2EncodingTarget
    {
        NO_ENCODING,
        // ReSharper disable once InconsistentNaming
        BASIS_ETC1S,
        BASIS_UASTC,
    }

    /// <summary>
    /// The enum representing the encoding quality for the KTX2 textures.
    /// </summary>
    public enum Ktx2BasisUEncodingQuality : uint
    {
        LOWEST = 32,
        LOW = 64,
        MEDIUM = 128,
        HIGH = 196,
        BEST = 255
    }

    // TODO: doc comment
    internal class Ktx2SettingsLookup
    {
        private static readonly Dictionary<int, Ktx2BasisUEncodingQuality> _encodingQualityMap = new()
        {
            { 0, Ktx2BasisUEncodingQuality.LOWEST },
            { 1, Ktx2BasisUEncodingQuality.LOW },
            { 2, Ktx2BasisUEncodingQuality.MEDIUM },
            { 3, Ktx2BasisUEncodingQuality.HIGH },
            { 4, Ktx2BasisUEncodingQuality.BEST },
        };
        
        // TODO: doc comment
        internal static Ktx2BasisUEncodingQuality GetEncodingQualityLevel(int index)
        {
            return _encodingQualityMap[index];
        }

        // TODO: doc comment
        internal static int GetIndex(Ktx2BasisUEncodingQuality qualityLevel)
        {
            for (int i = 0; i < _encodingQualityMap.Count; i++)
            {
                if (_encodingQualityMap[i] == qualityLevel)
                {
                    return i;
                }
            }
            
            throw new ArgumentOutOfRangeException(nameof(qualityLevel), qualityLevel, null);
        }
        
       
    }
    
    /// <summary>
    /// This class represents the settings related to KTX2 texture conversion, specifically the choice between UASTC and ETC1S compression formats.
    /// </summary>
    public class Ktx2Settings
    {
        
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
        public Ktx2EncodingTarget EncodeTarget { get; set; } = Ktx2EncodingTarget.BASIS_UASTC;
        
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
        public Ktx2BasisUEncodingQuality QualityLevel { get; set; } = Ktx2BasisUEncodingQuality.MEDIUM;

        /// <summary>
        /// Copies the current settings.
        /// </summary>
        /// <returns>The copy of settings.</returns>
        public Ktx2Settings Copy()
        {
            return new Ktx2Settings()
            {
                EncodeTarget = EncodeTarget,
                GenerateMipmaps = GenerateMipmaps,
                QualityLevel = QualityLevel,
            };
        }
    }
}
