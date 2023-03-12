using System;

namespace FractalSDK.Models.Api
{
    [Serializable]
    public class UserInfo
    {
        public string accountPublicKey;
        public string email;
        public string username;
        public UserWallets[] wallets;
    }
}
