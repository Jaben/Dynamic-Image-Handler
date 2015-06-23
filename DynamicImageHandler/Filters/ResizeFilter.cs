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
    internal class ResizeFilter : MappedParameterFilterBase<ResizeFilterParameters>
    {
        public ResizeFilter()
        {
            this.ActualOrder = 5;
        }

        public override bool ProcessMapped(ResizeFilterParameters @params, HttpContext context, ref Bitmap bitmap)
        {
            int imageResizeWidth = @params.Width ?? 0;
            int imageResizeHeight = @params.Height ?? 0;

            if (imageResizeWidth == 0 && imageResizeHeight == 0)
            {
                imageResizeWidth = bitmap.Width;
                imageResizeHeight = bitmap.Height;
            }

            bitmap = this.ImageTool.Resize(bitmap, imageResizeWidth, imageResizeHeight, @params.Constrain, @params.ForceSquare, @params.GetColor());

            return true;
        }
    }

    internal class ResizeFilterParameters : IImageParameterMapping
    {
        public ResizeFilterParameters()
        {
            this.Constrain = true;
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        [ParameterNames("resize_constrain")]
        public bool Constrain { get; set; }

        [ParameterNames("resize_square", "resize_force_square")]
        public bool ForceSquare { get; set; }

        [ParameterNames("resize_bgcolor", "resize_background")]
        public string BackgroundColor { get; set; }

        public bool IsValid()
        {
            return Width.HasValue || Height.HasValue;
        }

        public Color GetColor()
        {
            if (string.IsNullOrWhiteSpace(BackgroundColor))
            {
                return Color.White;
            }

            return Color.FromName(BackgroundColor);
        }
    }
}