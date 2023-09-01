using System.Runtime.Serialization;

namespace FractalSDK.Enums
{
    public enum Chain
    {
        [EnumMember(Value = "UNKNOWN")]
        UNKNOWN,

        [EnumMember(Value = "ETHEREUM")]
        ETHEREUM,

        [EnumMember(Value = "SOLANA")]
        SOLANA,

        [EnumMember(Value = "POLYGON")]
        POLYGON
    }
}

