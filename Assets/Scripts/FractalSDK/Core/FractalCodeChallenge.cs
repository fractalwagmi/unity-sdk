using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class FractalCodeChallenge
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
        var nonce = new char[128];
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
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
          .Replace('+', '-') // replace URL unsafe characters with safe ones
          .Replace('/', '_') // replace URL unsafe characters with safe ones
          .Replace("=", ""); // no padding
    }

}
