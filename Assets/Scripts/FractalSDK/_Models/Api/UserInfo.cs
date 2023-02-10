using System;

[Serializable]
public class UserInfo
{
    public string accountPublicKey;
    public string email;
    public string username;
    public UserWallets[] wallets;
}
