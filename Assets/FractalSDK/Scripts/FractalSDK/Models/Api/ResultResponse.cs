using System;

namespace FractalSDK.Models.Api
{
    [Serializable]
    public class ResultResponse
    {
        public string bearerToken;
        public string userId;
        public string refreshToken;
    }
}
