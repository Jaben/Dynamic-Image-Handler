// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
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
using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.Filters
{
    internal class WatermarkFilter : MappedParameterFilterBase<WatermarkFilterParameters>
    {
        public WatermarkFilter()
        {
            // happens last
            this.ActualOrder = 100;
        }

        public override bool ProcessMapped(WatermarkFilterParameters @params, HttpContext context, ref Bitmap bitmap)
        {
            bitmap = this.ImageTool.Watermark(bitmap, @params.Watermark, @params.FontSize, @params.GetWatermarkAlpha(), @params.GetWatermarkColor());
            return true;
        }
    }

    internal class WatermarkFilterParameters : IImageParameterMapping
    {
        public WatermarkFilterParameters()
        {
            this.FontSize = 15;
            this.Opacity = 15;
        }

        [ParameterNames("watermark")]
        public string Watermark { get; set; }

        [ParameterNames("watermark_color")]
        public string WatermarkColor { get; set; }

        [ParameterNames("watermark_fontsize")]
        public float FontSize { get; set; }

        [ParameterNames("watermark_opacity")]
        public float Opacity { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Watermark);
        }

        public byte GetWatermarkAlpha()
        {
            return (byte)Math.Round((this.Opacity/100) * 255);
        }

        public Color GetWatermarkColor()
        {
            if (string.IsNullOrWhiteSpace(WatermarkColor))
            {
                return Color.White;
            }

            return Color.FromName(WatermarkColor);
        }
    }
}