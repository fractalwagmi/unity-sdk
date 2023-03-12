namespace FractalSDK.Core
{
    public abstract class FractalConstants
    {
        //API URLs
        public const string AuthAPIRootURL = "https://auth-api.fractal.is";
        public const string APIRootURL = "https://api.fractal.is";
        public const string GetURL = "/auth/v2/approval/geturl";
        public const string Verify = "/auth/v2/approval/result";
        public const string GetInfo = "/sdk/v1/wallet/info";
        public const string GetCoins = "/sdk/v1/wallet/coins";
        public const string GetItems = "/sdk/v1/wallet/items";

        //Fractal Sign In Button Contents
        public const string ButtonLoading = "LOADING...";
        public const string ButtonLogin = "SIGN IN WITH FRACTAL";
    }
}

