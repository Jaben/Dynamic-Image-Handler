// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomFilter.cs" company="">
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
//   The zoom filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.Filters
{
    internal class ZoomFilter : MappedParameterFilterBase<ZoomFilterParameters>
    {
        public ZoomFilter()
        {
            this.ActualOrder = 5;
        }

        public override bool ProcessMapped(ZoomFilterParameters @params, HttpContext context, ref Bitmap bitmap)
        {
            bitmap = this.ImageTool.Zoom(bitmap, @params.Zoom ?? 1);

            return true;
        }
    }

    internal class ZoomFilterParameters : IImageParameterMapping
    {
        [ParameterNames("zoom")]
        public float? Zoom { get; set; }

        public bool IsValid()
        {
            return Zoom.HasValue;
        }
    }
}