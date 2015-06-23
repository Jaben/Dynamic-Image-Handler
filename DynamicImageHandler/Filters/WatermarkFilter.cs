// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

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