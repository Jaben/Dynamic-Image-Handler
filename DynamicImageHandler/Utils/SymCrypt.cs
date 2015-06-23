// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymCrypt.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman and CaptiveAire Systems
//	
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>
// <summary>
//   The sym crypt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DynamicImageHandler.Utils
{
    /// <summary>
    ///     The sym crypt.
    /// </summary>
    public class SymCrypt
    {
        #region Public Methods

        /// <summary>
        ///     The bytes to hex string.
        /// </summary>
        /// <param name="bytes">
        ///     The bytes.
        /// </param>
        /// <returns>
        ///     The bytes to hex string.
        /// </returns>
        public static string BytesToHexString(byte[] bytes)
        {
            var hexString = new StringBuilder();

            foreach (byte t in bytes)
            {
                hexString.Append(string.Format("{0:X2}", t));
            }

            return hexString.ToString();
        }

        /// <summary>
        ///     The create key.
        /// </summary>
        /// <returns>
        ///     The create key.
        /// </returns>
        public static string CreateKey()
        {
            using (var provider = new TripleDESCryptoServiceProvider())
            {
                provider.GenerateKey();
                return BytesToHexString(provider.Key);
            }
        }

        /// <summary>
        ///     The decrypt.
        /// </summary>
        /// <param name="InputText">
        ///     The input text.
        /// </param>
        /// <param name="cryptKey">
        ///     The crypt Key.
        /// </param>
        /// <returns>
        ///     The decrypt.
        /// </returns>
        public static string Decrypt(string InputText, ISymCryptKey cryptKey)
        {
            using (var rijndaelCipher = new RijndaelManaged())
            {
                var encryptedData = Convert.FromBase64String(InputText);

                using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(cryptKey.Key, cryptKey.Salt))
                using (var memoryStream = new MemoryStream(encryptedData))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var plainText = new byte[encryptedData.Length];
                    int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                    return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                }
            }
        }

        /// <summary>
        ///     The encrypt.
        /// </summary>
        /// <param name="inputText">
        ///     The input text.
        /// </param>
        /// <param name="cryptKey">
        ///     The crypt Key.
        /// </param>
        /// <returns>
        ///     The encrypt.
        /// </returns>
        public static string Encrypt(string inputText, ISymCryptKey cryptKey)
        {
            using (var rijndaelCipher = new RijndaelManaged())
            {
                var plainText = Encoding.Unicode.GetBytes(inputText);

                using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(cryptKey.Key, cryptKey.Salt))
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainText, 0, plainText.Length);
                    cryptoStream.FlushFinalBlock();

                    var cipherBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }

        #endregion
    }
}