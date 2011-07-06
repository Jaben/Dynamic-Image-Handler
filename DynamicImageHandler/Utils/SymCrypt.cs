// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymCrypt.cs" company="">
//   
// </copyright>
// <summary>
//   The sym crypt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageParameters
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// 	The sym crypt.
	/// </summary>
	public class SymCrypt
	{
		#region Public Methods

		/// <summary>
		/// 	The bytes to hex string.
		/// </summary>
		/// <param name="bytes">
		/// 	The bytes.
		/// </param>
		/// <returns>
		/// 	The bytes to hex string.
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
		/// 	The create key.
		/// </summary>
		/// <returns>
		/// 	The create key.
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
		/// 	The decrypt.
		/// </summary>
		/// <param name="InputText">
		/// 	The input text.
		/// </param>
		/// <param name="cryptKey">
		/// The crypt Key.
		/// </param>
		/// <returns>
		/// 	The decrypt.
		/// </returns>
		public static string Decrypt(string InputText, ISymCryptKey cryptKey)
		{
			using (var rijndaelCipher = new RijndaelManaged())
			{
				byte[] encryptedData = Convert.FromBase64String(InputText);

				using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(cryptKey.Key, cryptKey.Salt))
				{
					using (var memoryStream = new MemoryStream(encryptedData))
					{
						using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
						{
							var plainText = new byte[encryptedData.Length];
							int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
							return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
						}
					}
				}
			}
		}

		/// <summary>
		/// 	The encrypt.
		/// </summary>
		/// <param name="inputText">
		/// 	The input text.
		/// </param>
		/// <param name="cryptKey">
		/// The crypt Key.
		/// </param>
		/// <returns>
		/// 	The encrypt.
		/// </returns>
		public static string Encrypt(string inputText, ISymCryptKey cryptKey)
		{
			using (var rijndaelCipher = new RijndaelManaged())
			{
				byte[] plainText = Encoding.Unicode.GetBytes(inputText);

				using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(cryptKey.Key, cryptKey.Salt))
				{
					using (var memoryStream = new MemoryStream())
					{
						using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plainText, 0, plainText.Length);
							cryptoStream.FlushFinalBlock();

							byte[] cipherBytes = memoryStream.ToArray();
							return Convert.ToBase64String(cipherBytes);
						}
					}
				}
			}
		}

		#endregion
	}
}