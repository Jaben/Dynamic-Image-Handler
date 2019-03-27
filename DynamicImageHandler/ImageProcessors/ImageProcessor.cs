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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using DynamicImageHandler.Filters;
using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageProviders;
using DynamicImageHandler.ImageStores;
using DynamicImageHandler.ImageTools;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageProcessors
{
    public class ImageProcessor : IImageProcessor
    {
        readonly List<IImageFilter> _imageFilters;
        readonly List<IImagePrefilter> _imagePrefilters;
        readonly IImageStore _imageStore;
        readonly IImageTool _imageTool;

        readonly IImageProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcessor"/> class.
        /// </summary>
        public ImageProcessor()
            : this(Factory.ImageProvider, Factory.ImageTool, Factory.ImageStore, Factory.ImageFilters, Factory.ImagePrefilters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcessor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="imageTool">The image tool.</param>
        /// <param name="imageStore">The image store.</param>
        /// <param name="imageFilters">The image filters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// provider
        /// or
        /// imageTool
        /// or
        /// imageStore
        /// </exception>
        public ImageProcessor(
            IImageProvider provider,
            IImageTool imageTool,
            IImageStore imageStore,
            IEnumerable<IImageFilter> imageFilters,
            IEnumerable<IImagePrefilter> imagePrefilters)
        {
            this._provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this._imageTool = imageTool ?? throw new ArgumentNullException(nameof(imageTool));
            this._imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));

            this._imagePrefilters = imagePrefilters.IfNullEmpty().ToList();
            this._imageFilters = imageFilters.IfNullEmpty().OrderBy(s => s.Order).ToList();
        }

        /// <summary>
        /// Gets the image from cache or processes the image fresh and stores it.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parameters</exception>
        public byte[] GetProcessedImage(IImageParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            string key = parameters.Key;

            // attempt to get existing (cached) image
            var imageData = this._imageStore.GetImageData(key);

            if (imageData == null)
            {
                // no cached image available -- go ahead and process
                imageData = this.ProcessImageFromParameters(parameters);

                if (imageData == null)
                {
                    throw new ArgumentNullException("Processed Image is Null");
                }

                // persist the image
                this._imageStore.PutImageData(key, imageData);
            }

            return imageData;
        }

        /// <summary>
        /// Get an image from the provider and process
        /// </summary>
        /// <returns>
        /// </returns>
        byte[] ProcessImageFromParameters(IImageParameters parameters)
        {
            var data = this._provider.GetImageData(parameters);

            if (data == null)
            {
                throw new ArgumentNullException($"Image data returned null for ImageSrc: '{parameters.GetImageSrc()}'");
            }

            Bitmap outputImg = null;

            try
            {
                using (var sourceImageData = new MemoryStream(data))
                {
                    outputImg = (Bitmap)Image.FromStream(sourceImageData);

                    foreach (var prefilter in this._imagePrefilters)
                    {
                        prefilter.Process(outputImg);
                    }
                }

                foreach (var imageFilter in this._imageFilters)
                {
                    Bitmap oldOutputImage = outputImg;
                    bool modified = imageFilter.Process(parameters, ref outputImg);
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

                // Encode image using the image format
                return this._imageTool.Encode(outputImg, parameters.GetImageFormat());
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