using System;
using FractalSDK.Enums;

namespace FractalSDK.Models.Api
{
    [Serializable]
    public class Coin
    {
        public Int64 decimals;
        public string uiAmount;
        public string symbol;
        public string name;
        public string address;
        public string logoUri;
        public Chain chain;
    }
}
