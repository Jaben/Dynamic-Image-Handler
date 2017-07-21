// // --------------------------------------------------------------------------------------------------------------------
// // Copyright (c) 2009-2010 Esben Carlsen
// // Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
// // 
// // This library is free software; you can redistribute it and/or
// // modify it under the terms of the GNU Lesser General Public
// // License as published by the Free Software Foundation; either
// // version 2.1 of the License, or (at your option) any later version.
// 
// // This library is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// // Lesser General Public License for more details.
// 
// // You should have received a copy of the GNU Lesser General Public
// // License along with this library; if not, write to the Free Software
// // Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Web;

using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageProcessors;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler
{
    /// <summary>
    ///     Http Handler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        /// <summary>
        /// Gets the image processor.
        /// </summary>
        /// <value>
        /// The image processor.
        /// </value>
        protected IImageProcessor ImageProcessor { get; } = Factory.ImageProcessor;

        /// <summary>
        ///     Gets a value indicating whether IsReusable.
        /// </summary>
        public bool IsReusable => false;

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
            IImageParameters parameters = Factory.GetImageParameter;

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
            Factory.ImageProvider.AddImageLastUpdatedParameter(parameters);

            string key = parameters.Key;
            string eTag = $@"""{key}""";

            if (CheckETag(context, eTag))
            {
                return;
            }

            var imageData = this.ImageProcessor.GetProcessedImage(parameters);

            var httpResponse = context.Response;

            httpResponse.Cache.SetCacheability(HttpCacheability.Public);
            httpResponse.Cache.SetETag(eTag);
            httpResponse.Cache.SetExpires(DateTime.Now.AddYears(1));
            httpResponse.ContentType = $"image/{parameters.GetImageFormat().ToString().ToLower()}";
            httpResponse.OutputStream.Write(imageData, 0, imageData.Length);
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
    }
}