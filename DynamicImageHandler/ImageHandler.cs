// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2015 Jaben Cargman and CaptiveAire Systems
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using DynamicImageHandler.Filters;
using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageProviders;
using DynamicImageHandler.ImageStores;
using DynamicImageHandler.ImageTools;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler
{
    /// <summary>
    ///     Http Handler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        private static readonly IImageStore _imageStore = Factory.GetImageStore();

        private static readonly IImageTool _imageTool = Factory.GetImageTool();

        private readonly List<IImageFilter> _imageFilters = Factory.GetImageFilters().ToList();

        /// <summary>
        ///     Gets ImageFilters.
        /// </summary>
        public IList<IImageFilter> ImageFilters
        {
            get
            {
                return this._imageFilters;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsReusable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Retrieve or update cache, and write to output stream
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <exception cref="ApplicationException">
        ///     Unable to determine image parameter provider
        /// </exception>
        public void ProcessRequest(HttpContext context)
        {
            IImageParameters parameters = Factory.GetImageParameters();

            if (parameters == null)
            {
                throw new ApplicationException("Unable to determine image parameter provider");
            }

            // add query string to the parameters...
            parameters.AppendRawParameters(context.Request.QueryString.ToKeyValuePairs());

            if (string.IsNullOrEmpty(parameters.GetImageSrc()))
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

            ImageFormat imageFormat = parameters.GetImageFormat();

            var imageData = _imageStore.GetImageData(key);
            if (imageData == null)
            {
                imageData = this.GetImageData(parameters, context, imageFormat);
                if (imageData == null)
                {
                    return;
                }

                _imageStore.PutImageData(key, imageData);
            }

            var httpResponse = context.Response;

            httpResponse.Cache.SetCacheability(HttpCacheability.Public);
            httpResponse.Cache.SetETag(eTag);
            httpResponse.Cache.SetExpires(DateTime.Now.AddYears(1));
            httpResponse.ContentType = "image/" + imageFormat.ToString().ToLower();
            httpResponse.OutputStream.Write(imageData, 0, imageData.Length);
        }

        /// <summary>
        ///     The add filter.
        /// </summary>
        /// <param name="imageFilter">
        ///     The image filter.
        /// </param>
        public void AddFilter(IImageFilter imageFilter)
        {
            if (imageFilter == null)
            {
                throw new ArgumentNullException("imageFilter");
            }

            this.ImageFilters.Add(imageFilter);
        }

        /// <summary>
        ///     The remove filter.
        /// </summary>
        /// <param name="imageFilter">
        ///     The image filter.
        /// </param>
        public void RemoveFilter(IImageFilter imageFilter)
        {
            if (imageFilter == null)
            {
                throw new ArgumentNullException("imageFilter");
            }

            this.ImageFilters.Remove(imageFilter);
        }

        /// <summary>
        ///     Check if the ETag that sent from the client is match to the current ETag.
        ///     If so, set the status code to 'Not Modified' and stop the response.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="eTagCode">
        ///     The e Tag Code.
        /// </param>
        /// <returns>
        ///     The check e tag.
        /// </returns>
        private static bool CheckETag(HttpContext context, string eTagCode)
        {
            string ifNoneMatch = context.Request.Headers["If-None-Match"];

            if (!eTagCode.Equals(ifNoneMatch, StringComparison.Ordinal))
            {
                return false;
            }

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

        

        /// <summary>
        ///     Takes care of resizing image from url parameters
        /// </summary>
        /// <param name="parameters">
        ///     The parameters.
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
            if (provider == null)
            {
                throw new ApplicationException("Unable to determine image provider");
            }

            var data = provider.GetImageData(parameters);
            if (data == null)
            {
                return null;
            }

            Bitmap outputImg = null;

            try
            {
                using (var sourceImageData = new MemoryStream(data)) outputImg = (Bitmap)Image.FromStream(sourceImageData);

                foreach (var imageFilter in this.ImageFilters.OrderBy(s => s.Order))
                {
                    Bitmap oldOutputImage = outputImg;
                    bool modified = imageFilter.Process(parameters, context, ref outputImg);
                    if (modified)
                    {
                        // Dispose old bitmap
                        try
                        {
                            oldOutputImage.Dispose();
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    }
                }

                // Encode image
                return _imageTool.Encode(outputImg, imageFormat);
            }
            finally
            {
                if (outputImg != null)
                {
                    try
                    {
                        outputImg.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }
        }
    }
}