using LitJson;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;
using Disney.Mix.SDK;
using UnityEngine;
using System.Text;

namespace Disney.MobileNetwork
{
    public class KeyChainWindowsManager : KeyChainManager
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX_ARM || UNITY_EDITOR_OSX_ARM || UNITY_ANDROID || UNITY_EDITOR_ANDROID || UNITY_IOS || UNITY_EDITOR_IOS

        private const string APP_DATA_KEY = "cp.AppData";

        private readonly IKeychain keychain;

        private const int InitializationVectorSize = 16;

        private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

        private static readonly byte[] tempInitializationVector = new byte[16];

        private readonly AesManaged symmetricAlgorithm;

        private KeyChainManager keyChainManager;


        public static byte[] getChainKey;

        private static byte[] localStorageKey = new byte[32];

        // store loaded appData
        private Dictionary<string, string> appData;

        public KeyChainWindowsManager()
        {
            symmetricAlgorithm = new AesManaged();
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string DLL_NAME = "KeyChainWindows";
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        private const string DLL_NAME = "libKeyChainLinux";
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        // _cryptProtectData: input string -> out size and pointer to protected bytes
        // Return int success (1) or 0
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int _cryptProtectData(string dataIn, ref int dataOutSize, out IntPtr dataOut);

        // _cryptUnprotectData: input byte[] + length -> out pointer to ANSI string
        // Return int success (1) or 0
        [DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int _cryptUnprotectData(byte[] dataIn, int dataInLength, out IntPtr dataOut);

        // Helper wrapper for unprotecting bytes -> string (handles ptr marshalling & free)
        private static string CryptUnprotect(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            IntPtr ptr = IntPtr.Zero;
            int res = _cryptUnprotectData(data, data.Length, out ptr);
            if (res == 0 || ptr == IntPtr.Zero) return null;

            // Marshal ANSI string
            string s = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeCoTaskMem(ptr);
            return s;
        }
#endif

        public byte[] Decrypt2(byte[] bytes)
        {
            string text = keyChainManager.GetString("SessionUnlockKey");

            byte[] key;
            if (string.IsNullOrEmpty(text))
            {
                key = new byte[32];
                getChainKey = key;
                key = getChainKey;
            }
            else
            {
                key = Convert.FromBase64String(text);
            }

            if (bytes.Length <= 16)
            {
                throw new ArgumentException("Invalid byte array: " + BitConverter.ToString(bytes) + ". Must be over 16 bytes long.");
            }
            if (key.Length != 32)
            {
                throw new ArgumentException("Invalid key: " + BitConverter.ToString(key) + ". Must be 32 bytes long.");
            }

            symmetricAlgorithm.Key = key;
            Array.Copy(bytes, 0, tempInitializationVector, 0, 16);
            symmetricAlgorithm.IV = tempInitializationVector;
            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
            return cryptoTransform.TransformFinalBlock(bytes, 16, bytes.Length - 16);
        }

        public string Decrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            byte[] array = Convert.FromBase64String(text);
            byte[] array2 = new byte[32];
            byte[] array3 = new byte[array.Length - 32];
            Array.Copy(array, array2, 32);
            Array.ConstrainedCopy(array, 32, array3, 0, array.Length - 32);
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, array2);
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateDecryptor(bytes, bytes2))
                {
                    using (MemoryStream stream = new MemoryStream(array3))
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(stream2))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        public byte[] Encrypt2(byte[] bytes)
        {
            string text = null;

            byte[] key = null;

            text = getAppData()["SessionUnlockKey"];
            Debug.Log("Keys3: " + text);
            byte[] unused = Convert.FromBase64String(text);
            if (string.IsNullOrEmpty(text))
            {
                key = new byte[32];
                getChainKey = key;
                key = getChainKey;
            }
            else
            {
                key = Convert.FromBase64String(text);
            }

            if (unused.Length != 32)
            {
                throw new ArgumentException("Invalid key: " + BitConverter.ToString(unused) + ". Must be 32 bytes long.");
            }
            symmetricAlgorithm.Key = Convert.FromBase64String(getAppData()["SessionUnlockKey"]);
            rng.GetBytes(tempInitializationVector);
            symmetricAlgorithm.IV = tempInitializationVector;
            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
            byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            byte[] array2 = new byte[16 + array.Length];
            Array.Copy(tempInitializationVector, 0, array2, 0, 16);
            Array.Copy(array, 0, array2, 16, array.Length);
            return array2;
        }

