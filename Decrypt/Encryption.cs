using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
public class AdvancedEncryption
{
    private const int SaltSize = 32; // 256 bits
    private const int NonceSize = 12; // 96 bits for AES-GCM
    private const int TagSize = 16; // 128 bits authentication tag
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 100000; // PBKDF2 iterations

    /// <summary>
    /// Encrypts plaintext using AES-256-GCM (Authenticated Encryption)
    /// </summary>
    public static string Encrypt(string plaintext, string password)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        // Generate random salt and nonce
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        // Derive encryption key from password using PBKDF2
        byte[] key = DeriveKey(password, salt, KeySize, Iterations);

        // Prepare buffers
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[TagSize];

        // Encrypt using AES-GCM
        using (var aesGcm = new AesGcm(key))
        {
            aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        }

        // Combine: salt + nonce + ciphertext + tag
        byte[] result = new byte[SaltSize + NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, SaltSize + NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts ciphertext using AES-256-GCM with authentication verification
    /// </summary>
    public static string Decrypt(string encryptedText, string password)
    {
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        byte[] data = Convert.FromBase64String(encryptedText);

        // Validate minimum length
        int minLength = SaltSize + NonceSize + TagSize;
        if (data.Length < minLength)
            throw new CryptographicException("Invalid ciphertext format");

        // Extract components
        byte[] salt = new byte[SaltSize];
        byte[] nonce = new byte[NonceSize];
        byte[] ciphertext = new byte[data.Length - SaltSize - NonceSize - TagSize];
        byte[] tag = new byte[TagSize];

        Buffer.BlockCopy(data, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(data, SaltSize, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, SaltSize + NonceSize, ciphertext, 0, ciphertext.Length);
        Buffer.BlockCopy(data, SaltSize + NonceSize + ciphertext.Length, tag, 0, TagSize);

        // Derive key from password
        byte[] key = DeriveKey(password, salt, KeySize, Iterations);

        // Decrypt using AES-GCM
        byte[] plaintext = new byte[ciphertext.Length];
        using (var aesGcm = new AesGcm(key))
        {
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        return Encoding.UTF8.GetString(plaintext);
    }

    /// <summary>
    /// Derives a cryptographic key from password using PBKDF2
    /// </summary>
    private static byte[] DeriveKey(string password, byte[] salt, int keySize, int iterations)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(keySize);
        }
    }

    /// <summary>
    /// Encrypts a file using AES-256-GCM
    /// </summary>
    public static void EncryptFile(string inputFilePath, string outputFilePath, string password)
    {
        byte[] fileData = File.ReadAllBytes(inputFilePath);
        string encrypted = Encrypt(Convert.ToBase64String(fileData), password);
        File.WriteAllText(outputFilePath, encrypted);
    }

    /// <summary>
    /// Decrypts a file using AES-256-GCM
    /// </summary>
    public static void DecryptFile(string inputFilePath, string outputFilePath, string password)
    {
        string encryptedText = File.ReadAllText(inputFilePath);
        string decrypted = Decrypt(encryptedText, password);
        byte[] fileData = Convert.FromBase64String(decrypted);
        File.WriteAllBytes(outputFilePath, fileData);
    }

    public static byte[] DecryptBytes(byte[] encryptedBytes, string password)
    {
        if (encryptedBytes == null || encryptedBytes.Length == 0)
            throw new ArgumentException("Encrypted bytes cannot be null or empty", nameof(encryptedBytes));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Validate minimum length
        int minLength = SaltSize + NonceSize + TagSize;
        if (encryptedBytes.Length < minLength)
            throw new CryptographicException("Invalid encrypted data format");

        // Extract components
        byte[] salt = new byte[SaltSize];
        byte[] nonce = new byte[NonceSize];
        byte[] ciphertext = new byte[encryptedBytes.Length - SaltSize - NonceSize - TagSize];
        byte[] tag = new byte[TagSize];

        Buffer.BlockCopy(encryptedBytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(encryptedBytes, SaltSize, nonce, 0, NonceSize);
        Buffer.BlockCopy(encryptedBytes, SaltSize + NonceSize, ciphertext, 0, ciphertext.Length);
        Buffer.BlockCopy(encryptedBytes, SaltSize + NonceSize + ciphertext.Length, tag, 0, TagSize);

        // Derive key from password
        byte[] key = DeriveKey(password, salt, KeySize, Iterations);

        // Decrypt using AES-GCM
        byte[] plaintext = new byte[ciphertext.Length];
        using (var aesGcm = new AesGcm(key))
        {
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        return plaintext;
    }

    /// <summary>
    /// Generates a cryptographically secure random password
    /// </summary>
    public static string GenerateSecurePassword(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";
        byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] = chars[randomBytes[i] % chars.Length];
        }

        return new string(password);
    }

    /// <summary>
    /// Computes SHA-256 hash of a string
    /// </summary>
    public static string ComputeHash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexString(hashBytes);
    }
}