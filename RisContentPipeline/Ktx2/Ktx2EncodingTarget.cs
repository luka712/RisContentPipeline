namespace RisContentPipeline.Ktx2;

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
    
    ASTC_4X4 = 3
}