        public string Encrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 32);
            byte[] array = rfc2898DeriveBytes.Salt;
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateEncryptor(bytes, bytes2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(stream))
                            {
                                streamWriter.Write(text);
                            }
                        }
                        byte[] array2 = memoryStream.ToArray();
                        Array.Resize(ref array, array.Length + array2.Length);
                        Array.Copy(array2, 0, array, 32, array2.Length);
                        return Convert.ToBase64String(array);
                    }
                }
            }
        }

        private Dictionary<string, string> getAppData()
        {
            Dictionary<string, string> strs;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            string str = PlayerPrefs.GetString(APP_DATA_KEY, null);
            if (string.IsNullOrEmpty(str))
            {
                strs = new Dictionary<string, string>();
            }
            else
            {
                byte[] numArray = Convert.FromBase64String(str);

                // use CryptUnprotect wrapper which calls native _cryptUnprotectData
                string str1 = CryptUnprotect(numArray);
                if (string.IsNullOrEmpty(str1))
                {
                    try
                    {
                        str1 = Decrypt(str, "4C906C6AAF5C2CB4B581411A91091A8D");
                    }
                    catch
                    {
                        str1 = null;
                    }
                }

                if (string.IsNullOrEmpty(str1))
                {
                    strs = new Dictionary<string, string>();
                }
                else
                {
                    strs = JsonMapper.ToObject<Dictionary<string, string>>(str1);
                }
            }
#else
            string str = PlayerPrefs.GetString(APP_DATA_KEY, null);
            if (string.IsNullOrEmpty(str))
            {
                strs = new Dictionary<string, string>();
            }
            else
            {
                string str2 = Decrypt(str, "4C906C6AAF5C2CB4B581411A91091A8D");
                strs = JsonMapper.ToObject<Dictionary<string, string>>(str2);
            }
#endif
            return strs;
        }

        public override string GetString(string key)
        {
            string str;
            this.appData = this.getAppData();
            this.appData.TryGetValue(key, out str);
            return str;
        }

        protected override void Init()
        {
            this.appData = this.getAppData();

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (this.appData.TryGetValue("SessionUnlockKey", out string unlockKey) && !string.IsNullOrEmpty(unlockKey))
            {
                PlayerPrefs.SetString("SessionUnlockKey", unlockKey);
                PlayerPrefs.Save();
            }
#endif
        }

        public override void PutString(string key, string value)
        {
            this.appData[key] = value;
            this.setAppData(this.appData);
        }

        public override void RemoveString(string key)
        {
            if (this.appData.ContainsKey(key))
            {
                this.appData.Remove(key);
                this.setAppData(this.appData);
            }
        }

        private void setAppData(Dictionary<string, string> data)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (data == null)
            {
                PlayerPrefs.DeleteKey(APP_DATA_KEY);
                return;
            }

            string json = JsonMapper.ToJson(data);

            // Native protect: get bytes & store base64
            IntPtr ptr = IntPtr.Zero;
            int size = 0;
            int res = _cryptProtectData(json, ref size, out ptr);
            if (res == 0 || ptr == IntPtr.Zero || size <= 0)
            {
                // fallback: use Encrypt() to store playable data (so nothing is lost)
                PlayerPrefs.SetString(APP_DATA_KEY, Encrypt(json, "4C906C6AAF5C2CB4B581411A91091A8D"));
                return;
            }

            try
            {
                byte[] numArray = new byte[size];
                Marshal.Copy(ptr, numArray, 0, size);
                PlayerPrefs.SetString(APP_DATA_KEY, Convert.ToBase64String(numArray));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }

#else
            if (data == null)
            {
                PlayerPrefs.DeleteKey(APP_DATA_KEY);
                return;
            }

            string json = JsonMapper.ToJson(data);
            PlayerPrefs.SetString(APP_DATA_KEY, Encrypt(json, "4C906C6AAF5C2CB4B581411A91091A8D"));
#endif
        }

#endif
    }
}
