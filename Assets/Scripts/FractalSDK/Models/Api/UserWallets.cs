using System;
using FractalSDK.Enums;

namespace FractalSDK.Models.Api
{
    [Serializable]
    public class UserWallets
    {
        public string address;
        public Chain chain;
    }
}