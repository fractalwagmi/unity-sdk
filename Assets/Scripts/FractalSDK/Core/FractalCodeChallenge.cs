using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class FractalCodeChallange
{
    public string CodeVerifier;

    public string CodeChallenge;

    public void Init()
    {
        CodeVerifier = GenerateNonce();
        CodeChallenge = GenerateCodeChallenge(CodeVerifier);
    }

    public string GenerateNonce()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
        var random = new Random();
        var nonce = new char[32];
        for (int i = 0; i < nonce.Length; i++)
        {
            nonce[i] = chars[random.Next(chars.Length)];
        }

        return new string(nonce);
    }

    public string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        var b64Hash = Convert.ToBase64String(hash);
        return b64Hash;
    }

}
