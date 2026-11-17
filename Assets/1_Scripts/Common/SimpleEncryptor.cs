using System.Text;

/// <summary>기존 에셋 번들 인덱스의 반복 XOR 형식을 변환</summary>
public static class SimpleEncryptor
{
    /// <summary>UTF-8 문자열에 반복 XOR을 적용해 인덱스 바이트를 생성</summary>
    public static byte[] EncryptToBytes(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key)) return null;

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        return ApplyRepeatingXor(plainBytes, keyBytes);
    }

    /// <summary>반복 XOR 형식의 인덱스 바이트를 UTF-8 문자열로 복원.</summary>
    public static string DecryptToString(byte[] encryptedBytes, string key)
    {
        if (encryptedBytes == null || encryptedBytes.Length == 0) return string.Empty;
        if (string.IsNullOrEmpty(key)) return string.Empty;

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var plainBytes = ApplyRepeatingXor(encryptedBytes, keyBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>입력을 수정하지 않고 반복 XOR을 적용한 새 배열을 반환.</summary>
    private static byte[] ApplyRepeatingXor(byte[] inputBytes, byte[] keyBytes)
    {
        var resultBytes = new byte[inputBytes.Length];

        /*
        각 입력 바이트에 반복되는 키 바이트를 XOR
            키 위치는 키 바이트 길이를 기준으로 순환
        */
        for (var index = 0; index < inputBytes.Length; index++)
        {
            var keyByte = keyBytes[index % keyBytes.Length];
            resultBytes[index] = (byte)(inputBytes[index] ^ keyByte);
        }

        return resultBytes;
    }
}
