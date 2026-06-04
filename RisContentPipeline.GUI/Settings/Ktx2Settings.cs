namespace RisContentPipeline.GUI.Settings
{
    /// <summary>
    /// The enum representing the target encoding format for KTX2 textures.
    /// It indicates whether the textures should be encoded to Basis format during the build process or not.
    /// </summary>
    public enum Ktx2EncodingTarget
    {
        NO_ENCODING = 0,

        // ReSharper disable once InconsistentNaming
        BASIS_ETC1S = 1,
        BASIS_UASTC = 2,
    }

    /// <summary>
    /// The enum representing the encoding quality for the KTX2 textures.
    /// This is the quality of ETC1S compression.
    /// </summary>
    public enum Ktx2EncodingQuality : uint
    {
        LOWEST,
        LOW,
        MEDIUM,
        HIGH,
        BEST
    }

    // TODO: doc comment
    internal class Ktx2SettingsLookup
    {
        private static readonly Dictionary<Ktx2EncodingQuality, uint> _encodingQualityMapUastc = new()
        {
            [Ktx2EncodingQuality.LOWEST] = 0,
            [Ktx2EncodingQuality.LOW] = 1,
            [Ktx2EncodingQuality.MEDIUM] = 2,
            [Ktx2EncodingQuality.HIGH] = 3,
            [Ktx2EncodingQuality.BEST] = 4,
        };

        private static readonly Dictionary<Ktx2EncodingQuality, uint> _encodingQualityMapEtc1s = new()
        {
            [Ktx2EncodingQuality.LOWEST] = 32,
            [Ktx2EncodingQuality.LOW] = 64,
            [Ktx2EncodingQuality.MEDIUM] = 128,
            [Ktx2EncodingQuality.HIGH] = 196,
            [Ktx2EncodingQuality.BEST] = 255,
        };

        internal static uint GetEncodingQualityLevelValue(Ktx2EncodingTarget target, Ktx2EncodingQuality qualityLevel)
        {
            if (target == Ktx2EncodingTarget.BASIS_ETC1S)
            {
                return _encodingQualityMapEtc1s[qualityLevel];
            }

            return _encodingQualityMapUastc[qualityLevel];
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
        /// </summary>
        public Ktx2EncodingQuality QualityLevel { get; set; } = Ktx2EncodingQuality.MEDIUM;

        /// <summary>
        /// Gets the quality level value for the KTX2 texture,
        /// based on the selected encoding target and encoding quality.
        /// </summary>
        /// <returns>The value of quality level.</returns>
        public uint GetQualityLevelValue()
            => Ktx2SettingsLookup.GetEncodingQualityLevelValue(EncodeTarget, QualityLevel);

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