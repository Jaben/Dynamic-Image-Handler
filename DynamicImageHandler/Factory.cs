// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

using DynamicImageHandler.Filters;
using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageProviders;
using DynamicImageHandler.ImageStores;
using DynamicImageHandler.ImageTools;
using DynamicImageHandler.Properties;

namespace DynamicImageHandler
{
    public class Factory
    {
        private static readonly object _syncLock = new object();

        private static volatile IImageProvider _imageProvider;

        private static volatile IImageStore _imageStore;

        private static volatile IImageTool _imageTool;

        private static volatile IImageFilter[] _imageFilters;

        private static volatile Func<IImageParameters> _createParameters;

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IImageParameters GetImageParameters()
        {
            if (_createParameters == null)
            {
                lock (_syncLock)
                {
                    if (_createParameters == null)
                    {
                        _createParameters = () => ActivateType<IImageParameters>(Settings.Default.ImageParametersType);
                    }
                }
            }

            return _createParameters();
        }

        public static IImageProvider GetImageProvider()
        {
            if (_imageProvider != null)
            {
                return _imageProvider;
            }

            lock (_syncLock)
            {
                return _imageProvider ?? (_imageProvider = ActivateType<IImageProvider>(Settings.Default.ImageProviderType));
            }
        }

        public static IImageStore GetImageStore()
        {
            if (_imageStore != null)
            {
                return _imageStore;
            }

            lock (_syncLock)
            {
                return _imageStore ?? (_imageStore = ActivateType<IImageStore>(Settings.Default.ImageStoreType));
            }
        }

        private static T ActivateType<T>(string type) where T : class
        {
            Type activateType = Type.GetType(type);

            if (activateType == null)
            {
                throw new ConfigurationErrorsException(string.Format("Unable to resolve image store type: {0}", type));
            }

            return Activator.CreateInstance(activateType) as T;
        }

        public static IEnumerable<IImageFilter> GetImageFilters()
        {
            if (_imageFilters == null)
            {
                lock (_syncLock)
                {
                    if (_imageFilters == null)
                    {
                        var imageFilterTypes =
                            Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(
                                    s =>
                                    s.Namespace != null && !s.IsAbstract && s.IsClass
                                    && s.GetInterfaces().Any(i => i == typeof(IImageFilter)))
                                .Distinct()
                                .ToList();

                        _imageFilters = imageFilterTypes.Select(i => Activator.CreateInstance(i) as IImageFilter).ToArray();
                    }
                }
            }

            return _imageFilters;
        }

        public static IImageTool GetImageTool()
        {
            if (_imageTool != null)
            {
                return _imageTool;
            }

            lock (_syncLock)
            {
                return _imageTool ?? (_imageTool = ActivateType<IImageTool>(Settings.Default.ImageToolType));
            }
        }
    }
}