using System.Security.Cryptography;

public class EncryptionService
{
    private readonly string encryptionKey;
    private readonly string IV;
    public EncryptionService (IConfiguration configuration)
    {
        encryptionKey = configuration["EncryptionSettings:Key"] ?? throw new InvalidOperationException("Encryption key not found in configuration.");
        IV = configuration["EncryptionSettings:IV"] ?? throw new InvalidOperationException("IV not found in configuration.");
    }
    public string Encrypt(string input)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(encryptionKey);
        aes.IV = Convert.FromBase64String(IV);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(input);
        }
        
        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string encryptedInput)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(encryptionKey);
        aes.IV = Convert.FromBase64String(IV);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(encryptedInput));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}