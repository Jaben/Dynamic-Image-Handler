// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageHandler.cs" company="">
//
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman
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
//   Http Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Net;
	using System.Web;

	using DynamicImageHandler.Filters;
	using DynamicImageHandler.Utils;

	/// <summary>
	/// 	Http Handler
	/// </summary>
	public class ImageHandler : IHttpHandler
	{
		#region Constants and Fields

		/// <summary>
		/// 	Cache Manager Reference
		/// </summary>
		private static readonly IImageStore s_ImageStore = Factory.GetImageStore();

		/// <summary>
		/// 	The s_ image tool.
		/// </summary>
		private static readonly IImageTool s_ImageTool = Factory.GetImageTool();

		/// <summary>
		/// 	The m_ image filters.
		/// </summary>
		private readonly List<IImageFilter> m_ImageFilters =
			new List<IImageFilter>(
				new IImageFilter[]
					{
						new ResizeFilter(), // Default Filters
						new ZoomFilter(), new RotateFilter(), new GreyScaleFilter(), new WatermarkFilter()
					});

		#endregion

		#region Public Properties

		/// <summary>
		/// 	Gets ImageFilters.
		/// </summary>
		public IImageFilter[] ImageFilters
		{
			get
			{
				return this.m_ImageFilters.ToArray();
			}
		}

		/// <summary>
		/// 	Gets a value indicating whether IsReusable.
		/// </summary>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The add filter.
		/// </summary>
		/// <param name="imageFilter">
		/// 	The image filter.
		/// </param>
		public void AddFilter(IImageFilter imageFilter)
		{
			this.m_ImageFilters.Add(imageFilter);
		}

		/// <summary>
		/// 	Retrieve or update cache, and write to output stream
		/// </summary>
		/// <param name="context">
		/// </param>
		/// <exception cref="ApplicationException">
		/// 	Unable to determine image parameter provider
		/// </exception>
		public void ProcessRequest(HttpContext context)
		{
			IImageParameters parameters = Factory.GetImageParameters();

			if (parameters == null)
			{
				throw new ApplicationException("Unable to determine image parameter provider");
			}

			// add query string to the parameters...
			parameters.AddCollection(context);

			if (string.IsNullOrEmpty(parameters.ImageSrc))
			{
				// parameters aren't assigned...
				return;
			}

			// add file updated parameter if available...
			Factory.GetImageProvider().AddImageLastUpdatedParameter(parameters);

			string key = parameters.Key;

			string eTag = string.Format(@"""{0}""", key);

			if (CheckETag(context, eTag))
			{
				return;
			}

			ImageFormat imageFormat = GetImageFormat(parameters);

			byte[] imageData = s_ImageStore.GetImageData(key);
			if (imageData.IsNull())
			{
				imageData = this.GetImageData(parameters, context, imageFormat);
				if (imageData.IsNull())
				{
					return;
				}

				s_ImageStore.PutImageData(key, imageData);
			}

			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetETag(eTag);
			context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
			context.Response.ContentType = "image/" + imageFormat.ToString().ToLower();
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
		}

		/// <summary>
		/// 	The remove filter.
		/// </summary>
		/// <param name="imageFilter">
		/// 	The image filter.
		/// </param>
		public void RemoveFilter(IImageFilter imageFilter)
		{
			this.m_ImageFilters.Remove(imageFilter);
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	Check if the ETag that sent from the client is match to the current ETag.
		/// 	If so, set the status code to 'Not Modified' and stop the response.
		/// </summary>
		/// <param name="context">
		/// 	The context.
		/// </param>
		/// <param name="eTagCode">
		/// 	The e Tag Code.
		/// </param>
		/// <returns>
		/// 	The check e tag.
		/// </returns>
		private static bool CheckETag(HttpContext context, string eTagCode)
		{
			string ifNoneMatch = context.Request.Headers["If-None-Match"];
			if (eTagCode.Equals(ifNoneMatch, StringComparison.Ordinal))
			{
				context.Response.AppendHeader("Content-Length", "0");
				context.Response.StatusCode = (int)HttpStatusCode.NotModified;
				context.Response.StatusDescription = "Not modified";
				context.Response.SuppressContent = true;
				context.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.Response.Cache.SetETag(eTagCode);
				context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
				context.Response.End();
				return true;
			}

			return false;
		}

		/// <summary>
		/// 	The get image format.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <returns>
		/// </returns>
		private static ImageFormat GetImageFormat(IImageParameters parameters)
		{
			if (parameters.Parameters.ContainsKey("format"))
			{
				string imageTypeParameter = parameters.Parameters["format"];
				if (!string.IsNullOrEmpty(imageTypeParameter))
				{
					switch (imageTypeParameter.ToLowerInvariant())
					{
						case "png":
							return ImageFormat.Png;
						case "gif":
							return ImageFormat.Gif;
						case "jpg":
							return ImageFormat.Jpeg;
						case "tif":
							return ImageFormat.Tiff;
					}
				}
			}

			return ImageFormat.Jpeg;
		}

		/// <summary>
		/// 	Takes care of resizeing image from url parameters
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <param name="context">
		/// </param>
		/// <param name="imageFormat">
		/// </param>
		/// <returns>
		/// </returns>
		private byte[] GetImageData(IImageParameters parameters, HttpContext context, ImageFormat imageFormat)
		{
			IImageProvider provider = Factory.GetImageProvider();
			if (provider.IsNull())
			{
				throw new ApplicationException("Unable to determine image provider");
			}

			byte[] data = provider.GetImageData(parameters);
			if (data.IsNull())
			{
				return null;
			}

			using (var sourceImageData = new MemoryStream(data))
			{
				var outputImg = (Bitmap)Image.FromStream(sourceImageData);
				foreach (IImageFilter imageFilter in this.m_ImageFilters)
				{
					Bitmap oldOutputImage = outputImg;

					bool modified = imageFilter.Process(parameters, context, ref outputImg);
					if (modified)
					{
						// Dispose old bitmap
						oldOutputImage.Dispose();
					}
				}

				using (outputImg)
				{
					// Encode image
					return s_ImageTool.Encode(outputImg, imageFormat);
				}
			}
		}

		#endregion
	}
}