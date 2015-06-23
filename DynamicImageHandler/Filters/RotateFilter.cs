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

using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.Filters
{
    internal class RotateFilter : MappedParameterFilterBase<RotateFilterParameters>
    {
        public override bool ProcessMapped(RotateFilterParameters @params, HttpContext context, ref Bitmap bitmap)
        {
            bitmap = this.ImageTool.Rotate(bitmap, @params.Rotation ?? 0);

            return true;
        }
    }

    internal class RotateFilterParameters : IImageParameterMapping
    {
        [ParameterNames("rotate")]
        public float? Rotation { get; set; }

        public bool IsValid()
        {
            return Rotation.HasValue;
        }
    }
}