using System.Security.Cryptography;
using System.Text;

namespace pandx.Wheel.Security;

public class SimpleStringCipher
{
    private const int DefaultKeySize = 256;
    private readonly byte[] _initVectorBytes;

    static SimpleStringCipher()
    {
        DefaultSalt = Encoding.ASCII.GetBytes("9ucyfQic");
        DefaultInitVectorBytes = Encoding.ASCII.GetBytes("4mMefcLPMbz1Va0S");
        DefaultPassPhrase = "msh3hQSe7GHqyE9U";
        Instance = new SimpleStringCipher();
    }

    private SimpleStringCipher()
    {
        _initVectorBytes = DefaultInitVectorBytes;
    }

    public static SimpleStringCipher Instance { get; }
    private static string DefaultPassPhrase { get; }
    private static byte[] DefaultInitVectorBytes { get; }
    private static byte[] DefaultSalt { get; }

    public string Encrypt(string plainText, string? passPhrase = null, byte[]? salt = null, int? keySize = null,
        byte[]? initVectorBytes = null)
    {
        passPhrase = passPhrase ?? DefaultPassPhrase;
        salt = salt ?? DefaultSalt;
        keySize = keySize ?? DefaultKeySize;
        initVectorBytes = initVectorBytes ?? _initVectorBytes;

        byte[] encrypted;

        using var password = new Rfc2898DeriveBytes(passPhrase, salt, 8, HashAlgorithmName.SHA256);
        var keyBytes = password.GetBytes(keySize.Value / 8);

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = initVectorBytes;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string cipherText, string? passPhrase = null, byte[]? salt = null, int? keySize = null,
        byte[]? initVectorBytes = null)
    {
        passPhrase = passPhrase ?? DefaultPassPhrase;
        salt = salt ?? DefaultSalt;
        keySize = keySize ?? DefaultKeySize;
        initVectorBytes = initVectorBytes ?? _initVectorBytes;

        string plaintext;

        using var password = new Rfc2898DeriveBytes(passPhrase, salt, 8, HashAlgorithmName.SHA256);
        var keyBytes = password.GetBytes(keySize.Value / 8);

        // ReSharper disable once ConvertToUsingDeclaration
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = initVectorBytes;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}