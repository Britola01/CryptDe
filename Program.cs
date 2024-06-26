﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Digite 'encrypt' para criptografar ou 'decrypt' para descriptografar:");
        string command = Console.ReadLine();

        if (command == "encrypt")
        {
            Console.WriteLine("Digite o caminho do arquivo para criptografar:");
            string inputFile = Console.ReadLine();
            Console.WriteLine("Digite a senha:");
            string password = Console.ReadLine();
            EncryptFile(inputFile, password);
        }
        else if (command == "decrypt")
        {
            Console.WriteLine("Digite o caminho do arquivo para descriptografar:");
            string inputFile = Console.ReadLine();
            Console.WriteLine("Digite a senha:");
            string password = Console.ReadLine();
            DecryptFile(inputFile, password);
        }
        else
        {
            Console.WriteLine("Comando não reconhecido.");
        }
    }

    private static void EncryptFile(string inputFile, string password)
    {
        byte[] bytesToBeEncrypted = File.ReadAllBytes(inputFile);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
        string fileEncrypted = inputFile + ".aes";

        File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        Console.WriteLine("Arquivo criptografado salvo como: " + fileEncrypted);
    }

    private static void DecryptFile(string inputFile, string password)
    {
        byte[] bytesToBeDecrypted = File.ReadAllBytes(inputFile);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
        string fileDecrypted = inputFile.Replace(".aes", "");

        File.WriteAllBytes(fileDecrypted, bytesDecrypted);
        Console.WriteLine("Arquivo descriptografado salvo como: " + fileDecrypted);
    }

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
    {
        byte[] encryptedBytes = null;

        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }

        return encryptedBytes;
    }

    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    {
        byte[] decryptedBytes = null;

        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }
}
