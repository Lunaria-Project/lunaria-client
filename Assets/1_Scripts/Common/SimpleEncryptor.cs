using System.Text;

public static class SimpleEncryptor
{
    public static byte[] EncryptToBytes(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key)) return null;

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var resultBytes = new byte[plainBytes.Length];

        for (var index = 0; index < plainBytes.Length; index++)
        {
            var keyByte = keyBytes[index % keyBytes.Length];
            resultBytes[index] = (byte)(plainBytes[index] ^ keyByte);
        }

        return resultBytes;
    }

    public static string DecryptToString(byte[] encryptedBytes, string key)
    {
        if (encryptedBytes == null || encryptedBytes.Length == 0 || string.IsNullOrEmpty(key)) return string.Empty;

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var resultBytes = new byte[encryptedBytes.Length];

        for (var index = 0; index < encryptedBytes.Length; index++)
        {
            var keyByte = keyBytes[index % keyBytes.Length];
            resultBytes[index] = (byte)(encryptedBytes[index] ^ keyByte);
        }

        return Encoding.UTF8.GetString(resultBytes);
    }
}