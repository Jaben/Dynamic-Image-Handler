﻿// --------------------------------------------------------------------------------------------------------------------
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
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Web;

namespace DynamicImageHandler.ImageParameters
{
    public interface IImageParameters
    {
        string this[string parameter] { get; }

        string ImageSrc { get; }

        string Key { get; }

        IDictionary<string, string> Parameters { get; }

        void AddCollection(HttpContext context);
    }
}