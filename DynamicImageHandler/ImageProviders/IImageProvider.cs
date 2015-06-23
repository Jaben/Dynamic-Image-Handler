// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageProvider.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman and CaptiveAire Systems
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
//   Interface for an image provider. A class for retrieving image data. For example file file system, CMS or database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.ImageProviders
{
    /// <summary>
    ///     Interface for an image provider. A class for retrieving image data. For example file file system, CMS or database
    /// </summary>
    public interface IImageProvider
    {
        void AddImageLastUpdatedParameter(IImageParameters parameters);

        byte[] GetImageData(IImageParameters parameters);
    }
}