using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace IdleFrisbeeGolf.Core
{
    /// <summary>
    /// Handles JSON save/load with AES encryption and PlayerPrefs mirroring.
    /// </summary>
    public class SaveSystem
    {
        private const string BackupPrefsKey = "IdleFrisbeeGolf_Backup";
        private readonly string _savePath;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public SaveSystem(string fileName, string secret)
        {
            _savePath = Path.Combine(Application.persistentDataPath, fileName);
            using var sha = SHA256.Create();
            _key = sha.ComputeHash(Encoding.UTF8.GetBytes(secret));
            _iv = new byte[16];
            Array.Copy(_key, _iv, 16);
        }

        public void Save(object data)
        {
            var json = JsonUtility.ToJson(data);
            var encrypted = Encrypt(json);
            File.WriteAllText(_savePath, encrypted);
            PlayerPrefs.SetString(BackupPrefsKey, encrypted);
            PlayerPrefs.Save();
        }

        public bool TryLoad<T>(out T data) where T : class
        {
            string encrypted = null;
            if (File.Exists(_savePath))
            {
                encrypted = File.ReadAllText(_savePath);
            }
            else if (PlayerPrefs.HasKey(BackupPrefsKey))
            {
                encrypted = PlayerPrefs.GetString(BackupPrefsKey);
            }

            if (string.IsNullOrEmpty(encrypted))
            {
                data = null;
                return false;
            }

            try
            {
                var json = Decrypt(encrypted);
                data = JsonUtility.FromJson<T>(json);
                return data != null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Save load failed: {ex}");
                data = null;
                return false;
            }
        }

        private string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        private string Decrypt(string encrypted)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor();
            var bytes = Convert.FromBase64String(encrypted);
            var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
