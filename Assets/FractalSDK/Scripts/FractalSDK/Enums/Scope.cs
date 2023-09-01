using System.Runtime.Serialization;

namespace FractalSDK.Enums
{
    public enum Scope
    {
        /** Necessary for reading items in a wallet. */
        [EnumMember(Value = "items:read")]
        ITEMS_READ,

        /** Necessary for reading coins available in a wallet. */
        [EnumMember(Value = "coins:read")]
        COINS_READ,

        /** Necessary for identifying the user. */
        [EnumMember(Value = "identify")]
        IDENTIFY
    }
}


