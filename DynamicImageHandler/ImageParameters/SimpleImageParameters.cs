// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleImageParameters.cs" company="">
//   
// </copyright>
// <summary>
//   The simple image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageParameters
{
	#region Using

	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;

	#endregion

	/// <summary>
	/// 	The simple image parameters.
	/// </summary>
	public class SimpleImageParameters : IImageParameters
	{
		#region Constants and Fields

		/// <summary>
		/// 	The _parameters.
		/// </summary>
		protected SortedDictionary<string, string> _parameters = new SortedDictionary<string, string>();

		#endregion

		#region Public Properties

		/// <summary>
		/// 	Gets ImageSrc.
		/// </summary>
		public virtual string ImageSrc
		{
			get
			{
				if (this.Parameters.ContainsKey("src"))
				{
					return this.Parameters["src"];
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// 	Doesn't cache -- suggested pull once and reuse in local code.
		/// </summary>
		public virtual string Key
		{
			get
			{
				return this.MD5HashString(this.ParametersAsString(), 64);
			}
		}

		/// <summary>
		/// 	Gets Parameters.
		/// </summary>
		public virtual IDictionary<string, string> Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		#endregion

		#region Public Indexers

		/// <summary>
		/// 	The this.
		/// </summary>
		/// <param name = "parameter">
		/// 	The parameter.
		/// </param>
		public virtual string this[string parameter]
		{
			get
			{
				return this.Parameters.ContainsKey(parameter) ? this.Parameters[parameter] : null;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The add collection.
		/// </summary>
		/// <param name="context">
		/// 	The context.
		/// </param>
		public virtual void AddCollection(HttpContext context)
		{
			foreach (string key in context.Request.QueryString.Keys)
			{
				if (this.Parameters.ContainsKey(key))
				{
					this.Parameters[key] = context.Request.QueryString[key];
				}
				else if (!string.IsNullOrEmpty(context.Request.QueryString[key]))
				{
					this.Parameters.Add(key, context.Request.QueryString[key]);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	The m d 5 hash string.
		/// </summary>
		/// <param name="value">
		/// 	The value.
		/// </param>
		/// <param name="maxLength">
		/// 	The max length.
		/// </param>
		/// <returns>
		/// 	The m d 5 hash string.
		/// </returns>
		protected string MD5HashString(string value, int maxLength)
		{
			using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
			{
				byte[] data = Encoding.ASCII.GetBytes(value);
				data = cryptoServiceProvider.ComputeHash(data);

				string str = SymCrypt.BytesToHexString(data);

				if (str.Length > maxLength)
				{
					str = str.Substring(0, maxLength);
				}

				return str;
			}
		}

		/// <summary>
		/// 	The parameters as string.
		/// </summary>
		/// <returns>
		/// 	The parameters as string.
		/// </returns>
		protected virtual string ParametersAsString()
		{
			var builder = new StringBuilder();

			bool isFirst = true;

			// create a key for this item...
			foreach (var kv in this.Parameters.Where(kv => !string.IsNullOrEmpty(kv.Value)))
			{
				if (!isFirst)
				{
					builder.Append("&");
				}

				builder.AppendFormat("{0}={1}", kv.Key.ToLower(), kv.Value);
				isFirst = false;
			}

			return builder.ToString();
		}

		#endregion
	}
}