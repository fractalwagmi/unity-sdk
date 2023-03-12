using System;

namespace FractalSDK.Core
{
    [Serializable]
    public class FractalAPIRequestError : Exception
    {
        public FractalAPIRequestError() { }

        public FractalAPIRequestError(long code)
            : base($"HTTP Status Code: {code}")
        {

        }
    }

    [Serializable]
    public class FractalNotAuthenticated : Exception
    {
        public FractalNotAuthenticated() { }

    }

    [Serializable]
    public class FractalInvalidResponse : Exception
    {
        public FractalInvalidResponse() { }

    }

    [Serializable]
    public class FractalInvalidClientId : Exception
    {
        public FractalInvalidClientId() { }

        public FractalInvalidClientId(string clientId)
            : base($"Supplied ClientId: {clientId}")
        {

        }

    }
}