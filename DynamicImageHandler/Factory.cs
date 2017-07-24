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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

using DynamicImageHandler.Filters;
using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageProcessors;
using DynamicImageHandler.ImageProviders;
using DynamicImageHandler.ImageStores;
using DynamicImageHandler.ImageTools;
using DynamicImageHandler.Properties;

namespace DynamicImageHandler
{
    public class Factory
    {
        static Factory()
        {
            _imageProvider = new Lazy<IImageProvider>(
                () => ActivateTypeFromString<IImageProvider>(Settings.Default.ImageProviderType, "Image Provider"),
                LazyThreadSafetyMode.ExecutionAndPublication);

            _imageStore = new Lazy<IImageStore>(
                () => ActivateTypeFromString<IImageStore>(Settings.Default.ImageStoreType, "Image Store"),
                LazyThreadSafetyMode.ExecutionAndPublication);

            _imageTool = new Lazy<IImageTool>(
                () => ActivateTypeFromString<IImageTool>(Settings.Default.ImageToolType, "Image Tool"),
                LazyThreadSafetyMode.ExecutionAndPublication);

            _imageProcessor = new Lazy<IImageProcessor>(
                () => ActivateTypeFromString<IImageProcessor>(Settings.Default.ImageProcessorType, "Image Processor"),
                LazyThreadSafetyMode.ExecutionAndPublication);

            _imageFilters = new Lazy<IImageFilter[]>(() => LoadImageFilters(), LazyThreadSafetyMode.ExecutionAndPublication);

            _createParameters = () => ActivateTypeFromString<IImageParameters>(Settings.Default.ImageParametersType, "Image Parameters");
        }

        static readonly Lazy<IImageProvider> _imageProvider;

        static readonly Lazy<IImageStore> _imageStore;

        static readonly Lazy<IImageTool> _imageTool;

        static readonly Lazy<IImageFilter[]> _imageFilters;

        static readonly Lazy<IImageProcessor> _imageProcessor;

        static readonly Func<IImageParameters> _createParameters;

        static readonly ConcurrentDictionary<Type, Func<object>> _activatorLookup = new ConcurrentDictionary<Type, Func<object>>();

        static T ActivateTypeFromString<T>(string type, string name)
            where T : class
        {
            Type activateType = Type.GetType(type);

            if (activateType == null)
            {
                throw new ConfigurationErrorsException($"Unable to resolve {name} type: {type}");
            }

            return ActivateType<T>(activateType);
        }

        static T ActivateType<T>(Type activateType)
        {
            var newCtor = _activatorLookup.GetOrAdd(
                activateType,
                t =>
                    {
                        ConstructorInfo constructor = t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.Public,
                            null,
                            new Type[0],
                            null);

                        Expression<Func<object>> ctor =
                            Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(constructor), typeof(object)));

                        return ctor.Compile();
                    });

            return (T)newCtor();
        }

        static IImageFilter[] LoadImageFilters()
        {
            var imageFilterTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    s => s.Namespace != null && !s.IsAbstract && s.IsClass && s.GetInterfaces().Any(i => i == typeof(IImageFilter)))
                .Distinct()
                .ToList();

            return imageFilterTypes.Select(filterType => ActivateType<IImageFilter>(filterType)).ToArray();
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>

        #region Public Properties
        
        public static IImageParameters GetImageParameter => _createParameters();
        public static IImageProvider ImageProvider => _imageProvider.Value;
        public static IImageStore ImageStore => _imageStore.Value;
        public static IImageTool ImageTool => _imageTool.Value;
        public static IEnumerable<IImageFilter> ImageFilters => _imageFilters.Value;
        public static IImageProcessor ImageProcessor => _imageProcessor.Value;

        #endregion
    }
}