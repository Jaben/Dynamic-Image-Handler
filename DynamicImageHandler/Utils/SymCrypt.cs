// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
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
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DynamicImageHandler.Utils
{
    public class SymCrypt
    {
        public static string BytesToHexString(byte[] bytes)
        {
            var hexString = new StringBuilder();

            foreach (byte t in bytes)
            {
                hexString.AppendFormat("{0:X2}", t);
            }

            return hexString.ToString();
        }

        /// <exception cref="CryptographicException">The <see cref="T:System.Security.Cryptography.TripleDES" /> cryptographic service provider is not available. </exception>
        public static string CreateKey()
        {
            using (var provider = new TripleDESCryptoServiceProvider())
            {
                provider.GenerateKey();
                return BytesToHexString(provider.Key);
            }
        }

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="s" /> is null. </exception>
        /// <exception cref="FormatException">The length of <paramref name="s" />, ignoring white-space characters, is not zero or a multiple of 4. -or-The format of <paramref name="s" /> is invalid. <paramref name="s" /> contains a non-base-64 character, more than two padding characters, or a non-white space-character among the padding characters.</exception>
        public static string Decrypt(string inputText, ISymCryptKey cryptKey)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException("inputText");
            }
            if (cryptKey == null)
            {
                throw new ArgumentNullException("cryptKey");
            }

            using (var rijndaelCipher = new RijndaelManaged())
            {
                var encryptedData = Convert.FromBase64String(inputText);

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

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        public static string Encrypt(string inputText, ISymCryptKey cryptKey)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException("inputText");
            }
            if (cryptKey == null)
            {
                throw new ArgumentNullException("cryptKey");
            }

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
    }
}