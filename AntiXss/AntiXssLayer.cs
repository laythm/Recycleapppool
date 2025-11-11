using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
internal class AntiXssLayer
{
    private const int SaltSize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100000;

    internal static string a(string plaintext, string password)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        byte[] key = a_a(password, salt, KeySize, Iterations);

        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[TagSize];

        using (var aesGcm = new AesGcm(key))
        {
            aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        }

        byte[] result = new byte[SaltSize + NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, SaltSize + NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    private static byte[] a_a(string password, byte[] salt, int keySize, int iterations)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(keySize);
        }
    }

    public static byte[] ab(byte[] plainBytes, string password)
    {
        if (plainBytes == null || plainBytes.Length == 0)
            throw new ArgumentException("Plain bytes cannot be null or empty", nameof(plainBytes));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate random salt and nonce
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        // Derive encryption key from password using PBKDF2
        byte[] key = a_a(password, salt, KeySize, Iterations);

        // Prepare buffers
        byte[] ciphertext = new byte[plainBytes.Length];
        byte[] tag = new byte[TagSize];

        // Encrypt using AES-GCM
        using (var aesGcm = new AesGcm(key))
        {
            aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);
        }

        // Combine: salt + nonce + ciphertext + tag
        byte[] result = new byte[SaltSize + NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, SaltSize + NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize + ciphertext.Length, TagSize);

        return result;
    }

    internal static string abc(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexString(hashBytes);
    }
}