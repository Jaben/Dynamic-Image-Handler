// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenImageParameters.cs" company="">
//   
// </copyright>
// <summary>
//   The hidden image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageParameters
{
	#region Using

	using System;
	using System.Diagnostics;
	using System.Web;

	using DynamicImageHandler.Properties;

	#endregion

	/// <summary>
	/// 	The hidden image parameters.
	/// </summary>
	public class HiddenImageParameters : SimpleImageParameters
	{
		#region Constants and Fields

		/// <summary>
		/// 	The _current key.
		/// </summary>
		private static ISymCryptKey _currentKey;

		#endregion

		#region Properties

		/// <summary>
		/// 	Gets CurrentCryptKey.
		/// </summary>
		protected static ISymCryptKey CurrentCryptKey
		{
			get
			{
				return _currentKey ?? (_currentKey = new SymCryptKey(Settings.Default.ImageParameterKey));
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The get image class url.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <returns>
		/// 	The get image class url.
		/// </returns>
		public static string GetImageClassUrl(string parameters)
		{
			var paramClass = new HiddenImageParameters();

			paramClass.LoadParametersFromString(parameters);

			// get the encoded parameters
			string encodedParams = paramClass.CreateEncodedParameterString();

			// url encode and return...
			return string.Format("id={0}", HttpUtility.UrlEncode(encodedParams));
		}

		/// <summary>
		/// 	The add collection.
		/// </summary>
		/// <param name="context">
		/// 	The context.
		/// </param>
		public override void AddCollection(HttpContext context)
		{
			try
			{
				// process the hidden class into a string...
				if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
				{
					string classData = context.Request.QueryString["id"];
					this.LoadEncodedParameterString(classData);
				}
			}
			catch (Exception x)
			{
				Debug.WriteLine("Exception: " + x.Message);
			}
		}

		/// <summary>
		/// 	The create encoded parameter string.
		/// </summary>
		/// <returns>
		/// 	The create encoded parameter string.
		/// </returns>
		public virtual string CreateEncodedParameterString()
		{
			return SymCrypt.Encrypt(this.ParametersAsString(), CurrentCryptKey);
		}

		/// <summary>
		/// 	The load encoded parameter string.
		/// </summary>
		/// <param name="paramString">
		/// 	The param string.
		/// </param>
		public virtual void LoadEncodedParameterString(string paramString)
		{
			string decrypted = SymCrypt.Decrypt(paramString, CurrentCryptKey);
			this.LoadParametersFromString(decrypted);
		}

		/// <summary>
		/// 	The load parameters from string.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		public void LoadParametersFromString(string parameters)
		{
			string[] split = parameters.Split('&');

			foreach (string item in split)
			{
				string[] vars = item.Split('=');

				if (vars.Length == 2)
				{
					// add or overwrite the variable...
					if (this.Parameters.ContainsKey(vars[0].ToLower()))
					{
						this.Parameters[vars[0].ToLower()] = vars[1];
					}
					else
					{
						this.Parameters.Add(vars[0].ToLower(), vars[1]);
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// 	The crc 32.
	/// </summary>
	public class Crc32
	{
		#region Constants and Fields

		/// <summary>
		/// 	The table.
		/// </summary>
		private readonly uint[] table;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref = "Crc32" /> class.
		/// </summary>
		public Crc32()
		{
			uint poly = 0xedb88320;
			this.table = new uint[256];
			uint temp = 0;
			for (uint i = 0; i < this.table.Length; ++i)
			{
				temp = i;
				for (int j = 8; j > 0; --j)
				{
					if ((temp & 1) == 1)
					{
						temp = (temp >> 1) ^ poly;
					}
					else
					{
						temp >>= 1;
					}
				}

				this.table[i] = temp;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The compute checksum.
		/// </summary>
		/// <param name="bytes">
		/// 	The bytes.
		/// </param>
		/// <returns>
		/// 	The compute checksum.
		/// </returns>
		public uint ComputeChecksum(byte[] bytes)
		{
			uint crc = 0xffffffff;
			for (int i = 0; i < bytes.Length; ++i)
			{
				var index = (byte)((crc & 0xff) ^ bytes[i]);
				crc = (crc >> 8) ^ this.table[index];
			}

			return ~crc;
		}

		/// <summary>
		/// 	The compute checksum bytes.
		/// </summary>
		/// <param name="bytes">
		/// 	The bytes.
		/// </param>
		/// <returns>
		/// </returns>
		public byte[] ComputeChecksumBytes(byte[] bytes)
		{
			return BitConverter.GetBytes(this.ComputeChecksum(bytes));
		}

		#endregion
	}
}