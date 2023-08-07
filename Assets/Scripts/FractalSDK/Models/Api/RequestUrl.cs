using System;
using System.Collections.Generic;
using FractalSDK.Enums;

namespace FractalSDK.Models.Api
{
    [Serializable]
    public class RequestUrl
    {
        public string clientId;
        public string codeChallenge;
        public List<string> scopes;
    }
}
