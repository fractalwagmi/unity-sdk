namespace FractalSDK.Core
{
    public abstract class FractalConstants
    {
        //API URLs
        public const string AuthAPIRootURL = "https://auth-api.fractal.is";
        public const string APIRootURL = "https://api.fractal.is";
        public const string GetURL = "/auth/signin_v2/authorize";
        public const string Verify = "/auth/signin_v2/token";
        public const string GetInfo = "/sdk/v1/wallet/info";
        public const string GetCoins = "/sdk/v1/wallet/coins";
        public const string GetItems = "/sdk/v1/wallet/items";
        public const string SignTransaction = "/sdk/transaction/authorize";

        //Fractal Sign In Button Contents
        public const string ButtonLoading = "OndraFractal";
        public const string ButtonLogin = "SIGN IN WITH FRACTAL";
    }
}

