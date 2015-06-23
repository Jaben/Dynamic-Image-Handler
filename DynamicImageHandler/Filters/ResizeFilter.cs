// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

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